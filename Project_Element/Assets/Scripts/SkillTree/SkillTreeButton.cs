/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeButton
Description:        Button used for Skill Tree System
Date Created:       11/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    11/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour
{
    public Button SkillButton;
    public Text SkillPropertyText;
    public Text SkillCostText;
    public int SkillCost;
    public int SkillAmount;
    public string SkillProperty;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetButtonText()
    {
        if (SkillProperty == "EquipmentUI")
        {
            SkillPropertyText.text = "+" + SkillAmount.ToString() + "\n" + "Equipment Inventory";
        }
        else if (SkillProperty == "MaxHealth")
        {
            SkillPropertyText.text = "+" + SkillAmount.ToString() + "\n" + "Max Health";
        }
        else if (SkillProperty == "MaxStamina")
        {
            SkillPropertyText.text = "+" + SkillAmount.ToString() + "\n" + "Max Stamina";
        }
        else if (SkillProperty == "MaxDefense")
        {
            SkillPropertyText.text = "+" + SkillAmount.ToString() + "\n" + "Max Defense";
        }

        SkillCostText.text = "Cost: " + SkillCost.ToString();
    }
}
