using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Concurrent;

namespace SoraCore {
    public static class Constant {
        public const char CHAR_SYNCHRONOUS_IDLE = '▬';
        public const string SORA_NULL = "<b><color=red>[Sora:Null]</color></b>";
        public const string SORA_LOG = "<b><color=lime>[Sora:Info]</color></b>";
        public const string SORA_WARNING = "<b><color=yellow>[Sora:Warning]</color></b>";
    }

    /// <summary>
    /// Simple string builder pool with <see cref="ConcurrentBag{T}"/>
    /// </summary>
    public static class StringBuilderPool {
        const int INITIALIZE_SIZE = 1;
        const int CAPACITY = 256;
        static ConcurrentBag<StringBuilder> poolBag;
        static StringBuilderPool() {
            Reset();
        }

        /// <summary>
        /// Rebuild the pool from scratch
        /// </summary>
        public static void Reset() {
            poolBag = new ConcurrentBag<StringBuilder>();
            for (int i = 0; i < INITIALIZE_SIZE; i++) {
                poolBag.Add(new StringBuilder(CAPACITY));
            }
        }

        /// <summary>
        /// Get a <see cref="StringBuilder"/> from the pool
        /// </summary>
        public static StringBuilder Get() {
            if(!poolBag.TryTake(out StringBuilder sb)) {
                sb = new StringBuilder(CAPACITY);
            }

            return sb;
        }

        /// <summary>
        /// Add a <see cref="StringBuilder"/> to the pool
        /// </summary>
        public static void Return(StringBuilder sb) {
            sb.Clear();
            poolBag.Add(sb);
        }
    }
}