/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              RandomQuest
Description:        Generates a random quest, used with Decree system
Date Created:       17/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/02/2022
        - [Jeffrey] Created base implementation
    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootTiers;

public class RandomQuest : MonoBehaviour
{
    public GameObject m_NavMarker;
    //public Transform m_NavMarkerTransform;
    //public Color m_NavMarkerColor;
    //public NavWaypoint m_NavWaypoint;
    public Enemy m_TargetEnemy;
    public Pickups[] m_PotentialCollect;

    public string[] m_KillQuestNames;
    public string[] m_CollectQuestNames;

    public LootTier m_QuestTier;
    public Quest m_Quest;

    private void Awake()
    {
        GenerateRandomQuest();
        m_Quest.m_TasksArray = m_Quest.m_Tasks.ToArray();
    }

    public void GenerateRandomQuest()
    {
        // Set quest UI and managers
        m_Quest = gameObject.AddComponent<Quest>();
        //m_Quest.m_NavMarker = m_NavMarker;
        m_Quest.m_NavMarkerColor = new Color(0, 0, 0, 0);

        // Assign a tier
        m_QuestTier = LootTierCalculation.AssignLootRarity(false);

        m_Quest.m_IsDecreeQuest = true;

        // Based on the tier, Generate a quest of that rarity
        switch (m_QuestTier)
        {
            case LootTier.Common:
                GenerateCommonQuest();
                break;

            case LootTier.Rare:
                GenerateRareQuest();
                break;

            case LootTier.Heroic:
                GenerateHeroicQuest();
                break;

            case LootTier.Mythic:
                GenerateMythicQuest();
                break;
            default:
                break;
        }
    }

