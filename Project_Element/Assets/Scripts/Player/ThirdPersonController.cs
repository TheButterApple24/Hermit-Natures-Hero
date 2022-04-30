/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ThirdPersonController
Description:        Handles how the player will move when given input
Date Created:       05/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    05/10/2021
        - [Jeffrey] Started ThirdPersonController.
    06/10/2021
        - [Jeffrey] Attempted CharacterController movement. 
    08/10/2021
        - [Jeffrey] Scrapped CharacterController movement. Attempted Rigidbody movement. Created Basic Rigidbody movement.Added walk and jump. Implemented basic climbing.
    15/10/2021
        - [Jeffrey] Implemented Dodge and Sprint mechanics. Fixed issues with velocity while jumping.
    16/10/2021
        - [Jeffrey] Implemented Dodging and Sprinting
    18/10/2021
        - [Jeffrey] Implemented variable to handle when player is climbing
    19/10/2021
        - [Max] Added Attack Button/Function
    20/10/2021
        - [Max] Added Attack Button/Function.
        - [Zoe] Added m_IsHolding Variable to check is Player is holding an object such as the PuzzleCube.
    21/10/2021
        - [Zoe] m_IsHolding bool is changed to an InteractableBase and moved to Player script.
    04/11/2021
        - [Aaron] Set up the Swap Pet Function to change the pet in the Primary and Secondary slots with each other. 
        - [Aaron] Update UI icons to show the correction pet position.
	07/11/2021
        - [Jeffrey] Refactored Dodge system
        - [Jeffrey] Implemented variables for Lock-On
    08/11/2021
        - [Max] Cleaned up Attack Code. Removed RotateWeapon
        - [Zoe] Renamed m_IsAcceleratingForSprint to m_IsSprinting
    09/11/2021
        - [Aaron] Set up the Main Ability Input button that will call the Player's Use Main Ability 
    12/11/2021
        - [Max] Fixed Attack Command
	19/01/2022
        - [Jeffrey] Implemented swimming
    20/01/2021
        - [Max] Disables movement and inputs when player inputs are disabled
    03/02/2022
        - [Aaron] Updated Activate and Deactivate Sprint functions to account for updating pet speeds if/when the player has pets to follow them
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    22/02/2022
        - [Jeffrey] Implemented Player animations
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    09/03/2022
        - [Max] Weapon class refactor
    09/03/2022
        - [Max] Updated base ground movement values

 ===================================================*/

