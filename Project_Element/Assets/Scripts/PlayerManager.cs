/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PlayerManager
Description:        Allows the enemy to access the players location.
Date Created:       19/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/10/21
        - [Gerard] Created PlayerManager.
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    private static PlayerManager m_Instance = null;
    public static PlayerManager Instance { get { return m_Instance; } }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

    #endregion

    private Player m_Player = null;
    public Player Player { get { return m_Player; } }
    public void SetPlayer(Player aPlayer)
    {
        m_Player = aPlayer;
    }
}
