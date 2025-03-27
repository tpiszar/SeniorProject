using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MiniMapTracker : MonoBehaviour
{
    List<Transform> trackedObjects = new List<Transform>();
    List<Transform> trackers = new List<Transform>();

    public Transform miniReference;
    public float mapScale;

    public static MiniMapTracker instance;

    public GameObject[] trackerPrefabs;
    public GameObject barrierPrefab;

    public Transform playerBarrier;
    public Transform miniPlayerBarrier;
    bool noBarrier = false;

    public ParticleSystem destroyParticle;

    public float miniShrinkSpeed = 0.1f;

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
            //Destroy(miniPlayerBarrier.gameObject);

            ShrinkDestroy(miniPlayerBarrier);

            noBarrier = true;
        }

        for (int i = trackedObjects.Count - 1; i >= 0; i--)
        {
            if (!trackedObjects[i])
            {
                trackedObjects.RemoveAt(i);
                //Destroy(trackers[i].gameObject);

                ShrinkDestroy(trackers[i]);

                trackers.RemoveAt(i);
            }
            else
            {
                trackers[i].position = miniReference.TransformPoint(trackedObjects[i].position / mapScale);
                trackers[i].rotation = trackedObjects[i].rotation * miniReference.rotation;
            }
        }
    }

    public void ShrinkDestroy(Transform obj)
    {
        StartCoroutine(Shrink(obj));
    }

    IEnumerator Shrink(Transform obj)
    {
        Vector3 maxScale = obj.localScale;

        float timer = 0;
        while (timer < miniShrinkSpeed)
        {
            timer += Time.deltaTime;

            obj.localScale = Vector3.Lerp(maxScale, Vector3.zero, timer / miniShrinkSpeed);

            yield return null;
        }

        Instantiate(destroyParticle, obj.transform.position, Quaternion.identity);

        Destroy(obj.gameObject);
    }

    public void AddMapTracker(Transform obj, Enemytype type)
    {
        trackedObjects.Add(obj);
        GameObject newTracker = Instantiate(trackerPrefabs[(int)type], miniReference.TransformPoint(obj.position / mapScale), Quaternion.identity);
        trackers.Add(newTracker.transform);
    }

    public GameObject AddMapBarrier(Transform obj)
    {
        //trackedObjects.Add(obj);
        GameObject newBarrier = Instantiate(barrierPrefab, miniReference.TransformPoint(obj.position / mapScale), Quaternion.Inverse(obj.rotation) * miniReference.rotation);
        newBarrier.transform.localScale = obj.localScale / mapScale;
        //trackers.Add(newBarrier.transform);
        newBarrier.transform.parent = miniReference;

        return newBarrier;
    }

    public GameObject AddMapBarrier(Vector3 scale, Vector3 position, Quaternion rotation)
    {
        //trackedObjects.Add(obj);
        GameObject newBarrier = Instantiate(barrierPrefab, miniReference.TransformPoint(position / mapScale), Quaternion.Inverse(rotation) * miniReference.rotation);
        newBarrier.transform.localScale = scale / mapScale;
        //trackers.Add(newBarrier.transform);
        newBarrier.transform.parent = miniReference;

        return newBarrier;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
