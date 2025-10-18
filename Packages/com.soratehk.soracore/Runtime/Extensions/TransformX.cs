using System.Runtime.CompilerServices;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class TransformX {
        #region Position/Rotation/Scale
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPosition(this Transform transform, in Vector3 position) {
            transform.SetPositionAndRotation(position, transform.rotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetRotation(this Transform transform, in Quaternion rotation) {
            transform.SetPositionAndRotation(transform.position, rotation);
            return transform;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPosition(this Transform transform, in Vector3 position) {
            transform.SetLocalPositionAndRotation(position, transform.localRotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalX(this Transform transform, in float x) {
            transform.SetLocalPositionAndRotation(transform.localPosition.NewWithX(x), transform.localRotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalY(this Transform transform, in float y) {
            transform.SetLocalPositionAndRotation(transform.localPosition.NewWithY(y), transform.localRotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalZ(this Transform transform, in float z) {
            transform.SetLocalPositionAndRotation(transform.localPosition.NewWithZ(z), transform.localRotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalRotation(this Transform transform, in Quaternion rotation) {
            transform.SetLocalPositionAndRotation(transform.localPosition, rotation);
            return transform;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScale(this Transform transform, in Vector3 scale) {
            transform.localScale = scale;
            return transform;
        }
        #endregion
    }
}