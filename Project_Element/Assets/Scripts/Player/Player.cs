/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Player
Description:        Handles player and pet mechanics
Date Created:       06/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/10/2021
        - [Jeffrey] Created base class
    21/10/2021
        - [Jeffrey] Added variables. Implemented saving/loading.
        - [Zoe] m_HeldObject added
    30/10/2021
        - [Jeffrey] Added Quests
    02/11/2021
        - [Aaron] Created m_PrimaryPet and m_SecondaryPet
        - [Aaron] Created SwapPets functions; will replace primary pet with secondary pet and secondary with the primary pet.
    04/11/2021
        - [Aaron] Created m_PrimaryPetIcon and m_SecondaryPetIcon
        - [Aaron] Set up the UI for the pets, indicates which pet is currently primary and which is secondary.
    06/11/2021
        - [Zoe] Player now inherits from CharacterBase. HealthComponent, StaminaComponent, and Element have been moved.        
    07/11/2021
        - [Zoe] Start() is now an override of CharacterBase::Start() and calls base.Start()
        - [Max] Implemented Inventory implementation
		- [Jeffrey] Implemented variables for Lock-On
    08/11/2021
        - [Max] Added SetElement function in SwapPets and Attack method to Player
    09/11/2021
        - [Aaron] Set up the Use Main Ability functuion that will call the Pet's main ability to be used
    12/11/2021
        - [Max] Disabled Inputs while closing down Pause Screen and Upgrade Screen
    14/11/2021
        - [Zoe] Player's m_Element gets changed when swapping pets
    16/11/2021
        - [Max] Added m_IsInputDisabled condition to Attack function
    17/11/2021
        - [Max] Added Teleporters Menu
    19/11/2021
        - [Max] Fixed Teleporter Menu
    29/11/2021
        - [Aaron] Removed pet colour due to change in UI. No longer needs to access colour but icon images to represent the pets.
        - [Aaron] Added another private game object variable to track the main ability's icon. This icon changes depending on the currently active primary pet.
    30/11/2021
        - [Max] Added Comments
	02/12/2021
        - [Aaron] Updated Swap Pet function to swap the main ability icon depending on the cooldown state instead of always setting to full icon
        - [Aaron] Moved HUD initialization into a private function and call it at the end of player's start.
    05/12/2021
        - [Zoe] m_RespawnLocation added
        - [Zoe] OnDeath now sets the player to their Respawn location and resets health and stamina
        - [Aaron] Removed forced initialization of the Fire and Water Pet.
        - [Aaron] Created SetPrimaryPet and SetSecondaryPet functions for equipping pets to the player's pet slots
        - [Aaron] Updated HUD initialization. Now grabs the ability border and disables all elements until pets have been equipped.
    08/12/2021
        - [Aaron] Updated Pet Swap to swap position targets. Prevents Manager from sending the new primary pets to the same target after a Pet Swap.
	13/01/2022
        - [Aaron] Edited the inventory and skill list menu functions to account for the backpack menu being the way to access them.
    14/01/2022
        - [Aaron] Added a Death Menu variable and functions to open the menu and close the menu.
        - [Aaron] Changed OnDeath to call the Death Menu and moved the logic into Reset Player function, that can be called from the Death Menu
        - [Aaron] Updated Player Reset function to teleport any pets the player has with them.
        - [Aaron] Added functions for opening and closing the quest menu
    18/01/2022
        - [Zoe] Moved interaction code to the player. Added m_TargetObject, and m_InteractablesInRange
        - [Zoe] Added OnTriggerEnter, OnTriggerExit, CheckForClosestInteractable, Interact. 
    26/01/2022
        - [Aaron] Added Ultimate Ability variable to the player.
        - [Aaron] Re-ogranized Ultimate functions to set up the ultimate based on pet element types and run the correct ultimate script
    27/01/2022
        - [Jeffrey] Implemented EXP system
        - [Jeffrey] Implemented Multiple Tasks
        - [Aaron] Added a private Lock On Max Range variable to edit how far out the distance on lock on should be checking. Also added ultimate ability variables for 2 and 3.
        - [Aaron] Added variables for updating the Ultimate Ability Icon during gameplay.
        - [Aaron] Update Pet functions to swap out ultimate ability icon with the outline version.
        - [Max] Added Luck Modifier stat to Player
    27/01/2022
        - [Jeffrey] Implemented EXP system
    02/02/2022
        - [Max] Added Zoe's Bug Fix
		- [Jeffrey] Implemented EXP system
        - [Zoe] Fixed 2 bugs where TargetObject was not properly being set back to null when either initiating interaction, or moving out of range of the object
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    06/02/2022
        - [Aaron] Updated the Set Primary and Secondary Pet functions to only activate the Main and Ultimate UI elements once when the first pets are set
        - [Aaron] Added a check for a secondary pet, if it exits than the ultimate should swap based on the new pet.
    12/02/2022
        - [Max] Fixed Skill Tree Menu + Removed setting TargetObject to null after activating said object
    15/02/2022
        - [Max] Disabled player kinematic when closing the Teleport Menu
    17/02/2022
        - [Zoe] Fixed an issue where interaction particles would remain after you picked a plant
        - [Zoe] Slight formatting adjustments to CheckForClosestInteractable
    21/02/2022
        - [Zoe] Adding cycling between in-range interactables
        - [Zoe] Removed old health/stamina UI text
    22/02/2022
        - [Zoe] Fixed some small bugs with the interactable cycling (Interact() slightly rearranged as a result)
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    09/03/2022
        - [Max] Weapon class refactor
        - [Aaron] Updated the player properties instancer to set the default profile to have tutorial text turned on
    10/03/2022
        - [Aaron] Updated to work with Pet Refactor 2.0
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process
	15/03/2022
        - [Max] Added Weapon Sheath
        - [Max] Added Player Flinch, Sheathe, and Unsheathe animations
    25/03/2022
        -[Max] Added Pet Ultimate ability animations
    28/03/2022
        -[Max] Added Public Weapon Sheath function

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using Elements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Movement;

