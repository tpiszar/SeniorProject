using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.EventSystems.EventTrigger;

public class Wand : MonoBehaviour
{
    public CurrentHand hand;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    private List<Vector3> positionsList = new List<Vector3>();

    public float newPosThreshold = 0.05f;

    Transform camReference;
    public bool createGesture = false;
    public string newGestureName;
    public bool debug = false;
    public GameObject debugObj;
    private List<Gesture> trainingSet = new List<Gesture>();
    private List<Gesture> leftTrainingSet = new List<Gesture>();

    public GameObject[] rayObjects;
    public XRRayInteractor[] rayInteractors;
    public WandRayDynamic[] dynamicRays;
    int currentRay = 0;
    //public XRInteractorLineVisual rayVisual;
    XRGrabInteractable wandHeld;
    bool usingRay = false;

    public XRGrabInteractable wandGrababble;

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
        public bool unlocked = true;
        public string name;
        public int number;
        public float recognitionThreshold = 0.8f;
        public GameObject attackPrefab;
    }

    [SerializeField]
    public Spell[] spells;

    float lightningCharge = 0;
    public int lightningDamage;
    public LayerMask lightningMask;
    public LayerMask blockMask;
    public float lightningRange = 50;
    public float lightningRadius;
    public float maxChargeTime = 5;
    public float minChargeTime = 1;
    public float maxChargeMod = 2;
    public int jumpCount = 3;
    public float jumpMod = 0.5f;
    public float jumpRadius;
    public float jumpDelay = 1;

    public LineRenderer lightningRender;
    public static float lightningSpikesPerUnit = 3;
    public static float lightningSpikeOffset = 0.1f;

    [Range(0, 1)]
    public float primeHapticIntensity;
    public float primeHapticDuration;
    [Range(0, 1)]
    public float fireHapticIntensity;
    public float fireHapticDuration;
    [Range(0, 1)]
    public float magicGrabHapticIntensity;
    public float magicGrabHapticDuration;
    [Range(0, 1)]
    public float chargeIntensityMin;
    [Range(0, 1)]
    public float chargeIntensityMax;
    public float chargeDuration;

    // Start is called before the first frame update
    void Start()
    {
        //rayInteractor.interactablesSelected[0];
        //rayInteractor.interactionManager.SelectEnter(rayInteractor.GetComponent<IXRSelectInteractor>(), wandHeld.GetComponent<IXRSelectInteractable>());

        //TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
        //foreach (TextAsset gestureXml in gesturesXml)
        //{
        //    trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
        //}
        
        foreach (Spell spell in spells)
        {
            if (spell.unlocked)
            {
                TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>(spell.name + "/");
                foreach (TextAsset gesture in gesturesXml)
                {
                    trainingSet.Add(GestureIO.ReadGestureFromXML(gesture.text));
                }

                TextAsset[] gesturesXmlLeft = Resources.LoadAll<TextAsset>(spell.name + "Left/");
                foreach (TextAsset gesture in gesturesXmlLeft)
                {
                    leftTrainingSet.Add(GestureIO.ReadGestureFromXML(gesture.text));
                }
            }
        }
        

        //TextAsset[] gesturesXmlLeft = Resources.LoadAll<TextAsset>("GesturesLeft/");
        //foreach (TextAsset gestureXmlLeft in gesturesXmlLeft)
        //{
        //    leftTrainingSet.Add(GestureIO.ReadGestureFromXML(gestureXmlLeft.text));
        //}

        camReference = Camera.main.transform;

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();

        wandGrababble = grabbable;

        grabbable.selectEntered.AddListener(WandGrabbed);
        grabbable.selectExited.AddListener(WandReleased);
    }

    // Update is called once per frame
    void Update()
    {
        if (lightningCharge > 0)
        {
            lightningCharge -= Time.deltaTime;
            if (lightningCharge < maxChargeTime - minChargeTime)
            {
                TriggerHaptic(Mathf.Lerp(chargeIntensityMin, chargeIntensityMax, (maxChargeTime - lightningCharge) / maxChargeTime), chargeDuration);
            }
            if (lightningCharge < 0)
            {
                TriggerHaptic(fireHapticIntensity, fireHapticDuration);
            }
        }

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
                positionsList.Add(camReference.InverseTransformPoint(hand.movementSource.position));
                return;
            }
            Vector3 lastPos = positionsList[positionsList.Count - 1];
            Vector3 currentPos = camReference.InverseTransformPoint(hand.movementSource.position);
            if (Vector3.Distance(currentPos, lastPos) > newPosThreshold)
            {
                positionsList.Add(camReference.InverseTransformPoint(hand.movementSource.position));

                if (debug)
                {
                    Destroy(Instantiate(debugObj, hand.movementSource.position, Quaternion.identity), 3);
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
                string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/" + newGestureName, newGestureName, DateTime.Now.ToFileTime());

                #if !UNITY_WEBPLAYER
                    GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
                #endif

                trainingSet.Add(wandSwing);

                print(fileName);
            }
            else
            {
                string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/" + newGestureName + "Left", newGestureName, DateTime.Now.ToFileTime());

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
            GameObject fireball = Instantiate(spells[activeSpell].attackPrefab, shootPoint.position, Quaternion.identity);
            //rayInteractor.interactionManager.ForceSelect(rayInteractor, fireball.GetComponent<XRGrabInteractable>());
            wandHeld = fireball.GetComponent<XRGrabInteractable>();

            dynamicRays[currentRay].enabled = false;
            rayInteractors[currentRay].allowAnchorControl = false;
            rayInteractors[currentRay].useForceGrab = true;
            rayInteractors[currentRay].interactionManager.SelectEnter(rayInteractors[currentRay], wandHeld);

            fireball.transform.position = shootPoint.position;
        }
        else if (activeSpell == 2)
        {
            lightningCharge = maxChargeTime;
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
                dynamicRays[currentRay].enabled = true;
                rayInteractors[currentRay].allowAnchorControl = true;
                rayInteractors[currentRay].useForceGrab = false;

                //wandGrababble.interactionManager.SelectExit(hand.movementSource.GetComponent<XRDirectInteractor>(), wandHeld);

                //rayInteractors[currentRay].interactionManager.SelectExit(rayInteractors[currentRay], wandHeld);
                break;

            case 2: //Lightning


                float modifier = 1;
                if (lightningCharge < maxChargeTime - minChargeTime) 
                {
                    modifier += (maxChargeTime - lightningCharge - minChargeTime) / (maxChargeTime - minChargeTime) * maxChargeMod;
                }
                lightningCharge = 0;

                RaycastHit[] hits = Physics.SphereCastAll(shootPoint.position, lightningRadius, shootPoint.up, lightningRange, lightningMask);
                float minDist = float.MaxValue;
                BasicHealth minEn = null;
                for (int i = 0; i < hits.Length; i++)
                {
                    BasicHealth newEn = hits[i].transform.GetComponent<BasicHealth>();
                    
                    if (!newEn)
                    {
                        continue;
                    }

                    float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
                    if (newDist < minDist)
                    {
                        if (Physics.Raycast(shootPoint.position, shootPoint.up, lightningRange, blockMask))
                        {
                            minDist = newDist;
                            minEn = newEn;
                        }
                    }
                }

                if (minEn)
                {
                    minEn.Shock((int)(lightningDamage * modifier), jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay);

                    StartCoroutine(DrawLightning(minEn.transform.position));
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(shootPoint.position, shootPoint.up, out hit, lightningRange, blockMask))
                    {
                        StartCoroutine(DrawLightning(hit.point));
                    }
                    else
                    {
                        StartCoroutine(DrawLightning(shootPoint.position + shootPoint.up * lightningRange));
                    }
                }

                //RaycastHit hit;
                //if (Physics.SphereCast(shootPoint.position, lightningRadius, shootPoint.up, out hit, lightningRange, lightningMask))
                //{
                //    BasicHealth enemy = hit.transform.GetComponent<BasicHealth>();
                //    if (enemy)
                //    {
                //        enemy.Shock((int)(lightningDamage * modifier), jumpMod, jumpCount, jumpRadius, lightningMask, jumpDelay);

                //        StartCoroutine(DrawLightning(enemy.transform));
                //    }
                //}

                break;
        }

        TriggerHaptic(fireHapticIntensity, fireHapticDuration);

        primed = false;
        rayObjects[currentRay].SetActive(true);
        activeSpell = -1;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(shootPoint.position, lightningRadius);

    //    RaycastHit[] hits = Physics.SphereCastAll(shootPoint.position, lightningRadius, shootPoint.up, lightningRange, lightningMask);
    //    print(hits.Count());
    //    float minDist = float.MaxValue;
    //    BasicHealth minEn = null;
    //    int hitNum = -1;
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        BasicHealth newEn = hits[i].transform.GetComponent<BasicHealth>();
    //        float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
    //        if (newDist < minDist)
    //        {
    //            if (!Physics.Raycast(shootPoint.position, shootPoint.up, lightningRange, blockMask))
    //            {
    //                minDist = newDist;
    //                minEn = newEn;
    //                hitNum = i;
    //            }
    //        }
    //    }

    //    if (minEn)
    //    {
    //        Gizmos.color = Color.green;
    //        Vector3 sphereCastMidpoint = shootPoint.position + (shootPoint.up * hits[hitNum].distance);
    //        Gizmos.DrawWireSphere(sphereCastMidpoint, lightningRadius);
    //        Gizmos.DrawSphere(hits[hitNum].point, 0.1f);
    //        Debug.DrawLine(shootPoint.position, sphereCastMidpoint, Color.green);
    //    }
    //    else
    //    {
    //        Gizmos.color = Color.red;

    //    }
    //    Vector3 sphereCastMidpoint2 = shootPoint.position + (shootPoint.up * (lightningRange - lightningRadius));
    //    Gizmos.DrawWireSphere(sphereCastMidpoint2, lightningRadius);
    //    Debug.DrawLine(transform.position, sphereCastMidpoint2, Color.red);

    //    //RaycastHit hit;
    //    //if (Physics.SphereCast(shootPoint.position, lightningCharge, shootPoint.up, out hit, lightningRange, lightningMask))
    //    //{

    //    //}
    //    //else
    //    //{
    //    //    Gizmos.color = Color.red;
    //    //    Vector3 sphereCastMidpoint = shootPoint.position + (shootPoint.up * (lightningRange - lightningRadius));
    //    //    Gizmos.DrawWireSphere(sphereCastMidpoint, lightningRadius);
    //    //    Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
    //    //}
    //}

    IEnumerator DrawLightning(Vector3 enemy)
    {
        float distance = Vector3.Distance(transform.position, enemy);
        int numSegments = Mathf.CeilToInt(distance * lightningSpikesPerUnit);

        // Set the LineRenderer position count
        lightningRender.positionCount = numSegments + 2;  // +2 to account for start and end points

        // Set the start position (wand position)
        lightningRender.SetPosition(0, transform.position);

        // Add intermediate points with random offsets to create spikes
        for (int i = 1; i <= numSegments; i++)
        {
            // Interpolate between wand and enemy
            float t = (float)i / (numSegments + 1);  // Normalized position along the line
            Vector3 interpolatedPos = Vector3.Lerp(transform.position, enemy, t);

            // Apply a random offset to make it jagged (spike effect)
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset),
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset),
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset)
            );

            // Set the position in the LineRenderer
            lightningRender.SetPosition(i, interpolatedPos + randomOffset);
        }
        lightningRender.SetPosition(lightningRender.positionCount - 1, enemy);

        lightningRender.enabled = true;

        yield return new WaitForSeconds(jumpDelay);

        lightningRender.enabled = false;
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
