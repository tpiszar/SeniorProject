using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineMover : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> offsets = new List<Vector3>();

    public bool active = false;
    public bool offsetsSet = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }
        if (active)
        {
            if (!offsetsSet)
            {
                offsetsSet = true;
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    offsets.Add(lineRenderer.GetPosition(i) - lineRenderer.GetPosition(0));
                    print(offsets[i]);
                }
            }
        }

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, transform.position + offsets[i]);
        }
    }
}
