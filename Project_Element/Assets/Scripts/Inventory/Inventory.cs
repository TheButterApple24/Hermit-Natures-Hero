/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Inventory
Description:        Handles a character's inventory
Date Created:       07/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    07/11/2021
        - [Max] Started Inventory.
    08/11/2021
        - [Max & Zoe] Replaced isFull bool with IsFull bool function.
    15/11/2021
        - [Max] Started Json file import
    26/11/2021
        - [Jeffrey] Restarted and re-factored inventory to store data rather than pickup
    28/11/2021
        - [Jeffrey] Inventory now uses key input rather than just clicking
    29/11/2021
        - [Jeffrey] Added comments
    05/12/2021
        - [Zoe] Moved Destroy(potion) to the relevant potion scripts
    08/12/2021
        - [Zoe] Fixed a bug where Attack/Poison potions weren't being destroyed after instantiating
    26/01/2022
        - [Max] Added Armor Implementation
    30/01/2022
        - [Max] Debugged Unequipping related bug
    08/02/2022
        - [Max] Added Comments
    09/03/2022
        - [Max] Weapon class refactor
    11/03/2022
        - [Zoe] Added Audio Source and SFX for Menu Opening/Closing, Item Using/Equipping, Item Selection
    14/02/2022
        - [Zoe] Added functionality for assigning hotkeys to the potions (HotkeyPotion())
    06/04/2022
        - [Jeffrey] Reworked inventory to use scriptable objects

===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootTiers;

public enum InventoryItemType
{
    Weapon = 0,
    Armor = 1,
    Potion = 2,
    Reagent = 3
}

[System.Serializable]
public class Inventory : MonoBehaviour
{
    public InventoryItem[] InventoryDatabase;

    [HideInInspector] public List<ItemStack> m_CurrentEquipment;
    [HideInInspector] public List<ItemStack> m_CurrentPotions;
    [HideInInspector] public List<ItemStack> m_CurrentReagents;

    public Sprite[] RarityIcons;
    public Sprite[] SelectedIcons;

    public Transform m_DropTransform;

    [SkillTreeModifiable] public InventoryUI EquipmentUI;
    public InventoryUI m_PotionsUI;
    public InventoryUI m_CraftingUI;