public class Player : CharacterBase
{
    [HideInInspector] public string SaveName = "Placeholder";

    public PlayerUIManager PlayerUI;

    [Header("Game (Generic)")]
    public ThirdPersonController m_Controller;
    public CameraFollow m_FollowCamera;
    [HideInInspector] public InteractableBase m_HeldObject;
    public int m_LuckModifier = 0;
    public StaminaComponent m_StaminaComp;
    [HideInInspector] public bool m_HoldingObject;
    [HideInInspector] public Vector3 m_RespawnPosition;
    public GameObject m_PropertiesObject;
    public GameObject SheathSocket;
    public InteractionHandler InteractionHandler;

    [Header("Game (Pet)")]
    public Pet m_PrimaryPet = null;
    public Pet m_SecondaryPet = null;
    public GameObject m_Target;

    //[Header("Game (Quest)")]
    [HideInInspector] public List<Quest> m_AcceptedQuests;
    [HideInInspector] public int m_ActiveQuest;
    [HideInInspector] public List<string> CompletedQuestIDs;

    //[Header("Game (EXP)")]
    [HideInInspector] public int m_PlayerTotalEXP = 0;
    [HideInInspector] public int m_PlayerCurrentEXPForLevel = 0;
    [HideInInspector] public int m_PlayerEXPNeededForLevel = 20;
    [HideInInspector] public int m_PlayerLevel = 1;
    public int m_SkillPoints;

    [Header("Game (Ultimate)")]
    public UltimateAbility m_UltimateOne;
    public UltimateAbility m_UltimateTwo;
    public UltimateAbility m_UltimateThree;
    public UltimateAbility m_CurrentUltimateAbility;

    [HideInInspector] public bool m_IsMenuOpen = false;
    public Camera m_MainCamera;
    public Camera m_BackpackCamera;
    public PlayerProperties m_Properties;
    [HideInInspector] public bool m_IsInCombat = false;
    [HideInInspector] public bool m_IsInputDisabled = false;

    public List<EnemyBehaviour> m_SeenEnemies;
    public List<EnemyBehaviour> SeenEnemies { get { return m_SeenEnemies; } }

    [HideInInspector] public bool m_CanSheathWeapon = true;
    [HideInInspector] public bool IsWeaponInSheath = false;
    private bool m_CanUseLockOn = true;

    private float m_LockOnMaxRange = 25.0f;
    int m_EXPIncrement = 20;
    int m_EXPNeeded = 50;

    private bool m_IsPlayerResetCalled = false;

    private Vector3 m_SpringArmPosOnStart;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip[] m_FootstepSFX;

    public void Awake()
    {
        if (GameObject.Find("PlayerProperties") == null)
        {
            GameObject properties = Instantiate(m_PropertiesObject);
            properties.name = "PlayerProperties";
            m_Properties = properties.GetComponent<PlayerProperties>();
            SaveName = m_Properties.m_SaveName;
            m_Properties.m_MouseSensitivity = 500.0f;
            m_Properties.m_ToggleTutorial = true;
        }
        else
        {
            GameObject properties = GameObject.Find("PlayerProperties");
            m_Properties = properties.GetComponent<PlayerProperties>();
            SaveName = m_Properties.m_SaveName;
        }

        m_SeenEnemies = new List<EnemyBehaviour>();
        PlayerManager.Instance.SetPlayer(this);
        CompletedQuestIDs = new List<string>();
    }

    public override void Start()
    {
        base.Start();

        m_RespawnPosition = gameObject.transform.position;

        m_StaminaComp = gameObject.GetComponent<StaminaComponent>();

        m_Element = Element.None;

        SaveName = m_Properties.m_SaveName;

        if (GetComponent<PlayerSwimming>() != null)
        {
            m_SpringArmPosOnStart = GetComponent<PlayerSwimming>().m_SpringArm.transform.localPosition;
        }
    }

