/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Villager1Behaviour
Description:        Behaviour tree and functions
Date Created:       16/11/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    16/11/21
        - [Gerard] Created base class
    17/11/21
        - [Gerard] Set up basic behaviour tree and patrol sequence
    18/11/21
        - [Gerard] added basic interact functionality, fixed bugs in patrol
    31/03/22
        - [Max] Added Walking Animation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Villager1Behaviour : MonoBehaviour
{
    BehaviourTree m_Tree;
    NavMeshAgent m_Agent;
    Transform m_PlayerTarget;
    public enum ActionState { IDLE, WORKING };
    ActionState m_ActionState = ActionState.IDLE;

    Node.Status m_TreeStatus = Node.Status.RUNNING;

    public GameObject[] m_Waypoints;
    public bool[] m_WaitAtWaypoint;
    public int m_CurrentWP;
    int targetWP;
    public float m_Speed = 5.0f;
    public float m_DetectRadius;

    float m_NodeTimer = 0;
    public float m_WaitTime = 3.0f;
    Animator m_WalkingAnimator;

    // Start is called before the first frame update
    void Start()
    {
        // get nav mesh agent
        m_Agent = GetComponent<NavMeshAgent>();
        // get instance of the player
        m_PlayerTarget = PlayerManager.Instance.Player.transform;
        // set the speed of the nav mesh agent
        m_Agent.speed = m_Speed;

        m_Tree = new BehaviourTree();
        //create base node for behaviour tree
        Selector treeBase = new Selector("Tree Base");

        Sequence patrol = new Sequence("Patrol");

        Leaf canNotDetectPlayer = new Leaf("Can Not Detect Player", CanNotDetectPlayer);
        Leaf goToWaypoint = new Leaf("Go To Waypoint", GoToWaypoint);
        Leaf wait = new Leaf("Wait", Wait);
        Leaf faceNextWaypoint = new Leaf("FaceTarget", StartFacingTarget);

        patrol.AddChild(canNotDetectPlayer);
        patrol.AddChild(goToWaypoint);
        patrol.AddChild(wait);
        patrol.AddChild(faceNextWaypoint);

        treeBase.AddChild(patrol);

        // add the base to the behaviour tree
        m_Tree.AddChild(treeBase);
        // print out the behaviour tree in the debug log
        //m_Tree.PrintTree();

        // Get Animator
        m_WalkingAnimator = GetComponent<Animator>();

    }

    public Node.Status Wait()
    {
        if (m_WaitAtWaypoint[targetWP] == true)
        {
            // increase the node timer by deltatime
            m_NodeTimer += Time.deltaTime;
            // check if the node timer is less than the wait time
            if (m_NodeTimer < m_WaitTime)
            {
                // set the nav mesh agent to stopped
                m_Agent.isStopped = true;
                // return this node as running
                return Node.Status.RUNNING;
            }
            // check if the node timer is greater than the wait time
            if (m_NodeTimer >= m_WaitTime)
            {
                // reset the node timer
                m_NodeTimer = 0;
                // set the nav mesh agent to not stopped
                m_Agent.isStopped = false;
                // return this node as succesful
                return Node.Status.SUCCESS;
            }
            // if all else fails, return this node as failure
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status GoToWaypoint()
    {
        targetWP = m_CurrentWP;
        // call GoToTarget and pass in the current waypoint
        return GoToTarget(m_Waypoints[m_CurrentWP]);
    }
    public Node.Status GoToTarget(GameObject target)
    {
        // call GoToLocation and pass in the targets position, set the return value to Node.Status s
        Node.Status s = GoToLocation(target.transform.position);
        // check if s returned succesful
        if (s == Node.Status.SUCCESS)
        {
            // check if the distance between this NPC and the current waypoint is less than 3
            if (Vector3.Distance(this.transform.position, m_Waypoints[m_CurrentWP].transform.position) < 3)
            {
                // increment the current waypoint
                m_CurrentWP++;
                // check if you have reached the end of the list of waypoints
                if (m_CurrentWP >= m_Waypoints.Length)
                {
                    // reset the current waypoint to 0
                    m_CurrentWP = 0;
                }
                // set the nav mesh agent to not stopped
                m_Agent.isStopped = false;
                // return this node as succesful
                return Node.Status.SUCCESS;
            }
            // check if distance between the target and this NPC is less than or equal to the stopping distance
            if (Vector3.Distance(target.transform.position, transform.position) <= m_Agent.stoppingDistance)
            {
                // return this node as succesful
                return Node.Status.SUCCESS;
            }
            // if all else fails, return failure
            return Node.Status.FAILURE;
        }
        else
        {
            // if the Node.Status s does not return as succesful, return the value of s
            return s;
        }
    }
    public Node.Status GoToLocation(Vector3 destination)
    {
        // get the distance between the target position and this NPC
        float distanceToTarget = Vector3.Distance(destination, transform.position);
        // check if this behaviour tree's action state is idle
        if (m_ActionState == ActionState.IDLE)
        {
            // set the nav mesh agents destination
            m_Agent.SetDestination(destination);
            // set the behaviour tree's action state to working
            m_ActionState = ActionState.WORKING;
        }
        // check if the distance between the nav mesh agents target destination and its calculated end path position is greater than 2
        else if (Vector3.Distance(m_Agent.pathEndPosition, destination) >= 2)
        {
            // set the behaviour trees action state to idle
            m_ActionState = ActionState.IDLE;
            // return this node as failure
            return Node.Status.FAILURE;
        }
        // check if the distance to the target is less than 2
        else if (distanceToTarget < 2)
        {
            // set the behaviour trees action state to idle
            m_ActionState = ActionState.IDLE;
            // return this node as succesful
            return Node.Status.SUCCESS;
        }
        else if (Vector3.Distance(transform.position, m_PlayerTarget.position) < m_DetectRadius)
        {
            // set the behaviour trees action state to idle
            m_ActionState = ActionState.IDLE;
            // return this node as failure
            return Node.Status.FAILURE;
        }
        // if all else fails, return running
        return Node.Status.RUNNING;
    }

    public Node.Status CanNotDetectPlayer()
    {
        // get the distance between the player and this NPC
        float distance = Vector3.Distance(m_PlayerTarget.position, transform.position);
        // check if the distance is within the detection radius
        if (distance > m_DetectRadius)
        {
            // return this node as succesful
            return Node.Status.SUCCESS;
        }
        // if it is not within range, return failure
        return Node.Status.FAILURE;
    }

    public Node.Status Idle()
    {
        // set nav mesh agent to stopped
        m_Agent.isStopped = true;
        // return this node as succesful
        return Node.Status.SUCCESS;
    }

    public Node.Status StartFacingTarget()
    {
        if (m_WaitAtWaypoint[targetWP] == true)
        {
            Node.Status s = NowFacingTarget(m_Waypoints[m_CurrentWP]);
            // return this node based on NowFacingTarget
            return s;
        }
        // return this node as succesful
        return Node.Status.SUCCESS;
    }

    public Node.Status NowFacingTarget(GameObject target)
    {
        FaceTarget(target.transform.position);

        while (Vector3.Angle(transform.forward, (target.transform.position - transform.position).normalized) > 5.0f)
        {
            // return this node as succesful
            return Node.Status.RUNNING;
        }

        // return this node as succesful
        return Node.Status.SUCCESS;
    }

    // Update is called once per frame
    void Update()
    {
        // check if the base node of this behaviour tree has not returned succesful
        if (m_TreeStatus != Node.Status.SUCCESS)
        {
            // process the code contained within its children
            m_Tree.Process();
        }

        //check if the player is within detection radius
        if (Vector3.Distance(this.transform.position, m_PlayerTarget.position) < m_DetectRadius)
        {
            // call FaceTarget and pass in the players position
            FaceTarget(m_PlayerTarget.position);
            m_Agent.isStopped = true;
        }
        else
        {
            m_Agent.isStopped = false;
        }

        // Update Current Speed in Animator
        if (m_WalkingAnimator != null)
        {
            m_WalkingAnimator.SetFloat("CurrentSpeed", m_Agent.velocity.magnitude);
        }

    }

    private void FaceTarget(Vector3 target)
    {
        // get the direction from this enemy to the target
        Vector3 lookPos = target - transform.position;
        // set the y vector to 0
        lookPos.y = 0;
        // call Quaternion LookRotation and pass in the lookPos, set the return value to Quaternion rotation
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        // set this enemies rotation to the return value of Quaternion Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_Speed);
    }

    private void OnDrawGizmosSelected()
    {
        // draw a green sphere to show this enemies detection radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_DetectRadius);
    }
}
