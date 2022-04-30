/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  BreakableObject
Description:            An object that the player can destroy with their weapon. Can spawn loot (optional)
Date Created:           22/03/2021
Author:                 Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    22/03/2021
        - [Zoe] Created BreakableObject

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private LootSystem m_LootSystem;
    [SerializeField] protected HealthComponent m_HealthComp;
    public HealthComponent HealthComp { get { return m_HealthComp; } }

    public Transform DmgNumbersLocation;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        gameObject.tag = "Breakable";
        m_HealthComp.MaxHealth = 0.1f;

        if (DmgNumbersLocation == null)
        {
            DmgNumbersLocation = gameObject.transform;
        }

        if (m_LootSystem != null)
            m_LootSystem.GenerateRandomLoot();
    }
    virtual public void Break()
    {
        if (m_LootSystem != null)
            m_LootSystem.SpawnLoot();

        Destroy(gameObject);
    }
}
