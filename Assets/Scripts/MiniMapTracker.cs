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
    public GameObject barrierPrefab;

    public Transform playerBarrier;
    public Transform miniPlayerBarrier;
    bool noBarrier = false;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerBarrier)
        {
            miniPlayerBarrier.position = miniReference.TransformPoint(playerBarrier.position / mapScale);
        }
        else if (!noBarrier)
        {
            Destroy(miniPlayerBarrier.gameObject);
            noBarrier = true;
        }

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

    public void AddMapBarrier(Transform obj)
    {
        trackedObjects.Add(obj);
        GameObject newBarrier = Instantiate(barrierPrefab, miniReference.TransformPoint(obj.position / mapScale), Quaternion.Inverse(obj.rotation) * miniReference.rotation);
        newBarrier.transform.localScale = obj.localScale / mapScale;
        trackers.Add(newBarrier.transform);
    }
}
