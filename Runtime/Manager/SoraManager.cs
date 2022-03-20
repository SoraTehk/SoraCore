using SoraCore.Extension;
using static SoraCore.Constant;

namespace SoraCore.Manager {
    using System.Runtime.CompilerServices;
    using UnityEngine;
    
    using Debug = UnityEngine.Debug;

    public abstract class SoraManager : MonoBehaviour {
        protected static void LogWarningForEvent(string callerType, [CallerMemberName] string callerName = "") {
            string s1 = $"[{callerType}]".Bold();
            string s2 = $"{callerName}()".Bold();

            Debug.LogWarning($"{SoraWarning}{s1.Color("white")}: {s2} was requested but no {s1} picked it up.\n Make sure the persistent scene are fully loaded before method calls");
        }
    }
}
