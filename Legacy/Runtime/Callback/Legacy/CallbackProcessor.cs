using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SoraCore.Constant;
using Object = UnityEngine.Object;

namespace SoraCore.Callback
{
    public class CallbackProcessor
    {
        #region MonoBehaviourHook
        private class MonoBehaviourHook : MonoBehaviour
        {
            public List<CallbackProcessor> instances = new List<CallbackProcessor>();
            void Update()
            {
                foreach (CallbackProcessor instance in instances)
                    instance.Update?.Invoke();
            }

            void FixedUpdate()
            {
                foreach (CallbackProcessor instance in instances)
                    instance.FixedUpdate?.Invoke();
            }
        }

        private static MonoBehaviourHook _processorBehaviour;
        private void _AddToProcessorBehaviour()
        {
            // If not already have one then create
            if (!_processorBehaviour)
            {
                GameObject gameObj = new GameObject("CallbackProcessor");
                _processorBehaviour = gameObj.AddComponent<MonoBehaviourHook>();
            }

            _processorBehaviour.instances.Add(this);
        }
        #endregion

        private int _currentIndex;
        private List<Callback> _callbacks { get; } = new List<Callback>();
        public Action Update { get; private set; }
        public Action FixedUpdate { get; private set; }

        #region Constructors
        #region Default Update Methods
        void _DefaultProcessorUpdate()
        {
            //If the list is empty
            if (_callbacks.Count == 0)
                return;

            Callback currentCallback = _callbacks[_currentIndex];
            //Nothing to process
            if (currentCallback == null)
                return;

            //Hasn't reach callback 'EndTime' yet
            float currentTime;
            switch (currentCallback.TimeType)
            {
                case TimeType.Scaled:
                    currentTime = Time.timeSinceLevelLoad;
                    break;
                case TimeType.Unscaled:
                    currentTime = Time.unscaledTime;
                    break;
                case TimeType.Realtime:
                    currentTime = Time.realtimeSinceStartup;
                    break;
                case TimeType.FixedScaled:
                case TimeType.FixedUnscaled:
                    //Nothing to process
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (currentTime < currentCallback.EndTime)
                return;

            //Invoke callback
            InvokeCurrentCallback();
        }
        void _DefaultProcessorFixedUpdate()
        {
            //If the list is empty
            if (_callbacks.Count == 0)
                return;

            Callback currentCallback = _callbacks[_currentIndex];
            //Nothing to process
            if (currentCallback == null)
                return;

            //Hasn't reach callback 'EndTime' yet
            float currentTime;
            switch (currentCallback.TimeType)
            {
                case TimeType.Scaled:
                case TimeType.Unscaled:
                case TimeType.Realtime:
                    //Nothing to process
                    return;
                case TimeType.FixedScaled:
                    currentTime = Time.fixedTime;
                    break;
                case TimeType.FixedUnscaled:
                    currentTime = Time.fixedUnscaledTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (currentTime < currentCallback.EndTime)
                return;

            //Invoke callback
            InvokeCurrentCallback();
        }
        #endregion
        public CallbackProcessor()
        {
            _AddToProcessorBehaviour();
            Update = _DefaultProcessorUpdate;
            FixedUpdate = _DefaultProcessorFixedUpdate;
        }
        public CallbackProcessor(Action updateMethod, Action fixedUpdateMethod) : base()
        {
            if (updateMethod != null)
                Update = updateMethod;
            if (fixedUpdateMethod != null)
                FixedUpdate = fixedUpdateMethod;
        }
        #endregion

        // TODO: Make this more 'generic'
        public void Add(Callback callback, Object debugContext)
            => Add(callback, true, debugContext);
        public void Add(Callback callback, bool sortAndUpdate = true, Object debugContext = null)
        {


            if (callback.OnCallbackEvent == null)
            {
                Debug.LogWarning(SoraWarning + ": Callback event are null!", debugContext);
                if (debugContext == null)
                    throw new ArgumentNullException();
            }

            _callbacks.Add(callback/* .Clone() */);
            if (sortAndUpdate) SortAndUpdate();
        }
        public void Add(float interval, UnityEvent onCallbackEvent, TimeType timeType = TimeType.Scaled, bool sortAndUpdate = true, Object debugContext = null)
        {
            Add(new Callback(interval, onCallbackEvent, timeType), sortAndUpdate, debugContext);
        }

        public void SortAndUpdate()
        {
            _callbacks.Sort();
            _UpdateCallbackList();
        }

        public void InvokeCurrentCallback()
        {
            _callbacks[_currentIndex].OnCallbackEvent?.Invoke();
            _callbacks.RemoveAt(_currentIndex);
            //Update & get new callback if available
            _UpdateCallbackList();
        }

        private void _UpdateCallbackList()
        {
            //?This should return 0 (remove none)
            _callbacks.RemoveAll((Callback cb) => cb.OnCallbackEvent == null);

            //If the list is empty
            if (_callbacks.Count == 0)
                return;

            //Callbacks[0 -> (Count - 1)]
            //          H <- L (sorted by endTime)
            _callbacks.Sort();

            int lastIndex = _callbacks.Count - 1;

            //If the current callback are null
            if (_callbacks[_currentIndex] == null)
            {
                _callbacks[_currentIndex] = _callbacks[lastIndex];
                _callbacks.RemoveAt(lastIndex);
                return;
            }

            //If the new (endTime) are smaller then set the currentIndex to last
            if (_callbacks[lastIndex].EndTime < _callbacks[_currentIndex].EndTime)
            {
                _currentIndex = lastIndex;
            }
        }
    }
}