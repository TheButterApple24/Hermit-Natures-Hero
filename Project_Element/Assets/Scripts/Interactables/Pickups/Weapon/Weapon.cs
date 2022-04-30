/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Weapon
Description:        Handles weapon item
Date Created:       17/10/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Max] Started Weapon.
    18/10/2021
        - [Max] Added Attack Cooldown functionality to Weapon.
    19/10/2021
        - [Max] Added Combo and Element functions to Weapon Attack.
        - [Max] Integrated Pickups class (old Item class).
    20/10/2021
        - [Max] Implemented Weapon rotation + Retrieves Attack Target Enemy and calculates damage taken
    21/10/2021
        - [Max] Implemented Weapon pickup + enabled proper weapon position when climbing
    30/10/2021
        - [Max] Cleaned up code and implemented sequence diagrams functions. Added Final Damage float.
    31/10/2021
        - [Max] Moved bIsPoisonous to a public field.
    06/11/2021
        - [Zoe] Adjusted m_AttackTarget to be a CharacterBase and adjusted damage calculations and other fields accordingly
    07/11/2021
        - [Max] Re-Implemented Inventory and Material swap support
        - [Max] Modified m_bHitEnemy to m_bHit
    08/11/2021
        - [Max & Zoe] Fixed Weapon bug where main weapon and picked up weapon were both sent to Inventory
        - [Max & Zoe] Cleaned up code. Removed all Bs from bool fields 
        - [Zoe] m_FinalDamage is now set to negative when being sent to the HealthComponent, to accomodate the new function (TakeDamage -> ModifyHealth)
    11/11/2021
        - [Max] Added Randomized Loot Tier base attack values
    12/11/2021
        - [Max] Fixed Mesh Bug and added null checks for Attack Target
        - [Max] Fixed Equip Weapon Bug
    15/11/2021
        - [Zoe] Added functionality to turn on/off the button prompt HUD
    17/11/2021
        - [Zoe] Removed the base.Start
    18/11/2021
        - [Zoe] Added Poison functionality
    19/11/2021
        - [Zoe] Can no longer use objects while your hands are full of puzzle cube
        - [Max] Made SetMaterial Modular + Added GetOwner() function + Added Epic Loot Tier 
	30/11/2021
        - [Zoe] Added m_PoisonDamage
        - [Zoe] Modifiably Poison damage is now being sent to the poison DOTEffect
        - [Zoe] Poison gets removed when weapon is dropped
        - [Zoe] Changes Start to Awake and merged with existing Awake
    03/12/2021
        - [Max] Removed 1 Loot Tier.
        - [Max] Renamed Purple Tier to Heroic.
	05/12/2021
        - [Zoe] Temporarily removed dropping weapons with G as it was causing issues
    06/12/2021
        - [Max] Re-Changed Damage Tier system + Bug Fixes
    08/12/2021
        - [Zoe] Added a null check to GetOwner in BaseDamageTierCheck
    14/01/2022
        - [Max] Replaced Element Type Matchup calculation
    15/01/2022
        - [Max] Added Crit Rate/Dmg System + Reformatted Atk Dmg Calculation
    20/01/2022
        - [Max] Truncated m_FinalDamage (float to int) + Added m_IsHitCrit
    24/01/2022
        - [Max] Added Zero Dmg check to m_FinalDamage
    26/01/2022
        - [Max] Refactored LootTier Calculation
    30/01/2022
        - [Max] Fixed Box Collider bug
    02/02/2022
        - [Max] Added checks to AttachMainWeapon + Removed Weapon socket search + Removed m_IsInteractable
    04/02/2022
        - [Max] Added null owner check to SetMaterial function
    05/02/2022
        - [Max] Removed equip cooldown
    08/02/2022
        - [Max] Added Comments + Fixed Equip Bug
    15/02/2022
        - [Max] Organized variables
    16/02/2022
        - [Zoe] Interaction particles now change colour depending on rarity
    17/02/2022
        - [Zoe] Added a gradient affect for godlike weapon particles
        - [Zoe] Set the default loot tier to none (Randomized by default)
    09/03/2022
        - [Max] Refactored Weapon class
        - [Max] Renamed Weapon class public variables
    10/03/2022
        - [Aaron] Updated to work with Pet Refactor 2.0
        - [Max] Implemented WeaponStatGenerator
    13/03/2022
        - [Max] Changed Attack Trigger behavior
    15/03/2022
        - [Max] Unsheathing and Audio added
    
		
 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PickupTypes;
