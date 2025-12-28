using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BinaryPack.Attributes;

namespace Nitrox.Model.Subnautica.DataStructures.GameLogic;

[Serializable]
[DataContract]
public class CustomRecipe
{
    [DataMember(Order = 1)]
    public NitroxTechType TechType { get; set; }

    [DataMember(Order = 2)]
    public int CraftAmount { get; set; }

    [DataMember(Order = 3)]
    public List<CustomRecipeIngredient> Ingredients { get; set; }

    [IgnoreConstructor]
    protected CustomRecipe()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public CustomRecipe(NitroxTechType techType, int craftAmount, List<CustomRecipeIngredient> ingredients)
    {
        TechType = techType;
        CraftAmount = craftAmount;
        Ingredients = ingredients;
    }

    public override string ToString()
    {
        return $"{TechType} ({CraftAmount}x) <- [{string.Join(", ", Ingredients)}]";
    }
}
