/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Ultimate Three
Description:         Manages the ultimate three specific properties..
                           This class is reponsibile for ...
Date Created:       26/01/2021
Author:                Aaron
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/01/2022
        - [Aaron] Created the base class to help test the CurrentUltimate in the player script and add to the starting Ultimate Ability 3 game object
    04/02/2022
        - [Aaron] Went through script to clean up code, removing redunacies and place code in better spots, and fixing any bugs / noticable issues
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateThree : UltimateAbility
{
    [Header("Effect Properties")]
    public GameObject AreaEffectObject;
    public float AreaEffectAmount = 1.0f;
    public float AreaEffectDuration = 6.0f;

    // When the trigger collides with an enemy object
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy)
        {
            // Activate the AoE object, set it's position to the enemy position and turn on the effect timer
            if (AreaEffectObject)
            {
                AreaEffectObject.SetActive(true);
                AreaEffectObject.transform.position = enemy.gameObject.transform.position;
                AreaEffectObject.GetComponent<AoeEffect>().SetEffectAmountAndDuration(AreaEffectAmount, AreaEffectDuration);
                AreaEffectObject.GetComponent<AoeEffect>().StartAffectTimer();
                AreaEffectObject.GetComponent<AoeEffect>().PlaySound();

                this.gameObject.SetActive(false);
            }
        }
    }
}
