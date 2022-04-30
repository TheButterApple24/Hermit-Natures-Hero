/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CraftingMenu
Description:        Handles the functionality for creating potions from 2 reagents, referencing crafting recipes
Date Created:       02/02/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/02/2022
        - [Zoe] Created base class
    02/02/2022
        - [Zoe] Added Ingredient strings and UI input slots
    02/03/2022
        - [Zoe] Potion crafting implementation added
        - [Zoe] Resulting potions now get added to the inventory
        - [Zoe] Misc bug fixes
    02/11/2022
        - [Zoe] Added the Potion Popup, an image that appears when you've successfully made a potion
    02/21/2022
        - [Zoe] Crafting inventory slots get updated/refreshed upon opening the menu
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    08/03/2022
        - [Aaron] Added a first time use check to the open menu function.
    11/03/2022
        - [Zoe] Added Audio Source and SFX for Menu Opening/Closing

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PotionTypes;

public class CraftingMenu : MonoBehaviour
{
    [Header("Crafting")]
    public Player m_Player;
    public CraftingRecipe[] m_CraftingRecipes;

    public UIItemSlot m_InputSlot1;
    public UIItemSlot m_InputSlot2;

    public string m_Ingredient1;
    public string m_Ingredient2;

    public Sprite[] m_PotionPopupSprites;
    public float m_PopupDuration = 1.5f;

    private CraftingClickHandler m_ClickHandler;
    private float m_PopupTimer;
    private int m_NumCraftedPotions = 1;
    [SerializeField] private Image m_PotionPopupObject;
    [SerializeField] private Text m_PotionPopupCount;


