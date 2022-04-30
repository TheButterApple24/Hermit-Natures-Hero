/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestPickup
Description:        Quest Pickup object
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

public class QuestPickup : Pickups
{
    public bool DeleteObjectOnInteraction = true;
    public bool ShowTextPopup = false;

    public override void Start()
    {
        base.Start();
    }

    public override void Activate()
    {
        base.Activate();

        Player player = PlayerManager.Instance.Player;

        for (int i = 0; i < player.m_AcceptedQuests.Count; i++)
        {
            Quest quest = player.m_AcceptedQuests[i];
            if (quest.m_Tasks[quest.m_TaskIndex].GetType() == typeof(QuestTask_CollectSpecial))
            {
                QuestTask_CollectSpecial task = (QuestTask_CollectSpecial)quest.m_Tasks[quest.m_TaskIndex];

                if (task.m_TargetObjectType.GetComponent<InteractableBase>() == this)
                {
                    task.Collected = true;

                    if (ShowTextPopup)
                    {
                        // Pop up Collected Quest Pickup
                        HUDManager.Instance.UnlockedPopUp.SetActive(true);
                        HUDManager.Instance.UnlockedPopUpText.text = name.ToString() + " Collected!";
                    }

                    if (DeleteObjectOnInteraction)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
