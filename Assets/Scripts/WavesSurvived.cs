using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WavesSurvived : MonoBehaviour
{
    public TextMeshProUGUI results;

    private void OnEnable()
    {
        results.text = "You Survived " + WaveManager.Instance.curWave.ToString() + " Waves";
    }
}
