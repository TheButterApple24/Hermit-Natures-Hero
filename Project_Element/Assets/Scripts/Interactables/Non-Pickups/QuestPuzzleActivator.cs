/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestPuzzleActivator
Description:        Handles puzzle being activated for quests
Date Created:       01/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/11/2021
        - [Jeffrey] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPuzzleActivator : InteractableBase
{
    // Start is called before the first frame update
    public override void Activate()
    {
        if (GetComponent<QuestTask_Puzzle>())
        {
            QuestTask_Puzzle task = GetComponent<QuestTask_Puzzle>();

            if (task.m_IsCurrentlyActive)
            {
                task.m_IsPuzzleComplete = true;
            }
        }
    }


    public override void Deactivate()
    {
    }
}
