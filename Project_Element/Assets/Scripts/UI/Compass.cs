/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Compass
Description:        Rotates based on the Player's camera's world rotation, ensuring that the North point is always pointing "up"
Date Created:       03/12/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/02/2022
        - [Zoe] Created base class

 ===================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float zRot = PlayerManager.Instance.Player.m_FollowCamera.transform.rotation.eulerAngles.y;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zRot + 180.0f));
    }
}
