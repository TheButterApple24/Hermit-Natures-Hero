/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              QuestTask
Description:        Parent class for tasks
Date Created:       30/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    30/10/2021
        - [Jeffrey] Created base class
    31/01/2022
        - [Jeffrey] Implemented Multiple Tasks
    31/03/2022
        - [Jeffrey] Reworked Quest System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTask : MonoBehaviour
{
    public GameObject[] TaskObjects;
    public bool m_IsCurrentlyActive = false;

    public string m_TaskTip;

    public Transform m_NavMarkerTransform;

    public Color m_NavMarkerColor;
    public bool DestroyObjectOnCompletion = false;

    protected NavWaypoint m_NavMarker;
    public NavWaypoint NavMarker { get { return m_NavMarker; } }

    protected virtual void Start()
    {

    }

    /// <summary>
    /// Initializes a quest task
    /// </summary>
    public virtual void InitTask()
    {
        if (m_NavMarker == null)
        {
            m_NavMarker = Camera.main.gameObject.GetComponent<NavWaypoint>();
        }

        if (TaskObjects != null)
        {
            for (int i = 0; i < TaskObjects.Length; i++)
            {
                TaskObjects[i].SetActive(true);
            }
        }

        UpdateUI();

    }

    /// <summary>
    /// Updates task UI
    /// </summary>
    public virtual void UpdateUI()
    {
        if (m_IsCurrentlyActive)
        {
            HUDManager.Instance.QuestTipText.text = m_TaskTip;
            if (m_NavMarker != null && m_NavMarker.m_MarkerImage != null)
            {
                m_NavMarker.m_MarkerImage.color = m_NavMarkerColor;
                m_NavMarker.m_Target = m_NavMarkerTransform;
                m_NavMarker.m_MarkerImage.enabled = true;
            }
        }
    }

    /// <summary>
    /// Checks if task is completed
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckForCompletion()
    {
        return false;
    }
}
