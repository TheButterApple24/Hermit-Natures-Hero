/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              CreditsManager
Description:        Handles running credits
Date Created:       25/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/01/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public Animator m_Animator;

    bool m_IsPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlaying)
        {
            if (Input.anyKey)
            {
                StopCredits();
            }
        }
    }

    public void PlayCredits()
    {
        gameObject.SetActive(true);
        m_Animator.Play("Base Layer.Entry", 0, 0.0f);
        m_IsPlaying = true;
    }

    public void StopCredits()
    {
        gameObject.SetActive(false);
        m_Animator.StopPlayback();
        m_IsPlaying = false;
    }
}
