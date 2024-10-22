using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class VibrationProfile : MonoBehaviour
{
    [System.Serializable]
    public class Vibration
    {
        public float delay;
        [Range(0, 1)]
        public float intensity;
        public float duration;
    }

    public string Name;
    public Vibration[] vibrations;

    public VibrationProfile(Vibration[] a_vibrations)
    {
        vibrations = a_vibrations;
    }

    public void Play(XRBaseController controller)
    {
        if (!controller) { return; }

        StartCoroutine(PlaySequence(controller));
    }

    IEnumerator PlaySequence(XRBaseController controller)
    {
        foreach (Vibration v in vibrations)
        {
            yield return new WaitForSeconds(v.delay);

            controller.SendHapticImpulse(v.intensity, v.duration);
        }
    }
}
