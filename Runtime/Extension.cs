using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using static SoraCore.Constant;

namespace SoraCore.Extension {
    public static class ExtensionClass {
        private static readonly System.Random _random = new();
        ///<summary>
        /// Shuffle this list
        ///</summary>
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public enum ReturnState {
            TransformNull,
            Succeed,
            Null
        }

        //TODO: Learning stacktrace & rework this
        /// <summary>
        /// <see cref="Component.GetComponent{T}()"/> but with null checking
        /// </summary>
        /// <returns><see cref="ReturnState.TransformNull"/> | <see cref="ReturnState.Succeed"/> | <see cref="ReturnState.Null"/></returns>
        public static ReturnState GetComponentNullCheck<T>(this Component transform, ref T result, Object debugContext = null)
                                                  where T : Component {
            T[] results = null;
            ReturnState returnState = transform.GetComponentsNullCheck<T>(ref results, debugContext);
            result = results[0];
            return returnState;
        }

        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but with null checking
        /// </summary>
        /// <returns><see cref="ReturnState.TransformNull"/> | <see cref="ReturnState.Succeed"/> | <see cref="ReturnState.Null"/></returns>
        public static ReturnState GetComponentsNullCheck<T>(this Component transform, ref T[] results, Object debugContext = null)
                                                   where T : Component {
            // Declare Local variable
            StringBuilder sb;

            // If the transfrom are null then return
            if (transform == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                sb = StringBuilderPool.Get();
                sb.Append(SORA_NULL).Append(": <b>transform</b> are null.");
                sb.AppendLine().AppendLine();
                // Does this call have debugContext?
                if (!debugContext) sb.Append(SORA_NULL).Append(": <b>debugContext</b> parameter " + 
                                                               "were't declared in method call.");
                Debug.LogError(sb, debugContext);
                StringBuilderPool.Return(sb);
#endif
                return ReturnState.TransformNull;
            }

            // If successfully assigned then return
            T[] components = transform.GetComponents<T>();
            if (components.Length > 0)
            {
                results = components;
                return ReturnState.Succeed;
            }

            // Cant find any T in transform
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            sb = StringBuilderPool.Get();
            sb.Append(SORA_NULL).Append(": Cant find any <b>");
            sb.Append(typeof(T).Name).Append("</b>(type) in <b>");
            sb.Append(transform.name).Append("</b>.");
            Debug.LogError(sb, transform);
            StringBuilderPool.Return(sb);
#endif
            return ReturnState.Null;
        }

        ///<summary>
        /// CompareTag but works with enum
        ///</summary>
        public static bool CompareTag(this GameObject gameObject, Enum enumTag)
            => gameObject.CompareTag(enumTag.ToString());


#if UNITY_EDITOR
        private const string REGEX_PATTERN_SCENE_NAME = @".+\/(.+)\.unity";
        ///<summary>
        /// Get the scene name base on path with regex
        ///</summary>
        public static string GetName(this EditorBuildSettingsScene editorBuildSettingsScene)
            => Regex.Match(editorBuildSettingsScene.path, REGEX_PATTERN_SCENE_NAME).Groups[1].Value;
#endif
    }
}