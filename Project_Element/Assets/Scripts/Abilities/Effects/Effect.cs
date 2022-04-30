/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Effect
Description:        Manages the properties shared between different effects
Date Created:       18/11/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Aaron] Set up effects as an abstract class and created the starting variables and functions for duration and amount;
    28/01/2022
        - [Aaron] Moved Setter function into base class for other effects to use
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    09/02/2022
        - [Max] Fixed Inactive Coroutine Bug

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    protected CharacterBase m_AffectedCharacter;
    private float m_EffectDuration = 0.0f;
    private float m_EffectAmount = 0.0f;
    private bool m_IsActive = false;

    protected virtual void Awake()
    {
        m_AffectedCharacter = GetComponent<CharacterBase>();
    }

    public void SetEffectAmountAndDuration(float eAmount, float eDuration)
    {
        EffectAmount = eAmount;
        EffectDuration = eDuration;
    }

    public CharacterBase AffectedCharacter
    {
        get
        {
            return m_AffectedCharacter;
        }

        set
        {
            m_AffectedCharacter = value;
        }
    }

    protected bool IsActive
    {
        get
        {
            return m_IsActive;
        }

        set
        {
            m_IsActive = value;
        }
    }

    protected float EffectAmount
    {
        get
        {
            return m_EffectAmount;
        }

        set
        {
            m_EffectAmount = value;
        }
    }

    protected float EffectDuration
    {
        get
        {
            return m_EffectDuration;
        }

        set
        {
            m_EffectDuration = value;
        }
    }

    // Activate the effect and start the duration timer for effects that require Updating
    public void InitEffect()
    {
        // If GameObject is active
        if (gameObject.activeSelf)
        {
            IsActive = true;
            StartCoroutine(EndEffect());
        }
    }

    private IEnumerator EndEffect()
    {
        yield return new WaitForSeconds(EffectDuration);

        Destroy(this);
    }

    public void KillEffect()
    {
        Destroy(this);
    }

}
