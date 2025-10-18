using System.Runtime.CompilerServices;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class Vector3X {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithX(this in Vector3 self, float x) {
            return new Vector3(x, self.y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithY(this in Vector3 self, float y) {
            return new Vector3(self.x, y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithZ(this in Vector3 self, float z) {
            return new Vector3(self.x, self.y, z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithXY(this in Vector3 self, float x, float y) {
            return new Vector3(x, y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithXZ(this in Vector3 self, float x, float z) {
            return new Vector3(x, self.y, z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithYZ(this in Vector3 self, float y, float z) {
            return new Vector3(self.x, y, z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetX(this in Vector3 self, float x) {
            return new Vector3(self.x + x, self.y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetY(this in Vector3 self, float y) {
            return new Vector3(self.x, self.y + y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetZ(this in Vector3 self, float z) {
            return new Vector3(self.x, self.y, self.z + z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetXY(this in Vector3 self, float x, float y) {
            return new Vector3(self.x + x, self.y + y, self.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetXZ(this in Vector3 self, float x, float z) {
            return new Vector3(self.x + x, self.y, self.z + z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NewWithOffsetYZ(this in Vector3 self, float y, float z) {
            return new Vector3(self.x, self.y + y, self.z + z);
        }
    }
}