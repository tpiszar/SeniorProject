using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawns : MonoBehaviour
{
    public GameObject[] objectsToRespawn;
    GameObject[] copies;

    public static Respawns instance;

    public void Awake()
    {
        foreach (GameObject obj in objectsToRespawn)
        {
            obj.SetActive(false);
        }

        copies = new GameObject[objectsToRespawn.Length];

        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < copies.Length; i++)
        {
            GameObject newObj = Instantiate(objectsToRespawn[i]);
            newObj.transform.position = objectsToRespawn[i].transform.position;
            newObj.transform.rotation = objectsToRespawn[i].transform.rotation;
            newObj.transform.parent = objectsToRespawn[i].transform.parent;
            //newObj.transform.parent = transform;
            newObj.SetActive(true);
            copies[i] = newObj;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject obj in copies)
        {
            Destroy(obj);
        }
    }

    public GameObject earlyRespawn(float delay, int objNum)
    {
        GameObject newObj = Instantiate(objectsToRespawn[objNum]);
        newObj.transform.position = objectsToRespawn[objNum].transform.position;
        newObj.transform.rotation = objectsToRespawn[objNum].transform.rotation;
        newObj.transform.parent = objectsToRespawn[objNum].transform.parent;
        //newObj.transform.parent = transform; //Hopefully Not Necessary???
        newObj.SetActive(false);
        copies[objNum] = newObj;

        StartCoroutine(enable(delay, objNum));

        return newObj;
    }

    public int getObjNum(GameObject obj)
    {
        int objNum = -1;
        for (int i = 0; i < copies.Length; i++)
        {
            if (obj == copies[i])
            {
                objNum = i;
                break;
            }
        }

        return objNum;
    }

    private IEnumerator enable(float delay, int objNum)
    {
        yield return new WaitForSeconds(delay);

        copies[objNum].SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
