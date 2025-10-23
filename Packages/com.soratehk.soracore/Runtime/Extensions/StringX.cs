using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace SoraTehk.Extensions {
    public static partial class StringX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RTColorByHash(this string str) {
            if (string.IsNullOrEmpty(str)) return str;
            
            int hash = str.GetHashCode();
            
            byte r = (byte)((hash >> 16) & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)(hash & 0xFF);
            
            return ZString.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", r, g, b, str);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RTColorByHash(this string str, string color) {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(color)) return str;
            
            int hash = color.GetHashCode();
            
            byte r = (byte)((hash >> 16) & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)(hash & 0xFF);
            
            return ZString.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", r, g, b, str);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFriendlyKey(this string key) {
#if UNITY_EDITOR
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            return settings.FindAssetEntry(key)?.address ?? key;
#else
            return key;
#endif
        }
    }
}