using System;
using System.Runtime.Serialization;
using BinaryPack.Attributes;

namespace Nitrox.Model.Subnautica.DataStructures.GameLogic;

[Serializable]
[DataContract]
public class CustomRecipeIngredient
{
    [DataMember(Order = 1)]
    public NitroxTechType TechType { get; set; }

    [DataMember(Order = 2)]
    public int Amount { get; set; }

    [IgnoreConstructor]
    protected CustomRecipeIngredient()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public CustomRecipeIngredient(NitroxTechType techType, int amount)
    {
        TechType = techType;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"{Amount}x {TechType}";
    }
}
