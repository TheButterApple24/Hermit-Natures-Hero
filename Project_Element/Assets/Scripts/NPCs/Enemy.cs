/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Enemy
Description:        Base class that encompasses all shared enemy behaviour
Date Created:       06/10/2021
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    06/10/2021
        - [Jeffrey] Created base class
    06/11/2021
        - [Zoe] Made the Enemy inherit from CharacterBase, moved death behaviour to the overriden OnDeath
    07/11/2021
        - [Zoe] Start() is now an override of CharacterBase::Start() and calls base.Start()
    08/11/2021
        - [Max] Added Attack function
    19/01/2022
        - [Jeffrey] Implemented Loot System
    02/02/2022
        - [Gerard] Added health regeneration
    03/02/2022
        - [Max] Added OnTakeDamage virtual function + EnemyHealthBar Prefab / Logic
    04/02/2022
        - [Max] Fixed Health Regen Bug + Added null check for Weapon Prefab
    06/02/2022
        - [Max] Removed ModifyHealth call from OnTakeDamage
    08/02/2022
        - [Max] Added Comments
    16/02/2022
        - [Jeffrey] Added enemy to lore system
    09/03/2022
        - [Max] Weapon class refactor
    29/03/2022
        - [Max] Enemies' weapons can now be initialized when present in the scene

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;
using LootTiers;

public class Enemy : CharacterBase
{
    public GameObject m_LookAtPoint;
    public LootSystem m_EnemyLoot;
    public GameObject m_WeaponPrefab;
    public LootTier m_LootTier = LootTier.None;
    public GameObject m_HealthBarPrefab;
    public Transform DmgNumbersLocation;
    public List<GameObject> WeaponList;
    EnemyBehaviour m_EnemyBehaviour;
    public Animator m_Animator { get; private set; }

    [HideInInspector] public EnemySpawner m_Spawner;

    GameObject m_HealthBar;

    float m_RegenCounter;
    public float m_RegenAmount = 1.5f;
    public int m_LoreDataID = 0;
    Vector3 m_EnemyHeight = new Vector3(0.0f, 1.885f, 0.0f);

