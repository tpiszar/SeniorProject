using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightningDrawer : MonoBehaviour
{
    public LineRenderer[] lightningRenders;
    public ParticleSystem[] lightningParticles;
    public static float lightningSpikesPerUnit = 2;
    public static float lightningSpikeOffset = 0.2f;
    public static float lightningSpikeOffsetMax = 0.3f;

    public AudioClip zapSound;
    [Range(0.0001f, 1f)]
    public float zapVolume = 1;

    public GameObject hitParticle;

    private void Start()
    {
        foreach(ParticleSystem particle in lightningParticles)
        {
            particle.transform.parent = null;
        }
    }


    public void Draw(Vector3 posA, Vector3 posB, int lineNum, float disableDelay)
    {
        SoundManager.instance.PlayClip(zapSound, posA, zapVolume);

        StartCoroutine(DrawLightning(posA, posB, lightningRenders[lineNum], lightningParticles[lineNum], disableDelay));
    }

    IEnumerator DrawLightning(Vector3 posA, Vector3 posB, LineRenderer lightningRender, ParticleSystem lightningParticle, float disableDelay)
    {
        float distance = Vector3.Distance(posA, posB);
        int numSegments = Mathf.CeilToInt(distance * lightningSpikesPerUnit);

        lightningRender.positionCount = numSegments + 2;

        lightningRender.SetPosition(0, posA);

        for (int i = 1; i <= numSegments; i++)
        {
            float t = (float)i / (numSegments + 1);
            Vector3 interpolatedPos = Vector3.Lerp(posA, posB, t);

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset),
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset),
                UnityEngine.Random.Range(-lightningSpikeOffset, lightningSpikeOffset)
            );

            lightningRender.SetPosition(i, interpolatedPos + randomOffset);
        }
        lightningRender.SetPosition(lightningRender.positionCount - 1, posB);

        lightningRender.enabled = true;

        lightningParticle.transform.position = posB;
        lightningParticle.Play();

        yield return new WaitForSeconds(disableDelay);

        lightningRender.enabled = false;
    }

    private void OnDestroy()
    {
        foreach (LineRenderer line in lightningRenders)
        {
            if (line)
            {
                line.enabled = false;
            }
        }
    }
}
