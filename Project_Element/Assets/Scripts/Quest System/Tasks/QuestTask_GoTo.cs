/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_GoTo
Description:        Handles GoTo tasks
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

public class QuestTask_GoTo : QuestTask
{
    [HideInInspector] bool m_AtLocation = false;

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
            HUDManager.Instance.QuestTipText.text = m_TaskTip;
            m_NavMarker.m_MarkerImage.color = m_NavMarkerColor;
            m_NavMarker.m_Target = m_NavMarkerTransform;
            m_NavMarker.m_MarkerImage.enabled = true;
        }
    }

    public override bool CheckForCompletion()
    {
        // Check if player is at the target location

        if (m_AtLocation)
        {
            HUDManager.Instance.QuestTipText.text = "";
            m_NavMarker.m_MarkerImage.enabled = false;
            return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_IsCurrentlyActive)
        {
            if (other.tag == "Player")
            {
                m_AtLocation = true;
            }
        }
    }

}
