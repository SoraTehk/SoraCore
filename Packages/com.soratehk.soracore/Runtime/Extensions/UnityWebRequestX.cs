using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SoraTehk.Extensions {
    public static partial class UnityWebRequestX {
        public static T GetResultAs<T>(this UnityWebRequest uwr) where T : UObject {
            if (!uwr.isDone) throw new InvalidOperationException("UnityWebRequest has not completed yet.");
            if (uwr.result != UnityWebRequest.Result.Success) throw new InvalidOperationException($"UnityWebRequest failed: {uwr.error}");
            
            Type typeOfT = typeof(T);
            return typeOfT switch {
                _ when typeOfT == typeof(TextAsset) => (new TextAsset(uwr.downloadHandler.text) as T)!,
                _ when typeOfT == typeof(AssetBundle) => (DownloadHandlerAssetBundle.GetContent(uwr) as T)!,
                _ when typeOfT == typeof(AudioClip) => (DownloadHandlerAudioClip.GetContent(uwr) as T)!,
                _ when typeOfT == typeof(Texture2D) => (DownloadHandlerTexture.GetContent(uwr) as T)!,
                _ => throw new ArgumentOutOfRangeException(nameof(T), typeOfT, "Unsupported asset type")
            };
        }
    }
}