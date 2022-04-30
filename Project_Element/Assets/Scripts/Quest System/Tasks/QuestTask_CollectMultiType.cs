/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_CollectMultiType
Description:        Handles CollectMultiType task
Date Created:       30/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    30/10/2021
        - [Jeffrey] Created base class
    20/01/2022
        - [Jeffrey] Implemented logic
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks
    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTask_CollectMultiType : QuestTask
{
    public Pickups[] m_TargetObjects;

    public int[] m_TargetNumbers;
    public int[] m_CurrentNumbers;

    protected override void Start()
    {
        base.Start();
        m_CurrentNumbers = new int[m_TargetObjects.Length];
    }

    public override void InitTask()
    {
        base.InitTask();
        // Set Task to Active
        m_IsCurrentlyActive = true;

        if (HUDManager.Instance.QuestTipBG.gameObject.activeSelf != true)
            HUDManager.Instance.QuestTipBG.gameObject.SetActive(true);

        UpdateUI();
    }

    public override void UpdateUI()
    {
        for (int i = 0; i < m_TargetObjects.Length; i++)
        {
            HUDManager.Instance.QuestTipText.text = m_TaskTip + " (" + m_CurrentNumbers[i] + "/" + m_TargetNumbers[i] + ")";
        }
    }

    public override bool CheckForCompletion()
    {
        // Check if player has collected X amount of collectables
        int successCount = 0;

        for (int i = 0; i < m_TargetObjects.Length; i++)
        {
            if (m_TargetNumbers[i] == m_CurrentNumbers[i])
            {
                successCount++;
            }
        }

        if (successCount == m_TargetObjects.Length)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
