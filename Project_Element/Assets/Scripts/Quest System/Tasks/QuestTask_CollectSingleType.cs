/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_CollectSingleType
Description:        Handles CollectSingleType task
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

public class QuestTask_CollectSingleType : QuestTask
{
    public Pickups m_TargetObjectType;

    public int m_TargetNumber;
    [HideInInspector] public int m_CurrentNumber;

    public bool m_KeepItem = false;

    public override void InitTask()
    {
        base.InitTask();

        // Set Task to Active
        m_IsCurrentlyActive = true;

        if (HUDManager.Instance.QuestTipBG.gameObject.activeSelf != true)
            HUDManager.Instance.QuestTipBG.gameObject.SetActive(true);

        UpdateUI();

       // m_TargetObjectType = m_PickupPrefab.GetComponent<Pickups>()
    }

    public override void UpdateUI()
    {

        if (m_IsCurrentlyActive)
        {
            HUDManager.Instance.QuestTipText.text = m_TaskTip + " (" + m_CurrentNumber + "/" + m_TargetNumber + ")";
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
        ItemStack stack = PlayerManager.Instance.Player.m_Inventory.CheckForItemInInventory(m_TargetObjectType.m_ItemID);

        if (stack != null)
        {
            m_CurrentNumber = stack.ItemAmount;
            UpdateUI();
            if (stack.ItemAmount >= m_TargetNumber)
            {
                // Remove from Inventory
                if (!m_KeepItem)
                {
                    PlayerManager.Instance.Player.m_Inventory.CheckForItemInInventory(m_TargetObjectType.m_ItemID).ItemAmount -= m_TargetNumber;
                    PlayerManager.Instance.Player.m_Inventory.UpdateAllSlots();
                }

                // End task
                return true;
            }
        }

        return false;
    }
}
