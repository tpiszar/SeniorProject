using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRay : MonoBehaviour
{
    bool holding = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!WaveManager.LevelEnd || holding)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void isHolding(bool hold)
    {
        holding = hold;
        if (holding)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (!WaveManager.LevelEnd || holding)
        {
            gameObject.SetActive(false);
        }
    }
}
