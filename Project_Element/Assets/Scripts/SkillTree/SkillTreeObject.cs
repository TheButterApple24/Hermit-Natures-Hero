/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeObject
Description:        Handles data structures for Skill Tree Objects
Date Created:       11/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    11/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillTreeNode
{
    public string PointCost;
    public string SkillProperty;
    public string SkillAmount;
    public int BranchNumber;
    public int BranchStartNumber;
    public string InID;
    public string OutID;
    public bool HasBranches;
}

[System.Serializable]
public struct SkillTreeBranch
{
    public string OutID;
}

[System.Serializable]
public struct SkillTreeConnection
{
    public string InID;
    public string OutID;
}

[CreateAssetMenu(fileName = "SkillTreeObject", menuName = "Project Element/Skill Tree Object")]
public class SkillTreeObject : ScriptableObject
{
    public SkillTreeNode[] Nodes;
    public SkillTreeConnection[] Connections;
    public SkillTreeBranch[] Branches;
}
