using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TutorialTeleport : MonoBehaviour
{
    public int amount = 2;
    int count = 0;

    public GameObject[] crystals;

    public TutorialManager manager;

    bool teleportSet = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!teleportSet)
        {
            if (Teleport.Instance)
            {
                Teleport.Instance.onTeleport += OnTeleport;
                teleportSet = true;
            }
        }
    }

    void OnTeleport(float teleportTime)
    {
        count++;
        if (count == amount)
        {
            //Invoke("DestoryCrystals", teleportTime + 0.1f);
            StartCoroutine(DestroyCrystals(teleportTime));
            manager.NextTutorial();
        }
    }

    IEnumerator DestroyCrystals(float seconds)
    {
        yield return new WaitForSeconds(seconds + 0.2f);

        foreach (GameObject obj in crystals)
        {
            Destroy(obj);
        }
    }

    //public void DestroyCrystals()
    //{
    //    foreach (GameObject obj in crystals)
    //    {
    //        Destroy(obj);
    //    }
    //}
}
