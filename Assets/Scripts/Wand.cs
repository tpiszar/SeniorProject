using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.Linq;
using System;
using Unity.VisualScripting;

public class Wand : MonoBehaviour
{
    public CurrentHand hand;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    private List<Vector3> positionsList = new List<Vector3>();
    public Transform movementSource;

    public float newPosThreshold = 0.05f;

    Transform camReference;
    public bool createGesture = false;
    public string newGestureName;
    public bool debug = false;
    public GameObject debugObj;
    private List<Gesture> trainingSet = new List<Gesture>();
    private List<Gesture> leftTrainingSet = new List<Gesture>();

    public GameObject[] rayObjects;
   // public XRRayInteractor[] rayInteractors;
    int currentRay = 0;
    //public XRInteractorLineVisual rayVisual;
    XRGrabInteractable wandHeld;
    bool usingRay = false;

    public Transform shootPoint;
    public float force = 30;

    bool waitForNext = false;
    bool charging = false;
    bool primed = false;
    public float primeTime = 0.2f;
    float primer = 0;
    int activeSpell = -1;

    [System.Serializable]
    public class Spell
    {
        public string name;
        public int number;
        public float recognitionThreshold = 0.8f;
        public GameObject attackPrefab;
    }

    [SerializeField]
    public Spell[] spells;

    [Range(0, 1)]
    public float primeHapticIntensity;
    public float primeHapticDuration;
    [Range(0, 1)]
    public float fireHapticIntensity;
    public float fireHapticDuration;
    [Range(0, 1)]
    public float magicGrabHapticIntensity;
    public float magicGrabHapticDuration;

    // Start is called before the first frame update
    void Start()
    {
        //rayInteractor.interactablesSelected[0];
        //rayInteractor.interactionManager.SelectEnter(rayInteractor.GetComponent<IXRSelectInteractor>(), wandHeld.GetComponent<IXRSelectInteractable>());

        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
        foreach (TextAsset gestureXml in gesturesXml)
        {
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
        }

        TextAsset[] gesturesXmlLeft = Resources.LoadAll<TextAsset>("GesturesLeft/");
        foreach (TextAsset gestureXmlLeft in gesturesXmlLeft)
        {
            leftTrainingSet.Add(GestureIO.ReadGestureFromXML(gestureXmlLeft.text));
        }

        camReference = Camera.main.transform;

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(WandGrabbed);
        grabbable.selectExited.AddListener(WandReleased);
    }

    // Update is called once per frame
    void Update()
    {
        if (hand.noHand)
        {
            DisableControllerRays();
            this.enabled = false;
            return;
        }

        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(hand.inputSource), inputButton, out bool isPressed, inputThreshold);

        if (isPressed && !primed && !usingRay && !waitForNext)
        {
            charging = true;
            //rayObjects[currentRay].SetActive(false); //rayInteract.enabled

            if (positionsList.Count == 0)
            {
                positionsList.Add(camReference.InverseTransformPoint(movementSource.position));
                return;
            }
            Vector3 lastPos = positionsList[positionsList.Count - 1];
            Vector3 currentPos = camReference.InverseTransformPoint(movementSource.position);
            if (Vector3.Distance(currentPos, lastPos) > newPosThreshold)
            {
                positionsList.Add(camReference.InverseTransformPoint(movementSource.position));

                if (debug)
                {
                    Destroy(Instantiate(debugObj, movementSource.position, Quaternion.identity), 3);
                }
            }
            else
            {
                primer += Time.deltaTime;
                if (primer > primeTime)
                {
                    primer = 0;

                    if (positionsList.Count < 3)
                    {
                        return;
                    }

                    print("Stop Detect");
                    DetectGesture(true);
                }
            }
        }
        else if (!isPressed && primed)
        {
            Fire();
        }

        if (!isPressed && charging)
        {
            if (positionsList.Count < 3)
            {
                rayObjects[currentRay].SetActive(true);
                positionsList.Clear();
                primer = 0;
            }
            else
            {
                print("Last Second Detect");
                DetectGesture(true);
            }
        }

