using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Nitrox.Model.Serialization;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace Nitrox.Server.Subnautica.Models.GameLogic;

public class CustomRecipeManager
{
    private const string RECIPES_FILE_NAME = "recipes.json";

    public List<CustomRecipe> CustomRecipes { get; private set; } = new();
    public bool IsEnabled { get; }

    private readonly string saveDir;

    public CustomRecipeManager(SubnauticaServerConfig config, string saveDir)
    {
        IsEnabled = config.CustomRecipesEnabled;
        this.saveDir = saveDir;

        if (IsEnabled)
        {
            LoadRecipes();
        }
    }

    private void LoadRecipes()
    {
        string recipesPath = Path.Combine(saveDir, RECIPES_FILE_NAME);

        if (!File.Exists(recipesPath))
        {
            Log.Info($"Custom recipes enabled but {RECIPES_FILE_NAME} not found. Creating default file at: {recipesPath}");
            CreateDefaultRecipesFile(recipesPath);
        }

        try
        {
            string json = File.ReadAllText(recipesPath);
            RecipesFileData data = JsonConvert.DeserializeObject<RecipesFileData>(json);

            if (data?.Recipes != null)
            {
                foreach (RecipeJsonData recipeData in data.Recipes)
                {
                    CustomRecipe recipe = ConvertToCustomRecipe(recipeData);
                    if (recipe != null)
                    {
                        CustomRecipes.Add(recipe);
                    }
                }

                Log.Info($"Loaded {CustomRecipes.Count} custom recipes");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to load custom recipes from {recipesPath}");
        }
    }

    private CustomRecipe ConvertToCustomRecipe(RecipeJsonData data)
    {
        if (string.IsNullOrEmpty(data.TechType))
        {
            Log.Warn("Custom recipe has empty TechType, skipping");
            return null;
        }

        List<CustomRecipeIngredient> ingredients = new();

        if (data.Ingredients != null)
        {
            foreach (IngredientJsonData ingredientData in data.Ingredients)
            {
                if (string.IsNullOrEmpty(ingredientData.TechType))
                {
                    Log.Warn($"Ingredient in recipe {data.TechType} has empty TechType, skipping ingredient");
                    continue;
                }

                ingredients.Add(new CustomRecipeIngredient(
                    new NitroxTechType(ingredientData.TechType),
                    ingredientData.Amount > 0 ? ingredientData.Amount : 1
                ));
            }
        }

        return new CustomRecipe(
            new NitroxTechType(data.TechType),
            data.CraftAmount > 0 ? data.CraftAmount : 1,
            ingredients
        );
    }

    private void CreateDefaultRecipesFile(string path)
    {
        RecipesFileData defaultData = new()
        {
            Recipes = new List<RecipeJsonData>
            {
                new()
                {
                    TechType = "Knife",
                    CraftAmount = 1,
                    Ingredients = new List<IngredientJsonData>
                    {
                        new() { TechType = "Titanium", Amount = 2 },
                        new() { TechType = "Silicone", Amount = 1 }
                    }
                },
                new()
                {
                    TechType = "Scanner",
                    CraftAmount = 1,
                    Ingredients = new List<IngredientJsonData>
                    {
                        new() { TechType = "Titanium", Amount = 2 },
                        new() { TechType = "Battery", Amount = 1 }
                    }
                }
            }
        };

        try
        {
            string json = JsonConvert.SerializeObject(defaultData, Formatting.Indented);
            File.WriteAllText(path, json);
            Log.Info($"Created default {RECIPES_FILE_NAME}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to create default {RECIPES_FILE_NAME}");
        }
    }

    // JSON deserialization classes
    private class RecipesFileData
    {
        public List<RecipeJsonData> Recipes { get; set; }
    }

    private class RecipeJsonData
    {
        public string TechType { get; set; }
        public int CraftAmount { get; set; }
        public List<IngredientJsonData> Ingredients { get; set; }
    }

    private class IngredientJsonData
    {
        public string TechType { get; set; }
        public int Amount { get; set; }
    }
}
