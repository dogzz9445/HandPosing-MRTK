﻿using System;
using UnityEngine;

namespace HandPosing.Interaction
{
    public interface IGrabNotifier 
    {
        Action<GameObject> OnGrabStarted { get; set; }
        Action<GameObject, float> OnGrabAttemp { get; set; }
        Action<GameObject> OnGrabEnded { get; set; }

        Vector2 GrabFlexThresold { get; }
        float CurrentFlex();

        Snappable FindClosestSnappable();
    }
}