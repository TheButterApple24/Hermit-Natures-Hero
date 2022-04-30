/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              InventoryUI
Description:        Creates a bunch of ItemSlots that the inventory will use
Date Created:       26/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/11/2021
        - [Jeffrey] Created base class
    10/04/2022
        - [Jeffrey] Refactored Skill Tree

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject m_SlotPrefab;
    public int m_MaxSize;

    [HideInInspector] public List<UIItemSlot> m_ItemSlots;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < m_MaxSize; i++)
        {
            GameObject newSlot = Instantiate(m_SlotPrefab, transform);
            m_ItemSlots.Add(newSlot.GetComponent<UIItemSlot>());
        }
    }

    public void AddInventorySlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newSlot = Instantiate(m_SlotPrefab, transform);
            m_ItemSlots.Add(newSlot.GetComponent<UIItemSlot>());
        }
    }
}
