/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LocationLoreCollider
Description:        Allows the player to enter a location and add it to the database
Date Created:       14/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    14/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationLoreCollider : MonoBehaviour
{
    public int m_LocationIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        //If the player enters, add the location to the lore menu and delete the collider object.
        if (other.tag == "Player")
        {
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.m_LoreUnlockedIDs[m_LocationIndex] = true;
            Destroy(gameObject);
        }
    }
}
