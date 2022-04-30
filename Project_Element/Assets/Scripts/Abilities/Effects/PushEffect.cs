/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PushEffect
Description:        Pushes the enemy away from the player when hit back this effect. They should travel the push distance over the push duration.
Date Created:       18/11/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Aaron] Set up the variables and functions for duration and amount to be applied to a character.
    26/01/2022
        - [Aaron] Created a SetPushDirection function and variable responsible for passing a direction calculated outside the class into this class so KnockBack can be applied correctly.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushEffect : Effect
{
    Vector3 m_PushDirection;

    private void Update()
    {
        if (IsActive)
        {
            if (AffectedCharacter)
            {
                KnockBackEnemy(AffectedCharacter);
            }
        }
    }

    // Updates the character object's position in based on the direction of contact and strength of knockback over the course of a second.
    void KnockBackEnemy(CharacterBase charObj)
    {

            Vector3 abilityDir = m_PushDirection;
            Vector3 knockbackPos = charObj.gameObject.transform.position + (abilityDir * EffectAmount * Time.deltaTime);
            charObj.gameObject.transform.position = knockbackPos;
    }

    public void SetPushDirection(Vector3 dir)
    {
        m_PushDirection = dir;
    }
}
