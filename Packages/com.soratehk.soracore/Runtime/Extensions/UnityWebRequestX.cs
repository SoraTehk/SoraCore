using UnityEngine;
using UnityEngine.Networking;
using System;

namespace SoraTehk.Extensions {
    public static partial class UnityWebRequestX {
        public static T GetResultAs<T>(this UnityWebRequest uwr) where T : UObject {
            if (!uwr.isDone) throw new InvalidOperationException("UnityWebRequest has not completed yet.");
            if (uwr.result != UnityWebRequest.Result.Success) throw new InvalidOperationException($"UnityWebRequest failed: {uwr.error}");
            
            Type typeOfT = typeof(T);
            
            if (typeOfT == typeof(TextAsset)) return new TextAsset(uwr.downloadHandler.text) as T;
            if (typeOfT == typeof(AssetBundle)) return DownloadHandlerAssetBundle.GetContent(uwr) as T;
            if (typeOfT == typeof(AudioClip)) return DownloadHandlerAudioClip.GetContent(uwr) as T;
            if (typeOfT == typeof(Texture2D)) return DownloadHandlerTexture.GetContent(uwr) as T;
            
            throw new NotSupportedException($"Unsupported type {typeOfT}");
        }
    }
}