/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Ability
Description:        Abstract class that manages the shared functions and variables between it's child classes
Date Created:       01/11/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    01/11/2021
        - [Aaron] Set up the framework for this class, added the initial properties
    09/11/2021
        - [Aaron] Removed empty functions and moved to Main Ability class until we need them for the ultimate abilities.
        - [Aaron] Made protected variables public so they can be set in the editor
    17/11/2021
        - [Aaron] Moved Lifespan and active/deactive logic into the base Ability class. Both Ultimate and Main will require these functions and propertiees.
    29/11/2021
        - [Aaron] Added two Sprite variables that will set the specific ability's sprite in the editor. One full version and one outline (active v deactive)
    02/12/2021
        - [Aaron] Updated the cooldown timer to update the game's main ability HUD element
    08/12/2021
        - [Aaron] Added a check to the cooldown timer to ensure that only the primary pet's cooldown would affect the main ability icon.
    27/01/2022
        - [Aaron] Seperated Cooldown Timer into Main and Ultimate Cooldown Timers. For the different UI elements each affects.
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    05/02/2022
        - [Aaron] Overloaded SetStartPoint function to take a gameobject and use it's position and rotation for setting an ability's start point
    11/03/2022
        -[Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process
    13/03/2022
        -[Aaron] Updated the timers to use the instance managers instead of useing GameObject.Find, and made sure to update the correct sprite object
    25/03/2022
        -[Max] Added Pet Ability animations

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elements;

abstract public class Ability : MonoBehaviour
{
    // Memeber variables
    [Header("Ability Properties")]
    public GameObject AbilityOwner;
    public string AbilityName = "";
    public Element AbilityElementType = Element.Undefined;
    public float AbilityDamage = 1.0f;
    public float AbilityCooldown = 2.0f;
    public float AbilityLifespanMax = 1.0f;

    protected float m_Lifespan;
    private bool m_IsActive = false;
    private bool m_IsOnCooldown = false;


    // Update the spawn point for abilities 
    public void SetStartPoint()
    {
        //  Sets the spawn point for abilities based on the position and forward facing of the ability owner.
        Vector3 StartPoint = AbilityOwner.transform.position + AbilityOwner.transform.forward * 1.5f;
        this.transform.position = StartPoint;

        // Update the starting rotation to match the ability owner's
        this.transform.rotation = AbilityOwner.transform.rotation;
    }

    // Set the ability instance to active
    public void ActivateInstance()
    {
        this.gameObject.SetActive(true);
    }

    // Set the ability instance to in-active
    public void DeactivateInstance()
    {
        this.gameObject.SetActive(false);
    }

    // Get & Set function for Active state bool
    public bool IsActive
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

    // Get & Set function for Cooldown bool
    public bool OnCooldown
    {
        get
        {
            return m_IsOnCooldown;
        }

        set
        {
            m_IsOnCooldown = value;
        }
    }

    // Controls when an ability can be used, will allow spawning after the cooldown timer is finished
    public IEnumerator MainCooldownTimer()
    {
        yield return new WaitForSeconds(AbilityCooldown);

        // Set the cooldown to no longer being active
        OnCooldown = false;

        // Grab the Main Ability UI object and the Player object
        GameObject iconUI = HUDManager.Instance.PrimaryPetIconFill;
        GameObject player = PlayerManager.Instance.Player.gameObject;

        // Set the HUD's main ability icon back to the current prime pet's main ability icon (full version)
        if (this.AbilityOwner.GetComponent<Pet>() == player.GetComponent<Player>().m_PrimaryPet)
        {
            iconUI.GetComponent<Image>().fillAmount = 1.0f;
        }       
    }

    // Controls when an ultimate can be used, will allow spawning after the cooldown timer is finished
    public IEnumerator UltimateCooldownTimer()
    {
        yield return new WaitForSeconds(AbilityCooldown);

        // Set the cooldown to no longer being active
        OnCooldown = false;

        // Grab the Ultimate Ability UI object and the Player object
        GameObject player = PlayerManager.Instance.Player.gameObject;
        GameObject iconUI = HUDManager.Instance.UltimateAbilityIconFill;

        //iconUI.GetComponent<Image>().fillAmount = player.GetComponent<Player>().m_CurrentUltimateAbility.IconFillAmount;
    }

    public virtual void PlayStartSound()
    {
    }
}
