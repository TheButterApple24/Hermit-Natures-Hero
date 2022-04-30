/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                 Plant Ability 1
Description:        Manages any specific properties or logic for the Egg Vine uses to damage enemeies
Date Created:      06/12/2021
Author:               Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/12/2021
        - [Aaron] Set up the framework for this class
        - [Aaron] Created the ApplyRootEffect function to add the RootEffect script to the end on collision.
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

public class PlantAbility1 : MainAbility
{
    [Header("Effect Properties")]
    public float RootDuration = 4.0f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        // If it has collided into an enemy
        if (enemyObj != null)
        {
            // The enemy takes the ability's damage.
            ApplyDamage(enemyObj.gameObject);

            // The enemy has becomes rooted
            ApplyRootEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }
    }

    // Add a Root effect script to the specified gameobject, then set the duration of the script
    void ApplyRootEffect(GameObject affectedObj)
    {
        affectedObj.AddComponent<RootEffect>();
        affectedObj.GetComponent<RootEffect>().SetEffectAmountAndDuration(0.0f, RootDuration);
        affectedObj.GetComponent<RootEffect>().RootCharacter();
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.PlantPopIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.PlantPopIgnite);
    }
}
