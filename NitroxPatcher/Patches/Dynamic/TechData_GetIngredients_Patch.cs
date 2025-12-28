using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using NitroxClient.GameLogic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Patches TechData.GetIngredients to return custom recipe ingredients from the server.
/// </summary>
public sealed partial class TechData_GetIngredients_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method(() => TechData.GetIngredients(default));

    public static void Postfix(TechType techType, ref IList<IIngredient> __result)
    {
        if (!CustomRecipeData.IsInitialized)
        {
            return;
        }

        if (CustomRecipeData.TryGetRecipe(techType, out CustomRecipe recipe) && recipe.Ingredients != null)
        {
            List<Ingredient> customIngredients = new();
            foreach (CustomRecipeIngredient ingredient in recipe.Ingredients)
            {
                if (System.Enum.TryParse(ingredient.TechType.Name, out TechType ingredientTechType))
                {
                    customIngredients.Add(new Ingredient(ingredientTechType, ingredient.Amount));
                }
            }

            if (customIngredients.Count > 0)
            {
                __result = new ReadOnlyCollection<Ingredient>(customIngredients);
                Log.Debug($"[CustomRecipe] Replaced ingredients for {techType}: {customIngredients.Count} ingredients");
            }
        }
    }
}