    // Start is called before the first frame update
    public void Awake()
    {
        m_EnemyLoot.GenerateRandomLoot();

        if (DmgNumbersLocation == null)
        {
            DmgNumbersLocation = gameObject.transform;
        }

        if (WeaponSocket == null)
        {
            if (transform.Find("WeaponSocket") != null)
            {
                WeaponSocket = transform.Find("WeaponSocket").gameObject;
            }
        }

        CharacterBase owner = transform.GetComponent<CharacterBase>();

        if (owner.m_MainWeapon == null)
        {
            if (m_WeaponPrefab != null)
            {
                m_WeaponPrefab.GetComponent<Weapon>().WeaponLootTier = m_LootTier;
                m_EnemyLoot.m_Loot[0] = Instantiate<GameObject>(m_WeaponPrefab.gameObject, WeaponSocket.transform);
            }
            else
            {
                int num = Random.Range(0, WeaponList.Count);

                m_EnemyLoot.m_Loot[0] = Instantiate<GameObject>(WeaponList[num].gameObject, WeaponSocket.transform);
                m_LootTier = m_EnemyLoot.m_Loot[0].GetComponent<Weapon>().WeaponLootTier;
            }
        }
        else
        {
            m_EnemyLoot.m_Loot[0] = Instantiate<GameObject>(owner.m_MainWeapon.gameObject, WeaponSocket.transform);
            m_LootTier = m_EnemyLoot.m_Loot[0].GetComponent<Weapon>().WeaponLootTier;
        }

        m_EnemyLoot.m_Loot[0].GetComponent<Weapon>().Equip();
        m_WeaponPrefab = m_EnemyLoot.m_Loot[0];

        if (m_Spawner != null)
        {
            m_Spawner.SetEnemySpawner(this.gameObject);

        }

        m_EnemyBehaviour = GetComponent<EnemyBehaviour>();

        // If Health Bar Prefab exists
        if (m_HealthBarPrefab != null)
        {
            // Instantiate Health Bar Prefab
            m_HealthBar = Instantiate<GameObject>(m_HealthBarPrefab, this.transform);

            // Set Health Bar position to transform's offset position
            m_HealthBar.transform.position = transform.position + m_EnemyHeight;

            Canvas canvas = m_HealthBar.gameObject.GetComponent<Canvas>();

            // If Health Bar Canvas exists
            if (canvas != null)
            {
                // Set Canvas' worldCamera to Global Camera
                canvas.worldCamera = Camera.main;
            }

            Billboard billboard = m_HealthBar.gameObject.GetComponent<Billboard>();

            // If Health Bar Billboard exists
            if (billboard != null)
            {
                // Set Billboard's mainCamera to Global Camera
                billboard.m_MainCamera = Camera.main;
            }

            // Call Init on Health Bar and Set to inactive
            m_HealthBar.GetComponent<EnemyHealthBar>().Init(m_HealthComp.m_CurrentHealth, 0.0f, m_HealthComp.MaxHealth, m_Element);
            m_HealthBar.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If Health Bar Prefab and Health Bar exist
        if (m_HealthBarPrefab != null && m_HealthBar != null)
        {
            // If Enemy is in Combat
            if (m_EnemyBehaviour.IsInCombat())
            {
                // If Health Bar is NOT active
                if (!m_HealthBar.activeSelf)
                {
                    // Set Health Bar to active
                    m_HealthBar.GetComponent<EnemyHealthBar>().m_Slider.value = m_HealthComp.m_CurrentHealth;
                    m_HealthBar.SetActive(true);
                }
            }
            else
            {
                // If Health Bar is active
                if (m_HealthBar.activeSelf)
                {
                    // Set Health Bar to inactive;
                    m_HealthBar.SetActive(false);
                }
            }
        }

        // If Enemy is not in combat
        if (!m_EnemyBehaviour.IsInCombat())
        {
            // If Current Enemy Health is not at Max Health
            if (m_HealthComp.m_CurrentHealth != m_HealthComp.MaxHealth)
            {
                m_RegenCounter += Time.deltaTime;
                if (m_RegenCounter >= m_HealthComp.m_RegenRate)
                {
                    m_HealthComp.ModifyHealth(m_RegenAmount, false);
                    m_RegenCounter = 0.0f;
                }
            }
        }
    }

    public override void Attack()
    {
        if (m_MainWeapon != null)
        {
            if (m_MainWeapon.enabled && m_MainWeapon.CanAttack() && m_MainWeapon != null)
            {
                base.Attack();

                m_MainWeapon.Attack();
            }
        }
    }

    public override void OnTakeDamage(float damage, bool isCrit)
    {
        // if Health Component and Health Bar exist
        if (m_HealthComp != null && m_HealthBar != null)
        {
            // If damage is less than 0, adjust health slider towards 0. Otherwise, adjust health slider towards max health
            if (damage < 0.0f)
            {
                m_HealthBar.GetComponent<EnemyHealthBar>().AdjustSlider(Mathf.Abs(damage), m_HealthComp.m_CurrentHealth, false);
                m_EnemyBehaviour.m_Animator.Play("Flinch");
            }
            else
            {
                m_HealthBar.GetComponent<EnemyHealthBar>().AdjustSlider(Mathf.Abs(damage), m_HealthComp.m_CurrentHealth, true);
            }

        }

    }



    public override void OnDeath()
    {
        Player player = PlayerManager.Instance.Player;
        // If the player has a quest accepted
        if (player.m_AcceptedQuests.Count != 0)
        {
            // If the player has a kill task
            if (player.m_AcceptedQuests[player.m_ActiveQuest].m_Tasks[player.m_AcceptedQuests[player.m_ActiveQuest].m_TaskIndex].GetType() == typeof(QuestTask_KillSingleType))
            {
                QuestTask_KillSingleType task = (QuestTask_KillSingleType)player.m_AcceptedQuests[player.m_ActiveQuest].m_Tasks[player.m_AcceptedQuests[player.m_ActiveQuest].m_TaskIndex];

                // Update the kill number and text on the UI
                task.m_KillNumber++;
                task.UpdateUI();
            }
        }

        player.CheckForLevelUp(Random.Range(5, 11));

        player.SeenEnemies.Remove(m_EnemyBehaviour);

        if (m_HealthBar != null)
        {

            m_HealthBar.GetComponent<EnemyHealthBar>().Init(m_HealthComp.m_CurrentHealth, 0.0f, m_HealthComp.MaxHealth, m_Element);
            m_HealthBar.SetActive(false);
        }

        GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        manager.m_LoreUnlockedIDs[m_LoreDataID] = true;

        Weapon weapon = m_WeaponPrefab.GetComponent<Weapon>();

        if (weapon != null)
        {
            weapon.SetMaterial(null);
        }

        if (m_Spawner != null)
        {
            UpdateSpawnerCount();
        }

        if(gameObject.GetComponent<Effect>())
        {
            gameObject.GetComponent<Effect>().KillEffect();
        }

        m_EnemyLoot.SpawnLoot();
        gameObject.SetActive(false);

        m_HealthComp.Reset();
        m_HealthComp.m_CanTakeDamage = true;
        m_HealthBar.GetComponent<EnemyHealthBar>().Reset();
    }

    void UpdateSpawnerCount()
    {
        if (m_Spawner != null)
        {
            m_Spawner.m_EnemyCount -= 1;
        }
    }
}
