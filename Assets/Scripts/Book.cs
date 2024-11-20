using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Book : MonoBehaviour
{
    public float pointThreshold;

    public Animator animator;

    public Animator leftHand;
    public Animator rightHand;
    public Transform leftPoint;
    public Transform rightPoint;

    public CurrentHand hand;

    bool held = false;
    bool open = false;

    public GameObject[] leftPages;
    public GameObject[] rightPages;
    public int curPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();

        grabbable.selectEntered.AddListener(Grab);
        grabbable.selectExited.AddListener(Drop);
    }

    // Update is called once per frame
    void Update()
    {
        if (held && !open)
        {
            Open();
        }
        if (!held && open)
        {
            Close();
        }

        if (open)
        {
            if (hand.inputSource == XRNode.LeftHand)
            {
                if (Vector3.Distance(transform.position, rightPoint.position) < pointThreshold)
                {
                    rightHand.SetBool("Point", true);
                    rightPoint.gameObject.SetActive(true);
                }
                else
                {
                    rightHand.SetBool("Point", false);
                    rightPoint.gameObject.SetActive(false);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, leftPoint.position) < pointThreshold)
                {
                    leftHand.SetBool("Point", true);
                    leftPoint.gameObject.SetActive(true);
                }
                else
                {
                    leftHand.SetBool("Point", false);
                    leftPoint.gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void Turn(int page)
    {
        leftPages[curPage].SetActive(false);
        leftPages[page].SetActive(true);
        rightPages[curPage].SetActive(false);
        rightPages[page].SetActive(true);
        curPage = page;
    }

    void Open()
    {
        held = true;
        open = true;

        //leftPages[0].SetActive(true);
        //rightPages[0].SetActive(true);

        animator.SetBool("Open", held);
    }

    void Close()
    {
        held = false;
        open = false;

        //leftPages[curPage].SetActive(false);
        //rightPages[curPage].SetActive(false);

        animator.SetBool("Open", held);


        leftHand.SetBool("Point", false);
        leftPoint.gameObject.SetActive(false);

        rightHand.SetBool("Point", false);
        rightPoint.gameObject.SetActive(false);
    }

    void Grab(SelectEnterEventArgs args)
    {
        held = true;
    }

    void Drop(SelectExitEventArgs args)
    {
        held = false;
    }
}
