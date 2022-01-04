using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using SoraCore.Extension;

namespace SoraCore.Callback {
    public enum TimeType {
        Scaled,
        Unscaled,
        FixedScaled,
        FixedUnscaled,
        Realtime
    }

    public class Callback : IComparable<Callback>, IDeepClone<Callback>
    {
        public float Interval { get; }
        public UnityEvent OnCallbackEvent { get; }
        public TimeType TimeType { get; }
        public float StartTime { get; }
        public float EndTime { get; }

        #region Constructors
        public Callback(float interval, UnityEvent onCallbackEvent, TimeType timeType = TimeType.Scaled) {
            this.Interval = interval;
            this.OnCallbackEvent = onCallbackEvent;

            this.TimeType = timeType;
            switch(TimeType) {
                case TimeType.Scaled:
                    StartTime = Time.timeSinceLevelLoad;
                    break;
                case TimeType.Unscaled:
                    StartTime = Time.unscaledTime;
                    break;
                case TimeType.FixedScaled:
                    StartTime = Time.fixedDeltaTime;
                    break;
                case TimeType.FixedUnscaled:
                    StartTime = Time.fixedUnscaledTime;
                    break;
                case TimeType.Realtime:
                    StartTime = Time.realtimeSinceStartup;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EndTime = StartTime + interval;
        }
        #endregion

        #region IComparable
        // Sorted by descending order where null (event)     will be at 0    index,
        //                                  lowest 'EndTime' will be at last index
        public int CompareTo(Callback other) {
            //?This shouldn't happen in CallbackProcessor
            if(other == null)
                return 1;

            return other.EndTime.CompareTo(EndTime);
        }

        public Callback Clone()
            => new Callback(Interval, OnCallbackEvent, TimeType);
        #endregion
    }

}