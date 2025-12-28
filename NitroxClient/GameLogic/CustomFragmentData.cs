using System.Collections.Generic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace NitroxClient.GameLogic;

/// <summary>
/// Static storage for custom fragment requirements received from the server.
/// These are accessed by Harmony patches to modify fragment scan requirements.
/// </summary>
public static class CustomFragmentData
{
    /// <summary>
    /// Dictionary of custom fragment requirements keyed by TechType.
    /// </summary>
    public static Dictionary<TechType, CustomFragment> Fragments { get; } = new();

    /// <summary>
    /// Whether custom fragments have been loaded from the server.
    /// </summary>
    public static bool IsInitialized { get; set; }

    /// <summary>
    /// Clears all custom fragments.
    /// </summary>
    public static void Clear()
    {
        Fragments.Clear();
        IsInitialized = false;
    }

    /// <summary>
    /// Tries to get a custom fragment requirement for the given TechType.
    /// </summary>
    public static bool TryGetFragment(TechType techType, out CustomFragment fragment)
    {
        return Fragments.TryGetValue(techType, out fragment);
    }

    /// <summary>
    /// Adds or updates a custom fragment requirement.
    /// </summary>
    public static void SetFragment(TechType techType, CustomFragment fragment)
    {
        Fragments[techType] = fragment;
    }
}
