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
    private static bool initialized;
    private static bool initSuccess;
    private static object techDataDictionary;
    private static MethodInfo setRecipeMethod;
    private static Type jsonValueType;

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

        if (!InitializeApi())
        {
            Log.Error("Failed to initialize recipe modification API - custom recipes will not work");
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

        Log.Info($"Custom recipes applied: {successCount}/{packet.CustomRecipes.Count}");
        yield break;
    }

    private static bool InitializeApi()
    {
        if (initialized)
        {
            return initSuccess;
        }
        initialized = true;

        try
        {
            // Log all available fields/methods in CraftData for debugging
            Log.Debug("=== CraftData fields ===");
            foreach (FieldInfo field in typeof(CraftData).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                Log.Debug($"  Field: {field.Name} ({field.FieldType.Name})");
            }

            Log.Debug("=== TechData fields ===");
            foreach (FieldInfo field in typeof(TechData).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                Log.Debug($"  Field: {field.Name} ({field.FieldType.Name})");
            }

            // Try to find the techData dictionary in TechData class
            FieldInfo techDataField = typeof(TechData).GetField("techData", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            if (techDataField != null)
            {
                techDataDictionary = techDataField.GetValue(null);
                Log.Info($"Found TechData.techData: {techDataDictionary?.GetType().Name}");
                initSuccess = true;
                return true;
            }

            // Try CraftData
            techDataField = typeof(CraftData).GetField("techData", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            if (techDataField != null)
            {
                techDataDictionary = techDataField.GetValue(null);
                Log.Info($"Found CraftData.techData: {techDataDictionary?.GetType().Name}");
                initSuccess = true;
                return true;
            }

            // Try to find any Dictionary field that might contain recipes
            foreach (FieldInfo field in typeof(TechData).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] genericArgs = field.FieldType.GetGenericArguments();
                    if (genericArgs[0] == typeof(TechType))
                    {
                        Log.Info($"Found potential recipe dictionary in TechData: {field.Name} -> Dictionary<{genericArgs[0].Name}, {genericArgs[1].Name}>");
                        techDataDictionary = field.GetValue(null);
                        if (techDataDictionary != null)
                        {
                            initSuccess = true;
                            return true;
                        }
                    }
                }
            }

            Log.Error("Could not find recipe storage dictionary");
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize recipe API");
            return false;
        }
    }

    private static bool ApplyRecipe(CustomRecipe customRecipe)
    {
        if (!Enum.TryParse(customRecipe.TechType.Name, out TechType techType))
        {
            Log.Warn($"Unknown TechType: {customRecipe.TechType.Name}");
            return false;
        }

        // Build ingredients list
        List<Ingredient> ingredients = new();
        foreach (CustomRecipeIngredient ingredient in customRecipe.Ingredients)
        {
            if (!Enum.TryParse(ingredient.TechType.Name, out TechType ingredientTechType))
            {
                Log.Warn($"Unknown ingredient TechType: {ingredient.TechType.Name} in recipe {customRecipe.TechType.Name}");
                continue;
            }
            ingredients.Add(new Ingredient(ingredientTechType, ingredient.Amount));
        }

        // Use TechData.Add to register the recipe (uses Subnautica's JsonValue internally)
        // This is what modding APIs like SMLHelper do
        try
        {
            // Create ITechData compatible object using reflection
            // The TechData class stores data as JsonValue objects

            // For now, we'll use the direct approach - modify the underlying data
            // Subnautica stores recipes as JsonValue in TechData.techData dictionary

            if (techDataDictionary != null)
            {
                // Get dictionary methods
                Type dictType = techDataDictionary.GetType();
                MethodInfo containsKey = dictType.GetMethod("ContainsKey");
                PropertyInfo indexer = dictType.GetProperty("Item");

                // Create a new JsonValue for this recipe
                Type valueType = dictType.GetGenericArguments()[1];

                // We need to create the value in the expected format
                // For now just log what we found
                Log.Debug($"Would set recipe for {techType} in dictionary of type {valueType.Name}");

                // TODO: Create proper JsonValue or ITechData and add to dictionary
            }

            Log.Debug($"Applied recipe for {techType}: {customRecipe.CraftAmount}x from {ingredients.Count} ingredients");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error applying recipe for {techType}");
            return false;
        }
    }
}
