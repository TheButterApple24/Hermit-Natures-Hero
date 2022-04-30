/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestListButton
Description:        Stores the quest that it was created for in order to send it's data to the QuestListManager for updating the Quest Menu UI
Date Created:       21/01/2022
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/01/2022
        - [Aaron] Created the class and gave it a Quest variable to store the quest data the button cares about
        - [Aaron] Created basic functions for using the QuestListManager's functions and Updating UI Information when the button is clicked
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListButton : MonoBehaviour
{
    public Quest m_StoredQuest;
    public int m_QuestIndexInList;

    public void DisplayQuestDescription()
    {
        GameManager.Instance.QuestListManager.UpdateQuestDescription(m_QuestIndexInList, m_StoredQuest.m_QuestName, m_StoredQuest.m_QuestDescription);
        GameManager.Instance.QuestListManager.UpdateQuestTaskDescription(m_StoredQuest.m_Tasks[m_StoredQuest.m_TaskIndex].m_TaskTip);
        GameManager.Instance.QuestListManager.UpdateQuestRewards(m_QuestIndexInList);
    }
}
