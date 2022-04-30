/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CraftingRecipe
Description:        Stores the recipe to create a potion: The two reagents and the resulting potion
Date Created:       26/01/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/01/2021
        - [Zoe] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotionTypes;
using ReagentTypes;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Project Element/Crafting Recipe")]

public class CraftingRecipe : ScriptableObject
{
    public ReagentType Reagent1;
    public ReagentType Reagent2;
    public PotionType ResultingPotion;
}