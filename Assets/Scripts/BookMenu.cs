using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenu : MonoBehaviour
{
    public GameObject[] level2Unlocks;
    public GameObject[] level3Unlocks;

    public GameObject[] level2Locks;
    public GameObject[] level3Locks;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveLoad.level1Done)
        {
            foreach (GameObject question in level2Locks)
            {
                question.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject button in level2Unlocks)
            {
                button.SetActive(false);
            }
        }

        if (SaveLoad.level2Done)
        {
            foreach (GameObject question in level3Locks)
            {
                question.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject button in level3Unlocks)
            {
                button.SetActive(false);
            }
        }

        Destroy(gameObject);
    }
}
