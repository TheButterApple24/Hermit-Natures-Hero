/*=================================================== 
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Selector
Description:        Inherits from Node. Only runs the next child if the previous child fails. It succeeds when any child succeeds and fails when all children fail.
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created Selector.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
    {
        // Set the name of the node
        name = n;
    }

    public override Status Process()
    {
        // processes the current child and gets its status
        Status childstatus = children[currentChild].Process();
        // If the child node is running, continue running
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        // if the child returns succesful, return this selector node as succesful
        if(childstatus == Status.SUCCESS)
        {
            // reset current child to zero
            currentChild = 0;
            return Status.SUCCESS;
        }
        // if the child is not running or succesful, it has failed. Increment to the next child
        currentChild++;
        // check if the current child count has exceeded the number of children
        if(currentChild >= children.Count)
        {
            //reset the current child to zero
            currentChild = 0;
            // return this selector node as failure
            return Status.FAILURE;
        }
        // if all else fails, return this selector node as running
        return Status.RUNNING;
    }

}
