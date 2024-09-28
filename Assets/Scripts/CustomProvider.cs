using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomProvider : LocomotionProvider
{
    public void setLocomotion(LocomotionPhase phase)
    {
        locomotionPhase = phase;
    }

    protected override void Awake()
    {
        locomotionPhase = LocomotionPhase.Idle;
    }
}
