using CommandTerminal;
using SoraCore.Manager;
using UnityEngine;
using System.Collections.Generic;
using MyBox;

#if UNITY_EDITOR
using UnityEditor;
public partial class TerminalCommand
{
    [ButtonMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
    private void FindBlueprints()
    {
        Blueprints = SoraCore.EditorTools.EditorHelper.FindAssetsOfType<BlueprintSO>();
        EditorUtility.SetDirty(this);
    }
}
#endif


public partial class TerminalCommand : MonoBehaviour
{
    [field: SerializeField] public List<BlueprintSO> Blueprints { get; private set; }

    private void Awake()
    {
        Terminal.Shell.AddCommand("MGO.Preload", GameObjectManagerPreload, 1, 1);
        Terminal.Shell.AddCommand("MGO.Clear", GameObjectManagerClear, 1, 1);
        Terminal.Shell.AddCommand("MGO.ClearAll", GameObjectManagerClearAll, 0, 0);

    }

    private void GameObjectManagerPreload(CommandArg[] args)
    {
        if (Terminal.IssuedError) return;
        var bd = Blueprints[args[0].Int];
        GameObjectManager.Preload(bd);
    }

    private void GameObjectManagerClear(CommandArg[] args)
    {
        if (Terminal.IssuedError) return;
        var bd = Blueprints[args[0].Int];
        GameObjectManager.Clear(bd);
    }

    private void GameObjectManagerClearAll(CommandArg[] args)
    {
        if (Terminal.IssuedError) return;
        GameObjectManager.ClearAll();
    }
}
