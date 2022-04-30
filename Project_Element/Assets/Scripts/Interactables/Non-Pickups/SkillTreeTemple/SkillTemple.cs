/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              StoneLantern
Description:        Handles the interaction with the Skill Temple
Date Created:       12/04/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    12/04/2022
        - [Max] Created base class
    13/04/2022
        - [Max] Added null checks

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;
using System;

public class SkillTemple : InteractableBase
{
    [SerializeField]
    private JapaneseLantern JapaneseLanternLeft;
    [SerializeField]
    private JapaneseLantern JapaneseLanternRight;

    [SerializeField]
    private StoneLantern StoneLanternLeft;
    [SerializeField]
    private StoneLantern StoneLanternRight;

    [SerializeField]
    private SlidableDoor DoorLeft;
    [SerializeField]
    private SlidableDoor DoorRight;

    //[UniqueIdentifier]
    //public string SaveId;

    float m_AnimationLength = 0.5f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        UnlockTemple();
    }

    public override void Activate()
    {
        m_Player.PlayerUI.SkillTemple = this;

        if (DoorLeft != null && DoorLeft.isActiveAndEnabled)
        {
            DoorLeft.OpenDoor();
        }

        if (DoorRight != null && DoorRight.isActiveAndEnabled)
        {
            DoorRight.OpenDoor();
        }

        // Open Teleport Menu
        StartCoroutine(OpenMenuTimer());
    }

    public void UnlockTemple()
    {
        // Activate Japanese Lanterns
        ActivateJapaneseLanterns();

        // Activate Stone Lanterns
        ActivateStoneLanterns();
    }

    public void ActivateJapaneseLanterns()
    {
        if (JapaneseLanternLeft != null && JapaneseLanternLeft.isActiveAndEnabled)
        {
            JapaneseLanternLeft.Activate(Element.None);
        }

        if (JapaneseLanternRight != null && JapaneseLanternRight.isActiveAndEnabled)
        {
            JapaneseLanternRight.Activate(Element.None);
        }
    }

    public void ActivateStoneLanterns()
    {
        if (StoneLanternLeft != null && StoneLanternLeft.isActiveAndEnabled)
        {
            StoneLanternLeft.Activate();
        }

        if (StoneLanternRight != null && StoneLanternRight.isActiveAndEnabled)
        {
            StoneLanternRight.Activate();
        }
    }

    void OpenTeleportMenu()
    {
        Rigidbody rigidbody = m_Player.transform.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        // Open Temple Menu
        m_Player.PlayerUI.OpenSkillTreeMenu();

        Debug.Log("SkillTreeMenu");
    }

    public void CloseDoors()
    {
        if (DoorLeft != null && DoorLeft.isActiveAndEnabled)
        {
            DoorLeft.CloseDoor();
        }

        if (DoorRight != null && DoorRight.isActiveAndEnabled)
        {
            DoorRight.CloseDoor();
        }
    }

    public IEnumerator OpenMenuTimer()
    {
        yield return new WaitForSeconds(m_AnimationLength);

        m_Player.PlayerUI.OpenSkillTreeMenu();
    }
}
