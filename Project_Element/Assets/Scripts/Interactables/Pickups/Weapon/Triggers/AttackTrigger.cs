/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              AttackTrigger
Description:        Handles attack trigger for weapon
Date Created:       18/10/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/10/2021
        - [Max] Started AttackTrigger.
    19/10/2021
        - [Max] Added Enemy detection bool for Weapon Attack function.
    20/10/2021
        - [Max] Added Attack Target.
    30/10/2021
        - [Max] Added else statement to OnTriggerExit if statement.
    06/11/2021
        - [Zoe] Adjusted hit detection to use CharacterBase and not just Enemy
        - [Zoe] Edited OnTriggerExit to set values to null regardless of conditions
        - [Zoe] To accomodate Enemy and Player sharing a parent, hit detection now makes sure that the user and target aren't of the same type
    07/11/2021
        - [Max] Modified m_bHitEnemy to m_HasHit
    12/11/2021
        - [Max] Fixed Hit Detection and properly assignment of owner's tag
    19/11/2021
        - [Max] Cleaned up code
    09/03/2022
        - [Max] Weapon class refactor
        - [Max] Added Breakable object logic

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public Weapon m_Weapon;

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player" || other.tag == "Enemy" || other.tag == "Breakable") && other.isTrigger)
        {
            if (m_Weapon == null)
            {
                m_Weapon = transform.GetComponentInParent<CharacterBase>().m_MainWeapon;
            }

            // Gets colliding object's CharacterBase and Weapon
            HealthComponent healthComp = other.gameObject.GetComponent<HealthComponent>();

            // If colliding object's HealthComponent exists, weapon exists, and is NOT the same type of object as this object
            if (healthComp != null && m_Weapon != null && !IsSameType(healthComp.gameObject) && m_Weapon.IsParented() == true)
            {
                // Sets weapon's hit detection to true and assigns attack target to colliding object
                m_Weapon.SetHasHit(true);
                m_Weapon.AddAttackTarget(healthComp);
            }
            else
            {
                // Sets weapon's hit detection to false and assigns attack target to null
                m_Weapon.SetHasHit(false);
                //m_Weapon.SetAttackTarget(null);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy" || other.tag == "Breakable")
        {
            if (m_Weapon == null)
            {
                m_Weapon = transform.GetComponentInParent<CharacterBase>().m_MainWeapon;
            }

            // Gets colliding object's CharacterBase and Weapon
            HealthComponent healthComp = other.gameObject.GetComponent<HealthComponent>();

            // If colliding object's HealthComponent exists, weapon exists, and is NOT the same type of object as this object
            if (healthComp != null && m_Weapon != null && !IsSameType(healthComp.gameObject) && m_Weapon.IsParented() == true)
            {
                // Sets weapon's hit detection to true and assigns attack target to colliding object
                m_Weapon.SetHasHit(true);

                bool alreadyAdded = false;

                foreach (HealthComponent atkTarget in m_Weapon.GetAttackTargets())
                {
                    if (healthComp == atkTarget)
                    {
                        alreadyAdded = true;
                    }
                }

                if (!alreadyAdded)
                {
                    m_Weapon.AddAttackTarget(healthComp);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Gets this owner's Weapon
        //Weapon weapon = transform.parent.GetComponent<Weapon>();

        // If owner's weapon exists
        if (m_Weapon != null && m_Weapon.IsParented() == true)
        {
            // Sets weapon's hit detection to false and assigns attack target to null
            m_Weapon.SetHasHit(false);
            //m_Weapon.RemoveAttackTarget(healthComp);
        }
    }

    public void Update()
    {
        // If owner exists and this object's tag isn't already set to its owner's
        if (m_Weapon != null && m_Weapon.GetOwner() != null && transform.tag != m_Weapon.GetOwner().tag)
        {
            // Assign Owner's tag to this trigger
            transform.tag = m_Weapon.GetOwner().tag;
        }
    }

    public bool IsSameType(GameObject otherObject)
    {
        // If other CharacterBase exists
        if (otherObject != null)
        {
            // If both this' parent and other CharacterBase's tags match
            if (gameObject.transform.tag == otherObject.gameObject.tag)
            {
                return true;
            }
        }
        return false;
    }
}
