namespace SoraCore.EditorTools
{
	using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class EditorHelper
    {
		public static string[] FindAssetsGUIDOfType<T>(params string[] searchInFolders) where T : Object
			=> AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchInFolders);

		public static List<string> FindAssetsPathOfType<T>(params string[] searchInFolders) where T : Object
        {
			List<string> result = new();

			// Search for all asset of type T in project and..
			string[] assetGUIDS = FindAssetsGUIDOfType<T>(searchInFolders);
			foreach (string guid in assetGUIDS)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				// ...add it to the list
				result.Add(path);
			}

			return result;
		}

		public static List<T> FindAssetsOfType<T>(params string[] searchInFolders) where T : Object
		{
			List<T> result = new();
			List<string> assetPaths = FindAssetsPathOfType<T>(searchInFolders);

			foreach (string path in assetPaths)
            {
				T asset = AssetDatabase.LoadAssetAtPath<T>(path);
				if (asset != null) result.Add(asset);
			}

			return result;
		}
	}
}