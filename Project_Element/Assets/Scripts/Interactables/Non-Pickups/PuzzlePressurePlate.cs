/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PuzzlePressurePlate
Description:        Puzzle Component: Player can place puzzle cubes on this to activate certain InteractableBase objects.
Date Created:       21/10/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/10/2021
        - [Zoe] Started PuzzlePressurePlate
        - [Zoe] Cleaned up logic, added multiple IDs and option to stay on after activation. 

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePressurePlate : MonoBehaviour
{
    public int[] m_PlateIDs;
    public int m_NumCubesRequired = 1;
    public int m_CubeCount = 0;
    public InteractableBase m_ActivationTarget;
    public bool m_IsOn = false;
    public bool m_StayOnAfterTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        PuzzleCube cube = other.gameObject.GetComponent<PuzzleCube>();

        // If the plate was interacting with a puzzle cube.
        if (cube)
        {
            // Loops through all Cube IDs and Plate IDs to find a match.
            for (int i = 0; i < cube.m_CubeIDs.Length; i++)
            {
                for (int j = 0; j < m_PlateIDs.Length; j++)
                {
                    if (cube.m_CubeIDs[i] == m_PlateIDs[j])
                    {
                        // If there is a match, add to the Plates cube count
                        m_CubeCount++;

                        // If the number of cubes on the pressure plate matches the number of cubes required to turn on,
                        // AND the plate isn't already turned on, turn on the plate and activate the object attached to it.
                        if (m_CubeCount == m_NumCubesRequired && !m_IsOn)
                        {
                            ActivateObject();
                            m_IsOn = true;
                        }
                    }
                }
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PuzzleCube cube = other.gameObject.GetComponent<PuzzleCube>();

        // If the plate just stopped interacting wuth a cube
        if (cube)
        {
            // Loops through all Cube IDs and Plate IDs to find a match.
            for (int i = 0; i < cube.m_CubeIDs.Length; i++)
            {
                for (int j = 0; j < m_PlateIDs.Length; j++)
                {
                    if (cube.m_CubeIDs[i] == m_PlateIDs[j])
                    {
                        // If there is a match, remove from the Plate's cube count
                        m_CubeCount--;

                        // If the number of cubes on the pressure plate is lower than the required number,
                        // AND the plate is already on, 
                        // AND the plate doesn't remain on after being triggered, turn iff the plate and deactivate the object attached.
                        if (m_CubeCount < m_NumCubesRequired && m_IsOn && !m_StayOnAfterTrigger)
                        {
                            DeactivateObject();
                            m_IsOn = false;
                        }
                    }
                }
            }
        }
    }

    void ActivateObject()
    {
        // Activates the Interactable that is linked to the pressure plate
        if (m_ActivationTarget)
        {
            m_ActivationTarget.Activate();
        }
    }

    void DeactivateObject()
    {
        // Activates the Interactable that is linked to the pressure plate
        if (m_ActivationTarget)
        {
            m_ActivationTarget.Deactivate();
        }
    }
}
