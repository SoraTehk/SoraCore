using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SoraTehk.Extensions {
    public static partial class CollectionX {
        #region Shuffle
        /// <summary>
        /// Shuffles the elements of a list in-place using the Fisher–Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list) {
            if (list == null) throw new ArgumentNullException(nameof(list));
            
            for (int i = list.Count - 1; i > 0; i--) {
                int j = Global.GRandom.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        #endregion
        
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection) {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            
            foreach (var item in collection) {
                queue.Enqueue(item);
            }
        }
        
        public static IEnumerable<TValue> GetRangeBy<TKey, TValue>(this SortedList<TKey, TValue> sortedList,
            TKey lowerBound,
            TKey upperBound
        ) where TKey : IComparable<TKey> {
            //
            if (sortedList.Count == 0) yield break;
            var keys = sortedList.Keys;
            
            // Start
            int startIndex = BinarySearchFirst(keys, lowerBound);
            if (startIndex >= keys.Count) yield break;
            // End
            int endIndex = BinarySearchLast(keys, upperBound);
            // Enumerator
            for (int i = startIndex; i <= endIndex; i++) {
                yield return sortedList.Values[i];
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearchFirst<T>(IList<T> sortedList, T value) where T : IComparable<T> {
            int low = 0;
            int high = sortedList.Count - 1;
            
            while (low <= high) {
                int mid = low + ((high - low) >> 1);
                if (sortedList[mid].CompareTo(value) < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            
            return low;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearchLast<T>(IList<T> sortedList, T value) where T : IComparable<T> {
            int low = 0;
            int high = sortedList.Count - 1;
            
            while (low <= high) {
                int mid = low + ((high - low) >> 1);
                if (sortedList[mid].CompareTo(value) > 0)
                    high = mid - 1;
                else
                    low = mid + 1;
            }
            
            return high;
        }
        
        // https://devblogs.microsoft.com/dotnet/little-known-gems-atomic-conditional-removals-from-concurrentdictionary/
        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value) {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(
                new KeyValuePair<TKey, TValue>(key, value)
            );
        }
        public static bool UpdateOrRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict,
            TKey key,
            Func<TKey, TValue, TValue> updateValueFactory,
            Func<TKey, TValue, bool> shouldRemove
        ) where TKey : notnull {
            //
            while (dict.TryGetValue(key, out var currValue)) {
                var newValue = updateValueFactory(key, currValue);
                if (!dict.TryUpdate(key, newValue, currValue)) continue;
                
                // Check if we need to remove after update
                if (!shouldRemove(key, newValue)) return false;
                return dict.TryRemove(key, newValue);
            }
            // Key doesn't exist
            return false;
        }
    }
}