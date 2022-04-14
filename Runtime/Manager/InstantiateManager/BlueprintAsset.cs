namespace SoraCore.Manager.Instantiate
{
    using MyBox;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Blueprint", menuName = "SoraCore/Instantiate Manager/Blueprint")]
    public class BlueprintAsset : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public BlueprintCallbackAsset GetCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackAsset ReleaseCallback { get; private set; }
        [field: SerializeField] public BlueprintCallbackAsset DestroyCallback { get; private set; }

        [field: SerializeField] public bool EnablePooling { get; private set; }
        [field: SerializeField, ReadOnly("<EnablePooling>k__BackingField", true)] public int Preload { get; private set; } = 10;
        [field: SerializeField, ReadOnly("<EnablePooling>k__BackingField", true)] public int Capacity { get; private set; } = 10;
    }
}

