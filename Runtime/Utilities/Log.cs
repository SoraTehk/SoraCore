using SoraCore.Extension;
using static SoraCore.Constant;

namespace SoraCore
{
    using UnityEngine;

    public partial class SoraCore
    {
        public static void Log(string message, string callerType, Object ctx = null)
        {
            string s1 = $"[{callerType}]".Bold();
            Debug.LogWarning($"{SoraLog}{s1.Color("white")}: {message}\n\n", ctx);
        }

        public static void LogNull(string message, string callerType, Object ctx = null)
        {
            string s1 = $"[{callerType}]".Bold();
            Debug.LogWarning($"{SoraNull}{s1.Color("white")}: {message}\n\n", ctx);
        }

        public static void LogWarning(string message, string callerType, Object ctx = null)
        {
            string s1 = $"[{callerType}]".Bold();
            Debug.LogWarning($"{SoraWarning}{s1.Color("white")}: {message}\n\n", ctx);
        }

        public static void LogError(string message, string callerType, Object ctx = null)
        {
            string s1 = $"[{callerType}]".Bold();
            Debug.LogError($"{SoraError}{s1.Color("white")}: {message}\n\n", ctx);
        }
    }
}