using System.Reflection;
using NitroxClient.GameLogic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Patches TechData.GetCraftAmount to return custom recipe craft amount from the server.
/// </summary>
public sealed partial class TechData_GetCraftAmount_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method(() => TechData.GetCraftAmount(default));

    public static void Postfix(TechType techType, ref int __result)
    {
        if (!CustomRecipeData.IsInitialized)
        {
            return;
        }

        if (CustomRecipeData.TryGetRecipe(techType, out CustomRecipe recipe) && recipe.CraftAmount > 0)
        {
            __result = recipe.CraftAmount;
            Log.Debug($"[CustomRecipe] Replaced craft amount for {techType}: {recipe.CraftAmount}");
        }
    }
}
