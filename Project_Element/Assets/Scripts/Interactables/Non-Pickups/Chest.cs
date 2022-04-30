/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Chest
Description:        Handles interactable chests
Date Created:       19/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Jeffrey] Created base implementation
    27/01/2022
        - [Jeffrey] Implemented EXP System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableBase
{
    public LootSystem m_ChestLoot;
    public int m_Rarity;
    public int m_ChestEXP;

    public override void Start()
    {
        base.Start();

        m_ChestLoot.GenerateRandomLoot();

        m_ChestEXP = Random.Range(5, 20);
        m_RemoveFromListOnTrigger = true;
    }

    public override void Activate()
    {
        if (m_Player != null)
        {
            m_ChestLoot.SpawnLoot();
            m_Player.CheckForLevelUp(m_ChestEXP);
            Destroy(this.gameObject);
        }
    }
}