    public void LoadPlayer()
    {
#if UNITY_EDITOR

        // Do nothing

#else
        LoadPlayerSettings();

        // Grab player data from save file
        PlayerData data = SaveSystem.LoadGame(Application.persistentDataPath + "/saves/" + SaveName + ".element");

        // If a file was found, set variables accordingly
        if (data != null)
        {
            m_HealthComp.m_CurrentHealth = data.m_Health;

            if (data.m_RespawnPosition != null)
            {

                Vector3 newPos = new Vector3(data.m_RespawnPosition[0], data.m_RespawnPosition[1], data.m_RespawnPosition[2]);
                m_RespawnPosition = newPos;
            }

            gameObject.transform.position = m_RespawnPosition;

            m_Properties.m_SaveName = SaveName;

            m_PlayerTotalEXP = data.m_PlayerTotalEXP;
            m_PlayerEXPNeededForLevel = data.m_PlayerEXPNeededForLevel;
            m_PlayerLevel = data.m_PlayerLevel;
            m_PlayerCurrentEXPForLevel = data.m_PlayerCurrentEXPForLevel;
            m_SkillPoints = data.m_SkillPoints;

            // Skill Tree
            if (data.SkillButtonStates != null)
            {
                for (int i = 0; i < data.SkillButtonStates.Length; i++)
                {
                    SkillTreeManager.Instance.SkillTreeButtons[i].SkillButton.interactable = data.SkillButtonStates[i];
                }

                m_HealthComp.MaxHealth = data.MaxHealth;
                m_StaminaComp.MaxStamina = data.MaxStamina;
                m_HealthComp.MaxDefense = data.MaxDefense;
                m_Inventory.EquipmentUI.AddInventorySlots(Mathf.Abs(m_Inventory.EquipmentUI.m_MaxSize - data.InventorySize));
            }

            // Inventory
            if (data.EquipmentIDs != null)
            {
                for (int i = 0; i < data.EquipmentIDs.Length; i++)
                {
                    if (data.EquipmentIDs[i] != -1)
                    {
                        ItemStack stack = new ItemStack();
                        stack.ItemID = data.EquipmentIDs[i];
                        stack.ItemDamage = data.EquipmentDamages[i];
                        stack.ItemDefense = data.EquipmentDefenses[i];
                        stack.ItemRarity = (LootTiers.LootTier)data.EquipmentRarity[i];
                        stack.ItemType = (InventoryItemType)data.EquipmentType[i];
                        m_Inventory.AddEquipmentToInventory(stack);
                    }
                }
            }

            if (data.PotionIDs != null)
            {
                for (int i = 0; i < data.PotionIDs.Length; i++)
                {
                    if (data.PotionIDs[i] != -1)
                    {
                        ItemStack stack = new ItemStack();
                        stack.ItemID = data.PotionIDs[i];
                        stack.ItemAmount = data.PotionAmounts[i];
                        m_Inventory.AddPotionToInventory(stack);
                    }
                }
            }

            if(data.ReagentIDs != null)
            {
                for (int i = 0; i < data.ReagentIDs.Length; i++)
                {
                    if (data.ReagentIDs[i] != -1)
                    {
                        ItemStack stack = new ItemStack();
                        stack.ItemID = data.ReagentIDs[i];
                        stack.ItemAmount = data.ReagentAmounts[i];
                        m_Inventory.AddReagentToInventory(stack);
                    }
                }
            }

            if (data.Potion1ID != -1)
            {
                GameObject pickup = Instantiate(m_Inventory.InventoryDatabase[data.Potion1ID].ItemPrefab);
                Potion potion = pickup.GetComponent<Potion>();
                potion.InitPotion();
                HUDManager.Instance.PotionHotbarObject.AssignSlot(potion, 1);
            }

            if (data.Potion2ID != -1)
            {
                GameObject pickup2 = Instantiate(m_Inventory.InventoryDatabase[data.Potion2ID].ItemPrefab);
                Potion potion2 = pickup2.GetComponent<Potion>();
                potion2.InitPotion();
                HUDManager.Instance.PotionHotbarObject.AssignSlot(potion2, 2);
            }

            if (data.LoreUnlockedIDs != null)
                GameManager.Instance.m_LoreUnlockedIDs = data.LoreUnlockedIDs;

            if (data.UnlockedTeleportersIDs != null)
            {
                for (int i = 0; i < data.UnlockedTeleportersIDs.Length; i++)
                {
                    for (int j = 0; j < MenuManager.Instance.TeleportMenu.AllTeleportersList.Count; j++)
                    {
                        if (data.UnlockedTeleportersIDs[i] == MenuManager.Instance.TeleportMenu.AllTeleportersList[j].SaveId)
                        {
                            MenuManager.Instance.TeleportMenu.AllTeleportersList[j].UnlockTeleporterSaveSystem(null);
                        }
                    }
                }
            }

            if (data.CompletedQuestIDs != null)
            {
                for (int i = 0; i < data.CompletedQuestIDs.Length; i++)
                {
                    for (int j = 0; j < GameManager.Instance.QuestList.Length; j++)
                    {
                        if (data.CompletedQuestIDs[i] == GameManager.Instance.QuestList[j].SaveId)
                        {
                            GameManager.Instance.QuestList[j].RigToDestroy = true;
                            GameManager.Instance.QuestList[j].DestroyQuest();
                        }
                    }
                }
            }

            if (data.AcceptedQuestIDs != null)
            {
                for (int i = 0; i < data.AcceptedQuestIDs.Length; i++)
                {
                    for (int j = 0; j < GameManager.Instance.QuestList.Length; j++)
                    {
                        if (data.AcceptedQuestIDs[i] == GameManager.Instance.QuestList[j].SaveId)
                        {
                            m_AcceptedQuests.Add(GameManager.Instance.QuestList[j]);
                            m_AcceptedQuests[i].m_IsCurrentlyActive = true;

                            // Set active quest to this quest
                            m_ActiveQuest = 0;

                            SetQuestToActive();

                            m_AcceptedQuests[i].m_TaskIndex = data.TaskIndexs[i];

                            m_AcceptedQuests[i].m_Tasks[m_AcceptedQuests[i].m_TaskIndex].InitTask();
                            if (m_AcceptedQuests[i].GetNavMarker() != null)
                            {
                                m_AcceptedQuests[i].GetNavMarker().SetActive(false);
                            }
                            m_AcceptedQuests[i].IsAccepted = true;

                            // Add the quest to the quest menu
                            GameManager.Instance.QuestListManager.UpdateQuestMenu();
                        }
                    }
                }
            }

            PetManager petManager = PetManager.pmInstance;

            if (data.UnlockedPetIDs != null)
            {
                for (int i = 0; i < data.UnlockedPetIDs.Length; i++)
                {
                    if (data.UnlockedPetIDs[i] != "")
                    {
                        for (int j = 0; j < petManager.PetContainers.Length; j++)
                        {
                            if (petManager.Pets[j].SaveId == data.UnlockedPetIDs[i])
                            {
                                petManager.PetContainers[j].UnlockPetSaveSystem();
                            }
                        }
                    }
                }
            }


            if (m_PrimaryPet != null)
            {
                m_PrimaryPet.DeactivatePet();
                m_PrimaryPet = null;
            }

            if (m_SecondaryPet != null)
            {
                m_SecondaryPet.DeactivatePet();
                m_SecondaryPet = null;
            }

            // Assign Primary Pet
            for (int i = 0; i < petManager.Pets.Length; i++)
            {
                if (data.EquippedPetIDs != null)
                {
                    if (petManager.Pets[i].SaveId == data.EquippedPetIDs[0])
                    {
                        petManager.Pets[i].ActivatePet();
                        SetPrimaryPet(petManager.Pets[i]);
                    }
                }
            }

            // Assign Secondary Pet
            for (int i = 0; i < petManager.Pets.Length; i++)
            {
                if (data.EquippedPetIDs != null)
                {
                    if (petManager.Pets[i].SaveId == data.EquippedPetIDs[1])
                    {
                        petManager.Pets[i].ActivatePet();
                        SetSecondaryPet(petManager.Pets[i]);
                    }
                }
            }
            IntroCutscene.Instance.HasBeenPlayed = data.HasPlayedIntro;

            if (data.DestroyObjectChecks != null)
            {
                for (int i = 0; i < GameManager.Instance.SaveObjectList.Length; i++)
			    {
                    if (data.DestroyObjectChecks[i] == true)
	                {
                        Destroy(GameManager.Instance.SaveObjectList[i]);
	                }
			    }
            }
        }
#endif
    }

