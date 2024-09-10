using PDollarGestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureControl : MonoBehaviour
{
    public bool Creating = false;

    public Transform GestureParent;

    private List<Point> points = new List<Point>();
    private int strokeId = 0;

    public string newGestureName = "";

    private List<Gesture> trainingSet = new List<Gesture>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GestureParent.childCount; i++)
        {
            //-pos.position.y ?
            Vector3 camPoint = Camera.main.transform.InverseTransformPoint(GestureParent.GetChild(i).position);
            points.Add(new Point(camPoint.x, camPoint.y, camPoint.z, strokeId));
        }

        if (Creating)
        {
            string fileName = String.Format("{0}/{1}-{2}.xml", "Assets/PDollar/Resources/Gestures", newGestureName, DateTime.Now.ToFileTime());

            #if !UNITY_WEBPLAYER
                GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
            #endif

            print(fileName);
        }
        else
        {
            TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
            foreach (TextAsset gestureXml in gesturesXml)
                trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            print(gestureResult.GestureClass + " " + gestureResult.Score);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