using System.Collections;
using UnityEngine;
using Movement;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    public Player m_Player;
    public MovementState m_MovementState;

    [Header("Camera")]
    public Transform m_CameraTransform;
    public float m_TurnSmoothTime = 0.1f;

    [Header("In Air Movement")]
    public float m_InAirMoveAccel = 30.0f;
    public float m_InAirMaxHorizSpeed = 20.0f;
    public float m_InAirMaxVertSpeed = 50.0f;

    [Header("On Ground Movement")]
    public float m_TurnSmoothVelocity;
    public float m_OnGroundMoveAccel = 10.0f;
    public float m_DefaultSpeed = 5.0f;
    private float m_OnGroundMaxSpeed = 5.0f;
    public float m_OnGroundStopEaseSpeed = 10.0f;
    [HideInInspector] public bool m_IsClimbing;
    public float MaxOnGroundAnimSpeed = 10.0f;

    [Header("Sprint Settings")]
    public float m_SprintSpeed = 9.0f;
    public float m_SprintAccel = 9.0f;
    [HideInInspector] public bool m_IsCurrentlySprinting;

    [Header("Dodge Settings")]
    public float m_MaxDodgeForce = 10.0f;
    public LayerMask m_PlayerCheckMask;
    [HideInInspector] public bool m_HasDodged = false;
    [HideInInspector] public bool m_DodgeCoolingDown = false;

    [Header("Ground Checking")]
    //public bool m_IsGrounded = true;
    public float m_GroundDistance = 5.0f;
    public LayerMask m_GroundCheckMask;
    public Transform m_GroundChecker;
    public float m_CheckForGroundRadius = 0.5f;
    public float m_GroundResolutionOverlap = 0.05f;

    [Header("Jump Settings")]
    public float m_JumpSpeed = 10.0f;
    public float m_GravityAccel = -10.0f;

    [Header("Jiggle Settings")]
    public float m_JiggleFrequency = 0.0f;
    public float m_MaxJiggleOffset = 3.0f;

    [Header("Step Up Settings")]
    public bool m_IsInstantStepUp = false;
    public float m_StepUpEaseSpeed = 10.0f;
    public float m_MinAllowedSurfaceAngle = 15.0f;

    // [Header("Sitting")]

    [Header("Swimming")]
    public float m_SwimSpeed;
    public PlayerSwimming m_PlayerSwim;

    [Header("Physics")]
    public Rigidbody m_RigidBody;
    public bool m_IsCollidingWithWall = false;
    public bool m_SetRotation = false;

    public Vector3 m_Inputs = Vector3.zero;
    private Vector3 m_Velocity;
    private float m_CenterHeight;
    private float m_TimeTillNextJiggle;
    private float m_TimeInAir;
    private float m_JumpTimeLeft;
    private bool m_AllowJump;
    private float m_TimeLeftToAllowMidAirJump;
    private bool m_IsSprinting = false;
    private bool m_IsJumpingAnim = false;

    public Vector3 m_GroundVelocity { get; private set; }
    public Vector3 m_GroundAngularVelocity { get; private set; }
    public Vector3 m_GroundNormal { get; private set; }

    public Animator m_Animator { get; private set; }
    public RuntimeAnimatorController m_AnimatorController;

    public GameObject FootLocationObj;
    public float GroundCheckStartOffsetY = 0.5f;
    public float VectorVisualizeScale = 2.0f;
    public bool InstantStepUp = false;
    public float JumpPushOutOfGroundAmount = 0.5f;
    public float MaxJumpHoldTime = 0.5f;
    public float MaxMidAirJumpTime = 0.3f;

    public bool CheckForInput = false;
    private float m_SprintTimer = 0.0f;

    void Start()
    {

        //Initialing miscellaneous values
        //m_GroundCheckMask = ~LayerMask.GetMask("Player", "Ignore Raycast");

        m_RigidBody = GetComponent<Rigidbody>();

        m_Velocity = Vector3.zero;

        m_AllowJump = true;

        GameObject modelObject = transform.Find("HermitAnims").gameObject;

        m_Animator = modelObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        // If menus aren't open and inputs aren't disabled, allow inputs
        if (!m_Player.m_IsMenuOpen && !m_Player.m_IsInputDisabled)
        {
            if (m_MovementState != MovementState.Sitting)
            {
                if (m_MovementState == MovementState.Swimming)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        ActivateJump();
                    }
                }

                CheckForSprinting();

                float attackTrigger = Input.GetAxisRaw("Attack");
                // Keyboard
                if ((Input.GetButtonDown("Attack") &&
                    m_MovementState != MovementState.Swimming) ||
                    // Controller
                    (attackTrigger != 0 &&
                    m_MovementState != MovementState.Swimming)) // m_MovementState != MovementState.Climbing
                {
                    HandleAttack();
                }
                else if (m_MovementState == MovementState.OnGround && !m_IsJumpingAnim)
                {
                    // Disable swing

                }
            }

            if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
            {
                m_SprintTimer += Time.deltaTime;
                if (m_SprintTimer > 0.5f)
                {
                    CheckForInput = true;
                    m_SprintTimer = 0.0f;
                }

            }
            else
            {
                CheckForInput = false;
                m_SprintTimer = 0.0f;
            }

            /* ******************** PET ******************** */

            // Check for Swap Input
            if (Input.GetButtonDown("SwapPets"))
            {
                if (m_Player.m_PrimaryPet != null && m_Player.m_SecondaryPet != null)
                {
                    m_Player.SwapPets();
                }
            }

            // Check for Main Ability Input
            if (Input.GetButtonDown("MainAbility"))
            {
                if (m_Player.m_PrimaryPet != null)
                {
                    m_Player.UseMainAbility();
                }
            }

            // Check for Ultimate Ability Input
            if (Input.GetButtonDown("UltimateAbility"))
            {
                if (m_Player.m_PrimaryPet != null && m_Player.m_SecondaryPet != null)
                {
                    m_Player.UseUltimateAbility();
                }
            }


            /* ******************** MENU ******************** */

            // Pause Menu Input
            if (Input.GetButtonDown("PauseMenu"))
            {
                m_Player.PlayerUI.OpenPauseMenu();
            }

            if (Input.GetButtonDown("BackpackMenu"))
            {
                m_Player.PlayerUI.OpenBackpackMenu();
            }

            // Skill Tree Menu Input
            //float skillTreeTrigger = Input.GetAxisRaw("OpenSkillTree");
            //
            //if (skillTreeTrigger > 0)
            //{
            //    m_Player.PlayerUI.OpenSkillTreeMenu();
            //}
            //
            //if (Input.GetButtonDown("OpenSkillTree"))
            //{
            //    m_Player.PlayerUI.OpenSkillTreeMenu();
            //}
        }
        else if (m_Player.m_IsMenuOpen)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                // Program a way to close menu thats currently open
            }
        }


        UpdateJiggles();

        if (!m_Player.m_IsInputDisabled)
        {
            UpdateAnimations();
        }
    }

    void FixedUpdate()
    {
        //Update velocity from physics system
        m_Velocity = m_RigidBody.velocity;

        //Update ground info

        if (m_MovementState != MovementState.Swimming)
        {
            UpdateGroundInfo();
        }

        bool isJumping = false;
        if (!m_Player.m_IsInputDisabled && !m_Player.m_IsMenuOpen)
        {
            //Get input
            m_Inputs = Vector3.zero;
            m_Inputs.x = Input.GetAxis("Horizontal");
            m_Inputs.z = Input.GetAxis("Vertical");

            //Vector3 localMoveDir = m_Inputs;
            //
            //localMoveDir.Normalize();

            isJumping = Input.GetButton("Jump");
        }

        Vector3 localMoveDir = Vector3.zero;

        // Calculate direction
        Vector3 direction = new Vector3(m_Inputs.x, 0.0f, m_Inputs.z).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Make character face releative to the direction the camera is facing
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + m_CameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            localMoveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
        }

        localMoveDir.Normalize();

        //Get rotation from controller
        //Vector3 rotation = new Vector3(Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"),0.0f ); 
        //rotation.x = 0;// We want to lock rotation around y x axis   ---------->
        //transform.Rotate(rotation * Time.deltaTime * 25); //rotate object based on current mouse rotation
        // Update movement
        switch (m_MovementState)
        {
            case MovementState.OnGround:
                UpdateOnGround(localMoveDir, isJumping);
                break;
            case MovementState.InAir:
                UpdateInAir(localMoveDir, isJumping);
                break;
            case MovementState.Swimming:
                UpdateSwimming(localMoveDir, isJumping);
                break;
            case MovementState.Disable:
                break;

            default:
                //  DebugUtils.LogError("Invalid movement state: {0}", m_MovementState);
                break;
        }

    }

    public void UpdateStopping(float stopEaseSpeed)
    {
        //Ease down to the ground velocity to stop relative to the ground
        m_Velocity = MathUtils.LerpTo(stopEaseSpeed, m_Velocity, m_GroundVelocity, Time.fixedDeltaTime);
    }

    void UpdateGroundInfo()
    {
        //Clear ground info.  Doing this here can simplify the code a bit since we deal with cases where the
        //ground isn't found more easily
        m_GroundAngularVelocity = Vector3.zero;
        m_GroundVelocity = Vector3.zero;
        m_GroundNormal.Set(0.0f, 0.0f, 1.0f);

        //Check for the ground below the player
        m_CenterHeight = transform.position.y;

        float footHeight = FootLocationObj.transform.position.y;

        float halfCapsuleHeight = m_CenterHeight - footHeight;

        Vector3 rayStart = transform.position;
        rayStart.y += GroundCheckStartOffsetY;

        Vector3 rayDir = Vector3.down;

        float rayDist = halfCapsuleHeight + GroundCheckStartOffsetY - m_CheckForGroundRadius;

        //Find all of the surfaces overlapping the sphere cast
        RaycastHit[] hitInfos = Physics.SphereCastAll(rayStart, m_CheckForGroundRadius, rayDir, rayDist, m_GroundCheckMask.value);

        //Get the closest surface that is acceptable to walk on.  The order of the 
        RaycastHit groundHitInfo = new RaycastHit();
        bool validGroundFound = false;
        float minGroundDist = float.MaxValue;

        foreach (RaycastHit hitInfo in hitInfos)
        {
            //Check the surface angle to see if it's acceptable to walk on.  
            //Also checking if the distance is zero I ran into a case where the sphere cast was hitting a wall and
            //returning weird resuls in the hit info.  Checking if the distance is greater than 0 eliminates this 
            //case. 
            float surfaceAngle = MathUtils.CalcVerticalAngle(hitInfo.normal);
            if (surfaceAngle < m_MinAllowedSurfaceAngle || hitInfo.distance <= 0.0f)
            {
                continue;
            }

            if (hitInfo.distance < minGroundDist)
            {
                minGroundDist = hitInfo.distance;

                groundHitInfo = hitInfo;

                validGroundFound = true;
            }
        }

        if (!validGroundFound)
        {
            if (m_MovementState != MovementState.Disable)
            {
                SetMovementState(MovementState.InAir);
            }
            return;
        }

        //Step up
        Vector3 bottomAtHitPoint = MathUtils.ProjectToBottomOfCapsule(
          groundHitInfo.point,
          transform.position,
          halfCapsuleHeight * 2.0f,
          m_CheckForGroundRadius
          );

        float stepUpAmount = groundHitInfo.point.y - bottomAtHitPoint.y;
        m_CenterHeight += stepUpAmount - m_GroundResolutionOverlap;

        //Setting Ground Normal based on object normal we cought trough rayCast.
        m_GroundNormal = groundHitInfo.normal;

        //Set the movement state to be on ground
        if (m_MovementState != MovementState.Disable)
        {
            SetMovementState(MovementState.OnGround);
        }
    }

    void UpdateOnGround(Vector3 localMoveDir, bool isJumping)
    {
        //if movement is close to zero just stop
        if (localMoveDir.sqrMagnitude > MathUtils.CompareEpsilon)
        {

            //Since the movement calculations are easier to do with out taking the ground velocity into account
            //we are calculating the velocity relative to the ground
            Vector3 localVelocity = m_Velocity - m_GroundVelocity;

            //The world movement accelration
            //Vector3 moveAccel = CalcMoveAccel(localMoveDir);
            Vector3 moveAccel = localMoveDir;
            Debug.DrawLine(transform.position, transform.position + moveAccel * VectorVisualizeScale, Color.green);
            //Adjust acceleration to follow slope
            Vector3 groundTangent = moveAccel - Vector3.Project(moveAccel, m_GroundNormal);
            groundTangent.Normalize();
            moveAccel = groundTangent;

            //The velocity along the movement direction
            Vector3 velAlongMoveDir = Vector3.Project(localVelocity, moveAccel);

            //If we are changing direction, come to a stop first.  This makes the movement more responsive
            //since the stopping happens a lot faster than the acceleration typically allows
            if (Vector3.Dot(velAlongMoveDir, moveAccel) > 0.0f) // if dot product is positive than we are going forward
            {
                //Use a similar method to stopping to ease the movement to just be in the desired move direction
                //This makes turning more responsive
                localVelocity = MathUtils.LerpTo(m_OnGroundStopEaseSpeed, localVelocity, velAlongMoveDir, Time.fixedDeltaTime);
            }
            else //slow down when direction is changed
            {
                localVelocity = MathUtils.LerpTo(m_OnGroundStopEaseSpeed, localVelocity, Vector3.zero, Time.fixedDeltaTime);
            }

            //Apply acceleration to velocity
            moveAccel *= m_OnGroundMoveAccel;

            localVelocity += moveAccel * Time.fixedDeltaTime;

            localVelocity = Vector3.ClampMagnitude(localVelocity, m_OnGroundMaxSpeed);

            //Update the world velocity
            m_Velocity = localVelocity + m_GroundVelocity;
        }
        else
        {
            UpdateStopping(m_OnGroundStopEaseSpeed);
        }

        //Handle jump input
        if (isJumping)
        {
            ActivateJump();
        }
        else
        {
            m_AllowJump = true;
        }

        //Move the character controller
        ApplyVelocity(m_Velocity);

        //Ease the height up to the step up height
        Vector3 playerCenter = transform.position;

        if (InstantStepUp) //adjust capsule vertical position instantly, depending on raycast returned surface
        {
            playerCenter.y = m_CenterHeight;
        }
        else
        {
            playerCenter.y = MathUtils.LerpTo(m_StepUpEaseSpeed, playerCenter.y, m_CenterHeight, Time.deltaTime);
        }

        transform.position = playerCenter;

        //Reset time in air
        m_TimeInAir = 0.0f;
    }

    void UpdateInAir(Vector3 localMoveDir, bool isJumping)
    {
        //Check if move direction is large enough before applying acceleration
        if (localMoveDir.sqrMagnitude > MathUtils.CompareEpsilon)// if we are curently moving
        {
            //The world movement accelration
            // Vector3 moveAccel = CalcMoveAccel(localMoveDir);
            Vector3 moveAccel = localMoveDir;

            moveAccel *= m_InAirMoveAccel;

            m_Velocity += moveAccel * Time.fixedDeltaTime;

            //Clamp velocity
            m_Velocity = MathUtils.HorizontalClamp(m_Velocity, m_InAirMaxHorizSpeed);

            m_Velocity.y = Mathf.Clamp(m_Velocity.y, -m_InAirMaxVertSpeed, m_InAirMaxVertSpeed);
        }

        //Update mid air jump timer and related jump.  This timer is to make jump timing a little more forgiving 
        //by letting you still jump a short time after falling off a ledge.
        if (m_JumpTimeLeft <= 0.0f)
        {
            if (m_TimeLeftToAllowMidAirJump > 0.0f)
            {
                m_TimeLeftToAllowMidAirJump -= Time.fixedDeltaTime;

                if (isJumping)
                {
                    ActivateJump();
                }
                else
                {
                    m_AllowJump = true;
                }
            }
        }
        else
        {
            m_TimeLeftToAllowMidAirJump = 0.0f;
        }

        //Update gravity and jump height control
        if (m_JumpTimeLeft > 0.0f && isJumping)
        {
            m_JumpTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            m_Velocity.y += m_GravityAccel * Time.fixedDeltaTime;

            m_JumpTimeLeft = 0.0f;
        }

        //Move the character controller
        ApplyVelocity(m_Velocity);

        //Increment time in air
        m_TimeInAir += Time.deltaTime;
    }

    void UpdateSwimming(Vector3 localMoveDir, bool isJumping)
    {
        //Check if move direction is large enough before applying acceleration
        if (localMoveDir.sqrMagnitude > MathUtils.CompareEpsilon)// if we are curently moving
        {
            //The world movement accelration
            // Vector3 moveAccel = CalcMoveAccel(localMoveDir);
            Vector3 moveAccel = localMoveDir;

            moveAccel *= m_InAirMoveAccel;

            m_Velocity += moveAccel * Time.fixedDeltaTime;

            //Clamp velocity
            m_Velocity = MathUtils.HorizontalClamp(m_Velocity, m_InAirMaxHorizSpeed);

            m_Velocity.y = Mathf.Clamp(m_Velocity.y, -m_InAirMaxVertSpeed, m_InAirMaxVertSpeed);
        }

        //Update gravity and jump height control
        if (m_JumpTimeLeft > 0.0f && isJumping)
        {
            m_JumpTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            m_Velocity.y += m_GravityAccel * Time.fixedDeltaTime;

            m_JumpTimeLeft = 0.0f;
        }

        //Move the character controller
        ApplyVelocity(m_Velocity);

        //Increment time in air
        m_TimeInAir += Time.deltaTime;
    }

    void ApplyVelocity(Vector3 velocity)
    {
        Vector3 velocityDiff = velocity - m_RigidBody.velocity;

        m_RigidBody.AddForce(velocityDiff, ForceMode.VelocityChange);
    }

    void ActivateJump()
    {
        //The allowJump bool is to prevent the player from holding down the jump button to bounce up and down
        //Instead they will have to release the button first.
        if (m_AllowJump)
        {
            //Set the vertical speed to be the jump speed + the ground velocity
            m_Velocity.y = m_JumpSpeed + m_GroundVelocity.y;

            //This is to ensure that the player wont still be touching the ground after the jump
            transform.position += new Vector3(0.0f, JumpPushOutOfGroundAmount, 0.0f);

            //Set the jump timer
            m_JumpTimeLeft = MaxJumpHoldTime;

            m_AllowJump = false;
        }

        if (m_MovementState == MovementState.Swimming)
        {
            Vector3 dir = transform.forward + transform.up;
            dir.Normalize();
            m_RigidBody.AddForce(dir * 5, ForceMode.Impulse);
        }
    }

    Vector3 CalcMoveAccel(Vector3 localMoveDir)
    {
        Vector3 moveAccel = localMoveDir;

        //Transforms direction x, y, z from local space to world space.
        moveAccel = transform.TransformDirection(moveAccel);

        return moveAccel;
    }

    void SetMovementState(MovementState movementState)
    {
        switch (movementState)
        {
            case MovementState.OnGround:
                m_TimeLeftToAllowMidAirJump = MaxMidAirJumpTime;
                break;

            case MovementState.InAir:
                break;

            case MovementState.Disable:
                m_Velocity = Vector3.zero;
                ApplyVelocity(m_Velocity);
                break;

            default:
                // DebugUtils.LogError("Invalid movement state: {0}", movementState);
                break;
        }

        m_MovementState = movementState;
    }

    void UpdateJiggles()
    {
        if (m_JiggleFrequency <= 0.0f)
        {
            return;
        }

        //Update timer
        m_TimeTillNextJiggle -= Time.deltaTime;

        if (m_TimeTillNextJiggle <= 0.0f)
        {
            m_TimeTillNextJiggle = 1.0f / m_JiggleFrequency;

            //Approximate normal distribution
            float minRange = -1.0f;
            float maxRange = 1.0f;
            float offsetAmount = UnityEngine.Random.Range(minRange, maxRange);
            offsetAmount += UnityEngine.Random.Range(minRange, maxRange);
            offsetAmount += UnityEngine.Random.Range(minRange, maxRange);
            offsetAmount += UnityEngine.Random.Range(minRange, maxRange);
            offsetAmount /= 4.0f;

            offsetAmount *= m_MaxJiggleOffset;

            //Offset the player position
            Vector3 offset = UnityEngine.Random.onUnitSphere * offsetAmount;
            offset.y = Mathf.Abs(offset.y);

            transform.position += offset;
        }
    }

    void CheckForSprinting()
    {
        if (m_Player.m_StaminaComp.m_CurrentStamina > 0 && m_RigidBody.velocity != Vector3.zero)
        {
            // If toggle sprint is off
            if (m_Player.m_Properties != null)
            {
                if (!m_Player.m_Properties.m_ToggleSprint)
                {
                    // If player presses sprint and is allowed, activate sprint
                    if (Input.GetButton("Sprint") && m_MovementState == MovementState.OnGround && CheckForInput)
                    {
                        ActivateSprint();
                    }
                    else
                    {
                        DeactivateSprint();
                    }
                }
                else
                {
                    // If you are not sprinting and hit sprint, start. 
                    if (Input.GetButtonDown("Sprint") && !m_IsCurrentlySprinting && m_MovementState == MovementState.OnGround && CheckForInput)
                    {
                        m_IsCurrentlySprinting = true;
                        ActivateSprint();
                    }
                    // If you are sprinting and hit sprint, stop. 
                    else if (Input.GetButtonDown("Sprint") && m_IsCurrentlySprinting && CheckForInput)
                    {
                        m_IsCurrentlySprinting = false;
                        DeactivateSprint();
                    }
                }
            }
        }
        else
        {
            DeactivateSprint();
        }

        if (m_IsCurrentlySprinting || m_IsSprinting)
        {
            m_Player.m_StaminaComp.DrainStamina(m_Player.m_StaminaComp.m_SprintDrainRate * Time.deltaTime);
        }
    }

    void HandleAttack()
    {
        // If Main Weapon exists
        CharacterBase characterBase = GetComponent<CharacterBase>();

        if (characterBase.m_MainWeapon != null)
        {
            // If Main Weapon is Parented and Can Attack
            if (characterBase.m_MainWeapon.IsParented() && characterBase.m_MainWeapon.CanAttack() &&
                m_MovementState != MovementState.Swimming && m_MovementState != MovementState.Sitting)
            {
                if (m_Player.m_Target != null)
                {
                    Vector3 lookRot = m_Player.m_Target.transform.position;
                    lookRot.y = transform.position.y;

                    transform.LookAt(lookRot, Vector3.up);
                }
                // Attack
                characterBase.Attack();

                m_Animator.Play("Attack");
            }

        }
    }

    void UpdateAnimations()
    {
        if (Input.GetButtonDown("Jump"))
        {
            m_IsJumpingAnim = true;
            //m_Animator.Play("JumpStart");
        }
        else if (m_TimeInAir == 0)
        {
            m_IsJumpingAnim = false;
            //m_Animator.Play("JumpEnd");
        }

        m_Animator.SetBool("OnGround", !m_IsJumpingAnim);
        m_Animator.SetFloat("TimeInAir", m_TimeInAir);

        Vector3 localRelativeVelocity = m_Velocity - m_GroundVelocity;
        localRelativeVelocity = transform.InverseTransformDirection(localRelativeVelocity);
        localRelativeVelocity.y = 0.0f;

        m_Animator.SetFloat("ForwardSpeed", localRelativeVelocity.z / MaxOnGroundAnimSpeed);
        m_Animator.SetFloat("RightSpeed", localRelativeVelocity.x / MaxOnGroundAnimSpeed);
    }

    void ActivateSprint()
    {
        Vector3 direction = m_Velocity.normalized;
        if (!m_IsSprinting)
        {
            // If player is not accelaerating for sprint, apply force
            m_RigidBody.AddForce(direction * m_SprintAccel);
            m_IsSprinting = true;
            m_IsCurrentlySprinting = true;
        }
        m_OnGroundMaxSpeed = m_SprintSpeed;
        //m_DefaultSpeed = m_SprintSpeed;
        m_OnGroundMoveAccel = m_SprintSpeed;

        // Set the pet's speed to match the player's
        if (m_Player.m_PrimaryPet)
            m_Player.m_PrimaryPet.CurrentPetSpeed = m_SprintSpeed;

        if (m_Player.m_SecondaryPet)
            m_Player.m_SecondaryPet.CurrentPetSpeed = m_SprintSpeed;
    }

    public void DeactivateSprint()
    {
        m_OnGroundMaxSpeed = m_DefaultSpeed;
        m_OnGroundMoveAccel = m_DefaultSpeed;
        m_IsSprinting = false;
        m_IsCurrentlySprinting = false;

        // Set the pet's speed to match the player's
        if (m_Player.m_PrimaryPet)
            m_Player.m_PrimaryPet.CurrentPetSpeed = m_DefaultSpeed;

        if (m_Player.m_SecondaryPet)
            m_Player.m_SecondaryPet.CurrentPetSpeed = m_DefaultSpeed;
    }

    //This function is called when the script is loaded or a value is changed in the inspector.
    //Note that this will only called in the editor.
    void OnValidate()
    {
        m_TimeTillNextJiggle = 0.0f;
    }

    public void ResetVelocity()
    {
        m_Velocity = Vector3.zero;
        m_GroundVelocity = Vector3.zero;
        m_Animator.SetFloat("ForwardSpeed", 0);
        m_Animator.SetFloat("RightSpeed", 0);
    }
}
