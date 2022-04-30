/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              FastTravelManager
Description:        Handles the Teleporter Lists
Date Created:       17/11/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Max] Created base class
    19/11/2021
        - [Max] Added m_IsListFlipped to check m_AllTeleportersList status
    30/11/2021
        - [Max] Added Comments
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelManager : MonoBehaviour
{
    [HideInInspector] public Player Player;
    public int NumTeleportersUnlocked;
    public Teleporters CurrentTeleporter;
    public Teleporters TargetTeleporter;
    public List<Teleporters> AllTeleportersList;
    public List<Teleporters> UnlockedTeleportersList;
    public bool IsListFlipped;

    // Start is called before the first frame update
    public void Start()
    {
        Player = PlayerManager.Instance.Player;
        NumTeleportersUnlocked = 0;
        UnlockedTeleportersList = new List<Teleporters>();
        CurrentTeleporter = null;
        TargetTeleporter = null;
        IsListFlipped = false;
    }

    public void AddTeleporter(Teleporters teleporter)
    {
        // Add Teleporter to list of all Teleporters
        AllTeleportersList.Add(teleporter);
    }

    public void AddUnlockedTeleporter(Teleporters teleporter)
    {
        // Add Teleporter to list of unlocked Teleporters
        UnlockedTeleportersList.Add(teleporter);
    }
}
