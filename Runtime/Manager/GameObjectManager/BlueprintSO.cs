namespace SoraCore.Manager {
    using UnityEngine;
    using MyBox;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(fileName = "BlueprintData", menuName = "SoraCore/GameObject Manager/Blueprint Data")]
    public class BlueprintSO : ScriptableObject {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public BlueprintCallbackSO GetCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackSO ReleaseCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackSO DestroyCallback { get; private set; }

        [field: SerializeField] public bool EnablePooling { get; private set; }
        [ReadOnly("<EnablePooling>k__BackingField", true)] public int preload = 10;
        [ReadOnly("<EnablePooling>k__BackingField", true)] public int capacity = 10;
    }
}

