using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Concurrent;
using System;
using System.Text.RegularExpressions;

using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

using static Sora.Constant;

namespace Sora {
    public static class Constant {
        public const char CHAR_SYNCHRONOUS_IDLE = '▬';
        public const string SORA_NULL = "<b><color=red>[Sora/Null]</red></b>";
        public const string SORA_LOG = "<b><color=lime>[Sora/Info]</color></b>";
        public const string SORA_WARNING = "<b><color=yellow>[Sora/Warning]</color></b>";
    }

    public static class Util {
        /// <summary>
        /// Simple string builder pool with ConcurrentBag<T>
        /// </summary>
        public static class StringBuilderPool {
            const int INITIALIZE_SIZE = 1;
            const int CAPACITY = 512;
            static ConcurrentBag<StringBuilder> poolBag;

            static StringBuilderPool() {
                Reset();
            }
            public static void Reset() {
                poolBag = new ConcurrentBag<StringBuilder>();
                for (int i = 0; i < INITIALIZE_SIZE; i++) {
                    poolBag.Add(new StringBuilder(CAPACITY));
                }
            }

            public static StringBuilder GetFromPool() {
                StringBuilder sb;

                if(!poolBag.TryTake(out sb)) {
                    sb = new StringBuilder(CAPACITY);
                }

                return sb;
            }
            public static void AddToPool(StringBuilder sb) {
                sb.Clear();
                poolBag.Add(sb);
            }
        }

        /// <summary>
        /// Translate an angle to a vector (X-axis)
        /// </summary>
        public static Vector2 AngleToVector2D(float angle, bool isRadian = false) {
            float radians = isRadian ? angle
                                    : angle * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        /// <sumary>
        /// Translate a vector to an angle (X-axis)
        /// </summary>
        public static float VectorToAngle2D(Vector2 dir, bool isRadian = false) {
            float radians = Mathf.Atan2(dir.y, dir.x);

            return isRadian ? radians
                            : radians * Mathf.Rad2Deg;
        }

        /// <sumary>
        /// Apply an angle to a vector (X-axis)
        /// </summary>
        public static Vector2 ApplyAngleToVector2D(Vector2 origin, float angle, bool isRadian = false) {
            float degrees = isRadian ? angle * Mathf.Rad2Deg
                                    : angle;
            
            Vector2 dirVector = AngleToVector2D(VectorToAngle2D(origin) + degrees);
            return dirVector * origin.magnitude;
        }

        ///<summary>
        /// If x,y = 0 then keeping the rb current velocity value
        ///</summary>
        public static void SetVelocity2D(ref Rigidbody2D rb, Vector2 value) {
            float newX = value.x == 0 ? rb.velocity.x : value.x;
            float newY = value.y == 0 ? rb.velocity.y : value.y;

            rb.velocity = new Vector2(newX, newY);
        }

        ///<summary>
        /// Return a vector from point a to b;
        ///</summary>
        public static Vector2 GetVectorFromPoint(Vector2 a, Vector2 b) {
            return new Vector2(b.x - a.x, b.y - a.y);
        }

        /// <summary>
        /// Get direction float
        /// Left = -1
        /// Right = 1
        /// Else = 0
        /// </summary>
        public static float GetXDirFloatByRot(Transform transform) {
            return transform.eulerAngles.y % 360 == 0 ? 1f
                : (transform.eulerAngles.y + 180) % 360 == 0 ? -1
                : 0; //This line shouldn't be happened
        }
    }
    namespace Extension {
        using static Sora.Util;
        public enum ReturnState {
            TransformNull,
            Succeed,
            Null,
        }
        public static class ExtensionMethod {
            private static System.Random _random = new System.Random();
            ///<summary>
            /// Shuffle this list
            ///</summary>
            public static void Shuffle<T>(this IList<T> list)  
            {  
                int n = list.Count;  
                while (n > 1) {  
                    n--;  
                    int k = _random.Next(n + 1);  
                    T value = list[k];  
                    list[k] = list[n];  
                    list[n] = value;  
                }  
            }
            
            #region GetComponent(s)NullCheck
            //TODO: Learning stacktrace & rework this?
            ///<summary>
            /// GetComponent<T> but with null checking.
            ///</summary>
            public static ReturnState GetComponentNullCheck<T>(this Transform transform, ref T result, Object debugContext = null) 
                                                      where T : class {
                T[] results = null;
                ReturnState returnState = transform.GetComponentsNullCheck<T>(ref results, debugContext);
                result = results[0];
                return returnState;
            }
            ///<summary>
            /// GetComponents<T> but with null checking.
            ///</summary>
            public static ReturnState GetComponentsNullCheck<T>(this Transform transform, ref T[] results, Object debugContext = null)
                                                       where T : class {
                //Local variable for usage
                StringBuilder sb;

                //If the transfrom are null then return
                if(transform == null) {
                    sb = StringBuilderPool.GetFromPool();
                    sb.Append(SORA_NULL).Append(": <b>transform</b> are null!");
                    sb.AppendLine();
                    if(!debugContext) {
                        sb.Append(SORA_NULL).Append(": <b>debugContext</b> parameter were't declared in method call!");
                        Debug.LogWarning(sb);
                    }
                    sb.AppendLine();
                    sb.Append("<i> Method: <b>transform</b>.GetComponentNullCheck<T>(ref T input, bool @override = false, Object debugContext = null)</i>");
                    Debug.LogWarning(sb, debugContext);
                    StringBuilderPool.AddToPool(sb);
                    return ReturnState.TransformNull;
                }

                //If successfully assigned then return
                T[] resultsFromGet = transform.GetComponents<T>();
                if(resultsFromGet.Length > 0) {
                    results = resultsFromGet;
                    return ReturnState.Succeed;
                }

                //Else log a warning
                sb = StringBuilderPool.GetFromPool();
                sb.Append(SORA_NULL).Append(": Cant find any <b>");
                sb.Append(typeof(T).Name).Append("</b>(type) in <b>");
                sb.Append(transform.name).Append("</b>.");
                sb.AppendLine().AppendLine();
                sb.Append("<b> Method: GetComponentsNullCheck<T>(this Transform transform, ref T[] results, Object debugContext = null)</b>");
                Debug.LogWarning(sb, transform);
                StringBuilderPool.AddToPool(sb);

                return ReturnState.Null;
            }
            #endregion
            
            ///<summary>
            /// CompareTag but works with enum
            ///</summary>
            public static bool CompareTag(this GameObject gameObject, Enum enumTag)
                => gameObject.CompareTag(enumTag.ToString());

#if UNITY_EDITOR
            private const string REGEX_PATTERN_SCENE_NAME = @".+\/(.+)\.unity";
            ///<summary>
            /// Get the name base on path with regex
            ///</summary>
            public static string GetName(this EditorBuildSettingsScene editorBuildSettingsScene)
                => Regex.Match(editorBuildSettingsScene.path, REGEX_PATTERN_SCENE_NAME).Groups[1].Value;
#endif
        }
    }
}