using Elements;
using LootTiers;
using StatGenerator;

public enum WeaponSFX
{
    Swing,
    Unsheathe,
    Sheathe,
    Hit,
    CritHit,
}

public class Weapon : Pickups
{
    [Header("Loot Tier")]
    public LootTier WeaponLootTier = LootTier.None;

    [Header("Element")]
    public Element WeaponElement;
    public float ElementMultiplier;

    [Header("Damage")]
    //public float BaseDamage;
    public float BaseAttackSpeed = 0.7f;

    [Header("Poison")]
    public float PoisonDamage = 0.0f;
    public float PoisonDuration = 3.0f;

    [Header("Materials")]
    public Material BaseMaterial;
    public Material FireMaterial;
    public Material WaterMaterial;
    public Material PlantMaterial;

    [Header("Crit System")]
    public float CritRate = 6; // 6% CRIT RATE
    public float CritDamageMultiplier = 1.5f; // 1.5x CRIT DMG Multiplier

    [Header("Audio")]
    [SerializeField] AudioSource m_AudioSource;
    [SerializeField] AudioClip m_WeaponSwingSFX;
    [SerializeField] AudioClip m_UnsheathSFX;
    [SerializeField] AudioClip m_SheathSFX;

    [Header("Weapon Misc")]
    public LayerMask WeaponMask;

    int m_ComboCounter = 0;
    const float m_ComboCooldown = 3.0f;
    float m_ComboTimer;
    float m_ComboMultiplier = 1.0f;
    int m_PoisonHitsRemaining = 3;
    bool m_IsCrit;
    Vector3 m_ClimbPos = Vector3.zero;
    Vector3 m_GroundPos = Vector3.zero;

    bool m_IsParented = false;
    bool m_CanAttack;
    bool m_HasHit = false;
    List<HealthComponent> m_AttackTargets = null;
    float m_FinalAttackSpeed = 0.7f;
    bool m_IsPoisonous;
    WeaponInfo m_WeaponInfo = null;

    // Constants
    const int MIN_COMBO_COUNTER = 1;
    const int MAX_COMBO_COUNTER = 4;
    const float COMBO_COOLDOWN = 3.0f;
    const int TOTAL_POISON_HITS = 3;

    // public getters
    public CharacterBase GetOwner() { return m_WeaponInfo.Owner; }
    public bool IsParented() { return m_IsParented; }
    public bool IsParentedToPlayer() { return m_WeaponInfo.IsOwnerPlayer; }
    public bool CanAttack() { return m_CanAttack; }
    public bool HasHit() { return m_HasHit; }
    public List<HealthComponent> GetAttackTargets() { return m_AttackTargets; }
    public float GetFinalAttackSpeed() { return m_FinalAttackSpeed; }
    public bool IsPoisonous() { return m_IsPoisonous; }
    public int GetComboCounter() { return m_ComboCounter; }
    public bool IsHitCrit() { return m_IsCrit; }
    public int GetPoisonHitsRemaining() { return m_PoisonHitsRemaining; }
    public float GetBaseDamage() { return m_WeaponInfo.BaseDamage; }
    public AudioSource GetAudioSource() { return m_AudioSource; }


    // public setters
    public void SetOwner(CharacterBase owner) { m_WeaponInfo.Owner = owner; }
    public void SetParented(bool isParented) { m_IsParented = isParented; }
    public void SetParentedToPlayer(bool isParentedToPlayer) { m_WeaponInfo.IsOwnerPlayer = isParentedToPlayer; }
    public void SetCanAttack(bool canAttack) { m_CanAttack = canAttack; }
    public void SetHasHit(bool hasHit) { m_HasHit = hasHit; }
    public void AddAttackTarget(HealthComponent atkTarget) { m_AttackTargets.Add(atkTarget); }
    public void RemoveAttackTarget(HealthComponent atkTarget) { m_AttackTargets.Remove(atkTarget); }
    public void SetFinalAttackSpeed(float atkSpeed) { m_FinalAttackSpeed = atkSpeed; }
    public void SetPoisonous(bool isPoisonous) { m_IsPoisonous = isPoisonous; }
    public void SetComboCounter(int comboCounter) { m_ComboCounter = comboCounter; }
    public void SetHitCrit(bool isHitCrit) { m_IsCrit = isHitCrit; }
    public void SetPoisonHitsRemaining(int poisonHitsRemaining) { m_PoisonHitsRemaining = poisonHitsRemaining; }
    public void SetBaseDamage(float baseDamage) { m_WeaponInfo.BaseDamage = baseDamage; }


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        // Assigns random rarity to Weapon. Assign Loot Rarity must happen before WeaponStatGenerator
        AssignLootRarity();

