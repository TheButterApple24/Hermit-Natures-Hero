/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ItemSlotInteraction
Description:        Handles interaction with ItemSlots in the UI
Date Created:       26/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/11/2021
        - [Jeffrey] Created base class
    28/11/2021
        - [Jeffrey] Re-factored functions to set if slot is selected
    26/01/2022
        - [Max] Changes Weapons specific calls to Equipment
    16/02/2022
        - [Jeffrey] Implemented Highlights

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotInteraction : MonoBehaviour
{
    [SerializeField] private UIItemSlot m_CursorSlot = null;
    private ItemSlot m_CursorItemSlot;

    [SerializeField] private GraphicRaycaster m_Raycaster = null;
    private PointerEventData m_PointerEventData;

    [SerializeField] private EventSystem m_EventSystem = null;

    public Player m_Player;

    UIItemSlot m_LastHoveredSlot = null;

    void Start()
    {
        m_CursorItemSlot = new ItemSlot(m_CursorSlot);
    }

    void Update()
    {
        if (!m_Player.m_IsMenuOpen)
            return;

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            // Use Item
            HandleSlotLeftClick(CheckForSlot());
        }
    }

    private void HandleHighlight()
    {
        // Store the return value of CheckForSlot and check if its null. 
        UIItemSlot slot = CheckForSlot();
        if (slot != null)
        {
            // Grab the inventory ui from the parent object.
            InventoryUI inventory = slot.transform.parent.gameObject.GetComponent<InventoryUI>();

            // If it is not null, disable highlight for all slots
            if (inventory != null)
            {
                for (int i = 0; i < inventory.m_ItemSlots.Count; i++)
                {
                    inventory.m_ItemSlots[i].m_IsBeingHovered = false;
                }
            }

            // Store the slot and set IsBeingHighlighted to true
            m_LastHoveredSlot = slot;
            slot.m_IsBeingHovered = true;
        }
        else
        {
            // If the last hovered slot is not null, disable it
            if (m_LastHoveredSlot != null)
            {
                m_LastHoveredSlot.m_IsBeingHovered = false;
                m_LastHoveredSlot = null;
            }
        }
    }

    private void HandleSlotLeftClick(UIItemSlot clickedSlot)
    {
        if (clickedSlot != null && clickedSlot.m_ItemSlot == null)
        {
            m_Player.m_Inventory.DisableSelected();
            return;
        }

        int index = 0;
        foreach (UIItemSlot slot in m_Player.m_Inventory.m_PotionsUI.m_ItemSlots)
        {
            if (slot == clickedSlot)
            {
                //m_Player.m_Inventory.UsePotion(index);
                //m_Player.m_Inventory.UpdateAllSlots();

                m_Player.m_Inventory.DisableSelected();
                m_Player.m_Inventory.SelectPotion(index);
                return;
            }
            index++;
        }

        index = 0;
        foreach (UIItemSlot slot in m_Player.m_Inventory.EquipmentUI.m_ItemSlots)
        {
            if (slot == clickedSlot)
            {
                //m_Player.m_Inventory.EquipWeapon(index);
                //m_Player.m_Inventory.UpdateAllSlots();

                m_Player.m_Inventory.DisableSelected();
                m_Player.m_Inventory.SelectEquipment(index);
                return;
            }
            index++;
        }

    }

    private void HandleSlotRightClick(UIItemSlot clickedSlot)
    {
        //if (clickedSlot == null)
        //    return;

        //int index = 0;
        //foreach(UIItemSlot slot in m_Player.m_Inventory.m_WeaponsUI.m_ItemSlots)
        //{
        //    if (slot == clickedSlot)
        //    {
        //        m_Player.m_Inventory.RemoveFromInventory(index);
        //        m_Player.m_Inventory.UpdateAllSlots();
        //        return;
        //    }
        //    index++;
        //}
    }

    private UIItemSlot CheckForSlot()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot")
            {
                return result.gameObject.GetComponent<UIItemSlot>();
            }
        }

        return null;
    }
}
