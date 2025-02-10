using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructablePlant : MonoBehaviour
{
    public GameObject destruct;

    bool isDone = false;

    private void OnTriggerEnter(Collider other)
    {
        // Some Effect

        if (isDone || other.isTrigger) { return; }

        isDone = true;

        if (destruct != null)
        {
            Instantiate(destruct, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
