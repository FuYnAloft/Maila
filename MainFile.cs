using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace FuYn.Maila;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    private const string ModId = "FuYn.Maila"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        ModConfigRegistry.Register("FuYn.Maila", new MailaConfig());
        harmony.PatchAll();
    }
}