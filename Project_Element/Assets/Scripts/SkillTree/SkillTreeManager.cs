/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeManager
Description:        class used to manage the SkillTreeSystem
Date Created:       11/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    11/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[AttributeUsage(AttributeTargets.Field)]
public class SkillTreeModifiable : Attribute { }

public class SkillTreeManager : MonoBehaviour
{
    private static SkillTreeManager m_Instance = null;
    public static SkillTreeManager Instance { get { return m_Instance; } }

    public Text SkillPointsText;
    public Text PlayerLevelText;
    public Text EXPText;

    public GameObject[] SkillTreeTargets;
    public SkillTreeObject[] SkillTrees;
    public SkillTreeButton[] SkillTreeButtons;

    int m_SelectedTreeIndex = 0;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

    private void Start()
    {
        SetupButtons();
        MenuManager.Instance.SkillTreeScreen.SetActive(false);
    }

    public void UpdateSkillTreeUI()
    {
        SkillPointsText.text = "Skill Points: " + PlayerManager.Instance.Player.m_SkillPoints.ToString();
        PlayerLevelText.text = "Level: " + PlayerManager.Instance.Player.m_PlayerLevel.ToString();
        EXPText.text = "EXP Needed: " + PlayerManager.Instance.Player.m_PlayerCurrentEXPForLevel.ToString() + "/" + PlayerManager.Instance.Player.m_PlayerEXPNeededForLevel.ToString();
    }

    void SetupButtons()
    {
        for (int i = 0; i < SkillTrees[m_SelectedTreeIndex].Nodes.Length; i++)
        {
            SkillTreeButtons[i].SkillProperty = SkillTrees[m_SelectedTreeIndex].Nodes[i].SkillProperty;
            SkillTreeButtons[i].SkillCost = int.Parse(SkillTrees[m_SelectedTreeIndex].Nodes[i].PointCost);
            SkillTreeButtons[i].SkillAmount = int.Parse(SkillTrees[m_SelectedTreeIndex].Nodes[i].SkillAmount);

            int count = i;
            SkillTreeButtons[i].SkillButton.onClick.AddListener(() => ActivateSkill(count, SkillTreeButtons[count].SkillProperty, SkillTreeButtons[count].SkillCost, SkillTreeButtons[count].SkillAmount));
            SkillTreeButtons[i].SkillButton.interactable = false;
            SkillTreeButtons[i].SetButtonText();
        }
        SkillTreeButtons[0].SkillButton.interactable = true;
    }

    void ActivateSkill(int buttonIndex, string property, int cost, int amount)
    {
        bool succeeded = false;
        foreach (var component in SkillTreeTargets[0].GetComponents<Component>())
        {
            FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                SkillTreeModifiable attribute = Attribute.GetCustomAttribute(fields[i], typeof(SkillTreeModifiable)) as SkillTreeModifiable;

                if (attribute != null)
                {
                    if (fields[i].Name == property)
                    {
                        if (property == "MaxHealth")
                        {
                            if (component.GetType() == typeof(HealthComponent))
                            {
                                if (PlayerManager.Instance.Player.m_SkillPoints >= cost)
                                {
                                    HealthComponent healthComp = (HealthComponent)component;
                                    healthComp.MaxHealth += amount;
                                    PlayerManager.Instance.Player.m_SkillPoints -= cost;
                                    succeeded = true;
                                }
                            }
                        }
                        else if (property == "MaxStamina")
                        {
                            if (component.GetType() == typeof(StaminaComponent))
                            {
                                if (PlayerManager.Instance.Player.m_SkillPoints >= cost)
                                {
                                    StaminaComponent staminaComp = (StaminaComponent)component;
                                    staminaComp.MaxStamina += amount;
                                    PlayerManager.Instance.Player.m_SkillPoints -= cost;
                                    succeeded = true;
                                }
                            }
                        }
                        else if (property == "MaxDefense")
                        {
                            if (component.GetType() == typeof(HealthComponent))
                            {
                                if (PlayerManager.Instance.Player.m_SkillPoints >= cost)
                                {
                                    HealthComponent healthComp = (HealthComponent)component;
                                    healthComp.MaxDefense += amount;
                                    PlayerManager.Instance.Player.m_SkillPoints -= cost;
                                    succeeded = true;
                                }
                            }
                        }
                        else if (property == "EquipmentUI")
                        {
                            if (component.GetType() == typeof(Inventory))
                            {
                                if (PlayerManager.Instance.Player.m_SkillPoints >= cost)
                                {
                                    Inventory inventory = (Inventory)component;
                                    inventory.EquipmentUI.m_MaxSize += amount;
                                    inventory.EquipmentUI.AddInventorySlots(amount);

                                    for (int j = 0; j < amount; j++)
                                    {
                                        inventory.m_CurrentEquipment.Add(new ItemStack());
                                    }
                                    PlayerManager.Instance.Player.m_SkillPoints -= cost;
                                    succeeded = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (succeeded)
        {
            UnlockNextBranches(buttonIndex);
            UpdateSkillTreeUI();
        }
    }

    void UnlockNextBranches(int startIndex)
    {
        SkillTreeButtons[startIndex].SkillButton.interactable = false;

        SkillTreeNode currentNode = SkillTrees[m_SelectedTreeIndex].Nodes[startIndex];
        List<string> desiredIDs = new List<string>();

        for (int i = 0; i < SkillTrees[m_SelectedTreeIndex].Connections.Length; i++)
        {
            if (!currentNode.HasBranches)
            {
                if (currentNode.OutID == SkillTrees[m_SelectedTreeIndex].Connections[i].OutID)
                {
                    desiredIDs.Add(SkillTrees[m_SelectedTreeIndex].Connections[i].InID);
                }
            }
            else
            {
                for (int j = 0; j < currentNode.BranchNumber; j++)
                {
                    if (SkillTrees[m_SelectedTreeIndex].Branches[currentNode.BranchStartNumber + j].OutID == SkillTrees[m_SelectedTreeIndex].Connections[i].OutID)
                    {
                        desiredIDs.Add(SkillTrees[m_SelectedTreeIndex].Connections[i].InID);
                    }
                }
            }
        }

        if (desiredIDs.Count > 0)
        {
            for (int i = 0; i < SkillTrees[m_SelectedTreeIndex].Nodes.Length; i++)
            {
                for (int j = 0; j < desiredIDs.Count; j++)
                {
                    if (SkillTrees[m_SelectedTreeIndex].Nodes[i].InID == desiredIDs[j])
                    {
                        SkillTreeButtons[i].SkillButton.interactable = true;
                    }
                }
            }
        }
    }

}
