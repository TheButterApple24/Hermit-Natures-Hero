/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              RootEffect
Description:        Roots the enemy to the spot when hit back this effect. Alters speed to 0 and resets it at the end of the effect.
Date Created:       06/12/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Aaron] Set up the variables and functions for duration and amount to be applied to a character.
    08/12/2021
        - [Aaron] Added a hard coded fix to reset the enemies movement speed back to their original value of 5.0f
    30/01/2022
        - [Aaron] Fixed hard code by setting the original speed value in start and removed the other variables from the start, as the important one is passed in through the editor

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootEffect : Effect
{
    Enemy m_AffectedEnemy;
    float m_OriginalSpeed = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        if (AffectedCharacter)
        {
            m_AffectedEnemy = GetComponent<Enemy>();

            if (m_AffectedEnemy)
            {
                EnemyBehaviour enemy = GetComponent<EnemyBehaviour>();
                m_OriginalSpeed = enemy.m_Speed;
            }
        }
    }

    // Grab the enemy behaviour from the character object, set the speed to 0, and start the coroutine to reset their speed
    public void RootCharacter()
    {
        if (m_AffectedEnemy)
        {
            m_AffectedEnemy.gameObject.GetComponent<EnemyBehaviour>().m_Speed = 0;

            StartCoroutine(RootTimer());
        }
    }

    // Once the timer is finished, reset the character's speed and destroy this script
    IEnumerator RootTimer()
    {
        yield return new WaitForSeconds(EffectDuration);

        if (m_AffectedEnemy)
        {
            m_AffectedEnemy.gameObject.GetComponent<EnemyBehaviour>().m_Speed = m_OriginalSpeed;
        }

        Destroy(this);
    }
}
