/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PuzzleCube
Description:        Puzzle Component: Player can pick up and move the object to place on buttons/pressure plates.
Date Created:       19/10/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/10/2021
        - [Zoe] Started PuzzleCube.
    20/10/2021
        - [Zoe] Fixed bug where Cube wouldn't have physics when dropped.
        - [Zoe] Cleaned up logic, added player null check.
    21/10/2021
        - [Zoe] Added option for multiple IDs.
    01/11/2021
        - [Jeffrey] Puzzle cube now works with quests
    05/12/2021
        - [Zoe] Replaced temporary player object with m_Player
        - [Zoe] Fixed bug where cube would drop when dropping weapon from inventory

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCube : InteractableBase
{
    private bool m_IsParented = false;
    public int[] m_CubeIDs;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (m_PlayerController.m_IsClimbing)
        {
            Drop();
        }

        if (this == m_Player.m_HeldObject && !m_Player.m_IsMenuOpen && Input.GetButtonDown("DropWeapon"))
        {
            Drop();
        }

        float dropTrigger = Input.GetAxisRaw("OpenSkillTree");
        if (this == m_Player.m_HeldObject && !m_Player.m_IsMenuOpen && dropTrigger < 0)
        {
            Drop();
        }
    }

    public override void Activate()
    {
        if (m_Player != null)
        {
            // If the player isn't holding an object, nor holding this
            if (!m_IsParented && m_Player.m_HeldObject == null)
            {
                Hold();
            }
        }
    }

    public override void Deactivate()
    {
        Drop();
    }

    void Hold()
    {
        // Check if theres a player in the scene
        if (m_Player != null)
        {

            // Check if the player has quests
            if (m_Player.m_AcceptedQuests.Count > 0)
            {
                // Check if the current quest is active
                if (m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_IsCurrentlyActive)
                {
                    // Check if active task is a kill task
                    if (m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_Tasks[m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_TaskIndex].GetType() == typeof(QuestTask_Puzzle))
                    {
                        // Store task in temp variable
                        QuestTask_Puzzle task = (QuestTask_Puzzle)m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_Tasks[m_Player.m_AcceptedQuests[m_Player.m_ActiveQuest].m_TaskIndex];

                        // Enable pressure plate marker and set colour
                        task.NavMarker.m_MarkerImage.color = task.m_NavMarkerColor;
                        task.NavMarker.m_Target = task.m_GoalNavMarker;
                    }
                }
            }
        }

        // Parents the object an empty GameObject serving as a socket.
        gameObject.transform.parent = GameObject.Find("HoldSocket").transform;
        gameObject.transform.position = gameObject.transform.parent.position;
        m_IsParented = true;
        // Disables physics for the object.
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // Tells the player that they are holding an object.
        m_Player.m_HeldObject = this;
    }

    void Drop()
    {
        // Check if theres a player in the scene
        if (GameObject.Find("Player"))
        {
            Player player = GameObject.Find("Player").GetComponent<Player>();

            // Check if the player has quests
            if (player.m_AcceptedQuests.Count > 0)
            {
                // Check if the current quest is active
                if (player.m_AcceptedQuests[player.m_ActiveQuest].m_IsCurrentlyActive)
                {
                    // Check if active task is a kill task
                    if (player.m_AcceptedQuests[player.m_ActiveQuest].m_Tasks[player.m_AcceptedQuests[player.m_ActiveQuest].m_TaskIndex].GetType() == typeof(QuestTask_Puzzle))
                    {
                        // Store task in temp variable
                        QuestTask_Puzzle task = (QuestTask_Puzzle)player.m_AcceptedQuests[player.m_ActiveQuest].m_Tasks[player.m_AcceptedQuests[player.m_ActiveQuest].m_TaskIndex];

                        // Enable pressure plate marker and set colour
                        task.NavMarker.m_MarkerImage.color = task.m_NavMarkerColor;
                        task.NavMarker.m_Target = task.m_CubeTransform;
                    }
                }
            }
        }

        // Unparents the object from the player and lets them know they aren't holding an object anymore.
        gameObject.GetComponent<Rigidbody>().AddForce(m_Player.gameObject.transform.forward * 10);

        m_Player.m_HeldObject = null;
        m_Player.InteractionHandler.TargetObject = null;
        gameObject.transform.parent = null;
        m_IsParented = false;

        // Reenables physics on the object.
        gameObject.GetComponent<Rigidbody>().isKinematic = false;

        //m_CanInteract = false;
        //
        //m_ButtonPrompt.SetActive(false);
    }
}


