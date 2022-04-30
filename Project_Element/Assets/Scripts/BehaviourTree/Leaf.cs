/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Leaf
Description:        Inherits from Node and is used to perform actions or do logic checks.
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created Leaf.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public Leaf() { }
    public Leaf(string n, Tick pm)
    {
        // Set the name of the node and the name of the function it will process
        name = n;
        ProcessMethod = pm;
    }

    public override Status Process()
    {
        // Check that the node has a function to process
        if(ProcessMethod != null)
        {
            // Process the function
            return ProcessMethod();
        }
        // If the node does not have a functiuon to process, the node fails
        return Status.FAILURE;
    }

}
