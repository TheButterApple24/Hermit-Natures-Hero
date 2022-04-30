/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DefensePotion
Description:        Boosts the player's defense when used
Date Created:       17/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Zoe] Started DefensePotion.
    30/11/2021
        - [Zoe] Changed Start to Awake
        - [Zoe] Tidied up and commented code
    08/12/2021
        - [Zoe] Potion is now properly destroyed
    15/02/2022
        - [Zoe] Added a check to prevent multiple of the same effect stacking

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotionTypes;

public class DefensePotion : Potion
{
    public float m_DamageResistMultiplier = 0.2f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        InitPotion();
    }

    public override void InitPotion()
    {
        m_PotionType = PotionType.Defense;
        m_ItemID = 8;
        m_HUDTimerIcon = HUDManager.Instance.PotionTimerIcons[2];
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void ActivateEffect()
    {
        if (m_HUDTimerIcon.gameObject.activeSelf == false)
        {
            // Stores original damage resist value
            m_OriginalValue = m_Player.m_HealthComp.m_DamageResistance;

            // Applies new damage resist value
            m_Player.m_HealthComp.m_DamageResistance += m_DamageResistMultiplier;

            // Makes the Timer HUD appear
            m_HUDTimerIcon.gameObject.SetActive(true);

            // Runs Timer
            StartCoroutine("StartPotionTimer");
        }
    }

    protected override IEnumerator StartPotionTimer()
    {
        m_Activated = true;
        yield return new WaitForSeconds(m_EffectDuration);

        // Reverts damage resist to original value
        m_Player.m_HealthComp.m_DamageResistance = m_OriginalValue;

        // Makes the Timer HUD dissappear
        m_HUDTimerIcon.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
