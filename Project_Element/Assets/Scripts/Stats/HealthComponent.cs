/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              HealthComponent
Description:        Component that handles all health, invulnerability, damage-taking, and regeneration
Date Created:       02/11/2021
Author:             Zoe Purcell
Verified by:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/11/2021
        - [Zoe] Started HealthComponent
    03/11/2021
        - [Zoe] Changed Invulnerability duration from a temp hard-coded value to a variable (m_InvulDuration)
    06/11/2021
        - [Zoe] HealthComponent now affectd all CharacterBase children at the same time, rather than in separate if statements
        - [Zoe] Commented code
    07/11/2021
        - [Zoe] Added IsDead bool function
    08/11/2021
        - [Zoe] Changed TakeDamage to ModifyHealth to accomodate future healing functionality
    17/11/2021
        - [Zoe] HealthComponent now properly utilizes Mathf.Clamp
    18/11/2021
        - [Zoe] Adjusted Damage Resistance Formula
    20/01/2022
        - [Max] Added UI Numbers functionality + Crit check to ModifyHealth + Crit Color Text
    24/01/2022
        - [Max] Added Upgraded UI Numbers with animations
    25/01/2022
        - [Max] Fixed UI Numbers + Added Unique Player Health Regen UI Numbers and Damage Animation
    27/01/2022
        - [Max] Added Armor to Damage 
    31/01/2022
        - [Max] Added null check to m_HealthAnimator
    03/02/2022
        - [Max] Refactored UI Numbers
    06/02/2022
        - [Max] Added OnTakeDamage call
    08/02/2022
        - [Max] Added Comments
    19/02/2022
        - [Max] Added Audio
    19/02/2022
        - [Max] Refactored Damage Indicators

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class HealthComponent : MonoBehaviour
{
    [SkillTreeModifiable] public float MaxHealth = 100.0f;
    [SkillTreeModifiable] public float MaxDefense = 0.0f;
    public float m_InvulDuration = 0.5f;
    public float m_DamageResistance = 0.0f;
    public bool IsInvulnerable = false;

    [Header("Damage Numbers + HP Bar")]
    public GameObject m_DamageHealthPrefab;
    public Image m_HealthMeter;

    [Header("Audio")]
    [SerializeField] AudioSource m_AudioSource;
    [SerializeField] AudioClip m_HitSFX;
    [SerializeField] AudioClip m_CritSFX;

    [Header("Death")]
    [SerializeField] Animation m_DeathAnimation;
    [SerializeField] AudioClip m_DeathSFX;

    public float m_CurrentHealth;
    [HideInInspector] public float m_RegenRate = 5.0f;
    [HideInInspector] public bool m_CanTakeDamage = true;
    [HideInInspector] public CharacterBase m_Character;

    GameObject m_CombatUI;
    GameObject m_DamageHealthClone;
    Transform m_EnemyDmgLocation;


    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = MaxHealth;

        // Assigns the owning character
        m_Character = gameObject.GetComponent<CharacterBase>();

        // Get CombatUI object
        m_CombatUI = GameObject.Find("CombatUI");

        if (m_HealthMeter != null)
        {
            m_HealthMeter.fillAmount = m_CurrentHealth / MaxHealth;
        }
    }

    private void Update()
    {
        if (m_HealthMeter != null)
        {
            m_HealthMeter.fillAmount = m_CurrentHealth / MaxHealth;
        }
    }

    public void ModifyHealth(float value, bool isCrit)
    {
        if (value == 0)
            return;

        if (transform.tag == "Player" && IsInvulnerable)
            return;

        // If Damage Health Prefab exists
        if (m_DamageHealthPrefab != null)
        {
            // Instantiate Indicator Damage Health Prefab 
            m_DamageHealthClone = Instantiate<GameObject>(m_DamageHealthPrefab, m_CombatUI.transform);

            // Grab Damage Health Indicator
            DamageHealthIndicator damageHealthIndicator = m_DamageHealthClone.GetComponent<DamageHealthIndicator>();

            if (transform.tag == "Player")
            {
                damageHealthIndicator.PlayerInit(value);
            }
            else if (transform.tag == "Enemy")
            {
                Enemy enemy = GetComponent<Enemy>();
                m_EnemyDmgLocation = enemy.DmgNumbersLocation;

                damageHealthIndicator.GetComponentInChildren<TrackingUI>().m_TargetObject = enemy.gameObject;

                if (m_EnemyDmgLocation != null)
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, m_EnemyDmgLocation, enemy);
                }
                else
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, enemy.transform, enemy);
                }
            }
            else if (transform.tag == "Enemy")
            {
                Enemy enemy = GetComponent<Enemy>();
                m_EnemyDmgLocation = enemy.DmgNumbersLocation;

                damageHealthIndicator.GetComponentInChildren<TrackingUI>().m_TargetObject = enemy.gameObject;

                if (m_EnemyDmgLocation != null)
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, m_EnemyDmgLocation, enemy);
                }
                else
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, enemy.transform, enemy);
                }
            }
            else if (transform.tag == "Breakable")
            {
                BreakableObject obj = GetComponent<BreakableObject>();

                if (obj.DmgNumbersLocation != null)
                    m_EnemyDmgLocation = obj.DmgNumbersLocation;

                damageHealthIndicator.GetComponentInChildren<TrackingUI>().m_TargetObject = obj.gameObject;

                if (m_EnemyDmgLocation != null)
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, m_EnemyDmgLocation);
                }
                else
                {
                    damageHealthIndicator.EnemyInit(value, isCrit, obj.transform);
                }
            }
        }
        //If the value is negative (Damage Applied)
        if (value < 0)
        {
            if (m_Character != null)
            {
                m_Character.m_BloodEffects.Stop();
                m_Character.m_BloodEffects.Play();
            }

            //If the character is currently able to take damage (Not invulnerable)
            if (m_CanTakeDamage)
            {
                if (m_AudioSource != null)
                {
                    if (m_HitSFX != null)
                        m_AudioSource.PlayOneShot(m_HitSFX);

                    if (isCrit)
                    {
                        m_AudioSource.PlayOneShot(m_CritSFX);
                    }
                }

                // Apply Damage (With Armor)
                m_CurrentHealth += value * (1.0f - m_DamageResistance + MaxDefense);

                if (m_HealthMeter != null)
                {
                    m_HealthMeter.fillAmount = m_CurrentHealth / MaxHealth;
                }

                if (m_Character != null)
                {
                    // Call OnTakeDamage on affected character
                    m_Character.OnTakeDamage(value, isCrit);
                }

                //Clamp CurrentHealth so it isn't lower than 0
                m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, MaxHealth);

                if (gameObject.activeSelf)
                {
                    //Trigger Invulnerability Frames
                    StartCoroutine("TriggerInvulnerability");
                }

                //If Health is 0, the character is dead. Also Clamp so it's not below 0
                if (IsDead())
                {
                    StartCoroutine(HandleDeath());
                }
            }
        }
        //If the value is positive (Healing Applied)
        else if (value > 0)
        {
            //Apply Healing
            m_CurrentHealth += value;

            if (m_HealthMeter != null)
            {
                m_HealthMeter.fillAmount = m_CurrentHealth / MaxHealth;
            }
            
            //Clamp CurrentHealth so it isn't higher than the Maximum Health
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, MaxHealth);
        }
    }

    IEnumerator HandleDeath()
    {
        bool hasDeathSFX = false;
        bool hasDeathAnimation = false;

        //Grab the character that owns this
        if (m_DeathSFX != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_DeathSFX);
            hasDeathSFX = true;
        }

        // Play Death Animations
        if (m_DeathAnimation != null)
        {
            if (transform.tag == "Player")
            {
                ThirdPersonController thirdPersonController = transform.GetComponentInParent<ThirdPersonController>();
                thirdPersonController.m_Animator.Play("HermitDeath");
            }
            else if (transform.tag == "Enemy")
            {
                EnemyBehaviour enemyBehavior = transform.GetComponent<EnemyBehaviour>();
                enemyBehavior.m_Animator.Play("EnemyDeath");
            }
            hasDeathAnimation = true;
        }

        if (gameObject.tag == "Breakable")
        {
            BreakableObject obj = gameObject.GetComponent<BreakableObject>();
            obj.Break();
        }

        // Death Animation/SFX delay
        if (hasDeathAnimation)
        {
            if (hasDeathSFX)
            {
                if (m_DeathAnimation.clip.length > m_DeathSFX.length)
                {
                    yield return new WaitForSeconds(m_DeathAnimation.clip.length - m_DeathSFX.length);
                }
                else
                {
                    yield return new WaitForSeconds(m_DeathSFX.length - m_DeathAnimation.clip.length);
                }
            }

            yield return new WaitForSeconds(m_DeathAnimation.clip.length);
        }
        else if (hasDeathSFX)
        {
            yield return new WaitForSeconds(m_DeathSFX.length);
        }
        else
        {
            yield return new WaitForSeconds(m_HitSFX.length);
        }

        //Kill the character
        if (m_Character != null)
        {
            m_Character.OnDeath();
        }
    }

    public bool IsDead()
    {
        return m_CurrentHealth <= 0;
    }

    public void Reset()
    {
        m_CurrentHealth = MaxHealth;

        if (m_HealthMeter != null)
            m_HealthMeter.fillAmount = m_CurrentHealth / MaxHealth;

    }

    IEnumerator TriggerInvulnerability()
    {
        //Player is unable to take damage when invulnerable
        m_CanTakeDamage = false;

        //Wait a certain amount of time...
        yield return new WaitForSeconds(m_InvulDuration);

        //...before they can take damage again
        m_CanTakeDamage = true;
    }
}
