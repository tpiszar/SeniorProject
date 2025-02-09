using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructablePlant : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Some Effect

        if (other.isTrigger) { return; }

        print(other.gameObject.name);

        Destroy(gameObject);
    }
}
