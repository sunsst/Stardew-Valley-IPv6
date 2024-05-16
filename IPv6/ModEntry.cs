using StardewModdingAPI;

namespace IPv6;

/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Patch.MyPatch.Patching(helper, ModManifest.UniqueID);
    }
}