    void GenerateCommonQuest()
    {
        // 0 = kill, 1 = collect
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            // Create a kill task and assign the values.
            QuestTask_KillSingleType task = gameObject.AddComponent<QuestTask_KillSingleType>();

            task.m_TargetNumber = Random.Range(1, 5);
            task.m_TargetEnemy = m_TargetEnemy;
            task.m_TaskTip = "Kill Cultists";

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestName = m_KillQuestNames[Random.Range(0, m_KillQuestNames.Length)];
            m_Quest.m_QuestDescription = "Please kill " + task.m_TargetNumber + " cultists for me. When you are done, please return to the mailbox!";

            m_Quest.m_Tasks.Add(task);
        }
        else if (index == 1)
        {
            // Create a collect task and assign the values.
            QuestTask_CollectSingleType task = gameObject.AddComponent<QuestTask_CollectSingleType>();

            task.m_TargetNumber = Random.Range(1, 5);
            task.m_TargetObjectType = m_PotentialCollect[Random.Range(0, m_PotentialCollect.Length)];
            task.m_TaskTip = "Collect " + task.m_TargetObjectType.name;


            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please collect " + task.m_TargetNumber + " " + task.m_TargetObjectType.name + " for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_CollectQuestNames[Random.Range(0, m_CollectQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }

        GenerateRandomRewards();
    }

    void GenerateRareQuest()
    {
        // 0 = kill, 1 = collect
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            // Create a kill task and assign the values.
            QuestTask_KillSingleType task = gameObject.AddComponent<QuestTask_KillSingleType>();

            task.m_TargetNumber = Random.Range(6, 11);
            task.m_TargetEnemy = m_TargetEnemy;
            task.m_TaskTip = "Kill Cultists";

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please kill " + task.m_TargetNumber + " cultists for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_KillQuestNames[Random.Range(0, m_KillQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }
        else if (index == 1)
        {
            // Create a collect task and assign the values.
            QuestTask_CollectSingleType task = gameObject.AddComponent<QuestTask_CollectSingleType>();

            task.m_TargetNumber = Random.Range(6, 11);
            task.m_TargetObjectType = m_PotentialCollect[Random.Range(0, m_PotentialCollect.Length)];
            task.m_TaskTip = "Collect " + task.m_TargetObjectType.name;

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please collect " + task.m_TargetNumber + " " + task.m_TargetObjectType.name + " for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_CollectQuestNames[Random.Range(0, m_CollectQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }

        GenerateRandomRewards();
    }

    void GenerateHeroicQuest()
    {
        // 0 = kill, 1 = collect
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            // Create a kill task and assign the values.
            QuestTask_KillSingleType task = gameObject.AddComponent<QuestTask_KillSingleType>();

            task.m_TargetNumber = Random.Range(11, 16);
            task.m_TargetEnemy = m_TargetEnemy;
            task.m_TaskTip = "Kill Cultists";

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please kill " + task.m_TargetNumber + " cultists for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_KillQuestNames[Random.Range(0, m_KillQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }
        else if (index == 1)
        {
            // Create a collect task and assign the values.
            QuestTask_CollectSingleType task = gameObject.AddComponent<QuestTask_CollectSingleType>();

            task.m_TargetNumber = Random.Range(11, 16);
            task.m_TargetObjectType = m_PotentialCollect[Random.Range(0, m_PotentialCollect.Length)];
            task.m_TaskTip = "Collect " + task.m_TargetObjectType.name;

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please collect " + task.m_TargetNumber + " " + task.m_TargetObjectType.name + " for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_CollectQuestNames[Random.Range(0, m_CollectQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }

        GenerateRandomRewards();
    }

    void GenerateMythicQuest()
    {
        // 0 = kill, 1 = collect
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            // Create a kill task and assign the values.
            QuestTask_KillSingleType task = gameObject.AddComponent<QuestTask_KillSingleType>();

            task.m_TargetNumber = Random.Range(16, 21);
            task.m_TargetEnemy = m_TargetEnemy;
            task.m_TaskTip = "Kill Cultists";

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please kill " + task.m_TargetNumber + " cultists for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_KillQuestNames[Random.Range(0, m_KillQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }
        else if (index == 1)
        {
            // Create a collect task and assign the values.
            QuestTask_CollectSingleType task = gameObject.AddComponent<QuestTask_CollectSingleType>();

            task.m_TargetNumber = Random.Range(16, 21);
            task.m_TargetObjectType = m_PotentialCollect[Random.Range(0, m_PotentialCollect.Length)];
            task.m_TaskTip = "Collect " + task.m_TargetObjectType.name;

            // Set the name and description. Add it to the task list in the quest
            m_Quest.m_QuestDescription = "Please collect " + task.m_TargetNumber + " " + task.m_TargetObjectType.name + " for me. When you are done, please return to the mailbox!";
            m_Quest.m_QuestName = m_CollectQuestNames[Random.Range(0, m_CollectQuestNames.Length)];
            m_Quest.m_Tasks.Add(task);
        }

        GenerateRandomRewards();
    }

    void GenerateRandomRewards()
    {
        GameManager gm;
        int rewardsAmount;

        // Based on quest tier, generate loot for the quest
        switch (m_QuestTier)
        {
            case LootTier.Common:
                // Common = 1-3 rewards
                gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                rewardsAmount = Random.Range(1, 3);

                // Grab a reward from the loot system and give it an amount. Weapons don't stack so the amount is set to 0.
                for (int i = 0; i < rewardsAmount; i++)
                {
                    int rewardIndex = Random.Range(0, gm.m_PotentialChestLoot.Length);
                    m_Quest.m_QuestRewards.Add(gm.m_PotentialChestLoot[rewardIndex]);
                    if (gm.m_PotentialChestLoot[rewardIndex].GetComponent<Weapon>() != null || gm.m_PotentialChestLoot[rewardIndex].GetComponent<Armor>())
                    {
                        m_Quest.m_QuestRewardsAmount.Add(0);
                    }
                    else
                    {
                        m_Quest.m_QuestRewardsAmount.Add(Random.Range(1, 3));
                    }
                }

                m_Quest.QuestEXP = Random.Range(2, 4);
                break;

            case LootTier.Rare:
                // Rare = 3-5 rewards
                gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                rewardsAmount = Random.Range(3, 5);

                // Grab a reward from the loot system and give it an amount. Weapons don't stack so the amount is set to 0.
                for (int i = 0; i < rewardsAmount; i++)
                {
                    int rewardIndex = Random.Range(0, gm.m_PotentialChestLoot.Length);
                    m_Quest.m_QuestRewards.Add(gm.m_PotentialChestLoot[rewardIndex]);
                    if (gm.m_PotentialChestLoot[rewardIndex].GetComponent<Weapon>() != null || gm.m_PotentialChestLoot[rewardIndex].GetComponent<Armor>())
                    {
                        m_Quest.m_QuestRewardsAmount.Add(0);
                    }
                    else
                    {
                        m_Quest.m_QuestRewardsAmount.Add(Random.Range(3, 5));
                    }
                }

                m_Quest.QuestEXP = Random.Range(5, 6);
                break;

            case LootTier.Heroic:
                // Heroic = 5-8 rewards
                gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                rewardsAmount = Random.Range(5, 8);

                // Grab a reward from the loot system and give it an amount. Weapons don't stack so the amount is set to 0.
                for (int i = 0; i < rewardsAmount; i++)
                {
                    int rewardIndex = Random.Range(0, gm.m_PotentialChestLoot.Length);
                    m_Quest.m_QuestRewards.Add(gm.m_PotentialChestLoot[rewardIndex]);
                    if (gm.m_PotentialChestLoot[rewardIndex].GetComponent<Weapon>() != null || gm.m_PotentialChestLoot[rewardIndex].GetComponent<Armor>())
                    {
                        m_Quest.m_QuestRewardsAmount.Add(0);
                    }
                    else
                    {
                        m_Quest.m_QuestRewardsAmount.Add(Random.Range(5, 8));
                    }
                }
                m_Quest.QuestEXP = Random.Range(7, 10);
                break;

            case LootTier.Mythic:
                // Mythic = 8-12 rewards
                gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                rewardsAmount = Random.Range(8, 12);

                // Grab a reward from the loot system and give it an amount. Weapons don't stack so the amount is set to 0.
                for (int i = 0; i < rewardsAmount; i++)
                {
                    int rewardIndex = Random.Range(0, gm.m_PotentialChestLoot.Length);
                    m_Quest.m_QuestRewards.Add(gm.m_PotentialChestLoot[rewardIndex]);
                    if (gm.m_PotentialChestLoot[rewardIndex].GetComponent<Weapon>() != null || gm.m_PotentialChestLoot[rewardIndex].GetComponent<Armor>())
                    {
                        m_Quest.m_QuestRewardsAmount.Add(0);
                    }
                    else
                    {
                        m_Quest.m_QuestRewardsAmount.Add(Random.Range(8, 12));
                    }
                }
                m_Quest.QuestEXP = Random.Range(11, 15);
                break;
            default:
                break;
        }
    }
}
