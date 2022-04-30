/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Potion
Description:        Handles potion item
Date Created:       13/03/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    13/03/2021
        - [Zoe] Created base file

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionHotbar : MonoBehaviour
{
    private Potion m_Potion1;
    [SerializeField] private Image m_Slot1Image;
    [SerializeField] private Text m_Potion1CountText;

    private Potion m_Potion2;
    [SerializeField] private Image m_Slot2Image;
    [SerializeField] private Text m_Potion2CountText;

    [SerializeField] private Sprite[] m_PotionSprites;

// Update is called once per frame
    void Update()
    {
        // Use the potion in slot 1
        if (Input.GetButtonDown("PotionSlot1"))
        {
            if (m_Potion1 != null)
            {
                PlayerManager.Instance.Player.m_Inventory.UsePotion(m_Potion1.m_ItemID);
            }
        }

        // Use the potion in slot 2
        if (Input.GetButtonDown("PotionSlot2"))
        {
            if (m_Potion2 != null)
            {
                PlayerManager.Instance.Player.m_Inventory.UsePotion(m_Potion2.m_ItemID);
            }
        }

        float dpadInput = Input.GetAxisRaw("PotionSlotController");

        if (dpadInput < 0)
        {
            PlayerManager.Instance.Player.m_Inventory.UsePotion(m_Potion1.m_ItemID);
        }
        else if (dpadInput > 0)
        {
            PlayerManager.Instance.Player.m_Inventory.UsePotion(m_Potion2.m_ItemID);
        }

        // If the potion amount stored in the hotbar doesn't match what's in the inventory, update it

        if (m_Potion1 != null)
        {
            ItemStack potion1 = PlayerManager.Instance.Player.m_Inventory.HasItem(m_Potion1.m_ItemID);

            if (potion1 != null)
            {
                m_Potion1CountText.text = potion1.ItemAmount.ToString();
            }
            else
            {
                m_Potion1CountText.text = "0";
            }
        }

        if (m_Potion2 != null)
        {
            ItemStack potion2 = PlayerManager.Instance.Player.m_Inventory.HasItem(m_Potion2.m_ItemID);

            if (potion2 != null)
            {
                m_Potion2CountText.text = potion2.ItemAmount.ToString();
            }
            else
            {
                m_Potion2CountText.text = "0";
            }
        }
    }

    public void AssignSlot(Potion potion, int slotNum)
    {
        // Take the data from the inventory, update and enable this side of the hotbar
        if (slotNum == 1)
        {
            m_Slot1Image.sprite = m_PotionSprites[(int)potion.m_PotionType];
            //m_Potion1Count = potionCount;
            m_Potion1 = potion;
            m_Potion1CountText.text = PlayerManager.Instance.Player.m_Inventory.HasItem(m_Potion1.m_ItemID).ItemAmount.ToString();
            m_Slot1Image.gameObject.SetActive(true);
        }

        // Take the data from the inventory, update and enable this side of the hotbar
        if (slotNum == 2)
        {
            m_Slot2Image.sprite = m_PotionSprites[(int)potion.m_PotionType];
            //m_Potion2Count = potionCount;
            m_Potion2 = potion;
            m_Potion2CountText.text = PlayerManager.Instance.Player.m_Inventory.HasItem(m_Potion2.m_ItemID).ItemAmount.ToString();
            m_Slot2Image.gameObject.SetActive(true);
        }
    }

    public Potion GetPotion1() { return m_Potion1; }
    public Potion GetPotion2() { return m_Potion2; }
    public int GetPotion1Count() { return int.Parse(m_Potion1CountText.text); }
    public int GetPotion2Count() { return int.Parse(m_Potion2CountText.text); }
}
