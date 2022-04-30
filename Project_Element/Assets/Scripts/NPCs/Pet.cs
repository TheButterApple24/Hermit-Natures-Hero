/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Pet
Description:         Manages a Pet Object's properties. This script is responsible for controlling the pet's movement (position + rotation),
                           updating if the pet's attack target, and managing a specific main and passive ability.
Date Created:       01/11/2021
Author:                Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/11/2021
        -[Aaron] Set up the framework for this class, added the initial properties (name, element type, speed)
    02/11/2021
        -[Aaron] Added the passive ability variable and initialize it depending on the specific pet's element type
    04/11/2021
        -[Aaron] Added a Color variable to track each pet's current color for the Pet Icons in Player
    09/11/2021
        -[Aaron] Added the main ability variable and initalized it depending on the pet's ID number.
    10/11/2021
        -[Aaron] Added a function to allow the Pet to teleport to one of the player's pet socket offsets. Called after climbing
        -[Aaron] Added WaterAbility1 to the main ability initialization
    15/11/2021
        -[Aaron] Removed hardcoded initialization and converted the main ability and passive ability varaibles in public ones, set in the editor
        -[Aaron] Added a Pet Target variable and a method of updating that variable for locking on.
    16/11/2021
        -[Aaron] Updated SpawnAbility to call the needed functions in main ability for situations where the ability is inactive and can't call functions itself
        -[Aaron] Added a Facing Position Target to rotate the pet towards what they should be facing, for moving and standing still
    17/11/2021
        -[Aaron] Added a bool to switch between being at the target position and not at the target position. Changed the update to only update
        when not at it's target positon, and to look forward when at the target position.
        -[Aaron] Updated the spawn ability functuon to trigger the cool down and contain the bulk of the ability logic. This prevents a bug that
        keeps the ability from being used again once it becomes deactive.
    29/11/2021
        -[Aaron] Removed the colour variable and added two sprite variables to for the game's UI to use. Set in the editor
    30/11/2021
        -[Aaron] Edited the way the pet rotates to prevent infinite spin. Created a bool to help manage the state.
        -[Aaron] Renamed public functions for telling the pet when to update it's Goal Position to be more clear and created private functions to help manage inside the class.
    07/12/2021
        - [Aaron] Added a lock bool to check if the pet has been unlocked by the player. Use by the Pet Manager.
	08/12/2021
        - [Max] Fixed Bug where Pets wouldn't teleport to Player
    27/01/2022
        - [Aaron] Changed main ability to call main cooldown now there are also ultimate cooldowns
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    23/02/2022
        - [Aaron] Removed the update rotation bool and code relating to it. Set the Face Direction function to be public so the Pet Detection script can access it.
    24/02/2022
        - [Aaron] Added Trigger functions and Auto Aim function to the pet. However, code doesn't function as expect. Will need to be debugged.
    10/03/2022
        - [Aaron] Changed varaibles and functions for the auto aim functionality. Implemented detection code and pet facing target inside the trigger sphere.
    25/03/2022
        -[Max] Added Pet animations
    28/03/2022
        -[Max] Updated Pet animations to use Animations Events
    13/04/2022
        - [Aaron] Added a distance check function and timer to regularly check if the pet is within range of the player. 

 ===================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using Elements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Pet : MonoBehaviour
{
    [Header("Pet Properties")]
    public string PetName;
    public Element PetElementType = Element.Undefined;
    public float PetDetectionRadius = 25.0f;
    public PassiveAbility PassiveAbility;
    public MainAbility MainAbility;
    [UniqueIdentifier]
    public string SaveId;

    [Header("Pet Sprites")]
    public Sprite PetIconFill;
    public Sprite PetIconOutline;
    public Sprite PetProfileDisplayImage;

    GameObject m_PetPositionTarget;
    GameObject m_PetAbilityTarget;
    NavMeshAgent m_NavAgent;
    float m_CurrentPetSpeed;
    float m_RotationSpeed = 2.0f;
    float m_IconFillAmount = 1.0f;
    bool m_IsUpdatingGoalPosition = false;
    bool m_IsLocked = true;
    bool m_IsDistCheckReady = true;
    Animator m_Animator { get; set; }

    public Animator GetAnimator() { return m_Animator; }

    // Awake is called when a script instance is being loaded
    void Start()
    {
        // Set the NavMesh Agent member with the NavMeshAgent Component 
        m_NavAgent = GetComponent<NavMeshAgent>();

        // Set Animator
        GameObject modelObject = transform.Find("PetAnims").gameObject;

        if (modelObject != null)
        {
            m_Animator = modelObject.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (IsLocked)
            return;

        m_Animator.SetFloat("CurrentSpeed", m_NavAgent.velocity.magnitude);

        if (m_IconFillAmount < 1.0f)
        {
            m_IconFillAmount += Time.deltaTime / MainAbility.AbilityCooldown;

            if (PlayerManager.Instance.Player.m_PrimaryPet != this)
                return;
            else
                HUDManager.Instance.PrimaryPetIconFill.GetComponent<Image>().fillAmount = m_IconFillAmount;
        }
    }

    private void LateUpdate()
    {
        if (IsLocked)
            return;

        // Move the pet towards their Goal Position
        if (m_IsUpdatingGoalPosition == true)
        {
            MoveToGoalPosition();

            if (m_IsDistCheckReady == true)
            {
                m_IsDistCheckReady = false;
                float playerDist = Vector3.Distance(this.transform.position, PlayerManager.Instance.Player.transform.position);

                if (playerDist >= 30.0f)
                {
                    TeleportToPlayer();
                }

                StartCoroutine(DistanceCheckTimer());
            }
        }
    }

    public void SpawnAbility()
    {
        // Only will trigger the ability sequence if it's not already active AND not on cooldown
        if (MainAbility.IsActive == false && MainAbility.OnCooldown == false)
        {
            m_Animator.Play("BaseAttack");

            //StartCoroutine(AttackAnimationDelay());
        }
    }

    // Move the Pet to the player's socket location for that pet
    public void TeleportToPlayer()
    {
        if (m_NavAgent != null && m_PetPositionTarget != null)
        {
            m_NavAgent.Warp(m_PetPositionTarget.transform.position);
        }
    }

    // Used by the Player class to update the target object a pet's main ability will travel towards
    public void UpdatePetTarget(GameObject targetObj)
    {
        m_PetAbilityTarget = targetObj;
        MainAbility.UpdateAbilityTarget(m_PetAbilityTarget);
    }

    // Updates the pet's position until it has reached it's target
    public void StartUpdatingGoalPosition()
    {
        m_IsUpdatingGoalPosition = true;
    }

    // Stop updating the pet's position once it's reached it's target position
    public void StopUpdatingGoalPosition()
    {
        m_IsUpdatingGoalPosition = false;
    }

    // Set the Nav Destination target to move to goal position and set the FaceTargetPosition to rotate and face goal position
    void MoveToGoalPosition()
    {
        if (m_PetPositionTarget != null)
        {
            //m_Animator.SetBool("IsWalking", true);
            SetNavDestination(m_PetPositionTarget.transform.position);
            FaceTargetPosition(m_NavAgent.destination);
        }
    }

    // Will rotate the pet game object to face the direction it is travelling
    public void FaceTargetPosition(Vector3 destination)
    {
        Vector3 lookRot = destination - transform.position;
        lookRot.y = 0;

        if (lookRot != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookRot.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, m_RotationSpeed);
        }
    }

    // Check if the pet is colliding with the player. If so, move the pet out of the player's way
    private void OnCollisionStay(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            CapsuleCollider petCollider = this.gameObject.GetComponent<CapsuleCollider>();

            if (petCollider != null && m_IsLocked == false)
            {
                TeleportToPlayer();
            }
        }
    }

    // Check if an enemy is within the pet's targetting range and faces the pet towards them
    private void OnTriggerStay(Collider other)
    {
        if (IsLocked == true)
            return; 

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy)
        {
            AimAtNearestEnemy();

            if (m_PetAbilityTarget)
            transform.LookAt(m_PetAbilityTarget.transform.position);
        }
    }

    // Sphere cast to find all nearby colliders, then check each collider object for the Enemy tag. If enemies are hit, update the nearest enemy as the current target
    void AimAtNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, PetDetectionRadius);
        GameObject nearestObject = null;
        float nearestCollider = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                float distanceToCollider = Vector3.Distance(collider.transform.position, transform.position);

                if (distanceToCollider < nearestCollider)
                {
                    nearestCollider = distanceToCollider;
                    nearestObject = collider.gameObject;
                }
            }
        }
        UpdatePetTarget(nearestObject);
    }

    // Sets Unity's Pathfinding agent's goal position to reach
    void SetNavDestination(Vector3 targetPos)
    {
        m_NavAgent.destination = targetPos;
    }

    // Updates the speed of the Nav Mesh Agent
    void SetNavSpeed(float speed)
    {
        m_NavAgent.speed = speed;
    }

    // Get & Set for Position Target, changing the position they are aiming for
    public GameObject PetPositionTarget
    {
        get { return m_PetPositionTarget; }
        set { m_PetPositionTarget = value; }
    }

    // Get & Set for the pets speed, changing the speed will update the Nav Mesh Agent
    public float CurrentPetSpeed
    {
        get { return m_CurrentPetSpeed; }

        set
        {
            m_CurrentPetSpeed = value;
            SetNavSpeed(m_CurrentPetSpeed);
        }
    }

    // Get & Set for the locked status of the Pet, to check if the player can use that Pet in the Pet Manager
    public bool IsLocked
    {
        get { return m_IsLocked; }
        set { m_IsLocked = value; }
    }

    public float IconFillAmount
    {
        get { return m_IconFillAmount; }
        set { m_IconFillAmount = value; }
    }

    // Turns on the pet gameobjects's data 
    public void ActivatePet()
    {
        this.gameObject.SetActive(true);
    }

    // Turns off the pet gameobject's data 
    public void DeactivatePet()
    {
        this.gameObject.SetActive(false);
    }

    void OnValidate()
    {
        CreateSaveId();
    }

    //Creates a new save id if one isn't already created
    void CreateSaveId()
    {
        if (SaveId == "")
        {
            Guid guid = Guid.NewGuid();
            SaveId = guid.ToString();
        }

    }

    public void SpawnAbilityFromAnimation()
    {
        MainAbility.ActivateInstance();
        MainAbility.SetStartPoint();
        MainAbility.UpdateAbilityDirection();
        MainAbility.UseAbility();
        MainAbility.PlayStartSound();

        // Set ability to be on cooldown and start the cooldown timer 
        MainAbility.OnCooldown = true;
        StartCoroutine(MainAbility.MainCooldownTimer());
    }

    IEnumerator DistanceCheckTimer()
    {
        yield return new WaitForSeconds(10.0f);
        m_IsDistCheckReady = true;
    }
}