        if (!isPressed)
        {
            waitForNext = false;
        }
    }

    void DetectGesture(bool endCharge)
    {
        bool righty = hand.inputSource == XRNode.RightHand;

        Point[] points = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector3 pos = positionsList[i];
            points[i] = new Point(pos.x, pos.y, pos.z, 0);
        }

        Gesture wandSwing = new Gesture(points);

        if (createGesture)
        {
            if (righty)
            {
                string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/Gestures", newGestureName, DateTime.Now.ToFileTime());

                #if !UNITY_WEBPLAYER
                    GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
                #endif

                trainingSet.Add(wandSwing);

                print(fileName);
            }
            else
            {
                string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/GesturesLeft", newGestureName, DateTime.Now.ToFileTime());

                #if !UNITY_WEBPLAYER
                    GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
                #endif

                leftTrainingSet.Add(wandSwing);

                print(fileName);
            }
        }

        Result wandResult; 
        if (righty)
        {
            wandResult = PointCloudRecognizer.Classify(wandSwing, trainingSet.ToArray());
        }
        else
        {
            wandResult = PointCloudRecognizer.Classify(wandSwing, leftTrainingSet.ToArray());
        }

        print(wandResult.GestureClass + " " + wandResult.Score);

        foreach (Spell spell in spells)
        {
            if (wandResult.GestureClass == spell.name)
            {
                if (wandResult.Score > spell.recognitionThreshold)
                {
                    primed = true;
                    TriggerHaptic(primeHapticIntensity, primeHapticDuration);
                    print("PRIMED " + spell.name);
                    activeSpell = spell.number;
                }
                break;
            }
        }

        if (activeSpell < 0)
        {
            if (endCharge)
            {
                positionsList.Clear();
                charging = false;
                waitForNext = true;
            }
        }
        else
        {
            positionsList.Clear();
            charging = false;
        }

        if (activeSpell == 1)
        {
            //GameObject fireball = Instantiate(spells[activeSpell].attackPrefab, shootPoint.position, Quaternion.identity);
            ////rayInteractor.interactionManager.ForceSelect(rayInteractor, fireball.GetComponent<XRGrabInteractable>());
            //wandHeld = fireball.GetComponent<XRGrabInteractable>();
            //rayInteractor.interactionManager.SelectEnter(rayInteractor, wandHeld);
            //fireball.transform.position = shootPoint.position;
        }
    }

    void Fire()
    {
        //Attack
        switch (activeSpell)
        {
            case 0: //Energy Blast

                GameObject eBlast = Instantiate(spells[activeSpell].attackPrefab, shootPoint.position, Quaternion.identity);
                eBlast.transform.forward = shootPoint.up;
                eBlast.GetComponent<Rigidbody>().AddForce(shootPoint.up * force, ForceMode.Impulse);
                Destroy(eBlast, 3);

                break;

            case 1: //Fireball
                    //rayInteractor.interactionManager.SelectExit(rayInteractor, wandHeld);
                break;

            case 2: //Lightning

                break;
        }

        TriggerHaptic(fireHapticIntensity, fireHapticDuration);

        primed = false;
        rayObjects[currentRay].SetActive(true);
        activeSpell = -1;
    }

    public void TriggerHaptic(float intensity, float duration)
    {
        hand.controller.SendHapticImpulse(intensity, duration);
    }

    public void RayActive()
    {
        usingRay = true;
        TriggerHaptic(magicGrabHapticIntensity, magicGrabHapticDuration);
    }

    public void NoRay()
    {
        usingRay = false;
    }

    //public void EnableLine()
    //{
    //    rayVisual.enabled = true;
    //}

    //public void DisableLine()
    //{
    //    rayVisual.enabled = false;
    //}

    private void OnEnable()
    {
        SetControllerRay();
    }

    void WandGrabbed(SelectEnterEventArgs args)
    {
        SetControllerRay();
    }

    void WandReleased(SelectExitEventArgs args)
    {
        DisableControllerRays();
    }

    public void DisableControllerRays()
    {
        rayObjects[0].SetActive(false);
        rayObjects[1].SetActive(false);
    }

    void SetControllerRay()
    {
        if (hand.inputSource == XRNode.RightHand)
        {
            currentRay = 0;
            rayObjects[0].SetActive(true);
            rayObjects[1].SetActive(false);
        }
        else
        {
            currentRay = 1;
            rayObjects[0].SetActive(false);
            rayObjects[1].SetActive(true);
        }
    }
}
