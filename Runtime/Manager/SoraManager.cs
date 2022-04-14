using SoraCore.Extension;
using static SoraCore.Constant;

namespace SoraCore.Manager
{
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    public abstract class SoraManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T m_Instance;
        protected static T GetInstance()
        {
            if (m_Instance != null) return m_Instance;

            var instances = FindObjectsOfType<T>();

            // Only found 1
            if (instances.Length == 1)
            {
                m_Instance = instances[0];
                return m_Instance;
            }

            string s1 = $"[{nameof(T)}]".Bold();

            // Found nothing
            if (instances.Length == 0)
            {
                var gObj = new GameObject(nameof(T) + " (Runtime)");
                m_Instance = gObj.AddComponent<T>();
                SoraCore.LogWarning($"No manager of type {s1} found. Creating runtime instance. This could lead to some bug!", nameof(T), gObj);
                return m_Instance;
            }

            // Found more than one

            m_Instance = instances[0];
            SoraCore.LogWarning($"More than 1 manager of type {s1} found. Will use the first one in the list", nameof(T), instances[0].gameObject);
            return m_Instance;
        }

        protected static void LogWarningForEvent(string callerType, [CallerMemberName] string callerName = "")
        {
            string s1 = $"[{callerType}]".Bold();
            string s2 = $"{callerName}()".Bold();

            Debug.LogWarning($"{SoraWarning}{s1.Color("white")}: {s2} was requested but no {s1} picked it up.\n Make sure the persistent scene are fully loaded before method calls");
        }
    }
}
