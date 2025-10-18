using System;
using System.IO;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class PathX {
        public enum UriType {
            NONE = 0,
            LOCAL,
            REMOTE,
            UNITY_WEB_REQUEST
        }
        
        // TODO: Finish this
        public static UriType GetUriType(string uri) {
            if (string.IsNullOrEmpty(uri)) return UriType.NONE;
            
            uri = uri.Replace("\\", "/");
            var compareType =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                StringComparison.OrdinalIgnoreCase;
#else
                StringComparison.Ordinal;
#endif
            
            if (uri.StartsWith("http://", compareType) ||
                uri.StartsWith("https://", compareType)) {
                return UriType.REMOTE;
            }
            if (uri.StartsWith("file://", compareType)) {
                return UriType.UNITY_WEB_REQUEST;
            }
            if (uri.StartsWith(Application.persistentDataPath.Replace("\\", "/"), compareType)) {
#if UNITY_WEBGL && !UNITY_EDITOR
                return UriType.LOCAL;
#else
                return UriType.UNITY_WEB_REQUEST;
#endif
            }
            if (uri.StartsWith(Application.streamingAssetsPath.Replace("\\", "/"), compareType)) return UriType.UNITY_WEB_REQUEST;
            if (uri.StartsWith(Application.dataPath.Replace("\\", "/"), compareType)) {
#if UNITY_ANDROID || UNITY_IOS
                return UriType.NONE;
#else
                return UriType.UNITY_WEB_REQUEST;
#endif
            }
            if (uri.StartsWith(Application.temporaryCachePath.Replace("\\", "/"), compareType)) return UriType.LOCAL;
            if (Path.IsPathRooted(uri)) {
#if UNITY_WEBGL && !UNITY_EDITOR
                return UriType.NONE;
#else
                return UriType.UNITY_WEB_REQUEST;
#endif
            }
            
            return UriType.NONE;
        }
    }
}