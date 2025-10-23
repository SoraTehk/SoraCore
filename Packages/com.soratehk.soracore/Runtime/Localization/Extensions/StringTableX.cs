using System.Runtime.CompilerServices;
using UnityEngine.Localization.Tables;

namespace SoraTehk.Extensions {
    public static class StringTableX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetEntry(this StringTable strTbl, string key, out StringTableEntry? entry) {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            entry = strTbl.GetEntry(key) ?? null;
            return entry != null;
        }
    }
}