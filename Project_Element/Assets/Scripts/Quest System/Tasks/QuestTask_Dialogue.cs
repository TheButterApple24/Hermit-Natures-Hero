/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DecreeBoard
Description:        Handles the decree board system
Date Created:       02/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/02/2022
        - [Jeffrey] Created base implementation
    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTask_Dialogue : QuestTask
{
    public Dialogue m_QuestDialogue;
    //public bool m_PickupAfterCompletion = false;
    public override void InitTask()
    {
        base.InitTask();

        // Set Task to Active
        m_IsCurrentlyActive = true;

        HUDManager.Instance.QuestTipText.text = "";
        if (m_NavMarker != null)
        {
            m_NavMarker.m_MarkerImage.enabled = false;
        }

        m_QuestDialogue.StartDialogue();

        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (m_IsCurrentlyActive)
        {
            if (m_NavMarker != null)
            {
                HUDManager.Instance.QuestTipText.text = "";
                m_NavMarker.m_MarkerImage.color = m_NavMarkerColor;
                m_NavMarker.m_Target = m_NavMarkerTransform;
                m_NavMarker.m_MarkerImage.enabled = false;
            }
        }
    }

    public override bool CheckForCompletion()
    {
        if (m_IsCurrentlyActive)
        {
            if (m_QuestDialogue.m_IsComplete == true)
            {
                if (m_NavMarker != null)
                {
                    HUDManager.Instance.QuestTipText.text = "";
                    m_NavMarker.m_MarkerImage.enabled = false;
                }
                if (m_QuestDialogue.IsQuestDialogue)
                {
                    Destroy(m_QuestDialogue);
                }

                return true;
            }
        }

        return false;
    }

}
