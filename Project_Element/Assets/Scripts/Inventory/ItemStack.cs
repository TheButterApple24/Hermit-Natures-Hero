/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ItemStack
Description:        Handles storing pickup data
Date Created:       26/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/11/2021
        - [Jeffrey] Created base class
    26/01/2022
        - [Max] Added m_Defense to ItemStack
    28/01/2022
        - [Max] Added m_Type to ItemStack

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public int ItemAmount = 0;
    public int ItemID = -1;
    public string ItemName;
    public string ItemTitle;
    public string ItemDescription;
    public bool ItemEquipped = false;
    public bool ItemSelected = false;

    public float ItemDamage;
    public float ItemDefense;

    public LootTiers.LootTier ItemRarity;
    public InventoryItemType ItemType;
}
