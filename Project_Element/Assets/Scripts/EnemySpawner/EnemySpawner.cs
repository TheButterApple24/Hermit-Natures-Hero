/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              EnemySpawner
Description:        Handles spawning enemies at set times and limits
Date Created:       19/01/22
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/22
        - [Gerard] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_EnemyType;

    public GameObject[] PatrolPath1;
    public GameObject[] PatrolPath2;
    public GameObject[] PatrolPath3;

    public Transform m_PlayerTarget;

    Vector3 m_SpawnPos;
    Quaternion m_SpawnRot;

    public float m_SpawnFrequency;
    public float m_SpawnTimer = 10;
    float m_SpawnCounter = 0.0f;
    public int m_EnemyCount = 0;
    public int m_MaxEnemyCount;
    float m_DistanceToPlayer;
    public float m_SpawnRange;

    ObjectPool objectPool;

    private void OnDisable()
    {
        objectPool = PoolManager.GetPool(m_EnemyType.name, typeof(GameObject));
        if (objectPool != null)
            objectPool.ClearObjectPool();
    }

    private void Awake()
    {
        this.gameObject.SetActive(true);

        StartCoroutine(SpawnEnemy());
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);

        m_PlayerTarget = PlayerManager.Instance.Player.transform;
    }

    void Update()
    {
        m_DistanceToPlayer = Vector3.Distance(transform.position, m_PlayerTarget.position);

        if (m_EnemyCount < m_MaxEnemyCount)
        {
            if (m_DistanceToPlayer > m_SpawnRange)
            {
                m_SpawnCounter += Time.deltaTime;
                if (m_SpawnCounter > m_SpawnTimer)
                {
                    m_SpawnCounter = 0.0f;

                    StartCoroutine(SpawnEnemy());
                }
            }
        }
    }

    public void SetEnemySpawner(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().m_Spawner = this;
    }

    IEnumerator SpawnEnemy()
    {
        while (m_EnemyCount < m_MaxEnemyCount)
        {
            if (m_DistanceToPlayer > m_SpawnRange)
            {
                m_SpawnPos = transform.position;
                m_SpawnRot = transform.rotation;


                objectPool = PoolManager.GetPool(m_EnemyType.name, typeof(GameObject));

                if (m_EnemyType != null)
                {
                    GameObject Enemy = PoolManager.Get(m_EnemyType, m_SpawnPos, m_SpawnRot);


                    if (m_EnemyCount == 0)
                    {
                        Enemy.GetComponent<EnemyBehaviour>().SetPatrolWaypoints(PatrolPath1);
                    }
                    if (m_EnemyCount == 1)
                    {
                        Enemy.GetComponent<EnemyBehaviour>().SetPatrolWaypoints(PatrolPath2);
                    }
                    if (m_EnemyCount == 2)
                    {
                        Enemy.GetComponent<EnemyBehaviour>().SetPatrolWaypoints(PatrolPath3);
                    }
                    else if (m_EnemyCount > 2)
                    {
                        Enemy.GetComponent<EnemyBehaviour>().SetPatrolWaypoints(PatrolPath1);
                    }

                    SetEnemySpawner(Enemy);
                }


                m_EnemyCount += 1;

            }
            yield return new WaitForSeconds(m_SpawnFrequency);
        }
    }


    private void OnDrawGizmosSelected()
    {
        // draw a red sphere to show the melee radius of this enemy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SpawnRange);
    }
}