/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Pet Attacks
Description:        Manages the Animation Events triggered from Pet Attack animations in Unity
Date Created:       28/03/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    28/03/2022
        - [Max] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAttacks : MonoBehaviour
{
    public Pet Pet;
    private Player m_Player;

    // Base Attack Call
    public void BaseAttack()
    {
        if (Pet != null)
        {
            // Spawn E Ability
            Pet.SpawnAbilityFromAnimation();
        }
    }

    // Ultimate Attack Call from Primary Pet
    public void UltimateAttack()
    {
        m_Player = PlayerManager.Instance.Player;

        if (m_Player != null && Pet != null && m_Player.m_PrimaryPet == Pet)
        {
            // Spawn Ultimate Ability
            m_Player.SpawnUltimateFromAnimation();
        }
    }   
}
