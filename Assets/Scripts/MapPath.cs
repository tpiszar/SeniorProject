using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPath : MonoBehaviour
{
    public static MapPath Instance;

    public LineRenderer[] paths;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetClosestPoint(Vector3 targetPosition)//, out Quaternion targetRotation)
    {
        float minDistance = Mathf.Infinity;
        Vector3 closestPoint = targetPosition;

        //targetRotation = Quaternion.identity;

        foreach (LineRenderer path in paths)
        {
            int pointCount = path.positionCount;
            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector3 pointA = path.GetPosition(i);
                Vector3 pointB = path.GetPosition(i + 1);

                Vector3 closestPointOnSegment = ClosestPointOnLine(pointA, pointB, targetPosition);

                //targetRotation = Quaternion.FromToRotation(Vector3.forward, pointB - pointA);

                // Calculate the distance to the target position
                float distance = Vector3.Distance(targetPosition, closestPointOnSegment);

                // Update if this is the closest we've found so far
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closestPointOnSegment;
                }
            }
        }

        return closestPoint;
    }

    Vector3 ClosestPointOnLine(Vector3 pointA, Vector3 pointB, Vector3 targetPoint)
    {
        Vector3 ab = pointB - pointA;
        float t = Vector3.Dot(targetPoint - pointA, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t); // Ensure t is within [0, 1] so the point is on the segment
        return pointA + t * ab;
    }
}
