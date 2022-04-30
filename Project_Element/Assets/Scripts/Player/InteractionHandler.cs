/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              InteractionHandler
Description:        Handles the player's object interaction. (Moved from Player.cs)
Date Created:       03/07/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/07/2022
        - [Zoe] Created InteractionHandler

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private Player m_Player;
    private int m_InteractableIndex;
    public List<InteractableBase> m_InteractablesInRange;
    private InteractableBase m_TargetObject;
    [HideInInspector] public InteractableBase TargetObject { get { return m_TargetObject; } set { m_TargetObject = null; } }
    [HideInInspector] public int InteractableCount { get { return m_InteractablesInRange.Count; } }

    // Start is called before the first frame update
    void Start()
    {
        m_Player = PlayerManager.Instance.Player;
        m_InteractablesInRange = new List<InteractableBase>();
    }

    // Update is called once per frame
    void Update()
    {
        // If not in the menus and the interact button is pressed, interact with the closest object
        if (Input.GetButtonDown("Interact") && !m_Player.m_IsMenuOpen)
        {
            Interact();
        }

        // If there are interactable objects in range, calculate the closest one
        if (m_InteractablesInRange.Count != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") == 0 && m_Player.m_Controller.m_RigidBody.velocity != Vector3.zero)
                CalculateClosestInteractable();

            if (m_InteractablesInRange.Count > 1)
            {
                for (int i = 0; i < m_InteractablesInRange.Count; i++)
                {
                    if (m_InteractablesInRange[i] == m_TargetObject)
                    {
                        m_InteractableIndex = i;
                    }
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    m_InteractableIndex++;
                    if (m_InteractableIndex > m_InteractablesInRange.Count - 1)
                    {
                        m_InteractableIndex = 0;
                    }
                    m_TargetObject = m_InteractablesInRange[m_InteractableIndex];
                    m_TargetObject.ResetParticles();
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    m_InteractableIndex--;
                    if (m_InteractableIndex < 0)
                    {
                        m_InteractableIndex = m_InteractablesInRange.Count - 1;
                    }
                    m_TargetObject = m_InteractablesInRange[m_InteractableIndex];
                    m_TargetObject.ResetParticles();
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        InteractableBase interactable = other.gameObject.GetComponent<InteractableBase>();

        // If the player is close to an interactable object, add them to the List of obejcts in range
        if (interactable)
        {
            if (!m_InteractablesInRange.Contains(interactable))
            {
                m_InteractablesInRange.Add(interactable);
                interactable.m_CanInteract = true;
            }

            if (interactable == m_TargetObject)
            {
                m_TargetObject = null;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        InteractableBase interactable = other.gameObject.GetComponent<InteractableBase>();

        // If an interactable objects leave the player's range, remove it from the list
        if (interactable)
        {
            if (m_InteractablesInRange.Contains(interactable))
            {
                m_InteractablesInRange.Remove(interactable);
                interactable.m_CanInteract = false;

                if (interactable == m_TargetObject)
                {
                    m_TargetObject.m_InteractionParticles.Stop();
                    m_TargetObject = null;
                }
            }
        }
    }

    public void CalculateClosestInteractable()
    {
        float lowestDist = Mathf.Infinity;
        InteractableBase closestObject = null;

        for (int i = 0; i < m_InteractablesInRange.Count; i++)
        {
            // Calculate the distance between each interactable object in the list with the player
            float dist = Vector3.Distance(m_InteractablesInRange[0].transform.position, transform.position);

            // If the distance is lower than the last lowest distance, than this object is the one closest to the player (once the loop is done)
            if (dist < lowestDist)
            {
                lowestDist = dist;
            }
            closestObject = m_InteractablesInRange[i];
        }

        if (closestObject)
        {
            // If this object isn't being held (like a Puzzle Cube) and it isn't the player's current weapon, than this is the object we can interact with
            if (!closestObject.m_IsBeingHeld && closestObject != m_Player.m_HeldObject && closestObject != m_Player.m_MainWeapon)
            {
                // If the target object is not already this object, and the object can be interacted with
                if (m_TargetObject != closestObject && closestObject.m_IsInteractable)
                {
                    m_TargetObject = closestObject;
                    m_TargetObject.m_CanInteract = true;
                    m_TargetObject.ResetParticles();
                }
            }
        }
    }
    public void Interact()
    {
        // If the target object exists and we aren't holding it
        if (m_TargetObject != null && m_Player.m_HeldObject != m_TargetObject)
        {
            //Remove the object from the list of objects
            if (m_InteractablesInRange.Contains(m_TargetObject))
            {
                if (m_TargetObject.RemoveFromListOnTrigger)
                {
                    m_InteractablesInRange.Remove(m_TargetObject);
                    m_TargetObject.m_InteractionParticles.Stop();

                    // Activate the object
                    m_TargetObject.Activate();

                    // Recalculate the closest object
                    CalculateClosestInteractable();
                }
                else
                {
                    // Activate the object
                    m_TargetObject.Activate();
                }
            }
        }
    }

    public void Remove(InteractableBase obj)
    {
        if (m_InteractablesInRange.Contains(obj))
            m_InteractablesInRange.Remove(obj);

        if (obj == m_TargetObject)
            m_TargetObject = null;

        CalculateClosestInteractable();
    }
}
