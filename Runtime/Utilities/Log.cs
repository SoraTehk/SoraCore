using SoraCore.Extension;
using static SoraCore.Constant;

namespace SoraCore {
    using UnityEngine;

    public class SoraCore
    {
        public static void LogWarning(string message, string callerType, Object ctx = null) {
            string s1 = $"[{callerType}]".Bold();
            Debug.LogWarning($"{SORA_WARNING}{s1.Color("white")}: {message}\n\n", ctx);
        } 
    }
}