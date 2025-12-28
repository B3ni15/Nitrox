using System;
using System.Collections;
using System.Collections.Generic;
using NitroxClient.GameLogic.InitialSync.Abstract;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;
using Nitrox.Model.Subnautica.Packets;

namespace NitroxClient.GameLogic.InitialSync;

/// <summary>
/// Processes custom fragment requirements received from the server during initial sync.
/// Populates the CustomFragmentData dictionary which is used by Harmony patches
/// to override the game's fragment scan requirements.
/// </summary>
public sealed class CustomFragmentInitialSyncProcessor : InitialSyncProcessor
{
    public CustomFragmentInitialSyncProcessor()
    {
        // No dependencies - can run early
    }

    public override List<Func<InitialPlayerSync, IEnumerator>> Steps { get; } =
    [
        ApplyCustomFragments
    ];

    private static IEnumerator ApplyCustomFragments(InitialPlayerSync packet)
    {
        // Clear any previous fragments
        CustomFragmentData.Clear();

        if (packet.CustomFragments == null || packet.CustomFragments.Count == 0)
        {
            Log.Info("No custom fragments to apply");
            CustomFragmentData.IsInitialized = true;
            yield break;
        }

        Log.Info($"Applying {packet.CustomFragments.Count} custom fragments from server");

        int successCount = 0;
        foreach (CustomFragment customFragment in packet.CustomFragments)
        {
            try
            {
                if (ApplyFragment(customFragment))
                {
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to apply custom fragment for {customFragment.TechType}");
            }
        }

        CustomFragmentData.IsInitialized = true;
        Log.Info($"Custom fragments applied: {successCount}/{packet.CustomFragments.Count}");
        yield break;
    }

    private static bool ApplyFragment(CustomFragment customFragment)
    {
        if (!Enum.TryParse(customFragment.TechType.Name, out TechType techType))
        {
            Log.Warn($"Unknown TechType: {customFragment.TechType.Name}");
            return false;
        }

        // Store the fragment in our static dictionary
        // The Harmony patch (PDAScanner_GetEntryData_Patch) will read from this dictionary
        // and override the game's fragment requirements
        CustomFragmentData.SetFragment(techType, customFragment);

        Log.Debug($"Registered custom fragment for {techType}: {customFragment.TotalFragments} fragments required");
        return true;
    }
}
