using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.Linq;
using System;
using UnityEngine.SocialPlatforms;
using static Wand;

public class Wand : MonoBehaviour
{
    public CurrentHand hand;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    private List<Vector3> positionsList = new List<Vector3>();

    public float newPosThreshold = 0.05f;

    //BETA
    public float detectRate = 0.2f;
    float nextDetect = 0;

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

    public XRGrabInteractable wandGrabbable;

    public Transform shootPoint;
    public float force = 30;

    bool waitForNext = false;
    bool charging = false;
    bool primed = false;
    public float primeTime = 0.2f;
    float primer = 0;
    int activeSpell = -1;

    public LineRenderer trailLine;
    public float trailDissipateTime = 2f;

    public AudioSource chargingSound;

    [System.Serializable]
    public class Spell
    {
        public bool unlocked = true;
        public string name;
        public int number;
        public float recognitionThreshold = 0.8f;
        public float stopThreshold = 0.6f;
        public GameObject attackPrefab;
        public ParticleSystem chargeSystem;
        [SerializeField]
        public VibrationProfile vibration;
        public AudioSource chargeSound;
        public AudioSource fireSound;
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

    public LightningDrawer lightningDrawer;

    //public LineRenderer[] lightningRenders;
    //public static float lightningSpikesPerUnit = 3;
    //public static float lightningSpikeOffset = 0.2f;
    //public static float lightningSpikeOffsetMax = 0.2f;

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

        wandGrabbable = GetComponent<XRGrabInteractable>();

        wandGrabbable.selectEntered.AddListener(WandGrabbed);
        wandGrabbable.selectExited.AddListener(WandReleased);

        chargingSound.Pause();
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
            chargingSound.Pause();

