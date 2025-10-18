using System;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SoraTehk.Extensions {
    public static partial class AddressablesX {
        public static void SetUwrCacheBusting(bool isActive) {
            Addressables.WebRequestOverride -= HandleWebRequestOverride;
            if (isActive) Addressables.WebRequestOverride += HandleWebRequestOverride;
        }
        private static void HandleWebRequestOverride(UnityWebRequest uwr) {
            if (uwr.method != UnityWebRequest.kHttpVerbGET) return;
            
            string newUrl = uwr.url;
            
            string cacheBuster = DateTime.UtcNow.Ticks.ToString();
            newUrl += newUrl.Contains("?")
                ? "&v=" + cacheBuster
                : "?v=" + cacheBuster;
            
            uwr.url = newUrl;
        }
    }
}