    public void LoadPlayerSettings()
    {
        MenuManager.Instance.SettingsMenuObject.GetComponent<SettingsMenu>().LoadSettings(SaveName);

        if (m_FollowCamera != null && m_Properties != null)
        {
            m_FollowCamera.InputSensitivity = m_Properties.m_MouseSensitivity;

            m_FollowCamera.m_InvertXAxis = m_Properties.m_InvertXAxis;
            m_FollowCamera.m_InvertYAxis = m_Properties.m_InvertYAxis;
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SaveGame(this, SaveName);
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {
            m_RespawnPosition = gameObject.transform.position;
            SavePlayer();
            Debug.Log("Saved Test");
        }

        if (Input.GetButtonDown("SheathWeapon") && m_CanSheathWeapon && m_MainWeapon != null)
        {
            if (!IsWeaponInSheath)
            {
                if (SheathSocket != null) //&& !m_IsInCombat
                {
                    if (m_Controller.m_Animator != null)
                    {
                        SheathWeapon(true);
                    }

                    IsWeaponInSheath = true;
                }
            }
            else
            {
                if (WeaponSocket != null)
                {
                    if (m_Controller.m_Animator != null)
                    {
                        SheathWeapon(false);
                    }

                    IsWeaponInSheath = false;
                }
            }
        }

        //if (m_IsInCombat && IsWeaponInSheath)
        //{
        //    if (IsWeaponInSheath && WeaponSocket != null && m_Controller.m_Animator != null)
        //    {
        //        SheathWeapon(false);

        //        IsWeaponInSheath = false;
        //        m_CanSheathWeapon = true;
        //    }
        //}

        // Check if player is using input, Lock-On
        if (Input.GetButtonDown("Lock-On") && m_CanUseLockOn)
        {
            LockOnTarget();
            StartCoroutine(LockOnCooldown());
        }
        else
        {
            float lockOnTrigger = Input.GetAxis("Lock-On");
            if (lockOnTrigger != 0 && m_CanUseLockOn)
            {
                LockOnTarget();
                StartCoroutine(LockOnCooldown());
            }
        }

        if (m_Target == null && m_FollowCamera != null && !m_FollowCamera.m_IsPlayerReady)
        {
            if (m_PrimaryPet != null)
            {
                m_PrimaryPet.UpdatePetTarget(null);
            }
            if (m_SecondaryPet != null)
            {
                m_SecondaryPet.UpdatePetTarget(null);
            }
            m_Target = null;
            if (m_FollowCamera != null)
            {
                m_FollowCamera.m_Target = null;

                Vector3 rot = Vector3.zero;

                if (m_FollowCamera.transform.rotation.eulerAngles.x <= 180f)
                {
                    rot.x = m_FollowCamera.transform.rotation.eulerAngles.x;
                    rot.y = m_FollowCamera.transform.rotation.eulerAngles.y;
                }
                else
                {
                    rot.x = m_FollowCamera.transform.rotation.eulerAngles.x - 360f;
                    rot.y = m_FollowCamera.transform.rotation.eulerAngles.y;
                }

                m_FollowCamera.RotationX = rot.x;
                m_FollowCamera.RotationY = rot.y;

                m_FollowCamera.m_IsPlayerReady = true;
            }
        }

        // Check Quests for Completion
        for (int i = 0; i < m_AcceptedQuests.Count; i++)
        {
            if (m_AcceptedQuests[i] != null)
            {
                m_AcceptedQuests[i].CheckForCompletion();
            }
        }

        if (m_SeenEnemies.Count > 0)
            m_IsInCombat = true;
        else
            m_IsInCombat = false;

        // Update the Current Ultimate Icon if there is one and it's less than 1
        if (m_CurrentUltimateAbility != null)
        {
            if (m_CurrentUltimateAbility.IconFillAmount < 1.0f)
            {
                m_CurrentUltimateAbility.IconFillAmount += Time.deltaTime / m_CurrentUltimateAbility.AbilityCooldown;
                HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().fillAmount = m_CurrentUltimateAbility.IconFillAmount;
            }
        }

        // Debug Tool: See the range between the player and other objects. Helps to guage world distance.
        //float debugLineRange = 25.0f;
        //Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + (this.gameObject.transform.forward * debugLineRange));
    }

    private void LateUpdate()
    {
        if (!m_IsPlayerResetCalled && PlayerUI != null)
        {
            LoadPlayer();
            ResetPlayer();
            m_IsPlayerResetCalled = true;
        }
    }

    public void SetQuestToActive()
    {
        // If the player has quests
        if (m_AcceptedQuests.Count > 0)
        {
            // Set all quests to false
            foreach (Quest quest in m_AcceptedQuests)
            {
                quest.m_IsCurrentlyActive = false;
            }

            // Set the selected quest to true
            m_AcceptedQuests[m_ActiveQuest].m_IsCurrentlyActive = true;

            m_AcceptedQuests[m_ActiveQuest].m_Tasks[m_AcceptedQuests[m_ActiveQuest].m_TaskIndex].m_IsCurrentlyActive = true;
            m_AcceptedQuests[m_ActiveQuest].m_Tasks[m_AcceptedQuests[m_ActiveQuest].m_TaskIndex].UpdateUI();
        }
    }


    public void LockOnTarget()
    {
        // Find all enemies in the level
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        // If there is no current target
        if (m_Target == null)
        {
            float lowestDist = Mathf.Infinity;

            // Loop through each enemy in the array
            for (int i = 0; i < enemies.Length; i++)
            {
                float dist = Vector3.Distance(enemies[i].transform.position, transform.position);

                if (dist < m_LockOnMaxRange)
                {
                    if (dist < lowestDist)
                    {
                        lowestDist = dist;
                        //m_Target = enemies[i].m_LookAtPoint;
                        m_Target = enemies[i].gameObject;
                    }
                }

            }

            if (m_Target != null)
            {
                m_FollowCamera.m_Target = m_Target;
                m_FollowCamera.m_IsPlayerReady = false;
                if (m_PrimaryPet != null)
                {
                    m_PrimaryPet.UpdatePetTarget(m_Target);
                }
                if (m_SecondaryPet != null)
                {
                    m_SecondaryPet.UpdatePetTarget(m_Target);
                }
            }
        }
        // If the target is null, reset the variables
        else
        {
            if (m_PrimaryPet != null)
            {
                m_PrimaryPet.UpdatePetTarget(null);
            }
            if (m_SecondaryPet != null)
            {
                m_SecondaryPet.UpdatePetTarget(null);
            }
            m_Target = null;
            if (m_FollowCamera != null)
            {
                m_FollowCamera.m_Target = null;

                Vector3 rot = Vector3.zero;

                if (m_FollowCamera.transform.rotation.eulerAngles.x <= 180f)
                {
                    rot.x = m_FollowCamera.transform.rotation.eulerAngles.x;
                    rot.y = m_FollowCamera.transform.rotation.eulerAngles.y;
                }
                else
                {
                    rot.x = m_FollowCamera.transform.rotation.eulerAngles.x - 360f;
                    rot.y = m_FollowCamera.transform.rotation.eulerAngles.y;
                }

                m_FollowCamera.RotationX = rot.x;
                m_FollowCamera.RotationY = rot.y;

                m_FollowCamera.m_IsPlayerReady = true;
            }
        }
    }

    public void SetPrimaryPet(Pet pet)
    {
        // If first time setting a primary pet, activate the Main Ability UI
        if (m_PrimaryPet == null)
        {
            HUDManager.Instance.MainAbilityBorder.SetActive(true);
            HUDManager.Instance.PrimaryPetIconOutline.SetActive(true);
            HUDManager.Instance.PrimaryPetIconFill.SetActive(true);
        }

        // Set the primary pet and the target position it will move to
        m_PrimaryPet = pet;
        m_PrimaryPet.PetPositionTarget = transform.GetChild(5).gameObject;

        // Set the Primary Pet Icon with the Full version of the icon stored in the pet
        HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().sprite = m_PrimaryPet.PetIconFill;
        HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().fillAmount = m_PrimaryPet.IconFillAmount;
        HUDManager.Instance.PrimaryPetIconOutline.GetComponent<Image>().sprite = m_PrimaryPet.PetIconOutline;

        // Activate the pet's passive.
        m_PrimaryPet.PassiveAbility.ActivateAbility();

        // Set the player's Ultimate Ability variable once they have two pets to do so.
        if (m_SecondaryPet != null)
        {
            SetUltimateAbility();
        }

        // Set the pet's speed to match the Hermit's
        m_PrimaryPet.CurrentPetSpeed = m_Controller.m_DefaultSpeed;
    }

    public void SetSecondaryPet(Pet pet)
    {
        // If first time setting a secondary pet, activate the Main Ability UI
        if (m_SecondaryPet == null)
        {
            HUDManager.Instance.UltimateAbilityBorder.SetActive(true);
            HUDManager.Instance.UltimateAbilityIconOutline.SetActive(true);
            HUDManager.Instance.UltimateAbilityIconFill.SetActive(true);
            HUDManager.Instance.PetSwapIcon.SetActive(true);
        }

        // Set the secondary pet and the target position it will move to
        m_SecondaryPet = pet;
        m_SecondaryPet.PetPositionTarget = transform.GetChild(6).gameObject;

        // Set the player's Ultimate Ability variable once they have two pets to do so.
        SetUltimateAbility();

        // Set the Ultimate Ability Icon using the in game object and edit the sprite to be the current ultimate ability icon
        HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().sprite = m_CurrentUltimateAbility.AbilityIconFill;
        HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().fillAmount = m_CurrentUltimateAbility.IconFillAmount;
        HUDManager.Instance.UltimateAbilityIconOutline.GetComponent<Image>().sprite = m_CurrentUltimateAbility.AbilityIconOutline;

        // Set the pet's speed to match the Hermit's
        m_SecondaryPet.CurrentPetSpeed = m_Controller.m_DefaultSpeed;
    }

    public void SwapPets()
    {
        if (m_PrimaryPet != null && m_SecondaryPet != null)
        {
            // Store Primary Pet info in a temp variable
            Pet newSecondaryPet = m_PrimaryPet;

            // Turn off old Passive Ability
            m_PrimaryPet.PassiveAbility.DeactivateAbility();

            // Change the Primary Pet to hold the Secondary Pet's info
            m_PrimaryPet = null;
            m_PrimaryPet = m_SecondaryPet;

            // Turn on new Passive Ability & Swap Position Targets
            m_PrimaryPet.PassiveAbility.ActivateAbility();
            m_PrimaryPet.PetPositionTarget = transform.GetChild(5).gameObject;

            // If main weapon exists, Set main weapon's element to the primary pet's Elemental type & main weapon's material to its owner
            if (m_MainWeapon != null)
            {
                m_MainWeapon.WeaponElement = m_PrimaryPet.PetElementType;
                m_MainWeapon.SetMaterial(m_MainWeapon.GetOwner());
            }

            // Set Player's element to the primary pet's Elemental type
            m_Element = m_PrimaryPet.PetElementType;

            // Change the Secondary Pet to hold the temp variable stored info on the Primary Pet
            m_SecondaryPet = null;
            m_SecondaryPet = newSecondaryPet;

            // Change Main Ability Icon based on ability cooldown state
            SwapMainAbilityIcon();

            // Swap Position Targets
            m_SecondaryPet.PetPositionTarget = transform.GetChild(6).gameObject;

            // Set the Player's element type to the current primary pet's element type
            m_Element = m_PrimaryPet.PetElementType;
        }
    }

    void SwapMainAbilityIcon()
    {
        // Change the HUD Icons
        HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().sprite = m_PrimaryPet.PetIconFill;
        HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().fillAmount = m_PrimaryPet.IconFillAmount;
        HUDManager.Instance.PrimaryPetIconOutline.GetComponent<Image>().sprite = m_PrimaryPet.PetIconOutline;
    }

    public void UseMainAbility()
    {
        if (m_PrimaryPet.MainAbility.OnCooldown == true)
            return;

        // Use the primary pet's main ability
        m_PrimaryPet.SpawnAbility();

        // Change the main ability icon to the outline version
        m_PrimaryPet.IconFillAmount = 0.0f;
        HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().fillAmount = m_PrimaryPet.IconFillAmount;
    }

    public void SetUltimateAbility()
    {
        // Check the both Pet's Element Types
        Element primaryType = m_PrimaryPet.PetElementType;
        Element secondaryType = m_SecondaryPet.PetElementType;

        // If the equipped pets are fire and water, use Steam Bomb ultimate
        if (primaryType == Element.Fire && secondaryType == Element.Water)
        {
            m_CurrentUltimateAbility = m_UltimateOne;
        }
        else if (primaryType == Element.Water && secondaryType == Element.Fire)
        {
            m_CurrentUltimateAbility = m_UltimateOne;
        }
        // If the equipped pets are plant and water, use Mudslide ultimate
        else if (primaryType == Element.Water && secondaryType == Element.Plant)
        {
            m_CurrentUltimateAbility = m_UltimateTwo;
        }
        else if (primaryType == Element.Plant && secondaryType == Element.Water)
        {
            m_CurrentUltimateAbility = m_UltimateTwo;
        }
        // If the equipped pets are fire and plant, use Bonfire ultimate
        else if (primaryType == Element.Fire && secondaryType == Element.Plant)
        {
            m_CurrentUltimateAbility = m_UltimateThree;
        }
        else if (primaryType == Element.Plant && secondaryType == Element.Fire)
        {
            m_CurrentUltimateAbility = m_UltimateThree;
        }
        // No ultimate is currently equipped
        else
        {
            m_CurrentUltimateAbility = null;
        }

        if (m_CurrentUltimateAbility)
        {
            HUDManager.Instance.UltimateAbilityIconOutline.GetComponent<Image>().sprite = m_CurrentUltimateAbility.AbilityIconOutline;
            HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().sprite = m_CurrentUltimateAbility.AbilityIconFill;
            HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().fillAmount = m_CurrentUltimateAbility.IconFillAmount;
        }
    }

    // Runs the Steam Bomb ultimate ability sequence
    public void UseUltimateAbility()
    {
        if (m_CurrentUltimateAbility != null && m_CurrentUltimateAbility.OnCooldown == false)
        {
            m_PrimaryPet.GetAnimator().Play("UltAttack");
            m_SecondaryPet.GetAnimator().Play("UltAttack");

            //StartCoroutine(UltimateAbilityDelay());
        }
    }

    public void SpawnUltimateFromAnimation()
    {
        m_CurrentUltimateAbility.ActivateInstance();
        m_CurrentUltimateAbility.SetStartPoint();
        m_CurrentUltimateAbility.LaunchUltimate();
        m_CurrentUltimateAbility.PlayStartSound();

        // Set the cooldown as active and start the cooldown timer
        m_CurrentUltimateAbility.OnCooldown = true;
        StartCoroutine(m_CurrentUltimateAbility.UltimateCooldownTimer());

        // Change the main ability icon to the outline version
        m_CurrentUltimateAbility.IconFillAmount = 0.0f;
        HUDManager.Instance.UltimateAbilityIconFill.GetComponent<Image>().fillAmount = m_CurrentUltimateAbility.IconFillAmount;
    }

    public override void Attack()
    {
        // If Main Weapon exists and inputs aren't disabled
        if (m_MainWeapon != null && !m_IsInputDisabled)
        {
            // If Main Weapon is enabled, can attack, and isn't located on character's back
            if (m_MainWeapon.enabled && m_MainWeapon.CanAttack() && !IsWeaponInSheath)
            {
                base.Attack();

                m_MainWeapon.Attack();
            }
        }
    }

    public void ResetPlayer()
    {
        // If last checkpoint is reloaded
        // Close the death menu
        PlayerUI.CloseDeathMenu();

        m_Controller.m_RigidBody.isKinematic = true;

        // Move player to respawn location
        gameObject.transform.position = m_RespawnPosition;

        m_Controller.m_RigidBody.isKinematic = false;

        GetComponent<PlayerSwimming>().m_SpringArm.transform.localPosition = m_SpringArmPosOnStart;

        // Move player's pets to respawn location if the player has them
        if (m_PrimaryPet != null)
        {
            m_PrimaryPet.TeleportToPlayer();
        }

        if (m_SecondaryPet != null)
        {
            m_SecondaryPet.TeleportToPlayer();
        }

        // Reset health and stamina
        if (m_IsPlayerResetCalled)
        {
            m_HealthComp.Reset();
        }

        m_StaminaComp.Reset();
        m_HealthComp.m_HealthMeter.fillAmount = m_HealthComp.m_CurrentHealth / m_HealthComp.MaxHealth;

        m_Controller.m_MovementState = MovementState.OnGround;
    }

    public override void OnDeath()
    {
        // Play Death Anims

        PlayerUI.OpenDeathMenu();
    }

    IEnumerator LockOnCooldown()
    {
        m_CanUseLockOn = false;
        yield return new WaitForSeconds(0.25f);
        m_CanUseLockOn = true;
    }

    public void SheathWeapon(bool isBeingSheathed)
    {
        if (m_CanSheathWeapon && m_MainWeapon != null)
        {
            // Start Weapon Sheathing Coroutine
            StartCoroutine(SheathAnimation(isBeingSheathed));
            StartCoroutine(SheathCooldown());
        }
    }

    IEnumerator SheathCooldown()
    {
        m_CanSheathWeapon = false;
        yield return new WaitForSeconds(1.0f);
        m_CanSheathWeapon = true;
    }

    IEnumerator SheathAnimation(bool isBeingSheathed)
    {
        m_MainWeapon.SetCanAttack(false);
        m_CanSheathWeapon = false;

        if (isBeingSheathed)
        {
            m_Controller.m_Animator.Play("Sheathe");
            m_MainWeapon.PlaySFX(WeaponSFX.Sheathe);
            yield return new WaitForSeconds(0.64f);
            m_MainWeapon.PlaceWeaponInSheath(SheathSocket, true);
            IsWeaponInSheath = true;
        }
        else
        {
            m_Controller.m_Animator.Play("Unsheathe");
            m_MainWeapon.PlaySFX(WeaponSFX.Unsheathe);
            yield return new WaitForSeconds(0.44f);
            m_MainWeapon.PlaceWeaponInSheath(WeaponSocket, false);
            yield return new WaitForSeconds(0.2f);
            m_MainWeapon.SetCanAttack(true);
            IsWeaponInSheath = false;
        }
    }

    public void CheckForLevelUp(int expToAdd)
    {
        m_PlayerCurrentEXPForLevel += expToAdd;
        m_PlayerTotalEXP += expToAdd;

        if (m_PlayerCurrentEXPForLevel >= m_PlayerEXPNeededForLevel)
        {
            m_PlayerLevel += 1;
            m_SkillPoints += 1;
            m_PlayerCurrentEXPForLevel -= m_PlayerEXPNeededForLevel;
            if (m_PlayerEXPNeededForLevel > m_EXPNeeded)
            {
                m_EXPIncrement += 20;
                m_EXPNeeded += 50;
            }
            m_PlayerEXPNeededForLevel += m_EXPIncrement;
        }
    }

    public void ChangeActiveQuest(int questIndex)
    {
        for (int i = 0; i < m_AcceptedQuests.Count; i++)
        {
            m_AcceptedQuests[i].m_IsCurrentlyActive = false;

            for (int j = 0; j < m_AcceptedQuests[i].m_Tasks.Count; j++)
            {
                m_AcceptedQuests[i].m_Tasks[j].m_IsCurrentlyActive = false;
            }
        }

        m_AcceptedQuests[questIndex].m_IsCurrentlyActive = true;
        m_AcceptedQuests[questIndex].m_Tasks[m_AcceptedQuests[questIndex].m_TaskIndex].m_IsCurrentlyActive = true;
        m_ActiveQuest = questIndex;
        m_AcceptedQuests[m_ActiveQuest].m_Tasks[m_AcceptedQuests[m_ActiveQuest].m_TaskIndex].UpdateUI();
    }

    public override void OnTakeDamage(float damage, bool isCrit)
    {
        if (m_Controller.m_Animator != null)
        {
            m_Controller.m_Animator.Play("Flinch");
        }
    }

    public void PlayFootstepSFX()
    {
        if (m_FootstepSFX.Length > 0)
        {
            int index = Random.Range(0, m_FootstepSFX.Length);

            if (m_AudioSource != null)
            {
                m_AudioSource.PlayOneShot(m_FootstepSFX[index]);

            }
            else
            {
                Debug.LogError("Audio Source not assigned");
            }
        }
    }
}
