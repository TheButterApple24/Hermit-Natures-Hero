/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Node
Description:        The base class that all node type inherit from.
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created Node.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE };
    public Status status;
    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string name;

    public Node() { }
    public Node(string n)
    {
        // Set the name of the Node
        name = n;

    }

    public virtual Status Process()
    {
        // Process the logic of the current child
        return children[currentChild].Process();
    }

    public void AddChild(Node n)
    {
        // Add a child to the list of children
        children.Add(n);
    }

}
