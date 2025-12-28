using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Nitrox.Model.Serialization;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace Nitrox.Server.Subnautica.Models.GameLogic;

public class CustomFragmentManager
{
    private const string FRAGMENTS_FILE_NAME = "fragments.json";

    public List<CustomFragment> CustomFragments { get; private set; } = new();
    public bool IsEnabled { get; }

    private readonly string saveDir;

    public CustomFragmentManager(SubnauticaServerConfig config, string saveDir)
    {
        IsEnabled = config.CustomFragmentsEnabled;
        this.saveDir = saveDir;

        if (IsEnabled)
        {
            LoadFragments();
        }
    }

    private void LoadFragments()
    {
        string fragmentsPath = Path.Combine(saveDir, FRAGMENTS_FILE_NAME);

        if (!File.Exists(fragmentsPath))
        {
            Log.Info($"Custom fragments enabled but {FRAGMENTS_FILE_NAME} not found. Creating default file at: {fragmentsPath}");
            CreateDefaultFragmentsFile(fragmentsPath);
        }

        try
        {
            string json = File.ReadAllText(fragmentsPath);
            FragmentsFileData data = JsonConvert.DeserializeObject<FragmentsFileData>(json);

            if (data?.Fragments != null)
            {
                foreach (FragmentJsonData fragmentData in data.Fragments)
                {
                    CustomFragment fragment = ConvertToCustomFragment(fragmentData);
                    if (fragment != null)
                    {
                        CustomFragments.Add(fragment);
                    }
                }

                Log.Info($"Loaded {CustomFragments.Count} custom fragments");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to load custom fragments from {fragmentsPath}");
        }
    }

    private CustomFragment ConvertToCustomFragment(FragmentJsonData data)
    {
        if (string.IsNullOrEmpty(data.TechType))
        {
            Log.Warn("Custom fragment has empty TechType, skipping");
            return null;
        }

        if (data.TotalFragments <= 0)
        {
            Log.Warn($"Custom fragment {data.TechType} has invalid TotalFragments ({data.TotalFragments}), skipping");
            return null;
        }

        return new CustomFragment(
            new NitroxTechType(data.TechType),
            data.TotalFragments
        );
    }

    private void CreateDefaultFragmentsFile(string path)
    {
        FragmentsFileData defaultData = new()
        {
            Fragments = new List<FragmentJsonData>
            {
                new() { TechType = "Seaglide", TotalFragments = 4 },
                new() { TechType = "Seamoth", TotalFragments = 5 },
                new() { TechType = "Cyclops", TotalFragments = 6 },
                new() { TechType = "Constructor", TotalFragments = 5 },
                new() { TechType = "Beacon", TotalFragments = 3 },
                new() { TechType = "Gravsphere", TotalFragments = 3 }
            }
        };

        try
        {
            string json = JsonConvert.SerializeObject(defaultData, Formatting.Indented);
            File.WriteAllText(path, json);
            Log.Info($"Created default {FRAGMENTS_FILE_NAME}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to create default {FRAGMENTS_FILE_NAME}");
        }
    }

    // JSON deserialization classes
    private class FragmentsFileData
    {
        public List<FragmentJsonData> Fragments { get; set; }
    }

    private class FragmentJsonData
    {
        public string TechType { get; set; }
        public int TotalFragments { get; set; }
    }
}
