using System.Reflection;
using NitroxClient.GameLogic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Patches PDAScanner.GetEntryData to return custom fragment requirements from the server.
/// This modifies how many fragments need to be scanned to unlock a TechType.
/// </summary>
public sealed partial class PDAScanner_GetEntryData_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method(() => PDAScanner.GetEntryData(default));

    public static void Postfix(TechType key, ref PDAScanner.EntryData __result)
    {
        if (!CustomFragmentData.IsInitialized)
        {
            return;
        }

        if (__result == null)
        {
            return;
        }

        if (CustomFragmentData.TryGetFragment(key, out CustomFragment fragment) && fragment.TotalFragments > 0)
        {
            __result.totalFragments = fragment.TotalFragments;
            Log.Debug($"[CustomFragment] Replaced totalFragments for {key}: {fragment.TotalFragments}");
        }
    }
}
