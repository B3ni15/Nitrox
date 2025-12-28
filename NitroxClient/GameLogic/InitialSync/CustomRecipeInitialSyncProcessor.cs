using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NitroxClient.GameLogic.InitialSync.Abstract;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;
using Nitrox.Model.Subnautica.Packets;

namespace NitroxClient.GameLogic.InitialSync;

public sealed class CustomRecipeInitialSyncProcessor : InitialSyncProcessor
{
    private static FieldInfo techDataField;
    private static Type techDataType;
    private static FieldInfo craftAmountField;
    private static FieldInfo ingredientsField;
    private static Type ingredientType;
    private static ConstructorInfo ingredientConstructor;

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
        if (packet.CustomRecipes == null || packet.CustomRecipes.Count == 0)
        {
            Log.Info("No custom recipes to apply");
            yield break;
        }

        if (!InitializeReflection())
        {
            Log.Error("Failed to initialize CraftData reflection - custom recipes will not work");
            yield break;
        }

        Log.Info($"Applying {packet.CustomRecipes.Count} custom recipes from server");

        foreach (CustomRecipe customRecipe in packet.CustomRecipes)
        {
            try
            {
                ApplyRecipe(customRecipe);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to apply custom recipe for {customRecipe.TechType}");
            }
        }

        Log.Info("Custom recipes applied successfully");
        yield break;
    }

    private static bool InitializeReflection()
    {
        if (techDataField != null)
        {
            return true;
        }

        try
        {
            // Get CraftData.techData field (Dictionary<TechType, CraftData.TechData>)
            techDataField = typeof(CraftData).GetField("techData", BindingFlags.NonPublic | BindingFlags.Static);
            if (techDataField == null)
            {
                Log.Error("Could not find CraftData.techData field");
                return false;
            }

            // Get the TechData nested type
            techDataType = typeof(CraftData).GetNestedType("TechData", BindingFlags.NonPublic | BindingFlags.Public);
            if (techDataType == null)
            {
                Log.Error("Could not find CraftData.TechData type");
                return false;
            }

            // Get TechData fields
            craftAmountField = techDataType.GetField("_craftAmount", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ingredientsField = techDataType.GetField("_ingredients", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (craftAmountField == null || ingredientsField == null)
            {
                Log.Error("Could not find TechData fields");
                return false;
            }

            // Get Ingredient nested type
            ingredientType = typeof(CraftData).GetNestedType("Ingredient", BindingFlags.NonPublic | BindingFlags.Public);
            if (ingredientType == null)
            {
                Log.Error("Could not find CraftData.Ingredient type");
                return false;
            }

            ingredientConstructor = ingredientType.GetConstructor([typeof(TechType), typeof(int)]);
            if (ingredientConstructor == null)
            {
                Log.Error("Could not find Ingredient constructor");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize CraftData reflection");
            return false;
        }
    }

    private static void ApplyRecipe(CustomRecipe customRecipe)
    {
        if (!Enum.TryParse(customRecipe.TechType.Name, out TechType techType))
        {
            Log.Warn($"Unknown TechType: {customRecipe.TechType.Name}");
            return;
        }

        // Get the techData dictionary
        object techDataDict = techDataField.GetValue(null);
        if (techDataDict == null)
        {
            Log.Error("CraftData.techData is null");
            return;
        }

        // Create ingredients list
        Type ingredientListType = typeof(List<>).MakeGenericType(ingredientType);
        object ingredients = Activator.CreateInstance(ingredientListType);
        MethodInfo addMethod = ingredientListType.GetMethod("Add");

        foreach (CustomRecipeIngredient ingredient in customRecipe.Ingredients)
        {
            if (!Enum.TryParse(ingredient.TechType.Name, out TechType ingredientTechType))
            {
                Log.Warn($"Unknown ingredient TechType: {ingredient.TechType.Name} in recipe {customRecipe.TechType.Name}");
                continue;
            }

            object ingredientObj = ingredientConstructor.Invoke([ingredientTechType, ingredient.Amount]);
            addMethod.Invoke(ingredients, [ingredientObj]);
        }

        // Create new TechData instance
        object newTechData = Activator.CreateInstance(techDataType);
        craftAmountField.SetValue(newTechData, customRecipe.CraftAmount);
        ingredientsField.SetValue(newTechData, ingredients);

        // Update or add to dictionary
        Type dictType = techDataDict.GetType();
        MethodInfo containsKeyMethod = dictType.GetMethod("ContainsKey");
        MethodInfo setItemMethod = dictType.GetProperty("Item").GetSetMethod();
        MethodInfo addDictMethod = dictType.GetMethod("Add");

        bool exists = (bool)containsKeyMethod.Invoke(techDataDict, [techType]);

        if (exists)
        {
            setItemMethod.Invoke(techDataDict, [techType, newTechData]);
            Log.Debug($"Updated recipe for {techType}: {customRecipe}");
        }
        else
        {
            addDictMethod.Invoke(techDataDict, [techType, newTechData]);
            Log.Debug($"Added new recipe for {techType}: {customRecipe}");
        }
    }
}
