/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              StaminaPotion
Description:        Restores the player's stamina when used
Date Created:       17/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Zoe] Started StaminaPotion.
    30/11/2021
        - [Zoe] Changed Start to Awake
        - [Zoe] Tidied up and commented code
    08/12/2021
        - [Zoe] Potion is now properly destroyed
    29/02/2021
        - [Zoe] Replaced temporary player in Activate with m_Player that's already in InteractableBase
    15/02/2022
        - [Zoe] Added a check to prevent multiple of the same effect stacking
        - [Zoe] Fixed the regen boost from 2.0 to 1.2

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotionTypes;

public class StaminaPotion : Potion
{
    public float m_RegenBoostMultiplier = 1.2f;

    // Start is called before the first frame update
    public override void Start()
    {
        InitPotion();
        base.Start();
    }

    public override void InitPotion()
    {
        m_PotionType = PotionType.Stamina;
        m_ItemID = 9;
        m_HUDTimerIcon = HUDManager.Instance.PotionTimerIcons[0];
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void ActivateEffect()
    {
        if (m_HUDTimerIcon.gameObject.activeSelf == false)
        {
            // Stores the original regen value
            m_OriginalValue = m_Player.m_StaminaComp.m_StaminaRegenRate;

            // Applies new regen value
            m_Player.m_StaminaComp.m_StaminaRegenRate *= m_RegenBoostMultiplier;

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

        // Reverts regen rate back to its origin value
        m_Player.m_StaminaComp.m_StaminaRegenRate = m_OriginalValue;

        // Makes the Timer HUD dissappear
        m_HUDTimerIcon.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
