/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Pickups
Description:        Handles item interface
Date Created:       17/10/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Max] Started Pickups.
    08/11/2021
        - [Max] Called Player's AddToInventory. Removed m_IsPickupable
        - [Max] Clamped Inventory if Full
    17/11/2021
        - [Zoe] Now calls base.Activate to hide the button prompt
    19/11/2021
        - [Zoe] Can no longer use objects while your hands are full of puzzle cube
        - [Max] Check if Inventory exists
    01/02/2022
        - [Zoe] Cleaned up unused code

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PickupTypes;

public class Pickups : InteractableBase
{
    protected PickupType m_PickupType;
    public Sprite m_Icon;
    public int m_Category = 0;

    protected override void Awake()
    {
        base.Awake();
        m_RemoveFromListOnTrigger = true;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Activate()
    {
        if (m_ItemID != -1)
        {
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.m_LoreUnlockedIDs[m_ItemID] = true;
        }
    }
}
