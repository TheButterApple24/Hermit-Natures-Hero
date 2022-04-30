/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_CollectSpecial
Description:        Used when collecting a special object during a quest
Date Created:       31/03/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    31/03/2022
        - [Jeffrey] Created base implementation

 ===================================================*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTask_CollectSpecial : QuestTask
{
    public GameObject m_TargetObjectType;

    public int m_TargetNumber;
    [HideInInspector] public int m_CurrentNumber;

    public bool Collected = false;

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
            HUDManager.Instance.QuestTipText.text = m_TaskTip + " (" + m_CurrentNumber + "/" + m_TargetNumber + ")";
            m_NavMarker.m_MarkerImage.color = m_NavMarkerColor;
            m_NavMarker.m_Target = m_NavMarkerTransform;
            m_NavMarker.m_MarkerImage.enabled = true;
        }
    }

    public override bool CheckForCompletion()
    {
        if (Collected)
        {
            return true;
        }

        return false;
    }
}
