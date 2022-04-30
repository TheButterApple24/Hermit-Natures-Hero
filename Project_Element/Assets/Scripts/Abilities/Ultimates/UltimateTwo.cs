/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Ultimate Two
Description:        Manages the ultimate two specific properties..
                    This class is reponsibile for damaging enemies this script attaches too
Date Created:       26/01/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/01/2022
        - [Aaron] Created the base class to help test the CurrentUltimate in the player script and add to the starting Ultimate Ability 2 game object
    31/01/2022
        - [Aaron] Created the Apply Stun Effect function and Stun Duration variable, used them inside the On Collision Enter function
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateTwo : UltimateAbility
{
    [Header("Ultimate Properties")]
    public float StunDuration = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy)
        {
            enemy.m_HealthComp.ModifyHealth(-AbilityDamage, false);
            ApplyStunEffect(enemy.gameObject);
        }
    }

    void ApplyStunEffect(GameObject aObj)
    {
        // Add and initialize a Root (Stun) Effect
        aObj.AddComponent<RootEffect>();
        aObj.GetComponent<RootEffect>().SetEffectAmountAndDuration(0.0f, StunDuration);
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.MudslideLinger)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.MudslideLinger);
    }
}
