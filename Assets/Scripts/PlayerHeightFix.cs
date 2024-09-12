using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
///     Workaround for this issue https://discussions.unity.com/t/xr-rig-tracking-origin-mode-floor-doesnt-align-after-restart/336353
///     This issue seemed to happen only when putting the headset after the Editor started to play
/// </summary>
public class XROriginFloorFix : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;

    private bool isInitialized;

    private void Update()
    {
        if (IsHardwarePresent() && !isInitialized)
        {
            StartCoroutine(InitializeCoroutine());
            isInitialized = true;
        }
    }

    public static bool IsHardwarePresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);

        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator InitializeCoroutine()
    {
        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
        yield return new WaitForSeconds(0.1f);
        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
    }
}