/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Sequence
Description:        Inherits from Node. Only runs the next child if the previous child succeeds. It succeeds when all children succeed and fails when any child fails. 
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created Sequence.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string n)
    {
        // set the name of this sequence node
        name = n;
    }

    public override Status Process()
    {
        // processes the current child and gets its status
        Status childStatus = children[currentChild].Process();
        // if the child returns running, return this sequence node as running
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        // if the child returns failure, return this sequence node as the same
        if (childStatus == Status.FAILURE) return childStatus;

        // if the child has not returned running or failure, it has succeeded. increment to the next child
        currentChild++;
        // check if you have run out of children to process
        if(currentChild >= children.Count)
        {
            // reset current child to zero
            currentChild = 0;
            // return this sequence node as success
            return Status.SUCCESS;
        }
        // if all else fails return running
        return Status.RUNNING;
    }

}
