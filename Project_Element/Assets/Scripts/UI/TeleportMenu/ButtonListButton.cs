/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ButtonListButton
Description:        Handles the Button that's part of the Button List
Date Created:       18/11/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Max] Created base class
    19/11/2021
        - [Max] Fixed Teleporter Menu
    01/12/2021
        - [Max] Added Comments

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private ControlButtonList m_ControlButtonList;

    private string m_TextString;

    public void SetText(string textButton)
    {
        // Set Button Text to textButton
        m_TextString = textButton;
        m_Text.text = m_TextString;
    }

    public void OnClick()
    {
        // Call ButtonClicked when Button is clicked
        m_ControlButtonList.ButtonClicked(m_TextString);
    }
}
