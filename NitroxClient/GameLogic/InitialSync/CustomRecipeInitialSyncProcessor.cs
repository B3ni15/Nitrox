using System;
using System.Collections;
using System.Collections.Generic;
using NitroxClient.GameLogic.InitialSync.Abstract;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;
using Nitrox.Model.Subnautica.Packets;

namespace NitroxClient.GameLogic.InitialSync;

/// <summary>
/// Processes custom recipes received from the server during initial sync.
/// Populates the CustomRecipeData dictionary which is used by Harmony patches
/// to override the game's recipe retrieval methods.
/// </summary>
public sealed class CustomRecipeInitialSyncProcessor : InitialSyncProcessor
{
    public CustomRecipeInitialSyncProcessor()
    {
        // No dependencies - can run early
    }

    public override List<Func<InitialPlayerSync, IEnumerator>> Steps { get; } =
    [
        ApplyCustomRecipes
    ];

    private static IEnumerator ApplyCustomRecipes(InitialPlayerSync packet)
    {
        // Clear any previous recipes
        CustomRecipeData.Clear();

        if (packet.CustomRecipes == null || packet.CustomRecipes.Count == 0)
        {
            Log.Info("No custom recipes to apply");
            CustomRecipeData.IsInitialized = true;
            yield break;
        }

        Log.Info($"Applying {packet.CustomRecipes.Count} custom recipes from server");

        int successCount = 0;
        foreach (CustomRecipe customRecipe in packet.CustomRecipes)
        {
            try
            {
                if (ApplyRecipe(customRecipe))
                {
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to apply custom recipe for {customRecipe.TechType}");
            }
        }

        CustomRecipeData.IsInitialized = true;
        Log.Info($"Custom recipes applied: {successCount}/{packet.CustomRecipes.Count}");
        yield break;
    }

    private static bool ApplyRecipe(CustomRecipe customRecipe)
    {
        if (!Enum.TryParse(customRecipe.TechType.Name, out TechType techType))
        {
            Log.Warn($"Unknown TechType: {customRecipe.TechType.Name}");
            return false;
        }

        // Store the recipe in our static dictionary
        // The Harmony patches (TechData_GetIngredients_Patch and TechData_GetCraftAmount_Patch)
        // will read from this dictionary and override the game's recipe data
        CustomRecipeData.SetRecipe(techType, customRecipe);

        Log.Debug($"Registered custom recipe for {techType}: {customRecipe.CraftAmount}x from {customRecipe.Ingredients?.Count ?? 0} ingredients");
        return true;
    }
}
