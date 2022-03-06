using static SoraCore.Constant;

namespace SoraCore.Extension {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

    public static partial class Extension {
        private static readonly System.Random _random = new();
        ///<summary>
        /// Shuffle this list
        ///</summary>
        public static void Shuffle<T>(this IList<T> list) {
            for (int i = list.Count; i > 1; i--) {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// <see cref="Component.GetComponent{T}()"/> but with null checking
        /// </summary>
        public static T GetComponentNullCheck<T>(this Component transform, Object debugContext = null) where T : Component
            => transform.GetComponentsNullCheck<T>(debugContext)[0];


        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but with null checking
        /// </summary>
        public static T[] GetComponentsNullCheck<T>(this Component transform, Object debugContext = null) where T : Component {
            // If the transfrom are null then return
            if (!transform) {
                Debug.LogError($"{SoraNull}: <b>transform</b> are null.", debugContext);
                return null;
            }

            // If successfully assigned then return
            T[] components = transform.GetComponents<T>();
            if (components.Length > 0) return components;

            // Cant find any T in transform
            Debug.LogError($"{SoraNull}: Cant find any <b>{typeof(T).Name}</b>(type) in <b>{transform.name}</b>.", transform);
            return null;
        }

        ///<summary>
        /// CompareTag but works with enum
        ///</summary>
        public static bool CompareTag(this GameObject gameObject, Enum enumTag)
            => gameObject.CompareTag(enumTag.ToString());

        public static string Bold(this string str) => $"<b>{str}</b>";
        public static string Italic(this string str) => $"<i>{str}</i>";
        public static string Color(this string str, string color) => $"<color={color}>{str}</color>";
        public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
    }
}