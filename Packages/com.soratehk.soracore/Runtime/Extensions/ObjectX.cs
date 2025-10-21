using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

namespace SoraTehk.Extensions {
    public static partial class ObjectX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(this T? value, string paramName) where T : class {
            if (value == null) throw new InvalidOperationException($"{paramName} was null");
            
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(this T? value, string paramName) where T : struct {
            if (value == null) throw new InvalidOperationException($"{paramName} was null");
            
            return value.Value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ThrowIfNull<T>(this IEnumerable<T?> source, string paramName) where T : class {
            if (source == null) throw new InvalidOperationException($"{paramName} collection was null");
            
            foreach (var item in source) {
                if (item == null) throw new InvalidOperationException($"{paramName} contained a null element");
                yield return item;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ThrowIfNull<T>(this IEnumerable<T?> source, string paramName) where T : struct {
            if (source == null) throw new InvalidOperationException($"{paramName} collection was null");
            
            foreach (var item in source) {
                if (!item.HasValue) throw new InvalidOperationException($"{paramName} contained a null element");
                yield return item.Value;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask<T> ThrowIfNull<T>(this UniTask<T?> task, string paramName) where T : class {
            var value = await task;
            if (value == null) throw new InvalidOperationException($"{paramName} was null");
            
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask<T> ThrowIfNull<T>(this UniTask<T?> task, string paramName) where T : struct {
            var value = await task;
            if (value == null) throw new InvalidOperationException($"{paramName} was null");
            
            return value.Value;
        }
    }
}