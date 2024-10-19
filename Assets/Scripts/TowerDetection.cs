using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerDetection : MonoBehaviour
{
    public DetectionType detectionType = DetectionType.Close;

    public abstract Transform GetTarget();
}
