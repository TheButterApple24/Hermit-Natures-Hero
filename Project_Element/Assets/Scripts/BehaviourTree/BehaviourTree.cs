/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              BehaviourTree
Description:        Inherits from Node and acts as the base of a behaviour tree.
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created BehaviourTree.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        // Because this node acts as the root of any behaviour tree, its name will defrault to "Tree"
        name = "Tree";
    }

    public BehaviourTree(string n)
    {
        // Set a custom name for this node if you'd like
        name = n;
    }

    public override Status Process()
    {
        // processs the code contained within the current child node
        return children[currentChild].Process();
    }

    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treePrintout = "";
        // Create stack that will contain the name and level of each node
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        // Set the current node to the BehaviourTree node
        Node currentNode = this;
        // Create new node level and add it to the node stack
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode });
        // while the node stack contains more than 0 nodes
        while (nodeStack.Count != 0)
        {
            // creates and sets nextNode to the node at the top of the stack
            NodeLevel nextNode = nodeStack.Pop();
            // adds the number of dashes to mark the node level and the name of the node to the tree printout
            treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";
            // loops through all the chidlren of a node
            for (int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                // creates a new node level for each child of the previous node
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }
        // Print out behaviour tree to debug log
    }
}
