using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBond : MonoBehaviour
{
    public GameObject[] matchDestroyObjects;

    private void OnDestroy()
    {
        foreach(GameObject obj in matchDestroyObjects)
        {
            if (obj)
            {
                Destroy(obj);
            }
        }
    }
}
