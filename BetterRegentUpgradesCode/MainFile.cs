using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace BetterRegentUpgrades.BetterRegentUpgradesCode;

//You're recommended but not required to keep all your code in this package and all your assets in the BetterRegentUpgrades folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MagmaDys_BetterRegentUpgrades"; 
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}