/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              HealthPotion
Description:        Heals the player when used
Date Created:       17/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Zoe] Started HealthPotion.
    30/11/2021
        - [Zoe] Changed Start to Awake
        - [Zoe] Tidied up and commented code
        - [Zoe] Fixed m_HealPercentage not being public
    08/12/2021
        - [Zoe] Potion is now properly destroyed
    20/01/2022
        - [Max] Added False parameter to ModifyHealth function (NO CRIT)

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotionTypes;

public class HealthPotion : Potion
{
    public float m_HealPercentage = 0.20f;
    // Start is called before the first frame update

    public override void Start()
    {
        base.Start();
        InitPotion();
    }

    public override void InitPotion()
    {
        m_PotionType = PotionType.Health;
        m_ItemID = 6;
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void ActivateEffect()
    {
        // If the Player is missing health
        if (m_Player.m_HealthComp.m_CurrentHealth < m_Player.m_HealthComp.MaxHealth)
        {
            // Sends the healing amount to the health component
            m_Player.m_HealthComp.ModifyHealth((m_Player.m_HealthComp.MaxHealth * m_HealPercentage), false);
            m_Activated = true;
            Destroy(gameObject);
        }
    }
}
