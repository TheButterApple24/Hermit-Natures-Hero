/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestListManager
Description:        Updates the Quest Menu based on the quests the player is doing / has done. Creates QuestListButtons the player can
                    use to display more information and update the UI Quest Navigation information to help lead them to quest destination points.
Date Created:       14/01/2022
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Aaron] Created base class with basic button prefab and function that passes quest info to the UI objects

    21/01/2022
        - [Aaron] Seperated logic from single function into more dynamic functions that QuestListButton can send data to.
        - [Aaron] Removed old function that set up the buttons and displayed the information immediately for update functions called at player discresion
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestListManager : MonoBehaviour
{
    public GameObject m_QuestListButtonPrefab;

    public Player m_Player;
    public GameObject m_RewardPrefab;
    public GameObject m_RewardRectTransform;

    public GameObject m_QuestDescriptionTitle;
    public GameObject m_QuestDescription;
    public GameObject m_QuestTaskDescriptionTitle;
    public GameObject m_QuestTaskDescription;
    public GameObject m_QuestRewardTitle;

    List<GameObject> m_IconList;

    List<GameObject> m_ButtonList;

    public int m_ActiveQuestIndex = 0;

    private void Awake()
    {
        m_IconList = new List<GameObject>();
        m_ButtonList = new List<GameObject>();
    }

    private void Start()
    {
        m_Player = PlayerManager.Instance.Player;

        //// Set the default Quest Menu quest description text to blank
        //m_QuestDescriptionTitle = GameObject.Find("QuestDescriptionTitleText");
        m_QuestDescriptionTitle.GetComponent<Text>().text = "";

        //m_QuestDescription = GameObject.Find("QuestDescriptionPanel");
        m_QuestDescription.GetComponentInChildren<Text>().text = "";

        //m_QuestTaskDescriptionTitle = GameObject.Find("QuestTaskDescriptionTitleText");
        m_QuestTaskDescriptionTitle.GetComponent<Text>().text = "";

        //m_QuestTaskDescription = GameObject.Find("QuestTaskDescriptionPanel");
        m_QuestTaskDescription.GetComponentInChildren<Text>().text = "";

        //m_QuestRewardTitle = GameObject.Find("QuestRewardsTitle");
        m_QuestRewardTitle.GetComponent<Text>().text = "";
    }

    public void UpdateQuestMenu()
    {
        // If the container has any buttons in it
        //if (this.transform.childCount >= 1)
        if(m_ButtonList.Count != 0)
        {
            // Delete all the previous buttons
            //foreach (Transform child in this.transform)
            //{
            //    GameObject.Destroy(child.gameObject);
            //}

            foreach(GameObject obj in m_ButtonList)
            {
                Destroy(obj);
            }

            m_ButtonList.Clear();
        }



        // Grab the quest list from the player
        List<Quest> QuestList = m_Player.m_AcceptedQuests;

        // If the player has quests
        if (QuestList.Count > 0)
        {
            // Create a Quest List Button for each quest the player has accepted and store the quest in that button
            //foreach (Quest quest in QuestList)
            for (int i = 0; i < QuestList.Count; i++)
            {
                GameObject questObj = Instantiate(m_QuestListButtonPrefab);
                questObj.transform.SetParent(this.gameObject.transform, false);
                questObj.GetComponentInChildren<Text>().text = QuestList[i].m_QuestName;
                questObj.GetComponent<QuestListButton>().m_StoredQuest = QuestList[i];
                questObj.GetComponent<QuestListButton>().m_QuestIndexInList = i;

                m_ButtonList.Add(questObj);
            }
        }
        else
        {
            // Reset all descriptions
            m_QuestDescriptionTitle.GetComponent<Text>().text = "";
            m_QuestDescription.GetComponentInChildren<Text>().text = "";
            m_QuestTaskDescriptionTitle.GetComponent<Text>().text = "";
            m_QuestTaskDescription.GetComponentInChildren<Text>().text = "";
        }
    }

    // Updates the Quest Menu's Description section with a quest selected from the Quest Menu's Quest List
    public void UpdateQuestDescription(int questId, string questName, string questDescription)
    {
        m_QuestDescriptionTitle.GetComponent<Text>().text = questName;
        m_QuestDescription.GetComponentInChildren<Text>().text = questDescription;

        m_Player.ChangeActiveQuest(questId);
        m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_Tasks[m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_TaskIndex].UpdateUI();
        m_ActiveQuestIndex = questId;
    }

    // Updates the Quest Menu's Task Description section with a quest selected from the Quest Menu's Quest List
    public void UpdateQuestTaskDescription(string taskDescription)
    {
        m_QuestTaskDescriptionTitle.GetComponent<Text>().text = "Objectives";
        m_QuestTaskDescription.GetComponentInChildren<Text>().text = taskDescription;
    }

    // If quest has rewards, it shows them on the screen
    public void UpdateQuestRewards(int questId)
    {
        ClearIconsList();

        Quest quest = m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest];

        for (int i = 0; i < quest.m_QuestRewards.Count; i++)
        {
            GameObject icon = Instantiate(m_RewardPrefab, m_RewardRectTransform.transform);
            icon.GetComponent<Image>().sprite = quest.m_QuestRewards[i].GetComponent<Pickups>().m_Icon;
            if (quest.m_QuestRewardsAmount[i] > 0)
            {
                m_QuestRewardTitle.GetComponent<Text>().text = "Rewards";
                icon.transform.GetChild(0).GetComponent<Text>().text = quest.m_QuestRewardsAmount[i].ToString();
            }
            else
            {
                icon.transform.GetChild(0).GetComponent<Text>().text = "";
            }

            m_IconList.Add(icon);
        }
    }

    public void ClearIconsList()
    {
        // Clear Icons list
        foreach (GameObject obj in m_IconList)
        {
            Destroy(obj);
        }

        m_IconList.Clear();
    }

    public void UpdateMenuText()
    {
        if (m_ButtonList.Count > 0)
        {
            if (m_ActiveQuestIndex >= m_ButtonList.Count)
            {
                m_ActiveQuestIndex = 0;
            }
            QuestListButton button = m_ButtonList[m_ActiveQuestIndex].GetComponent<QuestListButton>();

            UpdateQuestDescription(button.m_QuestIndexInList, button.m_StoredQuest.m_QuestName, button.m_StoredQuest.m_QuestDescription);
            UpdateQuestTaskDescription(button.m_StoredQuest.m_Tasks[button.m_StoredQuest.m_TaskIndex].m_TaskTip);
            UpdateQuestRewards(button.m_QuestIndexInList);
        }
    }

}
