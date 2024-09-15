using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapTracker : MonoBehaviour
{
    List<Transform> trackedObjects = new List<Transform>();
    List<Transform> trackers = new List<Transform>();

    public Transform miniReference;
    public float mapScale;

    public static MiniMapTracker instance;

    public GameObject trackerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = trackedObjects.Count - 1; i >= 0; i--)
        {
            if (!trackedObjects[i])
            {
                trackedObjects.RemoveAt(i);
                Destroy(trackers[i].gameObject);
                trackers.RemoveAt(i);
            }
            else
            {
                trackers[i].position = miniReference.TransformPoint(trackedObjects[i].position / mapScale);
            }
        }
    }

    public void AddMapTracker(Transform obj)
    {
        trackedObjects.Add(obj);
        GameObject newTracker = Instantiate(trackerPrefab, miniReference.TransformPoint(obj.position / mapScale), Quaternion.identity);
        trackers.Add(newTracker.transform);
    }
}
