/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask_KillMultiTask
Description:        Handles KillMulti task
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

public class QuestTask_KillMultiType : QuestTask
{
    public GameObject[] m_TargetEnemies;

    public int[] m_TargetNumbers;

    public override void InitTask()
    {
        base.InitTask();
        // Get Player Character

        if (HUDManager.Instance.QuestTipBG.gameObject.activeSelf != true)
            HUDManager.Instance.QuestTipBG.gameObject.SetActive(true);
    }

    public override bool CheckForCompletion()
    {
        // Check if player has killed X amount of enemies

        // IF THEY HAVE
        return true;
    }
}
