/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  WaterAbility 2
Description:         Manages any specific properties or logic for the Hailstorm ability the Water Pet 2 uses to damage and slow enemies
Date Created:       08/02/2022
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/02/2022
        - [Aaron] Set up the framework for this class
        - [Aaron] Created Apply Slow function to apply an object with the slow effect script
        - [Aaron] Added and set up On Trigger Enter function to damage and slow enemies that collide with this ability instance
    18/02/2022
        - [Aaron] Changed the Trigger Enter code to Collision Enter code to match the changes in the prefab and clean up movement behaviour.
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAbility2 : MainAbility
{
    [Header("Effect Properties")]
    public float SlowDuration = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        // If it has collided into an enemy
        if (enemyObj != null)
        {
            // The enemy takes the ability's damage.
            ApplyDamage(enemyObj.gameObject);

            // The enemy sufferes from damage over time effect.
            ApplySlowEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }
    }

    // Applies a slow effect script to the affected Object, setting the duration and starting the slow effect
    void ApplySlowEffect(GameObject affectedObj)
    {
        affectedObj.AddComponent<SlowEffect>();
        affectedObj.GetComponent<SlowEffect>().SetEffectAmountAndDuration(0.0f, SlowDuration);
        affectedObj.GetComponent<SlowEffect>().SlowCharacter();
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.WaterStormIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.WaterStormIgnite);
    }
}
