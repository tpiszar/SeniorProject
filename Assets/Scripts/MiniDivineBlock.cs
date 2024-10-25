using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDivineBlock : MonoBehaviour
{
    public float duration = 20f;
    public float timer;

    public Transform bigBlock;
    public Transform smallHand;
    public Transform bigHand;
    public Transform miniReference;
    public float mapScale = 33.333f;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(this);
            Destroy(bigBlock.gameObject);
            return;
        }

        bigBlock.transform.position = miniReference.InverseTransformPoint(transform.position) * mapScale;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y -= miniReference.rotation.eulerAngles.y;
        bigBlock.rotation = Quaternion.Euler(rotation);
        //bigBlock.rotation = Quaternion.Inverse(transform.rotation)* miniReference.rotation;

        bigHand.transform.position = miniReference.InverseTransformPoint(smallHand.position) * mapScale;
        Vector3 handRotation = smallHand.rotation.eulerAngles;
        handRotation.y -= miniReference.rotation.eulerAngles.y;
        bigHand.rotation = Quaternion.Euler(handRotation);
        //bigHand.rotation = Quaternion.Inverse(smallHand.rotation) * miniReference.rotation;
    }

    private void OnEnable()
    {
        bigHand.gameObject.SetActive(true);
        bigBlock.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {

        bigHand.gameObject.SetActive(false);
    }
}
