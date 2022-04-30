/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LootSystem
Description:        Handles loot system for chests and enemies
Date Created:       19/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Jeffrey] Created base implementation
    03/02/2022
        - [Max] Fixed static loot weapon drop bug
    08/02/2022
        - [Max] Added Comments

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public GameObject[] m_Loot;
    public bool m_IsChest = false;
    [Range(1, 5)]
    [SerializeField] private int m_RandomLootMin = 2;
    [Range(1, 5)]
    [SerializeField] private int m_RandomLootMax = 6;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        if (m_RandomLootMin > m_RandomLootMax)
        {
            m_RandomLootMin = m_RandomLootMax;
        }

        if (m_RandomLootMax < m_RandomLootMin)
        {
            m_RandomLootMax = m_RandomLootMin;
        }
    }

    public void GenerateRandomLoot()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        m_Loot = new GameObject[Random.Range(m_RandomLootMin, m_RandomLootMax)];

        for (int i = 0; i < m_Loot.Length; i++)
        {
            if (m_IsChest)
            {
                m_Loot[i] = gameManager.m_PotentialChestLoot[Random.Range(0, gameManager.m_PotentialChestLoot.Length)];
            }
            else
            {
                m_Loot[i] = gameManager.m_PotentialEnemyLoot[Random.Range(0, gameManager.m_PotentialEnemyLoot.Length)];
            }
        }
    }

    public void SpawnLoot()
    {
        for (int i = 0; i < m_Loot.Length; i++)
        {
            GameObject lootObject = Instantiate(m_Loot[i], gameObject.transform.position, gameObject.transform.rotation);

            // If loot object is a Weapon
            if (lootObject.GetComponent<Weapon>() != null)
            {
                // Set kinematic to false and enable box collider
                lootObject.GetComponent<Rigidbody>().isKinematic = false;
                lootObject.GetComponent<BoxCollider>().enabled = true;
            }

            if (m_IsChest)
            {
                lootObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 2), 3, Random.Range(0, 2)), ForceMode.Impulse);
            }
            else
            {
                lootObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 5), 5, Random.Range(0, 5)), ForceMode.Impulse);
            }

        }
    }
}
