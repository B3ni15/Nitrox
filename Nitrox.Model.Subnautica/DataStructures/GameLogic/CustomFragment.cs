using System;
using System.Runtime.Serialization;
using BinaryPack.Attributes;

namespace Nitrox.Model.Subnautica.DataStructures.GameLogic;

/// <summary>
/// Represents a custom fragment configuration that specifies how many scans
/// are required to unlock a particular TechType.
/// </summary>
[Serializable]
[DataContract]
public class CustomFragment
{
    /// <summary>
    /// The TechType that this fragment unlocks (e.g., "Seamoth", "Seaglide").
    /// </summary>
    [DataMember(Order = 1)]
    public NitroxTechType TechType { get; set; }

    /// <summary>
    /// The number of fragments that need to be scanned to fully unlock this TechType.
    /// Default game values are typically 2-4 fragments. Set higher for increased difficulty.
    /// </summary>
    [DataMember(Order = 2)]
    public int TotalFragments { get; set; }

    [IgnoreConstructor]
    protected CustomFragment()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public CustomFragment(NitroxTechType techType, int totalFragments)
    {
        TechType = techType;
        TotalFragments = totalFragments;
    }

    public override string ToString()
    {
        return $"{TechType}: {TotalFragments} fragments";
    }
}
