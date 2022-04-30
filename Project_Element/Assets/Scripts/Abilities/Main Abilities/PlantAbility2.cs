/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Plant Ability 2
Description:         Manages any specific properties or logic for the Toxic Spray ability the Plant Pet 2 uses to damage and poison enemies
Date Created:       08/02/2022
Author:                Aaron Wilson
Verified By:          Jeffrey MacNab [13/04/2022]

Changelog:

    08/02/2022
        - [Aaron] Set up the framework for this class
        - [Aaron] Added the logic from fire ability 1 since results are similar but their particle effects and appearance will differ (Burn V. Poison)
    18/02/2022
        - [Aaron] Changed the Trigger Enter code to Collision Enter code to match the changes in the prefab and clean up movement behaviour.
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAbility2 : MainAbility
{
    [Header("Effect Properties")]
    public float DamageOverTimeAmount = 1.0f;
    public float DamageOverTimeDuration = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        // If it has collided into an enemy
        if (enemyObj != null)
        {
            // The enemy takes the ability's damage.
            ApplyDamage(enemyObj.gameObject);

            // The enemy sufferes from damage over time effect.
            ApplyDotEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }
    }

    // Add a DOT effect script to the specified gameobject, then set the damage and duration of the script
    void ApplyDotEffect(GameObject affectedObj)
    {
        affectedObj.AddComponent<DotEffect>();
        affectedObj.GetComponent<DotEffect>().SetEffectAmountAndDuration(DamageOverTimeAmount, DamageOverTimeDuration);
        affectedObj.GetComponent<DotEffect>().InitEffect();
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.PlantSprayIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.PlantSprayIgnite);
    }
}
