/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              GameManager
Description:        Stores multiple GameObjects that are needed by multiple scripts in the project
Date Created:       19/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Jeffrey] Created base class
    01/02/2022
        - [Zoe] Added m_PotionTimerIcons, so HUD can be altered when still inactive

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance = null;
    public static GameManager Instance { get { return m_Instance; } }

    public GameObject[] m_PotentialChestLoot;
    public GameObject[] m_PotentialEnemyLoot;
    public bool[] m_LoreUnlockedIDs;

    [SerializeField]
    private GameObject m_DialoguePromptPrefab;
    public GameObject DialoguePromptPrefab { get { return m_DialoguePromptPrefab; } }

    [SerializeField]
    private QuestListManager m_QuestListManager;
    public QuestListManager QuestListManager { get { return m_QuestListManager; } }

    [SerializeField]
    private Quest[] m_QuestList;
    public Quest[] QuestList { get { return m_QuestList; } }

    [SerializeField]
    private GameObject[] m_SaveObjectList;
    public GameObject[] SaveObjectList { get { return m_SaveObjectList; } }


    // Start is called before the first frame update
    void Start()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

