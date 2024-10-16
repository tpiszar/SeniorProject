using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> screens;

    public List<GameObject> enables;
    public List<GameObject> enableConditionalDisable;
    public List<GameObject> disables;

    public bool conditionalDisable = false;

    public List<Transform> keyObjects;

    public TutorialManager manager;

    public bool fullActivate = false;

    bool useKeys = true;

    // Start is called before the first frame update
    void Start()
    {
        if (keyObjects.Count == 0)
        {
            useKeys = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (useKeys)
        {
            for (int i = keyObjects.Count - 1; i >= 0; i--)
            {
                if (!keyObjects[i])
                {
                    keyObjects.RemoveAt(i);
                }
            }
            if (keyObjects.Count == 0)
            {
                manager.NextTutorial();
            }
        }
    }

    private void OnEnable()
    {
        foreach (GameObject go in screens)
        {
            go.SetActive(true);
        }
        
        if (fullActivate)
        {
            Activate();
        }
    }

    public void Activate()
    {
        foreach (GameObject go in enables)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in enableConditionalDisable)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in disables)
        {
            go.SetActive(false);
        }
    }

    private void OnDisable()
    {
        foreach(GameObject go in screens)
        {
            if (go)
            {
                go.SetActive(false);
            }

        }

        foreach (GameObject go in enables)
        {
            if (go)
            {
                go.SetActive(false);
            }
        }        

        if (conditionalDisable)
        {
            foreach (GameObject go in enableConditionalDisable)
            {
                if (go)
                {
                    go.SetActive(false);
                }

            }
        }
        else
        {
            foreach (GameObject go in enableConditionalDisable)
            {
                if (go)
                {
                    go.SetActive(true);
                }

            }
        }
    }
}
