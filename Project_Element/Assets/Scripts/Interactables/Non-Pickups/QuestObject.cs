/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestObject
Description:        Handles interactable objects that will be used for quests
Date Created:       01/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/11/2021
        - [Jeffrey] Created base class
    17/11/2021
        - [Zoe] Now calls base.Activate to hide the button prompt

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : InteractableBase
{

    public override void Start()
    {
        base.Start();
        m_RemoveFromListOnTrigger = true;
    }
    // Start is called before the first frame update
    public override void Activate()
    {
        // Calls InteractableBase.Activate() to hide the button prompt
        //base.Activate();

        // If object has a quest
        if (GetComponent<Quest>() != null && !GetComponent<Quest>().IsAccepted)
        {
            // If it is not active
            if (GetComponent<Quest>().m_IsCurrentlyActive == false)
            {
                // Call OnAccepted
                Quest quest = GetComponent<Quest>();
                quest.OnAccepted();
            }
        }
        else if (GetComponent<QuestTask_TalkTo>())
        {
            QuestTask_TalkTo task = GetComponent<QuestTask_TalkTo>();

            if (task.m_IsCurrentlyActive)
            {
                task.m_HasTalkedTo = true;
            }
        }
    }

    public override void Deactivate()
    {
    }
}
