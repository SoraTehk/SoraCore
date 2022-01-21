using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EC_Bool", menuName = "SoraCore/Events/Bool")]
public class BoolEventChannelSO : ScriptableObject
{
    public event UnityAction<bool> EventRequested;
    public void Raise(bool value) {
        if (EventRequested != null)
        {
            EventRequested.Invoke(value);
        } else
        {
            Debug.LogWarning("An <b>bool event</b> was requested, but nobody picked it up");
        }
    }
}
