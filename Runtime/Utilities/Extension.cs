namespace SoraCore.Extension
{
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static partial class Extension
    {
        /// <summary>
        /// <see cref="Component.GetComponent{T}()"/> but with null checking
        /// </summary>
        public static T GetComponentNullCheck<T>(this Component transform, Object debugContext = null) where T : Component
            => transform.GetComponentsNullCheck<T>(debugContext)[0];


        /// <summary>
        /// <see cref="Component.GetComponents{T}()"/> but with null checking
        /// </summary>
        public static T[] GetComponentsNullCheck<T>(this Component transform, Object debugContext = null) where T : Component
        {
            // If the transfrom are null then return
            if (!transform)
            {
                SoraCore.LogNull("<b>transform</b> are null.", nameof(transform), debugContext);
                return null;
            }

            // If successfully assigned then return
            T[] components = transform.GetComponents<T>();
            if (components.Length > 0) return components;

            // Cant find any T in transform
            string s1 = typeof(T).Name.Bold();
            string s2 = transform.name.Bold();
            SoraCore.LogNull($"Cant find any {s1}(type) in {s2}.", nameof(transform), transform);
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