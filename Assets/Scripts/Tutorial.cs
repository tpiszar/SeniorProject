using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public SimpleManager manager;

    public List<GameObject> enables;
    public List<GameObject> enableDontDisable;
    public List<GameObject> disables;

    public UIScript UI;
    public string nextScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    public virtual void Activate()
    {
        foreach (GameObject go in enables)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in enableDontDisable)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in disables)
        {
            go.SetActive(false);
        }
    }

    public virtual void Complete()
    {
        foreach (GameObject go in enables)
        {
            if (go)
            {
                go.SetActive(false);
            }
        }

        //foreach (GameObject go in enableDontDisable)
        //{
        //    if (go)
        //    {
        //        go.SetActive(true);
        //    }
        //}

        UI.SetScreen(nextScreen);
    }
}
