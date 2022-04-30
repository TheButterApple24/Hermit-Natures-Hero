/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CharacterBase
Description:        Base class that holds all shared information used by both players and enemies
Date Created:       06/11/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/11/2021
        - [Zoe] Started CharacterBase
    07/11/2021
        - [Zoe] Assigned the Health and Stamina Component in Start()
        - [Zoe] Renamed Health and Stamina Component to m_HealthComp and m_StaminaComp
        - [Max] Added Inventory
    08/11/2021
        - [Max] Added virtual Attack function. Also added MainWeapon
    30/01/2022
        - [Max] Added Armor to CharacterBase
	03/02/2022
        - [Max] Added virtual OnTakeDamage function
	15/03/2022
        - [Max] Attack Trigger Box GameObject + Activation call

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

public class CharacterBase : MonoBehaviour
{
    [Header("Game")]
    public HealthComponent m_HealthComp;
    public Element m_Element;
    public Inventory m_Inventory;
    public Weapon m_MainWeapon;
    public Armor m_MainArmor;
    public GameObject WeaponSocket;
    public GameObject AttackTriggerBox;

    public ParticleSystem m_BloodEffects;
    
    // Start is called before the first frame update
    virtual public void Start()
    {
        HealthComponent healthComp = gameObject.GetComponent<HealthComponent>();
        if (healthComp)
        {
            m_HealthComp = healthComp;
        }

        Inventory inventoryComp = gameObject.GetComponent<Inventory>();
        if (inventoryComp)
        {
            m_Inventory = inventoryComp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Attack()
    {
        // Activate Trigger Box
        AttackTriggerBox.SetActive(true);
    }

    public virtual void OnTakeDamage(float damage, bool isCrit)
    {
    }

    public virtual void OnDeath()
    {
    }

}
