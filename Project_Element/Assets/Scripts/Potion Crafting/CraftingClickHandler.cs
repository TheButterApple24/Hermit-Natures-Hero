/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CraftingClickHandler
Description:        Handles left and right click interaction with the reagent inventory, in the crafting menu
Date Created:       02/02/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/02/2022
        - [Zoe] Created base class
    02/03/2022
        - [Zoe] Commented code. Removed Start
        - [Zoe] Misc bug fixes
        - [Jeffrey] More bug fixes
    02/11/2022
        - [Zoe] Added the reagent name to be place above the input slots
    16/02/2022
        - [Jeffrey] Implemented Highlights
    10/03/2022
        - [Zoe] Added Audio Source and SFX for Reagent Selecting
    15/03/2022
        - [Zoe] Bug fixes, as well as adding a box that shows the reagent name on mouse over

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingClickHandler : MonoBehaviour
{
    private ItemSlot m_CursorItemSlot;
    private PointerEventData m_PointerEventData;

    public CraftingMenu m_CraftingMenu;
    public Player m_Player;

    public GameObject m_LeftClickPrompt;
    public GameObject m_RightClickPrompt;

    public Text m_LeftSlotName;
    public Text m_RightSlotName;

    UIItemSlot m_LeftClickedInventorySlot;
    UIItemSlot m_RightClickedInventorySlot;
    UIItemSlot m_LastHoveredSlot = null;

    [SerializeField] private GraphicRaycaster m_Raycaster = null;
    [SerializeField] private EventSystem m_EventSystem = null;

    [SerializeField] private GameObject m_LocationBox;
    [SerializeField] private Text m_LocationName;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_SelectionSFX;
    [SerializeField] private AudioClip m_ClickSFX;

    void Update()
    {
        // Skip button checks if the crafting menu is not open
        if (!m_Player.m_IsMenuOpen)
            return;

        HandleHighlight();

        // If left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            HandleSlotLeftClick(CheckForSlot());
        }

        // If right mouse button is clicked
        if (Input.GetMouseButtonDown(1))
        {
            HandleSlotRightClick(CheckForSlot());
        }

        if (m_LocationBox.activeSelf == true)
        {
            m_LocationBox.transform.position = Input.mousePosition;
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

            if (slot.m_ItemSlot != null)
            {
                // Enable the location text box and set it's location to the mouse
                m_LocationBox.SetActive(true);
                m_LocationBox.transform.position = Input.mousePosition;

                // Set the location name to the reagents's name
                m_LocationName.text = slot.m_ItemSlot.m_Stack.ItemName;
            }
        }
        else
        {
            // If the last hovered slot is not null, disable it
            if (m_LastHoveredSlot != null)
            {
                m_LastHoveredSlot.m_IsBeingHovered = false;
                m_LastHoveredSlot = null;

                m_LocationBox.SetActive(false);
            }
        }
    }

    private void HandleSlotLeftClick(UIItemSlot clickedSlot)
    {
        // If what you clicked on is a valid slot
        if (clickedSlot != null)
        {
            // If an item slot exists
            if (clickedSlot.m_ItemSlot != null)
            {
                // If the ingredient input slot doesn't have anything in it
                if (m_CraftingMenu.m_InputSlot1.m_ItemSlot == null)
                {
                    // Send the clicked-on reagent to the crafting menu, as the first ingredient
                    m_CraftingMenu.m_Ingredient1 = clickedSlot.m_ItemSlot.m_Stack.ItemTitle;

                    // Store the clicked-on slot. This allows you to place them back into the inventory
                    m_LeftClickedInventorySlot = clickedSlot;

                    // Set the name above the slot to the name of the reagent
                    m_LeftSlotName.text = clickedSlot.m_ItemSlot.m_Stack.ItemName;

                    // Create a new item stack in this slot
                    CreateStack(clickedSlot, m_CraftingMenu.m_InputSlot1);
                    m_LeftClickPrompt.SetActive(false);
                    m_AudioSource.PlayOneShot(m_SelectionSFX);
                }
                else
                {
                    if (clickedSlot == m_LeftClickedInventorySlot)
                    {
                        IncrementSlot(m_CraftingMenu.m_InputSlot1);
                        m_AudioSource.PlayOneShot(m_SelectionSFX);
                    }
                }
            }
        }
    }

    private void HandleSlotRightClick(UIItemSlot clickedSlot)
    {
        // If what you clicked on is a valid slot
        if (clickedSlot != null)
        {
            // If an item slot exists
            if (clickedSlot.m_ItemSlot != null)
            {
                // If the ingredient input slot doesn't have anything in it
                if (m_CraftingMenu.m_InputSlot2.m_ItemSlot == null)
                {
                    // Send the clicked-on reagent to the crafting menu, as the second ingredient
                    m_CraftingMenu.m_Ingredient2 = clickedSlot.m_ItemSlot.m_Stack.ItemTitle;

                    // Store the clicked-on slot. This allows you to place them back into the inventory
                    m_RightClickedInventorySlot = clickedSlot;

                    // Set the name above the slot to the name of the reagent
                    m_RightSlotName.text = clickedSlot.m_ItemSlot.m_Stack.ItemName;

                    // Create a new item stack in this slot
                    CreateStack(clickedSlot, m_CraftingMenu.m_InputSlot2);
                    m_RightClickPrompt.SetActive(false);
                    m_AudioSource.PlayOneShot(m_SelectionSFX);
                }
                else
                {
                    if (clickedSlot == m_RightClickedInventorySlot)
                    {
                        IncrementSlot(m_CraftingMenu.m_InputSlot2);
                        m_AudioSource.PlayOneShot(m_SelectionSFX);
                    }
                }
            }
        }
    }

    private UIItemSlot CheckForSlot()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot")
            {
                return result.gameObject.GetComponent<UIItemSlot>();
            }
        }

        return null;
    }

    public void IncrementSlot(UIItemSlot itemInputSlot)
    {
        // If the item slot exists
        if (itemInputSlot.m_ItemSlot != null)
        {
            // If the current stack is in the left input / "ingredient 1" slot
            if (itemInputSlot == m_CraftingMenu.m_InputSlot1)
            {
                // If there is currently items in the inventory that can be moved over
                if (m_LeftClickedInventorySlot.m_ItemSlot != null && m_LeftClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount > 0)
                {
                    // Remove from the old slot in the inventory
                    m_LeftClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount--;

                    // Update the inventory slot's count
                    m_LeftClickedInventorySlot.UpdateSlot();

                    UpdateUISlot(m_LeftClickedInventorySlot.m_ItemSlot.m_Stack);

                    // If there are no more of this item left in the stack
                    if (m_LeftClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount == 0)
                    {
                        // Delete the stack and empty the slot
                        DeleteStack(m_LeftClickedInventorySlot);
                        m_LeftClickedInventorySlot.m_ItemSlot = null;
                    }

                    // Add the item to the input position
                    itemInputSlot.m_ItemSlot.m_Stack.ItemAmount++;
                    m_AudioSource.PlayOneShot(m_ClickSFX);

                    // Update the input slot's count
                    itemInputSlot.UpdateSlot();
                }
            }
            // If the current stack is in the right input / "ingredient 2" slot
            else if (itemInputSlot == m_CraftingMenu.m_InputSlot2)
            {
                // If there is currently items in the inventory that can be moved over
                if (m_RightClickedInventorySlot.m_ItemSlot != null && m_RightClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount > 0)
                {
                    // Remove from the old slot in the inventory
                    m_RightClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount--;

                    // Update the inventory slot's count
                    m_RightClickedInventorySlot.UpdateSlot();

                    UpdateUISlot(m_RightClickedInventorySlot.m_ItemSlot.m_Stack);

                    // If there are no more of this item left in the stack
                    if (m_RightClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount == 0)
                    {
                        // Delete the stack and empty the slot
                        DeleteStack(m_RightClickedInventorySlot);
                        m_RightClickedInventorySlot.m_ItemSlot = null;
                    }

                    // Add the item to the input position
                    itemInputSlot.m_ItemSlot.m_Stack.ItemAmount++;

                    // Update the input slot's count
                    itemInputSlot.UpdateSlot();
                }
            }
        }
    }

    public void DecrementSlot(UIItemSlot itemInputSlot)
    {
        // If the item slot exists
        if (itemInputSlot.m_ItemSlot != null)
        {
            // If the current stack is in the left input / "ingredient 1" slot
            if (itemInputSlot == m_CraftingMenu.m_InputSlot1)
            {
                // If there is currently items in the input slot that can be moved back to the inventory
                if (itemInputSlot.m_ItemSlot.m_Stack.ItemAmount > 0)
                {
                    // If the slot in the inventory is empty
                    if (m_LeftClickedInventorySlot.m_ItemSlot == null)
                    {
                        CreateStack(itemInputSlot, m_LeftClickedInventorySlot);
                    }

                    // Add item back to the inventory
                    m_LeftClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount++;

                    // Update the inventory slot's count
                    m_LeftClickedInventorySlot.UpdateSlot();

                    // Remove item from the input position
                    itemInputSlot.m_ItemSlot.m_Stack.ItemAmount--;
                    m_AudioSource.PlayOneShot(m_ClickSFX);

                    // Update the input slot's count
                    itemInputSlot.UpdateSlot();

                    UpdateUISlot(m_LeftClickedInventorySlot.m_ItemSlot.m_Stack);

                    // If there are no more items in the input slot's item stack
                    if (itemInputSlot.m_ItemSlot.m_Stack.ItemAmount == 0)
                    {
                        // Delete the stack and reset the slot to null
                        DeleteStack(itemInputSlot);
                        itemInputSlot.m_ItemSlot = null;

                        // Remove the name above the slot and re-enable the mouse click prompt
                        m_LeftSlotName.text = "";
                        m_LeftClickPrompt.SetActive(true);
                    }
                }
            }
            // If the current stack is in the left input / "ingredient 1" slot
            else if (itemInputSlot == m_CraftingMenu.m_InputSlot2)
            {
                // If there is currently items in the input slot that can be moved back to the inventory
                if (itemInputSlot.m_ItemSlot.m_Stack.ItemAmount > 0)
                {
                    // If the slot in the inventory is empty
                    if (m_RightClickedInventorySlot.m_ItemSlot == null)
                    {
                        // Recreate the stack that was in it
                        CreateStack(itemInputSlot, m_RightClickedInventorySlot);
                    }

                    // Add item back to the inventory
                    m_RightClickedInventorySlot.m_ItemSlot.m_Stack.ItemAmount++;

                    // Update the inventory slot's count
                    m_RightClickedInventorySlot.UpdateSlot();

                    // Remove item from the input position
                    itemInputSlot.m_ItemSlot.m_Stack.ItemAmount--;

                    // Update the input slot's count
                    itemInputSlot.UpdateSlot();

                    UpdateUISlot(m_RightClickedInventorySlot.m_ItemSlot.m_Stack);

                    // If there are no more items in the input slot's item stack
                    if (itemInputSlot.m_ItemSlot.m_Stack.ItemAmount == 0)
                    {
                        // Delete the stack and reset the slot to null
                        DeleteStack(itemInputSlot);
                        itemInputSlot.m_ItemSlot = null;

                        // Remove the name above the slot and re-enable the mouse click prompt
                        m_RightSlotName.text = "";
                        m_RightClickPrompt.SetActive(true);
                    }
                }
            }
        }
    }

    public void CreateStack(UIItemSlot oldSlot, UIItemSlot inputSlot)
    {
        ItemStack newStack = new ItemStack();
        newStack.ItemID = oldSlot.m_ItemSlot.m_Stack.ItemID;
        newStack.ItemName = oldSlot.m_ItemSlot.m_Stack.ItemName;
        newStack.ItemTitle = oldSlot.m_ItemSlot.m_Stack.ItemTitle;
        newStack.ItemDescription = oldSlot.m_ItemSlot.m_Stack.ItemDescription;

        ItemSlot newSlot = new ItemSlot(inputSlot);
        newSlot.InsertStack(newStack);
        IncrementSlot(inputSlot);
    }

    public void DeleteStack(UIItemSlot itemSlot)
    {
        itemSlot.m_ItemSlot.EmptySlot();
        itemSlot.UnLink();
        itemSlot.Clear();
    }

    public void HandleEmptySlots()
    {
        // Loop through each of the players reagents and if any are null, reset them
        for (int i = 0; i < m_Player.m_Inventory.m_CurrentReagents.Count; i++)
        {
            if (m_Player.m_Inventory.m_CurrentReagents[i].ItemAmount <= 0)
            {
                m_Player.m_Inventory.m_CurrentReagents[i] = null;
                m_Player.m_Inventory.m_CurrentReagents[i] = new ItemStack();
            }
        }
    }

    public void UpdateUISlot(ItemStack stack)
    {
        // Loop through each of the players reagents and if the passed in id is found, copy the data
        for (int i = 0; i < m_Player.m_Inventory.m_CurrentReagents.Count; i++)
        {
            if (m_Player.m_Inventory.m_CurrentReagents[i].ItemID == stack.ItemID)
            { 
               m_Player.m_Inventory.m_CurrentReagents[i].ItemID = stack.ItemID;
               m_Player.m_Inventory.m_CurrentReagents[i].ItemAmount = stack.ItemAmount;
            }
        }
    }
}
