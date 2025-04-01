using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BookSmoother : MonoBehaviour
{
    Transform parent;
    bool isActive = false;

    public float maxDistance;
    public float maxAngle;

    public float moveFactor;
    public float rotateFactor;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //print("ANGLE: " + Quaternion.Angle(transform.rotation, parent.rotation) + " DIST: " + Vector3.Distance(transform.position, parent.position));

            if (Vector3.Distance(transform.position, parent.position) > maxDistance)
            {
                transform.position = parent.position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, parent.position, moveFactor * Time.unscaledDeltaTime);
            }

            if (Quaternion.Angle(transform.rotation, parent.rotation) > maxAngle)
            {
                transform.rotation = parent.rotation;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, parent.rotation, rotateFactor * Time.unscaledDeltaTime);
            }
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
