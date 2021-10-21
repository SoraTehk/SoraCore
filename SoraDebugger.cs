using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sora {
    public class SoraDebugger : MonoBehaviour
    {
        public Canvas prefab;
        private Canvas _debugCanvas;
        private TextMeshProUGUI _tmpUGUI;
        private string _displayText;

        public int lineOffset;
        public Vector2 xyOffset;

        public bool animatorState;
        private Animator _animator;

        public bool position;

        private void Awake() {
            _debugCanvas = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            float newXScale = _debugCanvas.transform.localScale.x/transform.localScale.x;
            float newYScale = _debugCanvas.transform.localScale.y/transform.localScale.y;
            float newZScale = _debugCanvas.transform.localScale.z/transform.localScale.z;
            _debugCanvas.transform.localScale = new Vector3(newXScale, newYScale, newZScale);
            _debugCanvas.transform.position = new Vector3(  _debugCanvas.transform.position.x + xyOffset.x,
                                                            _debugCanvas.transform.position.y + xyOffset.y,
                                                            0);
            
            _tmpUGUI = _debugCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();            
            _tmpUGUI.text = "";

            _animator = GetComponent<Animator>();
        }

        private void Update() {
            _displayText = "";
            for(int i = 0; i < lineOffset; i++) {
                _displayText += "\n";
            }

            if(animatorState) {

                _displayText += _animator.GetCurrentAnimatorClipInfo(0).Length > 0 ?
                                _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + "\n" :
                                "null";
            }

            if(position) {
                _displayText += "X:" + transform.position.x + "\n";
                _displayText += "Y:" + transform.position.y + "\n";
                _displayText += "Z:" + transform.position.z + "\n";  
            }

            _tmpUGUI.text = _displayText;
        }

        private void LateUpdate() {
            _debugCanvas.transform.LookAt(Camera.main.transform);
            _debugCanvas.transform.Rotate(0, 180, 0);
        }
    }
}
