/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  MainAbility
Description:         Manages the main ability specific properties that will store values it will need to modify.
                           This class is reponsibile for setting, shooting, and resetting main abilities. Specific abilities have different effects.
Date Created:       08/11/2021
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/11/2021
        - [Aaron] Set up the framework for this class, added the initial properties
    09/11/2021
        - [Aaron] Spawning + Update functions added with starting logic filled it. Sets the spawn point and update's an ability instance
    10/11/2021
        - [Aaron] Created a cooldown timer coroutine that will prevent abilities from being spawn until the cooldown period is over.
        - [Aaron] Updated the updating methods to prevent bug that allowed abilities to live longer than their lifespan.
    15/11/2021
        - [Aaron] Updated the direction the ability is being sent to account for lock on if the player has a target. Fixed bug where it would go up into sky or down into ground.
        - [Aaron] Created a method for applying the ability's damage to a gameobject in collided with.
        - [Aaron] Reformated existing functions to change from spawning/deleting to activating/deactivating a single instance.
    16/11/2021
        - [Aaron] Updated Lifespan to have a max lifespan that is set in the editor to update the lifespan back to original value once the lifespan hits 0.
        - [Aaron] Moved UpdateAbility from Update to a LateUpdate
        - [Aaron] Created an initial method for updating the direction an ability travels when no lock on target exists.
          (Currently not updating as expected and reverted to hacky directional code.)
        - [Aaron] Updated the StartPoint function for where the ability starts. Uses the AbilitySocket as the ability starting position
    17/11/2021
        - [Aaron] Change the spawning bool into an IsActive bool to reflect the code better. Also created a public getter and private setter. The
        public getter is used by the Pet script to trigger it's ability.
        - [Aaron] Moved Lifespan and active/deactive logic into the base Ability class. Both Ultimate and Main will require these functions and propertiees.
	20/01/2022
        - [Max] Added False parameter to ModifyHealth function (NO CRIT)
    25/01/2022
        - [Aaron] Change the Apply Damage function to make use of the global element check to change the damage output of an abilit based on the colliding elements
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    18/02/2022
        - [Aaron] Removed old transform movement code and replaced with rigidbody movement code.
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

public class MainAbility : Ability
{
    [Header("Main Ability Properties")]
    public float AbilitySpeed;

    GameObject m_AbilityTarget;
    Vector3 m_AbilityDirection;

    private void LateUpdate()
    {
        UpdateAbility();
    }

    // Update the target object, called through the pet class.
    public void UpdateAbilityTarget(GameObject targetObj)
    {
        m_AbilityTarget = targetObj;
    }

    // Update the direction the main ability will travel towards.
    public void UpdateAbilityDirection()
    {
        Vector3 dir = (AbilityOwner.transform.position + AbilityOwner.transform.forward) - AbilityOwner.transform.position;
        m_AbilityDirection = dir.normalized;
    }

    // Check if the ability is active or on cooldown. Set the lifepsan and activates the ability
    public void UseAbility()
    {
        if (IsActive == false && OnCooldown == false)
        {
            m_Lifespan = AbilityLifespanMax;
            IsActive = true;
        }
    }

    // Check if the gameobject is valid and if it's an enemy. If so, modify their health based on the damage of the ability
    protected void ApplyDamage(GameObject obj)
    {
        if (obj != null)
        {
            Enemy enemyObj = obj.GetComponent<Enemy>();

            if (enemyObj != null)
            {
                float totalDamage = AbilityDamage * ElementTypeMatchup.ElementalCheckMultiplier(AbilityElementType, enemyObj);
                enemyObj.m_HealthComp.ModifyHealth(-totalDamage, false);
            }
        }
    }

    private void UpdateAbility()
    {
        // Check if Abiliy has been triggered
        if (IsActive == true)
        {
            // Update it's lifespan and position until the lifespan is less than 0, then destroy the instance and reset the lifespan
            m_Lifespan -= Time.deltaTime;

            // Check if the ability has a target to go to
            if (m_AbilityTarget != null)
            {
                // Update it's position while it's alive
                if (m_Lifespan > 0.0f)
                {
                    // Set the direction to the distance between the target positon and owner's positon, normailized
                    Vector3 direction = (m_AbilityTarget.transform.position + m_AbilityTarget.transform.up) - this.transform.position;
                    direction.Normalize();

                    // Move the ability instance in the desired direction
                    Collider abilityCollider = GetComponent<Collider>();
                    abilityCollider.attachedRigidbody.velocity = direction * AbilitySpeed;
                }
                else
                {
                    // Reset the lifespan and deactivate the instance of the ability
                    m_Lifespan = AbilityLifespanMax;
                    IsActive = false;
                    DeactivateInstance();
                }
            }
            else
            {
                // Update it's position while it's alive
                if (m_Lifespan > 0.0f)
                {
                    // Move the ability instance in the desired direction
                    Collider abilityCollider = GetComponent<Collider>();
                    abilityCollider.attachedRigidbody.velocity = m_AbilityDirection * AbilitySpeed;
                }
                else
                {
                    // Reset the lifespan and deactivate the instance of the ability
                    m_Lifespan = AbilityLifespanMax;
                    IsActive = false;
                    DeactivateInstance();
                }
            }
        }
    }
}
