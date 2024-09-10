using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using Unity.VisualScripting;
using System.IO;

public class CompareGesture : MonoBehaviour
{
    //public Transform[] positions;
    public Transform path;

    private List<Point> points = new List<Point>();
    private int strokeId = 0;

    private List<Gesture> trainingSet = new List<Gesture>();

    private string message;
    private bool recognized;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < path.childCount; i++)
        {
            Vector3 camPoint = Camera.main.transform.InverseTransformPoint(path.GetChild(i).position);
            points.Add(new Point(camPoint.x, camPoint.y, camPoint.z, strokeId));
        }

        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        Gesture candidate = new Gesture(points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

        message = gestureResult.GestureClass + " " + gestureResult.Score;

        print(message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
