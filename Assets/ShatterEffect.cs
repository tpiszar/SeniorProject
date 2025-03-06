using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class ShatterEffect : MonoBehaviour
{
    public float duration;
    float timer = 0;

    Vector3 startScale;
    public Vector3 endScale;

    Vector3 childStartScale;

    List<Renderer> rends = new List<Renderer>();
    Material setMat;
    Color baseColor;

    public bool overrideAlpha = false;
    public float baseAlpha;

    public AnimationCurve easeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;

        childStartScale = transform.GetChild(0).localScale;

        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer rend = transform.GetChild(i).GetComponent<Renderer>();
            if (rend != null)
            {
                rends.Add(rend);
            }
        }
        setMat = rends[0].material;
        baseColor = rends[0].material.color;
        
        if (!overrideAlpha)
        {
            baseAlpha = baseColor.a;
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        t = Mathf.Clamp01(t);

        float easedT = easeOutCurve.Evaluate(t);

        Vector3 newScale = Vector3.Lerp(startScale, endScale, easedT);
        transform.localScale = newScale;

        RescaleChildren();

        baseColor.a = Mathf.Lerp(baseAlpha, 0, easedT);
        setMat.color = baseColor;

        foreach (Renderer r in rends)
        {
            r.material = setMat;
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void RescaleChildren()
    {
        Vector3 inverseScale = new Vector3(
            startScale.x / transform.localScale.x,
            startScale.y / transform.localScale.y,
            startScale.z / transform.localScale.z
        );

        // Apply inverse scale to children
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localScale = new Vector3(
                childStartScale.x * inverseScale.x,
                childStartScale.y * inverseScale.y,
                childStartScale.z * inverseScale.z
            );
        }
    }
}
