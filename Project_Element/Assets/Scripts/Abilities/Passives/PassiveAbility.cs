/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PassiveAbility
Description:        Manages the passive specific properties that will store values it will need to modify
Date Created:       01/11/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/11/2021
        - [Aaron] Set up the framework for this class, added the initial properties and activate/deactivate functions
    10/03/2022
        - [Aaron] Changed the player reference to be set using the manager.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbility : Ability
{
    public float ChangeAmount;

    protected Player m_PlayerChar;
    protected float m_OriginalValue;

    protected virtual void Start()
    {
        m_PlayerChar = PlayerManager.Instance.Player;
    }

    public virtual void ActivateAbility()
    {

    }

    public virtual void DeactivateAbility()
    {

    }

}
