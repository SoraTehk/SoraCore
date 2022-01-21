using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoraCore.Manager {
    [CreateAssetMenu(fileName = "SceneData", menuName = "SoraCore/Game Manager/SceneData")]
    public class SceneSO : ScriptableObject {
        [field: SerializeField] public int BuildIndex { get; private set; }

        [SerializeField] private List<PrefabSO> _preloadPrefabList;
        public List<PrefabSO> PreloadPrefabList
        {
            get { return _preloadPrefabList; }
        }



        public AssetReference sceneReference;
    }
}