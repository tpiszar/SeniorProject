using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System;

public class CreateGesture : MonoBehaviour
{
    //[SerializeField]
    //public Transform[] positions;
    public Transform path;

    private List<Point> points = new List<Point>();
    private int strokeId = 0;

    public string newGestureName = "";

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < path.childCount; i++)
        {
            Vector3 camPoint = Camera.main.transform.InverseTransformPoint(path.GetChild(i).position);
            points.Add(new Point(camPoint.x, camPoint.y, camPoint.z, strokeId));
            print(camPoint + " " + path.GetChild(i).name);
        }
        //foreach (Transform pos in positions)
        //{
        //    //-pos.position.y ?
        //    Vector3 camPoint = Camera.main.transform.InverseTransformPoint(pos.position);
        //    points.Add(new Point(camPoint.x, camPoint.y, camPoint.z, strokeId));
        //    //print(camPoint);
        //}

        string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/Gestures", newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
        GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

        print(fileName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
