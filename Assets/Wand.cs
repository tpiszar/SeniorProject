using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Wand : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Transform shootPoint;
    public GameObject blast;
    public float force = 30;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Attack);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Attack(ActivateEventArgs args)
    {
        if (active)
        {
            GameObject eBlast = Instantiate(blast, shootPoint.position, Quaternion.identity);
            eBlast.GetComponent<Rigidbody>().AddForce(shootPoint.up * force, ForceMode.Impulse);
            Destroy(eBlast, 3);
        }   

    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }
}
