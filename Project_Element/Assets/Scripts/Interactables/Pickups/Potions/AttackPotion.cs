/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              AttackPotion
Description:        Boosts the players damage when used
Date Created:       17/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Zoe] Started AttackPotion.
    30/11/2021
        - [Zoe] Changed Start to Awake
        - [Zoe] Tidied up and commented code
    08/12/2021
        - [Zoe] Potion is now properly destroyed
    15/02/2022
        - [Zoe] Added a check to prevent multiple of the same effect stacking
    09/03/2022
        - [Max] Weapon class refactor

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotionTypes;

public class AttackPotion : Potion
{
    public float m_DamageBoostMultiplier = 1.2f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        InitPotion();
    }

    public override void InitPotion()
    {
        m_PotionType = PotionType.Attack;
        m_ItemID = 7;
        m_HUDTimerIcon = HUDManager.Instance.PotionTimerIcons[1];
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void ActivateEffect()
    {
        if (m_HUDTimerIcon.gameObject.activeSelf == false)
        {
            // If the player is holding a weapon
            if (m_Player.m_MainWeapon != null)
            {
                // Store the original base damage
                m_OriginalValue = m_Player.m_MainWeapon.GetBaseDamage();

                // Multiply the base damage by the multiplier
                m_Player.m_MainWeapon.SetBaseDamage(m_Player.m_MainWeapon.GetBaseDamage() * m_DamageBoostMultiplier);

                // Makes the Timer HUD appear
                m_HUDTimerIcon.gameObject.SetActive(true);

                // Run Timer
                StartCoroutine("StartPotionTimer");
            }
        }
    }

    protected override IEnumerator StartPotionTimer()
    {
        m_Activated = true;
        yield return new WaitForSeconds(m_EffectDuration);

        // Revert damage to original value
        m_Player.m_MainWeapon.SetBaseDamage(m_OriginalValue);

        // Makes the Timer HUD dissappear
        m_HUDTimerIcon.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
