/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DecreeBoard
Description:        Handles the decree board system
Date Created:       17/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/02/2022
        - [Jeffrey] Created base implementation
    16/03/2022
        - [Aaron] Updated to include tutorial display for first time use

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecreeBoard : InteractableBase
{
    public List<RandomQuest> m_Quests;
    public GameObject m_DecreeMenu;
    public QuestTask m_TalkToTask;

    public GameObject[] m_QuestButtons;
    public Sprite[] m_DecreeTierSprites;

    [Header("Tutorial")]
    public string BoardTutorialText = "";

    int m_AcceptedIndex = 0;
    int m_CompletedIndex = 0;
    bool m_BoardUsed = false;

    public override void Start()
    {
        base.Start();
        // Loop through the quests and add a talk to task, also setup the decree board with the right information
        for (int i = 0; i < m_Quests.Count; i++)
        {
            m_Quests[i].m_Quest.m_Tasks.Add(m_TalkToTask);
            m_Quests[i].m_Quest.m_IsDecreeQuest = true;
            m_QuestButtons[i].GetComponent<Image>().sprite = m_DecreeTierSprites[(int)m_Quests[i].m_QuestTier];
            m_QuestButtons[i].gameObject.SetActive(true);
            m_QuestButtons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = m_Quests[i].m_Quest.m_QuestName;
            m_QuestButtons[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = m_Quests[i].m_Quest.m_QuestDescription;
            m_QuestButtons[i].gameObject.transform.GetChild(2).gameObject.SetActive(false);
            m_QuestButtons[i].GetComponent<Button>().interactable = true;
        }

        m_DecreeMenu.SetActive(false);
    }

    public override void Activate()
    {
        OpenDecreeMenu();
    }

    public void OpenDecreeMenu()
    {
        if (!m_Player.m_IsMenuOpen)
        {
            if (m_BoardUsed == false)
            {
                m_BoardUsed = true;
                string title = "Decree Board";
                m_Player.PlayerUI.DisplayTutorialOnFirstRun(title, BoardTutorialText);
            }

            // Turn off the HUD
            m_Player.PlayerUI.DisableGameHUD();

            m_DecreeMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_Player.m_FollowCamera.m_InMenu = true;

            Time.timeScale = 0;
            m_Player.m_IsMenuOpen = true;
        }

    }

    public void CloseDecreeMenu()
    {
        // Turn on the HUD
        m_Player.PlayerUI.EnableGameHUD();

        m_DecreeMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_Player.m_FollowCamera.m_InMenu = false;

        Time.timeScale = 1;

        StartCoroutine(m_Player.PlayerUI.DisableInputsTimer());
        m_Player.m_IsMenuOpen = false;

    }

    public void AssignQuestToPlayer(int index)
    {
        //Give the quest to the player.
        m_Quests[index].m_Quest.OnAccepted();
        m_AcceptedIndex++;
        m_QuestButtons[index].GetComponent<Button>().interactable = false;
        m_QuestButtons[index].transform.GetChild(2).gameObject.SetActive(true);

        // If the amount of quests accepted is greater than or equal to 2, disable all other buttons.
        if (m_AcceptedIndex >= 2)
        {
            for (int i = 0; i < 4; i++)
            {
                m_QuestButtons[i].GetComponent<Button>().interactable = false;

                if (!m_QuestButtons[i].transform.GetChild(2).gameObject.activeSelf)
                {
                    m_QuestButtons[i].SetActive(false);
                }
            }
        }
    }

    public void CheckForReset()
    {
        // If you have compelted 2 quests, reset the board
        m_CompletedIndex++;

        if (m_CompletedIndex >= 2)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
        // Reset all values to defau
        m_CompletedIndex = 0;
        m_AcceptedIndex = 0;

        for (int i = 0; i < 4; i++)
        {
            Destroy(m_Quests[i].m_Quest.m_Tasks[0]);
            Destroy(m_Quests[i].m_Quest);
        }

        for (int i = 0; i < 4; i++)
        {
            m_Quests[i].GenerateRandomQuest();
        }

        m_DecreeMenu.SetActive(true);

        for (int i = 0; i < m_Quests.Count; i++)
        {
            m_Quests[i].m_Quest.m_Tasks.Add(m_TalkToTask);
            m_Quests[i].m_Quest.m_IsDecreeQuest = true;
            m_QuestButtons[i].GetComponent<Image>().sprite = m_DecreeTierSprites[(int)m_Quests[i].m_QuestTier];
            m_QuestButtons[i].gameObject.SetActive(true);
            m_QuestButtons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = m_Quests[i].m_Quest.m_QuestName;
            m_QuestButtons[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = m_Quests[i].m_Quest.m_QuestDescription;
            m_QuestButtons[i].gameObject.transform.GetChild(2).gameObject.SetActive(false);
            m_QuestButtons[i].GetComponent<Button>().interactable = true;
        }

        m_DecreeMenu.SetActive(false);
    }
}
