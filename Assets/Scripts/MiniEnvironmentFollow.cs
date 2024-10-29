using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MiniEnvironmentFollow : MonoBehaviour
{
    //public Transform miniMap;

    public GameObject[] detectors;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.selectEntered.AddListener(Selected);
        grabbable.selectExited.AddListener(Released);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Selected(SelectEnterEventArgs args)
    {
        foreach (GameObject detector in detectors)
        {
            detector.SetActive(false);
        }
    }

    void Released(SelectExitEventArgs args)
    {
        foreach (GameObject detector in detectors)
        {
            detector.SetActive(true);
        }
    }
}
