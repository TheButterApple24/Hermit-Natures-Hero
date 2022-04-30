/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Quest
Description:        Handles what tasks are in the Quest
Date Created:       30/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    30/10/2021
        - [Jeffrey] Created base class

    20/01/2022
        - [Aaron] Added new variables to track the quest list manager for the quest menu and variables to for quest description / info
                  OnAccept function now adds the quest to the quest list.
        - [Aaron] Changed OnAccepted function to call an update function on the QuestListManager and no longer needs to use data directly 
                  from this script
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks

    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [HideInInspector] public bool m_IsCurrentlyActive = false;
    [HideInInspector] public bool IsAccepted = false;

    public GameObject ObjectToDestroy;
    public GameObject[] ObjectsToEnableAfterQuest;

    [HideInInspector] public List<QuestTask> m_Tasks;
    public QuestTask[] m_TasksArray;

    [HideInInspector] public int m_TaskIndex = 0;

    public GameObject m_NavMarkerPrefab;
    public Color m_NavMarkerColor;
    public float MarkerOffsetY = 0;

    public string m_QuestName;
    public string m_QuestDescription;

    [HideInInspector] public List<GameObject> m_QuestRewards;
    [HideInInspector] public List<int> m_QuestRewardsAmount;

    public int QuestEXP = 10;

    public GameObject[] m_QuestRewardsArray;
    public int[] m_QuestRewardsAmountArray;

    public bool m_IsDecreeQuest = false;
    public bool IsQueueQuest = false;

    public SkinnedMeshRenderer QuestMeshFilter;
    private Mesh m_QuestMesh;
    private GameObject m_NavMarker;

    [UniqueIdentifier]
    public string SaveId;

    public Quest NextQuestInQueue;

    public bool RigToDestroy = false;

    // Called when object is created
    private void Awake()
    {
        m_Tasks = new List<QuestTask>();
        m_QuestRewards = new List<GameObject>();
        m_QuestRewardsAmount = new List<int>();

        if (m_QuestRewardsArray != null)
        {
            for (int i = 0; i < m_QuestRewardsArray.Length; i++)
            {
                m_QuestRewards.Add(m_QuestRewardsArray[i]);
                m_QuestRewardsAmount.Add(m_QuestRewardsAmountArray[i]);
            }
        }

        if (m_TasksArray != null)
        {
            for (int i = 0; i < m_TasksArray.Length; i++)
            {
                m_Tasks.Add(m_TasksArray[i]);
            }
        }
        if (!m_IsDecreeQuest && !IsQueueQuest)
        {
            if (QuestMeshFilter != null)
            {
                m_QuestMesh = QuestMeshFilter.sharedMesh;

                m_NavMarker = Instantiate(m_NavMarkerPrefab, transform.position, transform.rotation);
                SpriteRenderer navSprite = m_NavMarker.GetComponent<SpriteRenderer>();
                navSprite.color = m_NavMarkerColor;

                Vector3 markerPosition = m_NavMarker.transform.localPosition;

                markerPosition.y += (((m_QuestMesh.bounds.size.y / 2) + m_QuestMesh.bounds.center.y) + (navSprite.size.y / 2)) + MarkerOffsetY;

                m_NavMarker.transform.localPosition = markerPosition;

                BobbingAnimation animation = m_NavMarker.AddComponent<BobbingAnimation>();
                animation.Speed = 3;
                animation.Amplutide = 0.05f;
            }
        }
    }

    /// <summary>
    /// Called when Quest is accepted
    /// </summary>
    public void OnAccepted()
    {
        if (!IsAccepted)
        {
            Player player = PlayerManager.Instance.Player;

            // When accepted, add quest to Player
            player.m_AcceptedQuests.Add(this);

            m_IsCurrentlyActive = true;

            // Set active quest to this quest
            player.m_ActiveQuest = player.m_AcceptedQuests.Count - 1;

            player.SetQuestToActive();

            m_Tasks[m_TaskIndex].InitTask();

            if (m_NavMarker != null)
            {
                m_NavMarker.SetActive(false);
            }

            // Add the quest to the quest menu
            GameManager.Instance.QuestListManager.UpdateQuestMenu();

            IsAccepted = true;
        }
    }

    /// <summary>
    /// Called when quest is compelted
    /// </summary>
    void OnCompleted()
    {
        m_IsCurrentlyActive = false;

        StartCoroutine("CompleteQuest");

    }

    // Update is called once per frame
    void Update()
    {
        if (IsAccepted)
        {
            CheckForCompletion();

            if (m_IsCurrentlyActive)
            {
                HUDManager.Instance.QuestTitleText.text = m_QuestName;
            }
        }
    }

    /// <summary>
    /// Will Check if a quest is completed
    /// </summary>
    public void CheckForCompletion()
    {
        if (m_TaskIndex < m_Tasks.Count)
        {
            m_Tasks[m_TaskIndex].m_IsCurrentlyActive = true;
            if (m_Tasks[m_TaskIndex].CheckForCompletion())
            {
                m_Tasks[m_TaskIndex].m_IsCurrentlyActive = false;
                m_TaskIndex++;

                // If there are no more tasks, quest is completed. Else, initalize the next task
                if (m_TaskIndex == m_Tasks.Count)
                {
                    OnCompleted();
                }
                else
                {
                    m_Tasks[m_TaskIndex].InitTask();
                    GameManager.Instance.QuestListManager.UpdateMenuText();
                }
            }
        }
    }

    /// <summary>
    /// Will run exiting code when Quest is completed
    /// </summary>
    /// <returns></returns>
    IEnumerator CompleteQuest()
    {
        // When quest is complete, give rewards

        // Display UI for 1 second
        HUDManager.Instance.QuestTipText.text = "Quest Completed!";

        Player player = PlayerManager.Instance.Player;

        // Remove quest from player, reset values
        player.m_AcceptedQuests.Remove(this);

        player.m_ActiveQuest = 0;
        player.SetQuestToActive();

        GameManager.Instance.QuestListManager.UpdateQuestMenu();
        GameManager.Instance.QuestListManager.ClearIconsList();
        GameManager.Instance.QuestListManager.UpdateMenuText();

        for (int i = 0; i < m_QuestRewards.Count; i++)
        {
            GameObject reward = Instantiate(m_QuestRewards[i]);
            if (reward != null)
            {
                if (m_QuestRewardsAmount[i] > 0)
                {
                    for (int j = 0; j < m_QuestRewardsAmount[i]; j++)
                    {
                        player.m_Inventory.AddToInventory(reward.GetComponent<Pickups>());
                    }
                }
                else
                {
                    player.m_Inventory.AddToInventory(reward.GetComponent<Pickups>());
                }
            }

            Destroy(reward);

        }

        PlayerManager.Instance.Player.CheckForLevelUp(QuestEXP);

        yield return new WaitForSeconds(3.0f);

        if (player.m_AcceptedQuests.Count == 0)
        {
            HUDManager.Instance.QuestTipText.text = "";
            HUDManager.Instance.QuestTipBG.gameObject.SetActive(false);
        }

        if (m_IsDecreeQuest)
        {
            GameObject.Find("EndlessDecreeBoard").GetComponent<DecreeBoard>().CheckForReset();
        }

        player.CompletedQuestIDs.Add(SaveId);

        player.SavePlayer();

        if (!m_IsDecreeQuest)
        {
            DestroyQuest();
        }
    }

    /// <summary>
    /// Will destroy quest objects when quest is over
    /// </summary>
    public void DestroyQuest()
    {
        for (int i = 0; i < m_Tasks.Count; i++)
        {
            if (m_Tasks[i].DestroyObjectOnCompletion)
            {
                Destroy(m_Tasks[i].gameObject);
            }
            else
            {
                Destroy(m_Tasks[i]);
            }
        }

        m_Tasks.Clear();

        if (m_NavMarker != null)
        {
            Destroy(m_NavMarker);
        }

        Destroy(GetComponent<QuestObject>());

        if (ObjectsToEnableAfterQuest != null)
        {
            for (int i = 0; i < ObjectsToEnableAfterQuest.Length; i++)
            {
                ObjectsToEnableAfterQuest[i].SetActive(true);
            }
        }

        if (NextQuestInQueue != null && !RigToDestroy)
        {
            NextQuestInQueue.OnAccepted();
        }

        if (ObjectToDestroy != null)
        {
            Destroy(ObjectToDestroy);

        }
        else
        {
            Destroy(this);
        }
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

    public GameObject GetNavMarker() { return m_NavMarker; }
}
