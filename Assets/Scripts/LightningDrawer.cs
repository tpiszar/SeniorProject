using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningDrawer : MonoBehaviour
{
    public LineRenderer[] lightningRenders;
    public static float lightningSpikesPerUnit = 3;
    public static float lightningSpikeOffset = 0.2f;
    public static float lightningSpikeOffsetMax = 0.2f;

    public void Draw(Vector3 posA, Vector3 posB, int lineNum, float disableDelay)
    {
        StartCoroutine(DrawLightning(posA, posB, lightningRenders[lineNum], disableDelay));
    }

    IEnumerator DrawLightning(Vector3 posA, Vector3 posB, LineRenderer lightningRender, float disableDelay)
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
