/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  SlowEffect
Description:         Alters speed of object with this script attached, reducing it by half and then resetting the original value once the duration is over
Date Created:       01/02/2022
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/02/2022
        - [Aaron] Set up the variables and functions for duration and amount to be applied to a character. Function's similar to Root Script

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEffect : Effect
{
    Enemy m_AffectedEnemy;
    float m_OriginalSpeed = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        if (AffectedCharacter)
        {
            m_AffectedEnemy = gameObject.GetComponent<Enemy>();

            if (m_AffectedEnemy)
            {
                EnemyBehaviour enemy = GetComponent<EnemyBehaviour>();
                m_OriginalSpeed = enemy.m_Speed;
            }
        }
    }

    // Grab the enemy behaviour from the character object, set the speed to 0, and start the coroutine to reset their speed
    public void SlowCharacter()
    {

        if (m_AffectedEnemy)
        {
            m_AffectedEnemy.gameObject.GetComponent<EnemyBehaviour>().m_Speed = m_OriginalSpeed * 0.5f;

            StartCoroutine(SlowTimer());
        }
    }

    // Once the timer is finished, reset the character's speed and destroy this script
    IEnumerator SlowTimer()
    {
        yield return new WaitForSeconds(EffectDuration);

        if (m_AffectedEnemy)
        {
            m_AffectedEnemy.gameObject.GetComponent<EnemyBehaviour>().m_Speed = m_OriginalSpeed;
        }

        Destroy(this);
    }
}