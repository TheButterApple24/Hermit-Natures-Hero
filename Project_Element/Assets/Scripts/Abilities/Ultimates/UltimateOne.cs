/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Ultimate One
Description:        Manages the ultimate one specific properties..
                    This class is reponsibile for moving differently than the other ultimates and exploding upon colliding, damaging any enemies in the radius.
Date Created:       26/01/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/01/2022
        - [Aaron] Created the base class to help test the CurrentUltimate in the player script and add to the starting Ultimate Ability 1 game object
    27/01/2022
        - [Aaron] Created an Update that rotates the ultimate along the Y-axis for a spin effect. Created collision function to handle deactivating the instance.
        - [Aaron] Created an Explode function that checks for coliders within a radius. If any are enemy colliders, it applied ability damage to them.
    31/01/2022
        - [Aaron] Added a variable to track debuff duration and a function to apply a weapon damage debuff to enemies hit by an explosion
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateOne : UltimateAbility
{
    [Header("Ultimate One Properties")]
    public float ExplosionRadius = 10.0f;
    public float ExplosionKnockback = 10.0f;
    public float DebuffDuration = 5.0f;

    private void Update()
    {
        // Rotate around the Y-axis when active in the world
        Vector3 startRot = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        Vector3 rotAmount = new Vector3(0.0f, 400.0f * Time.deltaTime, 0.0f);
        startRot += rotAmount;
        transform.Rotate(startRot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            Explode();
            IsActive = false;
            DeactivateInstance();
        }
    }

    private void Explode()
    {
        // Find and store all colliders within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        // Check each collider for an Enemy
        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            // If any colliders belong to an enemy, reduce their health by the ability damage and apply effects
            if (enemy)
            {
                enemy.m_HealthComp.ModifyHealth(-AbilityDamage, false);
                ApplyPushEffect(enemy.gameObject);
                ApplyDebuffEffect(enemy.gameObject);
            }
        }
    }

    void ApplyPushEffect(GameObject affectedObj)
    {
        // Calculate direction based on the ability position and character position, normalizing the result
        Vector3 direction = affectedObj.transform.position - this.transform.position;
        direction.Normalize();

        // Add and initialize the Pushback (knockback) effect
        affectedObj.AddComponent<PushEffect>();
        affectedObj.GetComponent<PushEffect>().SetPushDirection(direction);
        affectedObj.GetComponent<PushEffect>().SetEffectAmountAndDuration(ExplosionKnockback, 1.0f);
        affectedObj.GetComponent<PushEffect>().InitEffect();
    }

    void ApplyDebuffEffect(GameObject affectedObj)
    {
        // Add and initialize the Debuff effect
        affectedObj.AddComponent<DebuffEffect>();
        affectedObj.GetComponent<DebuffEffect>().SetEffectAmountAndDuration(0.0f, DebuffDuration);
        affectedObj.GetComponent<DebuffEffect>().ReduceDamage();
    }

    public override void PlayStartSound()
    {
        // Play Ignite sound effect when the steambomb is used
        if (AbilitySoundManager.sfxInstance.SteamBombIgnite)
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.SteamBombIgnite);
    }
}
