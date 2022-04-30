/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Fire Ability 2
Description:         Manages any specific properties or logic for the  Heatwave ability the Fire Pet 2 uses to damage and debuff enemies
Date Created:       08/02/2022
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/02/2022
        - [Aaron] Set up the framework for this class
        - [Aaron] Added an ApplyDebuff function that'll debuff affected objects
        - [Aaron] Added OnTriggerEnter to deactivate the ability and apply the damage + debuff effect when colliding with an enemy
    18/02/2022
        - [Aaron] Changed the Trigger Enter code to Collision Enter code to match the changes in the prefab and clean up movement behaviour.
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbility2 : MainAbility
{
    [Header("Effect Properties")]
    public float DebuffDuration = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        // If it has collided into an enemy
        if (enemyObj != null)
        {
            // The enemy takes the ability's damage.
            ApplyDamage(enemyObj.gameObject);

            // The enemy sufferes from damage over time effect.
            ApplyDebuffEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }
    }

    // Adds a Debuff Effect script to the affected object. Sets the effect's properties and initializes the effect + effect timer
    void ApplyDebuffEffect(GameObject affectedObj)
    {
        affectedObj.AddComponent<DebuffEffect>();
        affectedObj.GetComponent<DebuffEffect>().SetEffectAmountAndDuration(0.0f, DebuffDuration);
        affectedObj.GetComponent<DebuffEffect>().ReduceDefense();
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.FireIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.FireIgnite);
    }
}
