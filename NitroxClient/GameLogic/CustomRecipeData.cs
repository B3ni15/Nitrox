using System.Collections.Generic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace NitroxClient.GameLogic;

/// <summary>
/// Static storage for custom recipes received from the server.
/// These are accessed by Harmony patches to modify recipe retrieval.
/// </summary>
public static class CustomRecipeData
{
    /// <summary>
    /// Dictionary of custom recipes keyed by TechType.
    /// </summary>
    public static Dictionary<TechType, CustomRecipe> Recipes { get; } = new();

    /// <summary>
    /// Whether custom recipes have been loaded from the server.
    /// </summary>
    public static bool IsInitialized { get; set; }

    /// <summary>
    /// Clears all custom recipes.
    /// </summary>
    public static void Clear()
    {
        Recipes.Clear();
        IsInitialized = false;
    }

    /// <summary>
    /// Tries to get a custom recipe for the given TechType.
    /// </summary>
    public static bool TryGetRecipe(TechType techType, out CustomRecipe recipe)
    {
        return Recipes.TryGetValue(techType, out recipe);
    }

    /// <summary>
    /// Adds or updates a custom recipe.
    /// </summary>
    public static void SetRecipe(TechType techType, CustomRecipe recipe)
    {
        Recipes[techType] = recipe;
    }
}
