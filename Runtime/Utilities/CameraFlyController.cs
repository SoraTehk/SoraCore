using MyBox;
using SoraCore.Extension;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoraCore {
    public class CameraFlyController : MonoBehaviour {
        #region InteropServices

        /// <summary>
        /// https://www.pinvoke.net/default.aspx/user32.SetCursorPos
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/user32.GetCursorPos
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// http://www.pinvoke.net/default.aspx/Structures/POINT.html
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT {
            public int X;
            public int Y;

            public POINT(int x, int y) {
                X = x;
                Y = y;
            }

            //Conversions
            public static implicit operator System.Drawing.Point(POINT p) => new(p.X, p.Y);
            public static implicit operator POINT(System.Drawing.Point p) => new(p.X, p.Y);
            public static implicit operator Vector2(POINT p) => new(p.X, p.Y);
            // Subtracts one from another
            public static POINT operator -(POINT a, POINT b) => new(a.X - b.X, a.Y - b.Y);
        }

        #endregion

        [SerializeField] private float _moveSpeed = 10f;
        [MinMaxRange(1f, 10f)]
        [SerializeField] private RangedFloat _sprintRampingRange = new(3f, 10f);
        [SerializeField] private float _sprintRampingSpeed = 10f;
        [Range(0.1f, 1f)]
        [SerializeField] private float _cameraSensitivity = 0.4f;

        // Moving
        private Vector3 _axesInput;
        private bool _isSprinting;
        private float _sprintingTime;

        // Camera dragging
        private bool _isPanning;
        private POINT _cursorOrigin;
        private Quaternion _initLocalRot;
        private bool _lastFrameWasPanning;

        public InputActionMap ActionMap;
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _panAction;

        private void Awake() {
            ActionMap = new InputActionMap();

            _moveAction = ActionMap.AddAction("Move", InputActionType.Value);
            _moveAction.AddCompositeBinding("3DVector")
                .With("Up", "<Keyboard>/e")
                .With("Down", "<Keyboard>/q")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Forward", "<Keyboard>/w")
                .With("Backward", "<Keyboard>/s");

            _sprintAction = ActionMap.AddAction("Sprint", InputActionType.Button);
            _sprintAction.AddBinding("<Keyboard>/shift");

            _panAction = ActionMap.AddAction("Pan", InputActionType.Button);
            _panAction.AddBinding("<Mouse>/rightButton");

            ActionMap.Enable();
        }

        private void Update() {
            _axesInput = _moveAction.ReadValue<Vector3>();
            _isSprinting = _sprintAction.IsPressed();
            _isPanning = _panAction.IsPressed();
        }

        private void FixedUpdate() {
            float sprintMul = 1;
            // Reset sprinting time when the player is not sprinting
            if (_isSprinting) {
                _sprintingTime += Time.fixedDeltaTime;
                float sprintingTimeNormalized = Mathf.Clamp01(_sprintingTime / _sprintRampingSpeed);
                // Calculate sprint multiplier
                sprintMul = _isSprinting ? Mathf.Lerp(_sprintRampingRange.Min,
                                                      _sprintRampingRange.Max,
                                                      sprintingTimeNormalized) : 1;
            }
            else {
                _sprintingTime = 0;
            }

            // Apply translation
            transform.Translate(_moveSpeed * sprintMul * Time.fixedDeltaTime * _axesInput, Space.Self);

            // Camera panning when holding right-click
            if (_isPanning) {
                // Cursor.visible = false;
                _lastFrameWasPanning = true;

                // Cursor offset from origin
                GetCursorPos(out POINT currentCursorPos);
                POINT offset = currentCursorPos - _cursorOrigin;

                // Calculate x, y rotations
                Quaternion xQuat = Quaternion.AngleAxis(offset.X * _cameraSensitivity, Vector3.up);
                Quaternion yQuat = Quaternion.AngleAxis(offset.Y * _cameraSensitivity, Vector3.right);

                // Zero-ing out Z axis
                Vector3 outputEulerAngles = (_initLocalRot * (xQuat * yQuat)).eulerAngles;
                transform.rotation = Quaternion.Euler(outputEulerAngles.x, outputEulerAngles.y, 0);
            }
            else {
                //Cursor.visible = true;
                if (_lastFrameWasPanning) {
                    SetCursorPos(_cursorOrigin.X, _cursorOrigin.Y);
                    _lastFrameWasPanning = false;
                }
                else {
                    GetCursorPos(out _cursorOrigin);
                }

                _initLocalRot = transform.rotation;
            }
        }
    }
}