/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Fire Ability 1
Description:         Manages any specific properties or logic for the Fireball / Firespit the Fire Pet uses to damage enemeies
Date Created:       09/11/2021
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/11/2021
        - [Aaron] Set up the framework for this class
    15/11/2021
        - [Aaron] Override the OnTriggerEnter function to detect when the collider on a water ability has collided with an object
    16/11/2021
        - [Aaron] Changed Update to a LateUpdate that is used in base class now
        - [Aaron] Called reset start point before deactivating on collision
     04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    18/02/2022
        - [Aaron] Changed the Trigger Enter code to Collision Enter code to match the changes in the prefab and clean up movement behaviour.
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbility1 : MainAbility
{
    [Header("Effect Properties")]
    public float DamageOverTimeAmount = 1.0f;
    public float DamageOverTimeDuration = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Check for collision with an enemy object
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        if (enemyObj != null)
        {
            // If the sound clip exists, play the impact sound
            if (AbilitySoundManager.sfxInstance.FireImpact)
                AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.FireImpact);

            // The enemy takes the ability's damage and sufferes from damage over time effect.
            ApplyDamage(enemyObj.gameObject);
            ApplyDotEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }

        // Check for collision with an element barrier object
        ElementBarrier barrier = other.gameObject.GetComponent<ElementBarrier>();

        if (barrier)
        {
            barrier.CheckElementCollision(this);
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
        if (AbilitySoundManager.sfxInstance.FireIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.FireIgnite);
    }
}
