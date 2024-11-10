using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineHands : MonoBehaviour
{
    public float duration = 20f;
    float timer;

    public Transform l_smallHand;
    public Transform r_smallHand;
    public Transform l_bigHand;
    public Transform r_bigHand;

    public Transform l_smallBlock;
    public Transform r_smallBlock;
    public Transform l_bigBlock;
    public Transform r_bigBlock;

    public Transform miniReference;
    public float mapScale = 33.333f;

    public static bool leftDivine = false;
    public static bool rightDivine = false;

    public AudioSource startSound;

    // Start is called before the first frame update
    void Start()
    {
        leftDivine = false;
        rightDivine = false;

        startSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration || (leftDivine && rightDivine))
        {
            if (!leftDivine)
            {
                Destroy(l_smallBlock.gameObject);
                Destroy(l_bigBlock.gameObject);
            }
            if (!rightDivine)
            {
                Destroy(r_smallBlock.gameObject);
                Destroy(r_bigBlock.gameObject);
            }

            l_bigHand.gameObject.SetActive(false);
            r_bigHand.gameObject.SetActive(false);

            Destroy(this);

            return;
        }

        if (!leftDivine)
        {
            l_bigBlock.transform.position = miniReference.InverseTransformPoint(l_smallBlock.position) * mapScale;
            Vector3 lRotation = l_smallBlock.rotation.eulerAngles;
            lRotation.y -= miniReference.rotation.eulerAngles.y;
            l_bigBlock.rotation = Quaternion.Euler(lRotation);
        }
        if (!rightDivine)
        {
            r_bigBlock.transform.position = miniReference.InverseTransformPoint(r_smallBlock.position) * mapScale;
            Vector3 rRotation = r_smallBlock.rotation.eulerAngles;
            rRotation.y -= miniReference.rotation.eulerAngles.y;
            r_bigBlock.rotation = Quaternion.Euler(rRotation);
        }

        l_bigHand.transform.position = miniReference.InverseTransformPoint(l_smallHand.position) * mapScale;
        Vector3 lHandRotation = l_smallHand.rotation.eulerAngles;
        lHandRotation.y -= miniReference.rotation.eulerAngles.y;
        l_bigHand.rotation = Quaternion.Euler(lHandRotation);

        r_bigHand.transform.position = miniReference.InverseTransformPoint(r_smallHand.position) * mapScale;
        Vector3 rHandRotation = r_smallHand.rotation.eulerAngles;
        rHandRotation.y -= miniReference.rotation.eulerAngles.y;
        r_bigHand.rotation = Quaternion.Euler(rHandRotation);
    }
}