        if (m_WeaponInfo == null)
        {
            m_WeaponInfo = WeaponStatGenerator.GenerateWeaponInfo(WeaponLootTier, transform);
        }

        // If Owner exists
        if (m_WeaponInfo.Owner != null)
        {
            // Set Owner's Main Weapon to this
            m_WeaponInfo.Owner.m_MainWeapon = this;

            // Set Weapon's Element to match Owner's Element
            WeaponElement = m_WeaponInfo.Owner.m_Element;

            // Set Weapon's Material to match Owner's Element
            SetMaterial(m_WeaponInfo.Owner);
        }

        // Set Base Variables
        m_PickupType = PickupType.Weapon;
        m_ComboTimer = m_ComboCooldown;
        m_CanAttack = true;
        m_IsBeingHeld = false;
        m_ClimbPos = new Vector3(0.0f, 0.68f, -2.75f);
        m_IsPoisonous = false;
        m_AttackTargets = new List<HealthComponent>();

        // If Weapon Socket Object exists
        if (m_WeaponInfo.Owner != null)
        {
            if (m_WeaponInfo.Owner.WeaponSocket != null)
            {
                // Set Ground Position to Weapon Socket's position
                m_GroundPos = m_WeaponInfo.Owner.WeaponSocket.transform.position;
            }
        }
    }

    public override void Activate()
    {
        // If weapon interact cooldown is not on cooldown
        if (m_PlayerController)
        {
            // If no Weapon isn't already being held
            if (!m_IsParented)
            {
                // If Inventory and Main Weapon are valid
                if (m_Player.m_Inventory != null)
                {
                    base.Activate();

                    // Add to Inventory
                    m_Player.m_Inventory.AddToInventory(this);

                }
            }
        }

    }

    public override void Deactivate()
    {
        // If Player Controller exists
        if (m_PlayerController)
        {
            // If this Weapon is parented and Player possesses main weapon
            if (m_IsParented && m_Player.m_MainWeapon != null)
            {
                // Unequip this weapon
                UnEquip();
            }
        }
    }

    public void AssignLootRarity()
    {
        // If Weapon Loot Tier isn't Godlike (Dropped by Elemental Gods)
        if (WeaponLootTier == LootTier.None)
        {
            // Assign this weapon's loot tier
            WeaponLootTier = LootTierCalculation.AssignLootRarity(true);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        HandleParticleColors();

        // Handles Combo Time Window
        ComboTimerCheck();
    }

    private void HandleParticleColors()
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = m_InteractionParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        if (m_Player.InteractionHandler.TargetObject == this)
        {
            // If the weapon is Godlike, set the particle colour to the gradient of r/g/b
            if (WeaponLootTier == LootTier.Godlike)
            {
                colorOverLifetime.color = HUDManager.Instance.GodlikeColorGradient;
            }
            // If any other rarity, set the particle colour to the one that corresponds to its rarity
            else
            {
                colorOverLifetime.color = m_ParticleLootColours[(int)WeaponLootTier];
            }
        }
    }

    private void ComboTimerCheck()
    {
        // If Combo Counter is between 1 and 4 (inclusive)
        if (m_ComboCounter >= MIN_COMBO_COUNTER && m_ComboCounter <= MAX_COMBO_COUNTER)
        {
            // Decrement Combo Timer
            m_ComboTimer -= Time.deltaTime;

            // If Combo Timer has reached zero
            if (m_ComboTimer <= float.Epsilon)
            {
                // Reset Combo
                ResetCombo();
            }
        }
        else
        {
            // Sets Combo counter to 0
            m_ComboCounter = 0;
        }
    }

    public void PlaceWeaponInSheath(GameObject parent, bool isTrue)
    {
        // If Weapon parented, parent and place weapon in appropriate location
        if (m_IsParented) // m_PlayerController != null && !m_PlayerController.m_IsClimbing
        {
            transform.parent = parent.transform;
            transform.position = gameObject.transform.parent.position;

            if (isTrue)
            {
                transform.localRotation = Quaternion.Euler(-90.638f, 28.717f, 150.335f);
                m_CanAttack = false;
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0.0f, -90.0f, -40.0f);
                m_CanAttack = true;
            }
        }
    }

    public void SetMaterial(CharacterBase owner)
    {
        if (owner != null)
        {
            // If object is parented to Player and owner's element is NOT equal to weapon's element
            if (owner.m_MainWeapon.m_IsParented)
            {
                // Grab object's Mesh's MeshRenderer Component
                MeshRenderer meshRenderer = owner.m_MainWeapon.transform.GetComponent<MeshRenderer>();

                // If the object's Mesh's MeshRenderer Component exists
                if (meshRenderer != null)
                {
                    // Assign this MeshRenderer to correct material based on Element
                    switch (WeaponElement)
                    {
                        case Element.Fire:
                            meshRenderer.material = FireMaterial;
                            break;

                        case Element.Water:
                            meshRenderer.material = WaterMaterial;
                            break;

                        case Element.Plant:
                            meshRenderer.material = PlantMaterial;
                            break;

                        case Element.None:
                            meshRenderer.material = BaseMaterial;
                            break;

                        default:
                            Debug.LogError("Unexpected weapon element passed in");
                            break;
                    }
                }
            }
        }
        else
        {
            // Grab object's Mesh's MeshRenderer Component
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

            // If the object's Mesh's MeshRenderer Component exists
            if (meshRenderer != null)
            {
                meshRenderer.material = BaseMaterial;
            }
        }
    }

    public void Equip()
    {
        if (m_WeaponInfo.Owner != null) // && m_WeaponInfo.Owner.tag == "Enemy"
        {
            if (m_WeaponInfo.IsOwnerPlayer)
            {
                if (m_Player.m_PrimaryPet != null)
                {
                    // Activates primary pet ability if FirePet is Main Pet
                    if (m_Player.m_PrimaryPet.PetElementType == Element.Fire)
                    {
                        m_Player.m_PrimaryPet.PassiveAbility.ActivateAbility();
                    }

                    WeaponElement = m_Player.m_PrimaryPet.PetElementType;
                }
                else
                {
                    WeaponElement = m_Player.m_Element;
                }
            }
            else if (m_WeaponInfo.Owner.tag == "Enemy")
            {
                WeaponElement = m_WeaponInfo.Owner.m_Element;
            }

            if (m_WeaponInfo.Owner.WeaponSocket != null)
            {
                // Parents the object an empty GameObject serving as a socket.
                transform.parent = m_WeaponInfo.Owner.WeaponSocket.transform;
                m_IsParented = true;

                // Applies Parent's Transform Position to the object
                transform.position = gameObject.transform.parent.position;

                // Modify object's Local Rotation
                transform.localRotation = Quaternion.Euler(0.0f, -90.0f, -40.0f); //Quaternion.Euler(-33.0f, -6.98f, 0.0f) || gameObject.transform.parent.localRotation;

                // Gets object's Mesh's BoxCollider
                BoxCollider boxCollider = transform.GetComponent<BoxCollider>(); //Mesh Box Collider

                // If object's Mesh's BoxCollider exists
                if (boxCollider != null)
                {
                    // Disables Mesh's BoxCollider
                    boxCollider.enabled = false;
                }

                // Disables object's BoxCollider and enables Rigidbody's Physics
                GetComponent<Rigidbody>().isKinematic = true;

                // Tells the Owner that they are holding this object (as Main Weapon)
                m_WeaponInfo.Owner.m_MainWeapon = this;

                // Set AttackTrigger weapon to this
                m_WeaponInfo.Owner.AttackTriggerBox.GetComponent<AttackTrigger>().m_Weapon = this;

                // Set weapon to being held
                m_IsBeingHeld = true;

                // Sets the correct object material based on its element
                SetMaterial(m_WeaponInfo.Owner);
            }
        }
    }

    public void UnEquip()
    {
        // If Owner's Player component exists
        if (m_WeaponInfo.Owner != null && m_WeaponInfo.IsOwnerPlayer)
        {
            // If Player possesses Primary pet
            if (m_Player.m_PrimaryPet != null)
            {
                // Deactivate primary pet ability
                m_Player.m_PrimaryPet.PassiveAbility.DeactivateAbility();
            }
        }

        // Sets Weapon element to none
        WeaponElement = Element.None;

        // Sets the correct object material based on its element
        SetMaterial(m_WeaponInfo.Owner);

        // Unparents the object from the player and lets them know they aren't holding an object anymore.
        transform.parent = null;
        m_IsParented = false;

        // Enables object's BoxCollider and disables Rigidbody's Physics
        GetComponent<Rigidbody>().isKinematic = false;

        // Gets object's Mesh's BoxCollider
        BoxCollider boxCollider = transform.GetComponent<BoxCollider>(); //Mesh Box Collider

        // If object's Mesh's BoxCollider exists
        if (boxCollider != null)
        {
            // Disables Mesh's BoxCollider
            boxCollider.enabled = true;
        }

        m_IsBeingHeld = false;
        m_IsPoisonous = false;

        // Tells the owner that they are no longer holding an object
        m_WeaponInfo.Owner.m_MainWeapon = null;

        // Set AttackTrigger weapon to null
        m_WeaponInfo.Owner.AttackTriggerBox.GetComponent<AttackTrigger>().m_Weapon = this;
    }

    public void Attack()
    {
        if (m_WeaponInfo.Owner != null && m_WeaponInfo.IsOwnerPlayer)
        {
            m_Player.m_CanSheathWeapon = false;
        }

        StartCoroutine(HandleAttack());
    }

    private void ComboCounterCheck()
    {
        // If Combo Counter value is between 0 and 4 (inclusive)
        if (m_ComboCounter >= 0 && m_ComboCounter < MAX_COMBO_COUNTER)
        {
            // Increment Combo Counter
            m_ComboCounter++;

            // Resets Combo timer
            m_ComboTimer = 3.0f;
        }
        else
        {
            // Sets Combo timer to 0
            m_ComboCounter = 0;
        }

        if (m_ComboCounter == MAX_COMBO_COUNTER)
        {
            //Set Combo Multiplier to Heavy Attack Multiplier if Owner is Player
            if (m_WeaponInfo.IsOwnerPlayer)
            {
                m_ComboMultiplier = 1.5f;
            }
            else
            {
                m_ComboMultiplier = 1.0f;
            }

        }
        else
        {
            //Set Combo Multiplier to Light Attack Multiplier
            m_ComboMultiplier = 1.0f;
        }
    }

    public void ElementMultiplierCheck(GameObject atkTarget)
    {
        if (atkTarget.tag == "Player" || atkTarget.tag == "Enemy")
        {
            // Set Element Multiplier according to Element type matchup
            ElementMultiplier = ElementTypeMatchup.ElementalCheckMultiplier(WeaponElement, atkTarget.GetComponent<CharacterBase>());
        }
        else if (atkTarget.tag == "Breakable")
        {
            ElementMultiplier = 1.0f;
        }
    }

    private void CalculateAndInflictDamage(HealthComponent atkTarget)
    {
        // Attack Dmg = [(Base Atk Dmg * Combo Multiplier) * Crit Dmg Multiplier] * Elemental Multiplier
        float newDamage;

        // Checks if Weapon will inflict Crit damage
        m_IsCrit = CritDamageCheck();

        // Calculate Damage (Add Crit multiplier if true)
        newDamage = m_WeaponInfo.BaseDamage * m_ComboMultiplier * ElementMultiplier * (m_IsCrit ? CritDamageMultiplier : 1.0f);

        //Handle Enemy Health (with crit check)
        atkTarget.ModifyHealth(-Mathf.Max((int)newDamage, 1), m_IsCrit);
    }

    IEnumerator HandleAttackCooldown()
    {
        // Sets weapon so it can no longer attack while on cooldown
        m_CanAttack = false;

        // Animation cooldown timer
        yield return new WaitForSeconds(0.5f);

        // Attack Cooldown timer
        yield return new WaitForSeconds(m_FinalAttackSpeed);

        // Sets weapon so it can attack again
        m_CanAttack = true;
        m_AttackTargets.Clear();
        m_WeaponInfo.Owner.AttackTriggerBox.SetActive(false);
    }

    IEnumerator HandleAttack()
    {
        m_CanAttack = false;

        PlaySFX(WeaponSFX.Swing);

        if (m_WeaponInfo.IsOwnerPlayer)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
        }

        if (m_WeaponInfo.Owner.AttackTriggerBox.activeSelf && m_AttackTargets.Count > 0 && m_HasHit)
        {
            // Checks Combo Counter value
            ComboCounterCheck();

            foreach (HealthComponent atkTarget in m_AttackTargets)
            {
                if (atkTarget != null && atkTarget.gameObject != m_WeaponInfo.Owner)
                {
                    // Determines Elemental Multiplier of object on attack target
                    ElementMultiplierCheck(atkTarget.gameObject);

                    // Calculates final damage and inflicts it onto attack target
                    CalculateAndInflictDamage(atkTarget);

                    // If weapon is poisonous
                    if (m_IsPoisonous)
                    {
                        // Apply poison
                        ApplyPoison(atkTarget.gameObject);
                    }
                }
            }
        }
        else
        {
            // Set attack target to null
            m_AttackTargets.Clear();
        }

        // Attack Cooldown timer
        if (m_WeaponInfo.IsOwnerPlayer)
        {
            yield return new WaitForSeconds(m_FinalAttackSpeed);
        }

        // Sets weapon so it can attack again
        m_AttackTargets.Clear();
        m_WeaponInfo.Owner.AttackTriggerBox.SetActive(false);

        m_CanAttack = true;

        if (m_WeaponInfo.Owner != null && m_WeaponInfo.IsOwnerPlayer)
        {
            m_Player.m_CanSheathWeapon = true;
        }
    }

    public void ApplyPoison(GameObject atkTarget)
    {
        // When the weapon has poison hits, when hitting an enemy it will apply a "Damage Over Time" effect to the enemy that will deal damage
        if (m_PoisonHitsRemaining > 0 && atkTarget != null)
        {
            atkTarget.AddComponent<DotEffect>();
            atkTarget.GetComponent<DotEffect>().SetEffectAmountAndDuration(PoisonDamage, PoisonDuration);
            atkTarget.GetComponent<DotEffect>().InitEffect();
            m_PoisonHitsRemaining--;
        }
        if (m_PoisonHitsRemaining <= 0)
        {
            m_IsPoisonous = false;
            m_PoisonHitsRemaining = 3;
        }
    }

    public void ResetCombo()
    {
        // Sets Combo timer and Combo counter to their default values
        m_ComboTimer = 3.0f;
        m_ComboCounter = 0;
    }


    public bool CritDamageCheck()
    {
        // Create Crit Rate ID between 1 and 100.
        int critRateID = Random.Range(1, 101);

        // Create local luck modifier variable
        int luckModifier;

        // If Owner is Player
        if (m_WeaponInfo.IsOwnerPlayer)
        {
            // Set Luck Modifier to Player's
            luckModifier = m_WeaponInfo.Owner.GetComponent<Player>().m_LuckModifier;
        }
        else
        {
            // Set Luck Modifier to 0
            luckModifier = 0;
        }

        // If Crit Rate is positive
        if (CritRate + luckModifier >= 0)
        {
            // If Crit Rate ID is bigger/equal to 1 and within Crit Rate range, return true and set m_IsCrit to true. Otherwise, return false and set m_IsCrit to false.
            if (critRateID < 1)
            {
                m_IsCrit = false;
                return false;
            }
            else if (critRateID >= 1 && critRateID <= (CritRate + luckModifier))
            {
                m_IsCrit = true;
                Debug.Log("CRIT");
                return true;
            }
            else
            {
                m_IsCrit = false;
                return false;
            }
        }
        m_IsCrit = false;
        return false;
    }

    public void PlaySFX(WeaponSFX sfx)
    {
        switch(sfx)
        {
            case WeaponSFX.Swing:
                m_AudioSource.PlayOneShot(m_WeaponSwingSFX);
                break;
            case WeaponSFX.Unsheathe:
                m_AudioSource.PlayOneShot(m_UnsheathSFX);
                break;
            case WeaponSFX.Sheathe:
                m_AudioSource.PlayOneShot(m_SheathSFX);
                break;
            default:
                Debug.LogError("Unsupported Weapon Sound Effect (Weapon.cs)");
                break;
        }
    }
}