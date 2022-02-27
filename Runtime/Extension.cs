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
        public static T GetComponentNullCheck<T>(this Component transform, Object debugContext = null) where T : Component
            => transform.GetComponentsNullCheck<T>(debugContext)[0];


        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but with null checking
        /// </summary>
        public static T[] GetComponentsNullCheck<T>(this Component transform, Object debugContext = null) where T : Component {
            // If the transfrom are null then return
            if (!transform) {
                Debug.LogError($"{SORA_NULL}: <b>transform</b> are null. \n\n", debugContext);
                return null;
            }

            // If successfully assigned then return
            T[] components = transform.GetComponents<T>();
            if (components.Length > 0) return components;

            // Cant find any T in transform
            Debug.LogError($"{SORA_NULL}: Cant find any <b>{typeof(T).Name}</b>(type) in <b>{transform.name}</b>.", transform);
            return null;
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