    public Text m_TitleUI;
    public Text m_DescriptionUI;
    public Image m_IconUI;
    public Text m_DamageUI;
    public Image m_DamageIcon;
    public GameObject m_EquipPrompt;
    public GameObject m_DropPrompt;
    public GameObject m_Hotkey1Prompt;
    public GameObject m_Hotkey2Prompt;
    public Image m_RarityIcon;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_WeaponEquipSFX;
    [SerializeField] private AudioClip m_ArmorEquipSFX;
    [SerializeField] private AudioClip m_PotionUseSFX;
    [SerializeField] private AudioClip m_ClickSFX;
    [SerializeField] private AudioClip m_PickupSFX;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Initializes and imports Inventory values
    void Init()
    {
        // Set all current pickups to null
        for (int i = 0; i < EquipmentUI.m_MaxSize; i++)
        {
            m_CurrentEquipment.Add(new ItemStack());
        }

        for (int i = 0; i < m_PotionsUI.m_MaxSize; i++)
        {
            m_CurrentPotions.Add(new ItemStack());
        }

        for (int i = 0; i < m_CraftingUI.m_MaxSize; i++)
        {
            m_CurrentReagents.Add(new ItemStack());
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            CheckForStack();
        }

        if (Input.GetButtonDown("DropWeapon"))
        {
            DropEquipment();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HotkeyPotion(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HotkeyPotion(2);
        }

        float dropTrigger = Input.GetAxisRaw("OpenSkillTree");
        if (dropTrigger < 0)
        {
            int index = 0;

            // When Drop is pressed, check if equipment is selected
            foreach (ItemStack stack in m_CurrentEquipment)
            {
                if (stack.ItemSelected == true)
                {
                    // If equipment is selected, remove it from the inventory
                    RemoveFromInventory(index);
                    DisableSelected();
                    return;
                }
                index++;
            }
        }
    }

    public void CheckForStack()
    {
        int index = 0;

        // When Interact is pressed, check if equipment or potions are selected
        foreach (ItemStack stack in m_CurrentPotions)
        {
            if (stack.ItemSelected == true)
            {
                // If potion is selected, use it
                UsePotion(stack.ItemID);
                DisableSelected();
                UpdateAllSlots();
                return;
            }
            index++;
        }

        index = 0;
        foreach (ItemStack stack in m_CurrentEquipment)
        {
            if (stack.ItemSelected == true)
            {
                // If weapon is selected, equip it
                EquipEquipment(index);
                DisableSelected();
                return;
            }
            index++;
        }
    }

    /*
    public bool CheckIfObjectIsStackable(Pickups pickup)
    {
        foreach (ItemStack stack in m_PickupDatabase)
        {
            if (stack.id == pickup.m_ItemID)
            {
                if (stack.stackable)
                {
                    return true;
                }
            }
        }

        return false;
    }
    */
    public void AddEquipmentToInventory(ItemStack stack)
    {
        for (int i = 0; i < m_CurrentEquipment.Count; i++)
        {
            // Find the first stack that is empty
            if (m_CurrentEquipment[i].ItemID == -1)
            {
                // Create a new stack with the pickups data
                ItemStack newStack = new ItemStack();
                newStack.ItemID = InventoryDatabase[stack.ItemID].ItemID;
                newStack.ItemTitle = InventoryDatabase[stack.ItemID].ItemTitle;
                newStack.ItemDescription = InventoryDatabase[stack.ItemID].ItemDescription;
                newStack.ItemAmount = -1;
                newStack.ItemType = stack.ItemType;

                // Add appropriate weapon stats to stack
                newStack.ItemDamage = stack.ItemDamage;
                newStack.ItemDefense = stack.ItemDefense;
                newStack.ItemRarity = stack.ItemRarity;

                // Set the stack to the new stack
                m_CurrentEquipment[i] = newStack;

                // Create a new slot and insert the stack into the slot
                ItemSlot newSlot = new ItemSlot(EquipmentUI.m_ItemSlots[i]);
                newSlot.InsertStack(m_CurrentEquipment[i]);

                EquipmentUI.m_ItemSlots[i].m_SlotAmount.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void AddPotionToInventory(ItemStack stack)
    {
        for (int i = 0; i < m_CurrentPotions.Count; i++)
        {
            // Find the first stack that is empty
            if (m_CurrentPotions[i].ItemTitle == null)
            {
                // Create a new stack
                ItemStack newStack = new ItemStack();
                newStack.ItemID = InventoryDatabase[stack.ItemID].ItemID;
                newStack.ItemTitle = InventoryDatabase[stack.ItemID].ItemTitle;
                newStack.ItemDescription = InventoryDatabase[stack.ItemID].ItemDescription;
                newStack.ItemAmount = stack.ItemAmount;
                newStack.ItemType = InventoryItemType.Potion;

                // Set current stack to the new stack
                m_CurrentPotions[i] = newStack;

                // Create a new ItemSlot and insert the stack
                ItemSlot newSlot = new ItemSlot(m_PotionsUI.m_ItemSlots[i]);
                newSlot.InsertStack(m_CurrentPotions[i]);
                return;
            }
        }
    }

    public void AddReagentToInventory(ItemStack stack)
    {
        for (int i = 0; i < m_CurrentReagents.Count; i++)
        {
            // Find the first stack that is empty
            if (m_CurrentReagents[i].ItemTitle == null)
            {
                // Create a new stack
                ItemStack newStack = new ItemStack();
                newStack.ItemID = InventoryDatabase[stack.ItemID].ItemID;
                newStack.ItemName = InventoryDatabase[stack.ItemID].ItemName;
                newStack.ItemTitle = InventoryDatabase[stack.ItemID].ItemTitle;
                newStack.ItemDescription = InventoryDatabase[stack.ItemID].ItemDescription;
                newStack.ItemAmount = stack.ItemAmount;
                newStack.ItemType = InventoryItemType.Reagent;

                // Set current stack to the new stack
                m_CurrentReagents[i] = newStack;

                // Create a new ItemSlot and insert the stack
                ItemSlot newSlot = new ItemSlot(m_CraftingUI.m_ItemSlots[i]);
                newSlot.InsertStack(m_CurrentReagents[i]);
                return;
            }
        }
    }

    public void AddToInventory(Pickups pickup)
    {
        // If Pickup is a weapon
        if (pickup.GetComponent<Weapon>() != null)
        {
            // If Inventory is NOT full
            if (!IsFull(m_CurrentEquipment))
            {
                for (int i = 0; i < m_CurrentEquipment.Count; i++)
                {
                    // Find the first stack that is empty
                    if (m_CurrentEquipment[i].ItemTitle == null)
                    {
                        // Create a new stack with the pickups data
                        ItemStack newStack = new ItemStack();
                        newStack.ItemID = InventoryDatabase[pickup.m_ItemID].ItemID;
                        newStack.ItemTitle = InventoryDatabase[pickup.m_ItemID].ItemTitle;
                        newStack.ItemDescription = InventoryDatabase[pickup.m_ItemID].ItemDescription;
                        newStack.ItemAmount = 1;
                        newStack.ItemType = InventoryItemType.Weapon;

                        // Add appropriate weapon stats to stack
                        newStack.ItemDamage = pickup.GetComponent<Weapon>().GetBaseDamage();
                        newStack.ItemRarity = pickup.GetComponent<Weapon>().WeaponLootTier;

                        // Set the stack to the new stack
                        m_CurrentEquipment[i] = newStack;

                        // Destroy the pickup
                        Destroy(pickup.gameObject);

                        // Create a new slot and insert the stack into the slot
                        ItemSlot newSlot = new ItemSlot(EquipmentUI.m_ItemSlots[i]);
                        newSlot.InsertStack(m_CurrentEquipment[i]);
                        m_AudioSource.PlayOneShot(m_PickupSFX);
                        EquipmentUI.m_ItemSlots[i].m_SlotAmount.gameObject.SetActive(false);
                        return;
                    }
                }

            }
        }
        else if (pickup.GetComponent<Armor>() != null) // If pickup is armor
        {
            // If Inventory is NOT full
            if (!IsFull(m_CurrentEquipment))
            {
                for (int i = 0; i < m_CurrentEquipment.Count; i++)
                {
                    // Find the first stack that is empty
                    if (m_CurrentEquipment[i].ItemTitle == null)
                    {
                        // Create a new stack with the pickups data
                        ItemStack newStack = new ItemStack();
                        newStack.ItemID = InventoryDatabase[pickup.m_ItemID].ItemID;
                        newStack.ItemTitle = InventoryDatabase[pickup.m_ItemID].ItemTitle;
                        newStack.ItemDescription = InventoryDatabase[pickup.m_ItemID].ItemDescription;
                        newStack.ItemAmount = 1;
                        newStack.ItemType = InventoryItemType.Armor;

                        // Add appropriate armor stats to stack
                        newStack.ItemDefense = pickup.GetComponent<Armor>().m_BaseDefense;
                        newStack.ItemRarity = pickup.GetComponent<Armor>().m_LootTier;

                        // Set the stack to the new stack
                        m_CurrentEquipment[i] = newStack;

                        // Destroy the pickup
                        Destroy(pickup.gameObject);

                        // Create a new slot and insert the stack into the slot
                        ItemSlot newSlot = new ItemSlot(EquipmentUI.m_ItemSlots[i]);
                        newSlot.InsertStack(m_CurrentEquipment[i]);
                        m_AudioSource.PlayOneShot(m_PickupSFX);
                        EquipmentUI.m_ItemSlots[i].m_IsArmor = true;
                        EquipmentUI.m_ItemSlots[i].m_SlotAmount.gameObject.SetActive(false);
                        return;
                    }
                }

            }
        }
        else if (pickup.GetComponent<Potion>() != null) // If pickup is a potion
        {
            // Check if item is in the inventory
            foreach (ItemStack stack in m_CurrentPotions)
            {
                // If there is an object in the slot
                if (stack.ItemTitle != null)
                {
                    // If the pickup and stack have the same item id
                    if (stack.ItemID == pickup.m_ItemID)
                    {
                        // Increment the amount
                        stack.ItemAmount++;

                        // Update the ItemSlot
                        foreach (UIItemSlot slot in m_PotionsUI.m_ItemSlots)
                        {
                            slot.UpdateSlot();
                        }

                        // Destroy the pickup
                        Destroy(pickup.gameObject);
                        m_AudioSource.PlayOneShot(m_PickupSFX);

                        return;

                    }
                }
            }

            // Create a new stack
            ItemStack newStack = new ItemStack();
            newStack.ItemID = InventoryDatabase[pickup.m_ItemID].ItemID;
            newStack.ItemTitle = InventoryDatabase[pickup.m_ItemID].ItemTitle;
            newStack.ItemDescription = InventoryDatabase[pickup.m_ItemID].ItemDescription;
            newStack.ItemAmount++;
            newStack.ItemType = InventoryItemType.Potion;

            for (int i = 0; i < m_CurrentPotions.Count; i++)
            {
                // Check if the current slot is null
                if (m_CurrentPotions[i].ItemTitle == null)
                {
                    // Set current stack to the new stack
                    m_CurrentPotions[i] = newStack;

                    // Destroy the pickup
                    Destroy(pickup.gameObject);

                    // Create a new ItemSlot and insert the stack
                    ItemSlot newSlot = new ItemSlot(m_PotionsUI.m_ItemSlots[i]);
                    newSlot.InsertStack(m_CurrentPotions[i]);
                    m_AudioSource.PlayOneShot(m_PickupSFX);

                    return;
                }
            }
        }
        else if (pickup.GetComponent<Reagent>() != null)
        {
            // Check if item is in the inventory
            foreach (ItemStack stack in m_CurrentReagents)
            {
                // If there is an object in the slot
                if (stack.ItemTitle != null)
                {
                    // If the pickup and stack have the same item id
                    if (stack.ItemID == pickup.m_ItemID)
                    {
                        // Increment the amount
                        stack.ItemAmount++;

                        // Update the ItemSlot
                        foreach (UIItemSlot slot in m_CraftingUI.m_ItemSlots)
                        {
                            slot.UpdateSlot();
                        }

                        m_AudioSource.PlayOneShot(m_PickupSFX);

                        return;

                    }
                }
            }

            // Create a new stack
            ItemStack newStack = new ItemStack();
            newStack.ItemID = InventoryDatabase[pickup.m_ItemID].ItemID;
            newStack.ItemName = InventoryDatabase[pickup.m_ItemID].ItemName;
            newStack.ItemTitle = InventoryDatabase[pickup.m_ItemID].ItemTitle;
            newStack.ItemDescription = InventoryDatabase[pickup.m_ItemID].ItemDescription;
            newStack.ItemAmount++;

            for (int i = 0; i < m_CurrentReagents.Count; i++)
            {
                // Check if the current slot is null
                if (m_CurrentReagents[i].ItemTitle == null)
                {
                    // Set current stack to the new stack
                    m_CurrentReagents[i] = newStack;

                    // Create a new ItemSlot and insert the stack
                    ItemSlot newSlot = new ItemSlot(m_CraftingUI.m_ItemSlots[i]);
                    newSlot.InsertStack(m_CurrentReagents[i]);
                    m_AudioSource.PlayOneShot(m_PickupSFX);

                    return;
                }
            }
        }
    }


    public void RemoveFromInventory(int inventoryIndex)
    {
        // if Inventory is NOT empty
        if (m_CurrentEquipment.Count != 0)
        {
            // If the inventory has the index
            if (m_CurrentEquipment.Count > inventoryIndex)
            {

                // If equipment at the index is equipped
                if (m_CurrentEquipment[inventoryIndex].ItemEquipped)
                {
                    // Grab the player
                    Player player = GameObject.Find("Player").GetComponent<Player>();

                    // Set the players appropriate equipment stats to the stacks stats, then drop the equipment and set equipment to null

                    if (m_CurrentEquipment[inventoryIndex].ItemType == InventoryItemType.Weapon)
                    {
                        player.m_MainWeapon.SetBaseDamage(m_CurrentEquipment[inventoryIndex].ItemDamage);
                        player.m_MainWeapon.WeaponLootTier = (LootTier)m_CurrentEquipment[inventoryIndex].ItemRarity;
                        player.m_MainWeapon.Deactivate();
                        player.m_MainWeapon = null;
                    }
                    else if (m_CurrentEquipment[inventoryIndex].ItemType == InventoryItemType.Armor)
                    {
                        player.m_MainArmor.m_BaseDefense = m_CurrentEquipment[inventoryIndex].ItemDefense;
                        player.m_MainArmor.m_LootTier = (LootTier)m_CurrentEquipment[inventoryIndex].ItemRarity;
                        player.m_MainArmor.Deactivate();
                        player.m_MainArmor = null;
                    }
                }
                else
                {
                    // Instantiate object
                    GameObject pickup = Instantiate(InventoryDatabase[m_CurrentEquipment[inventoryIndex].ItemID].ItemPrefab);
                    pickup.transform.parent = null;
                    pickup.transform.position = m_DropTransform.position;

                    Weapon weapon = pickup.GetComponent<Weapon>();
                    Armor armor = pickup.GetComponent<Armor>();

                    // Set appropriate equipment stats to the stacks stats
                    if (weapon != null)
                    {
                        weapon.SetParented(false);
                        weapon.SetBaseDamage(m_CurrentEquipment[inventoryIndex].ItemDamage);
                        weapon.WeaponLootTier = m_CurrentEquipment[inventoryIndex].ItemRarity;
                    }
                    else if (armor != null)
                    {
                        armor.m_BaseDefense = m_CurrentEquipment[inventoryIndex].ItemDamage;
                        armor.m_LootTier = m_CurrentEquipment[inventoryIndex].ItemRarity;
                    }
                }

                // Set the stack to null and then to a new stack
                m_CurrentEquipment[inventoryIndex] = null;
                m_CurrentEquipment[inventoryIndex] = new ItemStack();

                // Unlink the item slot
                EquipmentUI.m_ItemSlots[inventoryIndex].UnLink();

                return;
            }
        }
    }

    public void UsePotion(int potionIndex)
    {
        // if Inventory is NOT empty
        if (m_CurrentPotions.Count != 0)
        {
            ItemStack stack = HasItem(potionIndex);

            // If the inventory has the index
            if (stack != null)
            {
                // Instantiate Object and activate the potion
                GameObject pickup = Instantiate(InventoryDatabase[stack.ItemID].ItemPrefab);
                pickup.GetComponent<Potion>().InitPotion();
                pickup.GetComponent<Potion>().ActivateEffect();

                // If the potion is activated
                if (pickup.GetComponent<Potion>().m_Activated)
                {
                    //Play SFX
                    m_AudioSource.PlayOneShot(m_PotionUseSFX);

                    // Decrement the ampunt
                    stack.ItemAmount--;

                    // If the amount is less than or = to zero, set the stack to null and unlink the item slot
                    if (stack.ItemAmount <= 0)
                    {
                        stack = null;
                        stack = new ItemStack();
                        m_PotionsUI.m_ItemSlots[GetPotionInventoryIndex(potionIndex)].UnLink();
                        m_CurrentPotions[GetPotionInventoryIndex(potionIndex)] = stack;
                    }
                    else
                    {
                        m_PotionsUI.m_ItemSlots[GetPotionInventoryIndex(potionIndex)].UpdateSlot();
                    }
                }
                else
                {
                    Destroy(pickup);
                }

                return;
            }
        }
    }

    public void EquipEquipment(int inventoryIndex)
    {
        // if Inventory is NOT empty
        if (m_CurrentEquipment.Count != 0)
        {
            // If the inventory has the index
            if (m_CurrentEquipment.Count > inventoryIndex)
            {
                // If equipment isn't already equipped
                if (m_CurrentEquipment[inventoryIndex].ItemEquipped != true)
                {
                    // Check type of equipment
                    if (m_CurrentEquipment[inventoryIndex].ItemType == InventoryItemType.Weapon)
                    {
                        // Set all other weapons equipped to false
                        foreach (ItemStack stack in m_CurrentEquipment)
                        {
                            if (stack != null && stack.ItemType == InventoryItemType.Weapon)
                            {
                                stack.ItemEquipped = false;
                            }
                        }
                    }
                    else if (m_CurrentEquipment[inventoryIndex].ItemType == InventoryItemType.Armor)
                    {
                        // Set all other armor equipped to false
                        foreach (ItemStack stack in m_CurrentEquipment)
                        {
                            if (stack != null && stack.ItemType == InventoryItemType.Armor)
                            {
                                stack.ItemEquipped = false;
                            }
                        }
                    }

                    // Grab Pickup object and grab Player
                    GameObject pickup = (GameObject)Instantiate(InventoryDatabase[m_CurrentEquipment[inventoryIndex].ItemID].ItemPrefab);
                    Player player = GameObject.Find("Player").GetComponent<Player>();
                    Weapon weapon = pickup.GetComponent<Weapon>();
                    Armor armor = pickup.GetComponent<Armor>();

                    // Check what type of equipment pickup is
                    if (weapon != null)
                    {
                        // if the player has a weapon already
                        if (player.m_MainWeapon != null)
                        {
                            Destroy(player.m_MainWeapon.gameObject);
                            player.m_MainWeapon = null;

                            //// Destroy the weapon and set the main weapon to null
                            //if (player.m_MainWeapon.WeaponSocket.transform.childCount > 0)
                            //{
                            //    Destroy(player.m_MainWeapon.WeaponSocket.transform.GetChild(0).gameObject);
                            //    player.m_MainWeapon = null;
                            //}

                        }

                        //Play SFX
                        m_AudioSource.PlayOneShot(m_WeaponEquipSFX);

                        // Equip on appropriate weapon and set its stats
                        weapon.SetOwner(player);
                        weapon.SetParentedToPlayer(true);
                        weapon.SetBaseDamage(m_CurrentEquipment[inventoryIndex].ItemDamage);
                        weapon.WeaponLootTier = m_CurrentEquipment[inventoryIndex].ItemRarity;
                        weapon.Equip();

                        // Place weapon in appropriate position
                        if (player.SheathSocket != null && player.WeaponSocket != null)
                        {
                            if (player.IsWeaponInSheath)
                            {
                                weapon.PlaceWeaponInSheath(player.SheathSocket, true);
                            }
                            else
                            {
                                weapon.PlaceWeaponInSheath(player.WeaponSocket, false);
                            }
                        }

                        // Disable Equipped Icon on all ItemSlots
                        for (int i = 0; i < EquipmentUI.m_ItemSlots.Count; i++)
                        {
                            if (!EquipmentUI.m_ItemSlots[i].m_IsArmor)
                            {
                                EquipmentUI.m_ItemSlots[i].EquippedIcon.SetActive(false);
                            }
                        }
                    }
                    else if (armor != null)
                    {
                        // If the player has Armour already
                        if (player.m_MainArmor != null)
                        {
                            // Destroy the armor and set main armor to null
                            if (player.m_MainArmor.gameObject != null)
                            {
                                player.m_HealthComp.m_DamageResistance -= player.m_MainArmor.m_BaseDefense * 0.01f;
                                Destroy(player.m_MainArmor.gameObject);
                                player.m_MainArmor = null;
                            }
                        }

                        //Play SFX
                        m_AudioSource.PlayOneShot(m_ArmorEquipSFX);

                        // Equip on appropriate armor and set its stats
                        armor.m_BaseDefense = m_CurrentEquipment[inventoryIndex].ItemDefense;
                        armor.m_LootTier = m_CurrentEquipment[inventoryIndex].ItemRarity;
                        armor.Equip();

                        // Disable Equipped Icon on all ItemSlots
                        for (int i = 0; i < EquipmentUI.m_ItemSlots.Count; i++)
                        {
                            if (EquipmentUI.m_ItemSlots[i].m_IsArmor)
                            {
                                EquipmentUI.m_ItemSlots[i].EquippedIcon.SetActive(false);
                            }
                        }
                    }

                    // Enable Equipped Icon
                    EquipmentUI.m_ItemSlots[inventoryIndex].EquippedIcon.SetActive(true);

                    // Set equipped to true
                    m_CurrentEquipment[inventoryIndex].ItemEquipped = true;
                    return;
                }
            }
        }
    }
    public ItemStack HasItem(int itemIndex)
    {
        for (int i = 0; i < m_CurrentPotions.Count; i++)
        {
            if (m_CurrentPotions[i].ItemID == itemIndex)
            {
                return m_CurrentPotions[i];
            }
        }

        return null;
    }

    public int GetPotionInventoryIndex(int itemIndex)
    {
        for (int i = 0; i < m_CurrentPotions.Count; i++)
        {
            if (m_CurrentPotions[i].ItemID == itemIndex)
            {
                return i;
            }
        }

        return -1;
    }

    public bool IsFull(List<ItemStack> stack)
    {
        // Returns whether Inventory is full or not
        for (int i = 0; i < stack.Count; i++)
        {
            // If it finds a null stack, return true
            if (stack[i].ItemTitle == null)
            {
                return false;
            }
        }
        return true;
    }

    // Updates ItemSlot data
    public void UpdateAllSlots()
    {
        foreach (UIItemSlot slot in EquipmentUI.m_ItemSlots)
        {
            if (slot.m_ItemSlot != null)
            {
                if (slot.m_ItemSlot.m_Stack != null)
                {
                    if (slot.m_ItemSlot.m_Stack.ItemAmount <= 0)
                    {
                        slot.m_ItemSlot.m_Stack = null;
                        slot.m_ItemSlot.m_Stack = new ItemStack();
                        slot.UnLink();
                    }
                }
            }
            slot.UpdateSlot();
        }

        foreach (UIItemSlot slot in m_PotionsUI.m_ItemSlots)
        {

            if (slot.m_ItemSlot != null)
            {
                if (slot.m_ItemSlot.m_Stack != null)
                {
                    if (slot.m_ItemSlot.m_Stack.ItemAmount <= 0)
                    {
                        int potionID = slot.m_ItemSlot.m_Stack.ItemID;
                        slot.m_ItemSlot.m_Stack = null;
                        slot.m_ItemSlot.m_Stack = new ItemStack();
                        m_PotionsUI.m_ItemSlots[GetPotionInventoryIndex(potionID)].UnLink();
                        m_CurrentPotions[GetPotionInventoryIndex(potionID)] = new ItemStack();
                    }
                }
            }
            slot.UpdateSlot();
        }

        foreach (UIItemSlot slot in m_CraftingUI.m_ItemSlots)
        {

            if (slot.m_ItemSlot != null)
            {
                if (slot.m_ItemSlot.m_Stack != null)
                {
                    if (slot.m_ItemSlot.m_Stack.ItemAmount <= 0)
                    {
                        slot.m_ItemSlot.m_Stack = null;
                        slot.m_ItemSlot = null;
                    }
                }
            }
            slot.UpdateSlot();
        }
    }

    // Sets potion to selected and updates UI
    public void SelectPotion(int index)
    {
        m_CurrentPotions[index].ItemSelected = true;

        m_TitleUI.text = m_CurrentPotions[index].ItemTitle;
        m_DescriptionUI.text = m_CurrentPotions[index].ItemDescription;
        m_IconUI.sprite = InventoryDatabase[m_CurrentPotions[index].ItemID].ItemIcon;
        m_RarityIcon.enabled = false;

        m_TitleUI.gameObject.SetActive(true);
        m_DescriptionUI.gameObject.SetActive(true);
        m_IconUI.gameObject.SetActive(true);

        m_EquipPrompt.SetActive(true);
        m_Hotkey1Prompt.SetActive(true);
        m_Hotkey2Prompt.SetActive(true);

        m_AudioSource.PlayOneShot(m_ClickSFX);
    }

    // Takes an index and sets it to selected. Will set UI to items stats
    public void SelectEquipment(int index)
    {
        m_CurrentEquipment[index].ItemSelected = true;

        m_TitleUI.text = m_CurrentEquipment[index].ItemTitle;
        m_DescriptionUI.text = m_CurrentEquipment[index].ItemDescription;
        m_IconUI.sprite = InventoryDatabase[m_CurrentEquipment[index].ItemID].ItemIcon;

        // Show appropriate stat depending on type of equipment
        if (m_CurrentEquipment[index].ItemType == InventoryItemType.Weapon)
        {
            m_DamageUI.text = m_CurrentEquipment[index].ItemDamage.ToString();
            m_DamageIcon.sprite = SelectedIcons[0];
        }
        else if (m_CurrentEquipment[index].ItemType == InventoryItemType.Armor)
        {
            m_DamageUI.text = m_CurrentEquipment[index].ItemDefense.ToString();
            m_DamageIcon.sprite = SelectedIcons[1];
        }

        m_RarityIcon.sprite = RarityIcons[(int)m_CurrentEquipment[index].ItemRarity];

        m_RarityIcon.enabled = true;
        m_TitleUI.gameObject.SetActive(true);
        m_DescriptionUI.gameObject.SetActive(true);
        m_IconUI.gameObject.SetActive(true);
        m_DamageUI.gameObject.SetActive(true);
        m_EquipPrompt.SetActive(true);
        m_DropPrompt.SetActive(true);

        m_AudioSource.PlayOneShot(m_ClickSFX);
    }

    public void DropEquipment()
    {
        int index = 0;

        // When Drop is pressed, check if equipment is selected
        foreach (ItemStack stack in m_CurrentEquipment)
        {
            if (stack.ItemSelected == true)
            {
                // If equipment is selected, remove it from the inventory
                RemoveFromInventory(index);
                DisableSelected();
                return;
            }
            index++;
        }
    }

    // Disables all selected items and disables the UI
    public void DisableSelected()
    {
        foreach (ItemStack stack in m_CurrentPotions)
        {
            stack.ItemSelected = false;
        }

        foreach (ItemStack stack in m_CurrentEquipment)
        {
            stack.ItemSelected = false;
        }

        m_TitleUI.gameObject.SetActive(false);
        m_DescriptionUI.gameObject.SetActive(false);
        m_IconUI.gameObject.SetActive(false);
        m_DamageUI.gameObject.SetActive(false);
        m_EquipPrompt.SetActive(false);
        m_DropPrompt.SetActive(false);
        m_Hotkey1Prompt.SetActive(false);
        m_Hotkey2Prompt.SetActive(false);

        m_TitleUI.text = "";
        m_DescriptionUI.text = "";
        m_DamageUI.text = "";
    }

    public void HotkeyPotion(int slotNum)
    {
        // If 1 or 2 wasn't entered, log and error and quit
        if (slotNum != 1 && slotNum != 2)
        {
            Debug.LogError("Incorrect number inputted into HotkeyPotion(): " + slotNum.ToString());
            return;
        }

        int index = 0;

        foreach (ItemStack stack in m_CurrentPotions)
        {
            // If the selected potion exists
            if (stack.ItemSelected == true)
            {
                // Instantiate and initialize the potion in order to send the data
                GameObject pickup = Instantiate(InventoryDatabase[m_CurrentPotions[index].ItemID].ItemPrefab);
                Potion potion = pickup.GetComponent<Potion>();
                potion.InitPotion();

                // Send the necessary data to the hotbar
                HUDManager.Instance.PotionHotbarObject.AssignSlot(potion, slotNum);

                // Disable the selection HUD
                DisableSelected();
                UpdateAllSlots();

                return;
            }
            index++;
        }
    }

    public ItemStack CheckForItemInInventory(int itemId)
    {
        for (int i = 0; i < m_CurrentEquipment.Count; i++)
        {
            if (m_CurrentEquipment[i].ItemID == itemId)
            {
                return m_CurrentEquipment[i];
            }
        }

        for (int i = 0; i < m_CurrentPotions.Count; i++)
        {
            if (m_CurrentPotions[i].ItemID == itemId)
            {
                return m_CurrentPotions[i];
            }
        }

        for (int i = 0; i < m_CurrentReagents.Count; i++)
        {
            if (m_CurrentReagents[i].ItemID == itemId)
            {
                return m_CurrentReagents[i];
            }
        }

        return null;
    }
}
