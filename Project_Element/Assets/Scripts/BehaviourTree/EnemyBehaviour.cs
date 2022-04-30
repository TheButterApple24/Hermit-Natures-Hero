/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              EnemyBehaviour
Description:        Contains the logic and structure for the EnemyBehaviour behaviour tree.
Date Created:       20/10/21
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/21
        - [Gerard] Created EnemyBehaviour.
        - [Gerard] Set up basic behaviour tree.
    02/02/22
        - [Max] Fixed bug related to setting enemy's weapon.
    04/02/22
        - [Max] Set up temporary m_IsInCombat for Enemy Health Bars
    16/02/22
        - [Max] Setting Combat mode also changes Player Combat mode
    12/03/22
        - [Max] Refactored enemy weapons to call Awake method
    20/02/22
        - [Zoe] Tweaked how IsCombat is being set to true in order for the player to properly enter combat

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    BehaviourTree m_Tree;
    NavMeshAgent m_Agent;
    public Transform m_PlayerTarget;
    HealthComponent m_HealthComponent;

    public GameObject testPlayerInstance;

    public enum ActionState { IDLE, WORKING };
    ActionState m_ActionState = ActionState.IDLE;

    Node.Status m_TreeStatus = Node.Status.RUNNING;

    public GameObject[] m_Waypoints;
    public int m_CurrentWP;
    int targetWP;
    public float m_Speed;
    float m_PatrolSpeed = 3.5f;
    float m_ChaseSpeed = 5.0f;

    public float m_DetectRadius = 30.0f;
    public float m_AggroRadius = 25.0f;
    public float m_MeleeRadius = 2.0f;
    float m_DistanceToPlayer;
    float m_CurrentHealth;

    float m_NodeTimer = 0;
    public float m_WaitTime = 3.0f;

    Vector3 m_DirToPlayer;
    Ray m_Ray;

    public Animator m_Animator { get; private set; }

    public void SetPatrolWaypoints(GameObject[] patrolWP)
    {
        m_Waypoints = patrolWP;
    }

    public void SetNavMeshAgent(NavMeshAgent agent)
    {
        m_Agent = agent;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, m_DirToPlayer, out hit, m_DetectRadius))
        {
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody.gameObject.name == "Player")
                {
                    Debug.DrawRay(transform.position, m_DirToPlayer * hit.distance, Color.red);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    private void OnEnable()
    {
        m_CurrentWP = 0;
        if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
        {
            m_PlayerTarget = PlayerManager.Instance.Player.transform;
        }
        if (m_PlayerTarget != null)
        {
            m_DistanceToPlayer = Vector3.Distance(m_PlayerTarget.position, transform.position);
        }
    }

    public void ResetPlayerinfo()
    {
        // m_PlayerTarget = null;
        m_DistanceToPlayer = float.MaxValue;
    }

    private void OnDisable()
    {
        ResetPlayerinfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        testPlayerInstance = PlayerManager.Instance.Player.gameObject;

        // Assign the player and the player controller. If Player.Awake hasn't been called yet, this will be set again in Start
        if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
        {
            m_PlayerTarget = PlayerManager.Instance.Player.transform;
        }

        m_Ray = new Ray(transform.position, -m_DirToPlayer);

        m_HealthComponent = GetComponent<HealthComponent>();

        m_CurrentHealth = m_HealthComponent.m_CurrentHealth;

        // Get Animator Controller
        GameObject modelObject = transform.Find("CultistAnims").gameObject;
        m_Animator = modelObject.GetComponent<Animator>();

        // get nav mesh agent
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.enabled = false;

        // get instance of the player
        m_PlayerTarget = PlayerManager.Instance.Player.transform;

        // set the speed of the nav mesh agent
        m_Speed = m_PatrolSpeed;
        m_Agent.speed = m_Speed;

        // set the weapon prefab to enemy's main weapon
        Enemy enemy = transform.GetComponent<Enemy>();
        enemy.m_MainWeapon = enemy.m_WeaponPrefab.GetComponent<Weapon>();

        // activate enemies weapon
        enemy.m_MainWeapon.Start();
        enemy.m_MainWeapon.Equip();

        // create new behaviour tree
        m_Tree = new BehaviourTree();

        //create base node for behaviour tree
        Selector treeBase = new Selector("Tree Base");

        // create all sequence nodes needed
        Sequence escape = new Sequence("Run Away");
        Sequence patrol = new Sequence("Patrol");
        Sequence chase = new Sequence("Chase");
        Sequence attack = new Sequence("Attack");
        Sequence beginMoveToWaypoint = new Sequence("Begin Move To Waypoint");
        //create all selector nodes needed
        Selector patrolWaypoints = new Selector("Patrol Waypoints");
        // create all leaf nodes needed
        Leaf isLowHealth = new Leaf("Is Low Health", IsLowHealth);
        Leaf isNotLowHealth = new Leaf("Is Not Low Health", IsNotLowHealth);
        Leaf canNotDetectPlayer = new Leaf("Can Not Detect Player", CanNotDetectPlayer);
        Leaf canDetectPlayer = new Leaf("Can Detect Player", CanDetectPlayer);
        Leaf goToPlayer = new Leaf("Move To Player", GoToPlayer);
        Leaf inMeleeRange = new Leaf("In Melee Range", InMeleeRange);
        Leaf attackPlayer = new Leaf("Attack Player", AttackPlayer);
        Leaf goToWaypoint = new Leaf("Move To Waypoint", GoToWP);
        Leaf wait = new Leaf("Wait", Wait);
        Leaf runAway = new Leaf("Run Away", RunAway);
        Leaf faceNextWaypoint = new Leaf("FaceTarget", StartFacingTarget);

        // add all children to begin move to waypoint node
        beginMoveToWaypoint.AddChild(goToWaypoint);
        beginMoveToWaypoint.AddChild(wait);
        beginMoveToWaypoint.AddChild(faceNextWaypoint);
        beginMoveToWaypoint.AddChild(canNotDetectPlayer);

        // add all children to escape node
        escape.AddChild(isLowHealth);
        escape.AddChild(runAway);

        // add all children to attack node
        attack.AddChild(isNotLowHealth);
        attack.AddChild(inMeleeRange);
        attack.AddChild(attackPlayer);
        //attack.AddChild(wait);

        // add all children to chase node
        chase.AddChild(canDetectPlayer);
        chase.AddChild(goToPlayer);

        // add all children to patrol node
        patrol.AddChild(canNotDetectPlayer);
        patrol.AddChild(beginMoveToWaypoint);

        // add all children to tree base node
        treeBase.AddChild(escape);
        treeBase.AddChild(attack);
        treeBase.AddChild(chase);
        treeBase.AddChild(patrol);

        // add the base to the behaviour tree
        m_Tree.AddChild(treeBase);

        // print out the behaviour tree in the debug log
        //m_Tree.PrintTree();
    }

    public Node.Status RunAway()
    {
        Vector3 targetPosition = m_Ray.GetPoint(5);

        m_Agent.SetDestination(targetPosition);

        return Node.Status.SUCCESS;
    }

    public Node.Status AttackPlayer()
    {
        // get the distance between the player and this enemy
        //float distance = Vector3.Distance(m_PlayerTarget.position, transform.position);
        // check if it is within melee range
        if (m_DistanceToPlayer <= m_MeleeRadius && CanSeePlayer())
        {
            FaceTarget(m_PlayerTarget.position);
            // call the character base attack function
            GetComponent<CharacterBase>().Attack();

            // Play Attack Animation
            m_Animator.Play("Attack");

            // return this node as running
            return Node.Status.SUCCESS;
        }
        else
        {
            // if the player is not within range, the attack fails
            return Node.Status.FAILURE;
        }
    }

    public Node.Status InMeleeRange()
    {
        // check if it is within melee range, and return success
        if (m_DistanceToPlayer <= m_MeleeRadius && CanSeePlayer())
        {
            return Node.Status.SUCCESS;
        }
        // if not within melee range, return failure
        return Node.Status.FAILURE;
    }

    public Node.Status Wait()
    {
        // check if the player is within aggro range
        if (m_DistanceToPlayer < m_AggroRadius && CanSeePlayer())
        {
            // set the nav mesh agent to not stopped
            m_Agent.isStopped = false;
            // set the destination to the player
            m_Agent.SetDestination(m_PlayerTarget.position);
            // return this node as failed
            return Node.Status.FAILURE;
        }

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

    public Node.Status IsLowHealth()
    {
        if (m_CurrentHealth < m_HealthComponent.MaxHealth / 3)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status IsNotLowHealth()
    {
        if (m_CurrentHealth > m_HealthComponent.MaxHealth / 3)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status CanDetectPlayer()
    {
        // if the distance is within the detection radius, return succesful
        if (m_DistanceToPlayer <= m_DetectRadius && CanSeePlayer())
        {
            m_Agent.isStopped = true;
            return Node.Status.SUCCESS;
        }
        else
        {
            m_Agent.isStopped = false;
            // if the distance is not within the detection radius, return failure
            return Node.Status.FAILURE;
        }
    }

    public Node.Status CanNotDetectPlayer()
    {
        // if the distance is within the detection radius, return succesful
        if (m_DistanceToPlayer <= m_DetectRadius && CanSeePlayer())
        {
            m_Agent.isStopped = false;
            return Node.Status.FAILURE;
        }
        else
        {
            m_Agent.isStopped = true;
            // if the distance is not within the detection radius, return failure
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status GoToWP()
    {
        targetWP = m_CurrentWP;
        // call GoToTarget and pass in the current waypoint
        return GoToTarget(m_Waypoints[m_CurrentWP]);
    }

    public Node.Status GoToPlayer()
    {
        // call GoToTarget and pass in the instance of the player
        return GoToTarget(PlayerManager.Instance.Player.gameObject);
    }

    public Node.Status GoToTarget(GameObject target)
    {
        // call GoToLocation and pass in the targets position, set the return value to Node.Status s
        Node.Status s = GoToLocation(target.transform.position);

        // check if s returned succesful
        if (s == Node.Status.SUCCESS)
        {
            // check if the distance between this enemy and the current waypoint is less than 3
            if (Vector3.Distance(this.transform.position, m_Waypoints[m_CurrentWP].transform.position) < 3)
            {
                // increment the current waypoint
                m_CurrentWP++;
                // check if you have reached the end of the list of waypoints
                if (m_CurrentWP >= m_Waypoints.Length)
                {
                    // reset the current waypiont to 0
                    m_CurrentWP = 0;
                }
                // set the nav mesh agent to not stopped
                m_Agent.isStopped = false;
                // return this node as succesful
                return Node.Status.SUCCESS;
            }
            // check if distance between the target and this enemy is less than or equal to the stopping distance
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
            if ((s == Node.Status.RUNNING) && (target.tag == "Player") && (Vector3.Distance(target.transform.position, transform.position) > m_DetectRadius))
            {
                m_Agent.SetDestination(m_Waypoints[m_CurrentWP].gameObject.transform.position);
                return Node.Status.FAILURE;
            }
            // if the Node.Status s does not return as succesful, return the value of s
            return s;
        }
    }

    public Node.Status GoToLocation(Vector3 destination)
    {
        // get the distance between the target position and this enemy
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
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

        // check if the distance to the player is less than the aggro radius
        if (m_DistanceToPlayer <= m_AggroRadius && CanSeePlayer())
        {
            m_Agent.isStopped = false;
            // set the nav mesh agents destination to the player
            m_Agent.SetDestination(m_PlayerTarget.position);
            // return this node as failure
            return Node.Status.FAILURE;
        }

        // if all else fails, return running
        return Node.Status.RUNNING;

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
        if (m_Waypoints.Length > 1)
        {
            Node.Status s = NowFacingTarget(m_Waypoints[m_CurrentWP]);
            // return this node based on NowFacingTarget
            return s;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status NowFacingTarget(GameObject target)
    {
        FaceTarget(target.transform.position);

        Vector3 EnemyForwardFace = transform.forward;
        EnemyForwardFace.y = 0;
        Vector3 TargetDir = (target.transform.position - transform.position).normalized;
        TargetDir.y = 0;

        while (Vector3.Angle(EnemyForwardFace, TargetDir) > 5.0f)
        {
            // return this node as succesful
            return Node.Status.RUNNING;
        }

        // return this node as succesful
        return Node.Status.SUCCESS;
    }

    // Update is called once per frame
    public void Update()
    {
        if (m_Agent != null)
        {
            m_Agent.enabled = true;

            if (m_Agent.isActiveAndEnabled)
            {
                if (m_Animator != null)
                {
                    m_Animator.SetFloat("CurrentSpeed", m_Agent.velocity.magnitude);
                }
            }

            if (m_PlayerTarget != null)
            {
                // get the distance between the player and this enemy
                m_DistanceToPlayer = Vector3.Distance(transform.position, m_PlayerTarget.position);
                m_DirToPlayer = m_PlayerTarget.position - transform.position;
                m_Ray.direction = -m_DirToPlayer;
                m_Ray.origin = transform.position;
                //check if the player is within sight
                if (CanSeePlayer() && m_CurrentHealth > m_HealthComponent.MaxHealth / 3)
                {
                    // call FaceTarget and pass in the players position
                    FaceTarget(m_PlayerTarget.position);
                }
                else
                {
                    m_Agent.enabled = true;
                    m_Agent.isStopped = false;
                }
            }

            // check if the distance is within the detection radius
            if (IsInCombat())
            {
                if (!PlayerManager.Instance.Player.SeenEnemies.Contains(this))
                    PlayerManager.Instance.Player.SeenEnemies.Add(this);
            }
            else
            {
                if (PlayerManager.Instance.Player.SeenEnemies.Contains(this))
                    PlayerManager.Instance.Player.SeenEnemies.Remove(this);
            }



            m_CurrentHealth = m_HealthComponent.m_CurrentHealth;


            // check if the base node of this behaviour tree has not returned succesful
            if (m_TreeStatus != Node.Status.SUCCESS)
            {
                // process the code contained within its children
                if (m_Tree != null)
                    m_Tree.Process();
            }
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
        // draw a red sphere to show the melee radius of this enemy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MeleeRadius);
        // draw a yellow sphere to show this enemies aggro radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_AggroRadius);
        // draw a green sphere to show this enemies detection radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_DetectRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(m_Ray);
    }

    public bool IsInCombat()
    {
        if (m_DistanceToPlayer < m_DetectRadius && CanSeePlayer())
        {
            // Set Speed to Chase Speed
            m_Speed = m_ChaseSpeed;
            m_Agent.speed = m_Speed;
            return true;
        }
        else /*if (m_DistanceToPlayer >= m_DetectRadius && !CanSeePlayer())*/
        {
            // Set Speed to Patrol Speed
            m_Speed = m_PatrolSpeed;
            m_Agent.speed = m_Speed;
            return false;
        }
        //return false;
    }
}

