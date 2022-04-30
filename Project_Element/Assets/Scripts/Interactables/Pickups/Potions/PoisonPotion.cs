/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PoisonPotion
Description:        Applies poison to the Player's weapon when used
Date Created:       17/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Zoe] Started PoisonPotion.
    19/11/2021
        - [Zoe] Added null check for the Weapon, to prevent an error if you dropped your weapon before the poison ended
    30/11/2021
        - [Zoe] Changed Start to Awake
        - [Zoe] Tidied up and commented code
        - [Zoe] Added m_PoisonAmount to properly modify poison damage.
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

public class PoisonPotion : Potion
{
    public float m_PoisonAmount = 10.0f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        InitPotion();
    }

    public override void InitPotion()
    {
        m_PotionType = PotionType.Poison;
        m_ItemID = 10;
        m_HUDTimerIcon = HUDManager.Instance.PotionTimerIcons[3];
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ActivateEffect()
    {
        if (m_HUDTimerIcon.gameObject.activeSelf == false)
        {
            // If player is holding a weapon
            if (m_Player.m_MainWeapon != null)
            {
                // Set the player's weapon to poisonous
                m_Player.m_MainWeapon.SetPoisonous(true);

                // Set the poison damage to the potion's damage
                // This ensures that all weapons will have the same poison damage when it's modified, rather than editing the value in every weapon
                m_Player.m_MainWeapon.PoisonDamage = m_PoisonAmount;

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
        
        // If player is still holding a weapon
        if (m_Player.m_MainWeapon != null)
        {
            // Removes the poison from the weapon
            m_Player.m_MainWeapon.SetPoisonous(false);
        }

        // Makes the Timer HUD dissappear
        m_HUDTimerIcon.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
