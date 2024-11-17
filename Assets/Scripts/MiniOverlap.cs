using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniOverlap : MonoBehaviour
{
    public CreateTower mini;


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Blocker"))
    //    {
    //        if (mini != GetComponentInParent<CreateTower>())
    //        { 
    //            mini.overlap++;
    //            print("OVERLAP:" + mini.overlap);
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Blocker"))
    //    {
    //        if (mini != GetComponentInParent<CreateTower>())
    //        {
    //            mini.overlap--;
    //            print("OVERLAP:" + mini.overlap);
    //        }
    //    }
    //}
}
