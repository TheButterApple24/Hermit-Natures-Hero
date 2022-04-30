/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_KillSingleType
Description:        Handles KillSingleTask task
Date Created:       30/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    30/10/2021
        - [Jeffrey] Created base class
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks
    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTask_KillSingleType : QuestTask
{
    public CharacterBase m_TargetEnemy;

    public int m_TargetNumber;

    [HideInInspector] public int m_KillNumber = 0;

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
        if (m_IsCurrentlyActive)
        {
            HUDManager.Instance.QuestTipText.text = m_TaskTip + " (" + m_KillNumber + "/" + m_TargetNumber + ")";
            if (m_NavMarker != null)
            {
                m_NavMarker.m_MarkerImage.color = m_NavMarkerColor;
                m_NavMarker.m_Target = m_NavMarkerTransform;
                m_NavMarker.m_MarkerImage.enabled = false;
            }
        }
    }

    public override bool CheckForCompletion()
    {
        // Check if player has killed X amount of enemy
        if (m_KillNumber >= m_TargetNumber)
        {
            return true;
        }

        return false;
    }
}
