/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Ultimate Ability
Description:        Manages the ultimate specific properties that will store values it will need to modify.
                    This class is reponsibile for setting, shooting, and resetting the ultimate abilities. Specific abilities have different effects.
Date Created:       25/01/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/01/2022
        - [Aaron] Created the base class to test the CurrentUltimate in the player script
    26/01/2022
        - [Aaron] Set up the ability direction and rotation. Created FixedUpdate and now move the ability based on the direction and spawn position.
    27/01/2022
        - [Aaron] Changed the Update to take into account lifespan. Added in cooldown checks and handling to prevent ability spam.
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateAbility : Ability
{
    [Header("Ultimate Ability Properties")]
    public Sprite AbilityIconFill;
    public Sprite AbilityIconOutline;
    public float AbilitySpeed = 10.0f;

    Vector3 m_AbilityDirection;
    float m_IconFillAmount = 1.0f;

    private void LateUpdate()
    {
        // Update when the instance is active
        if (IsActive == true)
        {
            // Removed lifespan each frame
            m_Lifespan -= Time.fixedDeltaTime;

            // Move the Ultimate until lifespan hits 0 (or collision occurs), turning off the instance when it does
            if (m_Lifespan > 0.0f)
            {
                // Move the ability instance in the desired direction
                Collider abilityCollider = GetComponent<Collider>();
                abilityCollider.attachedRigidbody.velocity = m_AbilityDirection * AbilitySpeed;
            }
            else
            {
                IsActive = false;
                DeactivateInstance();
            }
        }
    }

    public void LaunchUltimate()
    {
        if (IsActive != true && OnCooldown != true)
        {
            // Set how long the instance should be active for
            m_Lifespan = AbilityLifespanMax;

            // Set the direction and rotation of the instance, and activate the cooldown timer, before activating the ability instance
            SetAbilityDirection();
            SetAbilityRotation();
            IsActive = true;
        }
    }

    // Set the direction the ability will move towards based on the position of the owner when the call was made.
    private void SetAbilityDirection()
    {
        Vector3 dir = (AbilityOwner.transform.position + AbilityOwner.transform.forward) - AbilityOwner.transform.position;
        m_AbilityDirection = dir.normalized;
    }

    // Set the rotation the ability to the rotation of the owner when the call was made.
    private void SetAbilityRotation()
    {
        transform.rotation = AbilityOwner.transform.rotation;
    }

    public float IconFillAmount
    {
        get { return m_IconFillAmount; }
        set { m_IconFillAmount = value; }
    }
}
