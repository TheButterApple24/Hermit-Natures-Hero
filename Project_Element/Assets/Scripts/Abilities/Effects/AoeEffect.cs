/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Aoe Effect
Description:            Damage over time effect that will apply an amount of damage to a health component as well as slowing enemies in the collision area. Applies a slow effect once the enemy leaves the collision bounds
                            Unlike other effects, this script isn't added and removed during gameplay. This will be attached to the Area Of Effect object that is controlled by Ultimate Three script.
Date Created:           01/02/2022
Author:                 Aaron Wilson
Verified By:            Jeffrey MacNab [13/04/2022]

Changelog:

    01/02/2022
        - [Aaron] Set up the script and created it's initial values for slowing and reducing enemies that are colliding with this object.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEffect : Effect
{
    float m_Reset = 1.0f;
    public bool m_CanHurtPlayer = false;

    // Check if any enemies are inside the area of effect
    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        // If there are enemies inside the area of effect
        if (enemy)
        {
            // Use reset timer to ensure that enemies are damage every second they remain inside the area
            m_Reset -= Time.deltaTime;

            if (m_Reset <= 0.0f)
            {
                m_Reset = 1.0f;
                enemy.m_HealthComp.ModifyHealth(-EffectAmount, false);
            }
        }
        else
        {
            if (other.gameObject.tag == "Player" && m_CanHurtPlayer)
            {
                m_Reset -= Time.deltaTime;

                if (m_Reset <= 0.0f)
                {
                    m_Reset = 1.0f;
                    PlayerManager.Instance.Player.m_HealthComp.ModifyHealth(-2.0f, false);
                }
            }
        }
    }

    // When an enemy leaves the area of effect, they gain a small movement debuff / slow effect
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        // If the enemies leave the area of effect
        if (enemy)
        {
            ApplySlowEffect(enemy.gameObject);
        }
    }

    void ApplySlowEffect(GameObject affectedObj)
    {
        affectedObj.AddComponent<SlowEffect>();
        affectedObj.GetComponent<SlowEffect>().SetEffectAmountAndDuration(0.0f, 3.0f);
        affectedObj.GetComponent<SlowEffect>().SlowCharacter();
    }

    // Once the duration end, reset the parent's trigger and turn off this area of effect object
    IEnumerator AffectTimer()
    {
        yield return new WaitForSeconds(EffectDuration);

        this.gameObject.SetActive(false);
    }

    // Starts the affect timer from outside the class
    public void StartAffectTimer()
    {
        StartCoroutine(AffectTimer());
    }

    public void PlaySound()
    {
        // Play Ignite sound effect when a fireability is used
        if (AbilitySoundManager.sfxInstance.BonfireLinger)
        {
            AbilitySoundManager.sfxInstance.AbilityAudio.loop = true;
            AbilitySoundManager.sfxInstance.AbilityAudio.PlayOneShot(AbilitySoundManager.sfxInstance.SteamBombIgnite);
        }
    }
}