            DisableControllerRays();
            this.enabled = false;
            return;
        }

        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(hand.inputSource), inputButton, out bool isPressed, inputThreshold);

        if (isPressed && !primed && !usingRay && !waitForNext)
        {
            chargingSound.Play();

            charging = true;
            //rayObjects[currentRay].SetActive(false); //rayInteract.enabled

            if (positionsList.Count == 0)
            {
                positionsList.Add(camReference.InverseTransformPoint(transform.position));//hand.movementSource.position));
                trailLine.positionCount = 1;
                trailLine.SetPosition(0, transform.position);
                trailLine.enabled = true;

                return;
            }
            Vector3 lastPos = positionsList[positionsList.Count - 1];
            Vector3 currentPos = camReference.InverseTransformPoint(transform.position);//hand.movementSource.position); //BETA
            if (Vector3.Distance(currentPos, lastPos) > newPosThreshold)
            {
                positionsList.Add(currentPos);//hand.movementSource.position)); //BETA
                trailLine.positionCount++;
                trailLine.SetPosition(trailLine.positionCount - 1, transform.position);

                if (debug)
                {
                    Destroy(Instantiate(debugObj, transform.position, Quaternion.identity), 3);
                }

                //BETA
                primer = 0;
                nextDetect += Time.deltaTime;
                if (nextDetect > detectRate && !createGesture)
                {
                    nextDetect = 0;
                    DetectGesture(false);
                }
            }
            else
            {
                primer += Time.deltaTime;
                if (primer > primeTime)
                {
                    primer = 0;

                    //BETA
                    nextDetect = 0;

                    if (positionsList.Count < 3)
                    {
                        return;
                    }


                    //print("Stop Detect");
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
                trailLine.positionCount = 0;
                //Invoke("DisableTrail", trailDissipateTime);
                primer = 0;
            }
            else
            {
                //print("Last Second Detect");
                DetectGesture(true);
            }
        }

        if (!isPressed)
        {
            waitForNext = false;

            //BETA
            nextDetect = 0;
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

                //print(fileName);
            }
            else
            {
                string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/" + newGestureName + "Left", newGestureName, DateTime.Now.ToFileTime());

                #if !UNITY_WEBPLAYER
                    GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
                #endif

                leftTrainingSet.Add(wandSwing);

                //print(fileName);
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

        if (debug)
        {
            //print(wandResult.GestureClass + " " + wandResult.Score);
        }

        foreach (Spell spell in spells)
        {
            if (wandResult.GestureClass == spell.name)
            {
                if ((!endCharge && wandResult.Score > spell.recognitionThreshold) || (endCharge & wandResult.Score > spell.stopThreshold))
                {
                    primed = true;
                    //TriggerHaptic(primeHapticIntensity, primeHapticDuration);
                    spell.vibration.Play(hand.controller);

                    if (spell.chargeSound)
                    {
                        spell.chargeSound.Play();
                    }

                    //print("PRIMED " + spell.name);
                    activeSpell = spell.number;
                    spell.chargeSystem.Play();
                }
                break;
            }
        }

        if (activeSpell < 0)
        {
            if (endCharge)
            {
                positionsList.Clear();

                trailLine.positionCount = 0;
                //Invoke("DisableTrail", trailDissipateTime);

                if (createGesture)
                {
                    charging = false;
                    waitForNext = true;
                }

                //BETA
            }
        }
        else
        {
            positionsList.Clear();

            trailLine.positionCount = 0;
            //Invoke("DisableTrail", trailDissipateTime);
            charging = false;
        }

        if (activeSpell == 1)
        {
            DisableControllerRays();

            GameObject fireball = Instantiate(spells[activeSpell].attackPrefab, shootPoint.position, Quaternion.identity);
            //rayInteractor.interactionManager.ForceSelect(rayInteractor, fireball.GetComponent<XRGrabInteractable>());
            wandHeld = fireball.GetComponent<XRGrabInteractable>();

            //dynamicRays[currentRay].enabled = false;
            //rayInteractors[currentRay].allowAnchorControl = false;
            //rayInteractors[currentRay].useForceGrab = true;

            //rayInteractors[currentRay].interactionManager.SelectEnter(rayInteractors[currentRay], wandHeld);

            // Potential Solution hand grabs and attach point is set so it appears to be on the tip of the wand
            fireball.transform.GetChild(0).position = wandGrabbable.attachTransform.position;
            fireball.transform.GetChild(0).rotation = wandGrabbable.attachTransform.rotation;
            wandGrabbable.interactionManager.SelectEnter(hand.interactor, wandHeld);

            fireball.transform.position = shootPoint.position;
        }
        else if (activeSpell == 2)
        {
            lightningCharge = maxChargeTime;
        }
    }

    void Fire()
    {
        spells[activeSpell].chargeSystem.Stop();

        if (spells[activeSpell].fireSound)
        {
            spells[activeSpell].fireSound.Play();
        }

        //Attack
        switch (activeSpell)
        {
            case 0: //Energy Blast

                GameObject eBlast = Instantiate(spells[activeSpell].attackPrefab, shootPoint.position, Quaternion.identity);
                eBlast.transform.forward = shootPoint.up;
                eBlast.GetComponent<Rigidbody>().AddForce(shootPoint.up * force, ForceMode.Impulse);
                Destroy(eBlast, 10);

                break;

            case 1: //Fireball
                //dynamicRays[currentRay].enabled = true;
                //rayInteractors[currentRay].allowAnchorControl = true;
                //rayInteractors[currentRay].useForceGrab = false;

                SetControllerRay();

                // Using Hand
                if (wandHeld)
                {
                    wandGrabbable.interactionManager.SelectExit(hand.interactor, wandHeld);
                }

                //rayInteractors[currentRay].interactionManager.SelectExit(rayInteractors[currentRay], wandHeld);
                break;

            case 2: //Lightning


                //spells[activeSpell].chargeSystem.Stop();

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
                    if (!hits[i].transform.CompareTag("Enemy"))
                    {
                        continue;
                    }

                    BasicHealth newEn = hits[i].transform.GetComponentInParent<BasicHealth>();
                    if (!newEn)
                    {
                        continue;
                    }

                    float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
                    if (newDist < minDist)
                    {
                        if (!Physics.Raycast(shootPoint.position, shootPoint.up, Vector3.Distance(transform.position, newEn.transform.position), blockMask))
                        {
                            minDist = newDist;
                            minEn = newEn;
                        }
                    }
                }

                if (minEn)
                {
                    EnemyBarrier barrier = minEn.Shock((int)(lightningDamage * modifier), jumpMod, jumpCount, jumpRadius, lightningMask, lightningDrawer, jumpDelay);

                    if (barrier)
                    {
                        Vector3 barrierPos = barrier.transform.position + (shootPoint.position - barrier.transform.position).normalized * barrier.transform.lossyScale.x / 2;
                        lightningDrawer.Draw(shootPoint.position, barrierPos, jumpCount - 1, jumpDelay);
                    }
                    else
                    {
                        lightningDrawer.Draw(shootPoint.position, minEn.transform.position, jumpCount - 1, jumpDelay);
                    }

                    //StartCoroutine(DrawLightning(transform.position, minEn.transform.position, lightningRenders[jumpCount - 1]));
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(shootPoint.position, shootPoint.up, out hit, lightningRange, blockMask))
                    {
                        lightningDrawer.Draw(transform.position, hit.point, jumpCount - 1, jumpDelay);

                        //StartCoroutine(DrawLightning(transform.position, hit.point, lightningRenders[jumpCount - 1]));
                    }
                    else
                    {
                        lightningDrawer.Draw(transform.position, shootPoint.position + shootPoint.up * lightningRange, jumpCount - 1, jumpDelay);

                        //StartCoroutine(DrawLightning(transform.position, shootPoint.position + shootPoint.up * lightningRange, lightningRenders[jumpCount - 1]));
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


    // VISUALIZE SPHERE CAST
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(shootPoint.position, lightningRadius);

    //    RaycastHit[] hits = Physics.SphereCastAll(shootPoint.position, lightningRadius, shootPoint.up, lightningRange, lightningMask);
    //    float minDist = float.MaxValue;
    //    BasicHealth minEn = null;
    //    int hitNum = -1;
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        BasicHealth newEn = hits[i].transform.GetComponentInParent<BasicHealth>();
    //        float newDist = (transform.position - hits[i].transform.position).sqrMagnitude;
    //        if (newDist < minDist)
    //        {
    //            if (!Physics.Raycast(shootPoint.position, shootPoint.up, Vector3.Distance(transform.position, newEn.transform.position), blockMask))
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
    //        Vector3 sphereCastMidpoint = shootPoint.position + (shootPoint.up * ((lightningRange - lightningRadius) / 2));
    //        Gizmos.DrawWireSphere(sphereCastMidpoint, lightningRadius);
    //        Debug.DrawLine(shootPoint.position, sphereCastMidpoint, Color.red);

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

    public void TriggerHaptic(float intensity, float duration)
    {
        if (hand.noHand)
        {
            return;
        }

        hand.controller.SendHapticImpulse(intensity, duration);
    }

    void DisableTrail()
    {
        trailLine.enabled = false;
        trailLine.positionCount = 0;
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