    [Header("Tutorial")]
    bool m_CraftingUsed = false;
    public string CraftingTutorialText = "";

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_OpenSFX;
    [SerializeField] private AudioClip m_CraftSFX;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        m_ClickHandler = gameObject.GetComponent<CraftingClickHandler>();
        m_Player = PlayerManager.Instance.Player;
    }

    private void Update()
    {
        // If the new potion popup is currently on screen
        if (m_PotionPopupObject.gameObject.activeSelf == true)
        {
            // Advance the timer
            m_PopupTimer += Time.unscaledDeltaTime;

            // If the timer is complete, OR the player left-clicked to skip the popup
            if (m_PopupTimer >= m_PopupDuration || Input.GetMouseButton(0))
            {
                // Turn off the popup
                m_PotionPopupObject.gameObject.SetActive(false);

                // Reset the timer
                m_PopupTimer = 0.0f;
                m_NumCraftedPotions = 1;
            }
        }
    }

    public void OpenCraftingMenu()
    {
        if (m_CraftingUsed == false)
        {
            m_CraftingUsed = true;
            string titleText = "Crafting Menu";
            m_Player.PlayerUI.DisplayTutorialOnFirstRun(titleText, CraftingTutorialText);
        }

        m_Player.PlayerUI.DisableBackpackMenuButtons();
        gameObject.SetActive(true);

        m_AudioSource.PlayOneShot(m_OpenSFX);
        InventoryUI inventory = transform.GetChild(0).GetComponent<InventoryUI>();

        // If it is not null, disable highlight for all slots
        if (inventory != null)
        {
            for (int i = 0; i < inventory.m_ItemSlots.Count; i++)
            {
                inventory.m_ItemSlots[i].UpdateSlot();
            }
        }
    }

    public void CloseCraftingMenu()
    {
        m_ClickHandler.HandleEmptySlots();
        EmptyInputSlots();
        m_Player.PlayerUI.EnableBackpackMenuButtons();
        m_AudioSource.PlayOneShot(m_OpenSFX);
        gameObject.SetActive(false);
    }

    public void MixPotion()
    {
        // If there is an item in both input slots
        if (m_InputSlot1.m_ItemSlot != null && m_InputSlot2.m_ItemSlot != null)
        {
            // Get the recipe that matches the two ingredients, if any
            CraftingRecipe recipe = GetValidRecipe(m_Ingredient1, m_Ingredient2);

            // If a matching recipe was found
            if (recipe != null)
            {
                while (m_InputSlot1.m_ItemSlot.m_Stack.ItemAmount > 0 && m_InputSlot2.m_ItemSlot.m_Stack.ItemAmount > 0)
                {
                    // Add the resulting potion to the inventory
                    AddPotionToInventory(recipe);

                    m_NumCraftedPotions++;

                    // Consume 1 item from slot 1
                    m_InputSlot1.m_ItemSlot.m_Stack.ItemAmount--;

                    // Update the slot's count
                    m_InputSlot1.UpdateSlot();

                    // Consume 1 item from slot 2
                    m_InputSlot2.m_ItemSlot.m_Stack.ItemAmount--;

                    // Update the slot's count
                    m_InputSlot2.UpdateSlot();
                }

                if (m_InputSlot1.m_ItemSlot.m_Stack.ItemAmount == 0)
                {
                    // Delete and clear the slot's item stack
                    m_ClickHandler.DeleteStack(m_InputSlot1);
                    m_ClickHandler.m_LeftSlotName.text = "";
                    m_ClickHandler.HandleEmptySlots();
                }

                if (m_InputSlot2.m_ItemSlot.m_Stack.ItemAmount == 0)
                {
                    // Delete and clear the slot's item stack
                    m_ClickHandler.DeleteStack(m_InputSlot2);
                    m_ClickHandler.m_RightSlotName.text = "";
                    m_ClickHandler.HandleEmptySlots();
                }
            }
        }
    }
    public CraftingRecipe GetValidRecipe(string ingredient1, string ingredient2)
    {
        // Loop through each known crafting recipe
        foreach (CraftingRecipe recipe in m_CraftingRecipes)
        {
            // If the item in slot 1 matches one of the two ingredients in the recipe
            if (ingredient1 == recipe.Reagent1.ToString() || ingredient1 == recipe.Reagent2.ToString())
            {
                // If the item in slot 2 matches one of the two ingredients in the recipe
                if (ingredient2 == recipe.Reagent1.ToString() || ingredient2 == recipe.Reagent2.ToString())
                {
                    if (ingredient1 != ingredient2)
                    {
                        // return this recipe, as it was match
                        return recipe;
                    }
                }
            }
        }

        // A matching recipe was not found
        return null;
    }

    public void AddPotionToInventory(CraftingRecipe recipe)
    {
        GameObject potion = new GameObject();

        // Check the resulting potion from the matching recipe, and add it to the inventory
        // Additionally setting the sprite for the "Potion Crafted!" Popup to the respective potion
        switch (recipe.ResultingPotion)
        {
            case PotionType.Health:
                potion.AddComponent<HealthPotion>();
                m_PotionPopupObject.sprite = m_PotionPopupSprites[0];
                break;
            case PotionType.Stamina:
                potion.AddComponent<StaminaPotion>();
                m_PotionPopupObject.sprite = m_PotionPopupSprites[1];
                break;
            case PotionType.Attack:
                potion.AddComponent<AttackPotion>();
                m_PotionPopupObject.sprite = m_PotionPopupSprites[2];
                break;
            case PotionType.Defense:
                potion.AddComponent<DefensePotion>();
                m_PotionPopupObject.sprite = m_PotionPopupSprites[3];
                break;
            case PotionType.Poison:
                potion.AddComponent<PoisonPotion>();
                m_PotionPopupObject.sprite = m_PotionPopupSprites[4];
                break;
        }

        // Initialize and activate the potion (add to inventory)
        potion.GetComponent<Potion>().InitPotion();
        potion.GetComponent<Potion>().Activate();

        // Turn on the popup
        m_PotionPopupCount.text = "x " + m_NumCraftedPotions.ToString();
        m_PotionPopupObject.gameObject.SetActive(true);
        m_AudioSource.PlayOneShot(m_CraftSFX);
    }

    public void EmptyInputSlots()
    {
        // While there is an existing item in the first/left item slot
        while (m_InputSlot1.m_ItemSlot != null && m_InputSlot1.m_ItemSlot.m_Stack != null)
        {
            // If there is more than 0 items
            if (m_InputSlot1.m_ItemSlot.m_Stack.ItemAmount > 0)
            {
                // Decrement the count and move the item back to the inventory
                m_ClickHandler.DecrementSlot(m_InputSlot1);
            }
        }

        // While there is an existing item in the second/right item slot
        while (m_InputSlot2.m_ItemSlot != null && m_InputSlot2.m_ItemSlot.m_Stack != null)
        {
            // If there is more than 0 items
            if (m_InputSlot2.m_ItemSlot.m_Stack.ItemAmount > 0)
            {
                // Decrement the count and move the item back to the inventory
                m_ClickHandler.DecrementSlot(m_InputSlot2);
            }
        }
    }
}
