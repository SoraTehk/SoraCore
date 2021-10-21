using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using MyBox;
using Sora.Extension;

namespace Sora {
    public class UIValueLink : MonoBehaviour
    {
        public MinMaxFloat minMax;
        [ReadOnly] public float linkedValue;
        [Space(20)]
        public Slider slider;
        public TMP_InputField inputField;
        public UnityEvent<float> OnValueApplied;

        void Awake() {
            transform.Find("Slider").GetComponentNullCheck<Slider>(ref slider, transform);
            transform.Find("InputField (TMP)").GetComponentNullCheck<TMP_InputField>(ref inputField, transform);
        }

        void Start() {
            
        }

        #region UpdateValue (Dynamic methods for UnityEevent)
        public void UpdateValue(float value) {
            linkedValue = Mathf.Clamp(value, minMax.Min, minMax.Max);
        }
        public void UpdateValue(string text) {
            linkedValue =  Mathf.Clamp(System.Convert.ToSingle(text), minMax.Min, minMax.Max);
        }
        #endregion

        public void ApplyValue() {
            slider.value = linkedValue;
            inputField.text = linkedValue.ToString();
            OnValueApplied?.Invoke(linkedValue);
        }
    }
}