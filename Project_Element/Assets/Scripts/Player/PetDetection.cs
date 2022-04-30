/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Pet Detection
Description:        A trigger script used on a player Pet Socket to detect when a pet is within destination range
Date Created:       01/11/2021
Author:             Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Aaron] Created framework for the class.
        - [Aaron] Added trigger exit and enter functions to call the Pet's move and stop moving functions, in order to improve pet movement.
    01/12/2021
        - [Aaron] Updated names of called pet functions.
    23/02/2022
        - [Aaron] Added a check to only update the position and rotation of the player's equipped pets. Also now update their facing from the enter/exit the trigger.
    08/03/2022
        - [Aaron] Added a null check for the primary and secondary pet on the enter and exit trigger

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetDetection : MonoBehaviour
{
    // When a Pet is within range of their destination, they should stop updating and face forward
    void OnTriggerEnter(Collider other)
    {
        Pet petObj = other.gameObject.GetComponent<Pet>();
        Player playerObj = transform.parent.GetComponent<Player>();

        if (petObj == null)
        {
            return;
        }

        if (playerObj.m_PrimaryPet != null && petObj.gameObject == playerObj.m_PrimaryPet.gameObject ||
            playerObj.m_SecondaryPet != null && petObj.gameObject == playerObj.m_SecondaryPet.gameObject)
        {
            petObj.StopUpdatingGoalPosition();
            Vector3 faceDirection = playerObj.transform.position + playerObj.transform.forward;
            petObj.FaceTargetPosition(faceDirection);
        }
    }

    // When the player moves, the Pet should begin updating again to be within their destination range and facing toward it
    void OnTriggerExit(Collider other)
    {
        Pet petObj = other.gameObject.GetComponent<Pet>();
        Player playerObj = transform.parent.GetComponent<Player>();

        if (petObj == null)
        {
            return;
        }

        if (playerObj.m_PrimaryPet != null && petObj.gameObject == playerObj.m_PrimaryPet.gameObject ||
            playerObj.m_SecondaryPet != null && petObj.gameObject == playerObj.m_SecondaryPet.gameObject)
        {
            petObj.StartUpdatingGoalPosition();
            petObj.FaceTargetPosition(transform.position);
        }
    }
}
