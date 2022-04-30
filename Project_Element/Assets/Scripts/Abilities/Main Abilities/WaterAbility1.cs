/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                 WaterAbility1
Description:        Manages the properties of the Water Blast ability that the Water Pet 1 uses to damage and knock back enemies
Date Created:      09/11/2021
Author:               Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/11/2021
        - [Aaron] Set up the framework for this class
    15/11/2021
        - [Aaron] Override the OnTriggerEnter function to detect when the collider on a water ability has collided with an object
    16/11/2021
        - [Aaron] Changed Update to a LateUpdate that is used in base class now
        - [Aaron] Called reset start point before deactivating on collision
    17/11/2021
        - [Aaron] Added a push back to occur to the enemy when they are hit by a water blast. Put logic in KnockBackEnemy function
    26/30/2022
        - [Aaron] Updated the ability to make use of the push script's new function and calculate the push directing in the apply Push Function
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

public class WaterAbility1 : MainAbility
{
    [Header("Effect Properties")]
    public float KnockbackAmount = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // If it has collided into an enemy
        Enemy enemyObj = other.gameObject.GetComponentInParent<Enemy>();

        if (enemyObj != null)
        {
            // The enemy takes the ability's damage and is pushed backward
            ApplyDamage(enemyObj.gameObject);
            ApplyPushEffect(enemyObj.gameObject);

            // Deactive the ability instance
            IsActive = false;
            DeactivateInstance();
        }
    }

    // Calculate the push direction and add a PushEffect script to the specified object, setting the information it requires.
    void ApplyPushEffect(GameObject affectedObj)
    {
        Vector3 direction = CalculatePushDirection(affectedObj);

        affectedObj.AddComponent<PushEffect>();
        affectedObj.GetComponent<PushEffect>().SetPushDirection(direction);
        affectedObj.GetComponent<PushEffect>().SetEffectAmountAndDuration(KnockbackAmount, 1.0f);
        affectedObj.GetComponent<PushEffect>().InitEffect();
    }

    // Calculate the direction to move the object based on the ability's position and the enemy's position, normalizing the result
    Vector3 CalculatePushDirection(GameObject obj)
    {
        Vector3 direction = obj.transform.position - this.transform.position;
        return direction.normalized;
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.WaterSprayIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.WaterSprayIgnite);
    }
}