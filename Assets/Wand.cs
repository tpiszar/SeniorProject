using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.Linq;
using System;
using UnityEngine.Events;

public class Wand : MonoBehaviour
{
    public XRNode inputSource;
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

    public XRRayInteractor rayInteractor;
    public XRInteractorLineVisual rayVisual;
    bool usingRay = false;

    public Transform shootPoint;
    public GameObject blast;
    public float force = 30;
    bool active = true;

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

        public Spell(string name, int number)
        {
            this.name = name;
            this.number = number;
        }

        public Spell(string name, int number, float recognitionThreshold)
        {
            this.name = name;
            this.number = number;
            this.recognitionThreshold = recognitionThreshold;
        }
    }
    [SerializeField]
    public Spell[] spells;

    // Start is called before the first frame update
    void Start()
    {
        //XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        //grabbable.activated.AddListener(Attack);

        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
        foreach (TextAsset gestureXml in gesturesXml)
        {
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
        }

        camReference = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        if (isPressed && !primed && !usingRay && !waitForNext)
        {
            charging = true;
            rayInteractor.enabled = false;

            if (positionsList.Count == 0)
            {
                positionsList.Add(camReference.InverseTransformPoint(movementSource.position));
                return;
            }
            Vector3 lastPos = positionsList[positionsList.Count - 1];
            if (Vector3.Distance(camReference.InverseTransformPoint(movementSource.position), lastPos) > newPosThreshold)
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

                    DetectGesture();
                }
            }
        }
        else if (!isPressed && primed)
        {
            //Attack
            switch (activeSpell)
            {
                case 0: //Energy Blast

                    GameObject eBlast = Instantiate(blast, shootPoint.position, Quaternion.identity);
                    eBlast.transform.forward = shootPoint.up;
                    eBlast.GetComponent<Rigidbody>().AddForce(shootPoint.up * force, ForceMode.Impulse);
                    Destroy(eBlast, 3);

                    break;

                case 1: //Fireball

                    break;

                case 2: //Lightning

                    break;
            }

            primed = false;
            rayInteractor.enabled = true;
            activeSpell = -1;
        }

        if (!isPressed && charging)
        {
            if (positionsList.Count < 3)
            {
                rayInteractor.enabled = true;
                positionsList.Clear();
                primer = 0;
            }
            else
            {
                DetectGesture();
            }
        }

        if (!isPressed)
        {
            waitForNext = false;
        }
    }

    void DetectGesture()
    {

        Point[] points = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector3 pos = positionsList[i];
            points[i] = new Point(pos.x, pos.y, pos.z, 0);
        }

        Gesture wandSwing = new Gesture(points);

        if (createGesture)
        {
            string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/Gestures", newGestureName, DateTime.Now.ToFileTime());

            #if !UNITY_WEBPLAYER
                GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
            #endif

            trainingSet.Add(wandSwing);

            print(fileName);
        }

        Result wandResult = PointCloudRecognizer.Classify(wandSwing, trainingSet.ToArray());

        print(wandResult.GestureClass + " " + wandResult.Score);

        //Subject to change
        positionsList.Clear();
        charging = false;

        foreach (Spell spell in spells)
        {
            if (wandResult.GestureClass == spell.name)
            {
                if (wandResult.Score > spell.recognitionThreshold)
                {
                    primed = true;
                    print("PRIMED " + spell.name);
                    activeSpell = spell.number;
                }
                break;
            }
        }

        if (activeSpell < 0)
        {
            waitForNext = true;
        }
    }

    public void RayActive()
    {
        usingRay = true;
    }

    public void NoRay()
    {
        usingRay = false;
    }

    public void EnableLine()
    {
        rayVisual.enabled = true;
    }

    public void DisableLine()
    {
        rayVisual.enabled = false;
    }

    public void UnParent()
    {
        transform.parent = null;
    }
}
