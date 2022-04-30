/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              WhileNode
Description:        Inherits from Node. Only has two children, a condition and an action. 
Date Created:       03/11/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/11/21
        - [Gerard] Created WhileNode.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhileNode : Node
{
    public WhileNode(string n)
    {
        // set the name of this while node
        name = n;
    }

    public override Status Process()
    {
        // check that this node does not have more than 2 children
        if (children.Count > 2) return Status.FAILURE;
        // process both children, the condition and the action
        Status conditionStatus = children[0].Process();
        Status actionStatus = children[1].Process();
        // if either children return running, return this while node as running
        if (conditionStatus == Status.RUNNING || actionStatus == Status.RUNNING) return Status.RUNNING;
        // if either if the children fail, return this while node as failure
        if (conditionStatus == Status.FAILURE || actionStatus == Status.FAILURE) return Status.FAILURE;
        
        // if the action succeeds, return this while node as success
        if(actionStatus == Status.SUCCESS)
        {
            return Status.SUCCESS;
        }
        // if all else fails, return running
        return Status.RUNNING;
    }
}
