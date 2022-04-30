/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ControlButtonList
Description:        Handles the Button List
Date Created:       19/11/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/11/2021
        - [Max] Created base class
    19/11/2021
        - [Max] Fixed Teleporter Menu
    01/12/2022
        - [Max] Added Comments
    24/02/2022
        - [Max] Removed Secret Teleporter Buttons

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonList : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ButtonPrefab;

    [SerializeField]
    private List<GameObject> m_ButtonList;

    public void CreateButton(string buttonName, bool isEnabled)
    {
        // Instantiate new Button
        GameObject newButton = Instantiate(m_ButtonPrefab) as GameObject;

        // Set new button as visible
        newButton.SetActive(true);

        // If Teleporter linked to this Button is unlocked/clickable
        if (isEnabled)
        {
            // Set Button Text as teleporter name
            newButton.GetComponent<ButtonListButton>().SetText(buttonName);

            // Set Button as interactable
            newButton.GetComponentInChildren<Button>().interactable = true;
        }
        //else
        //{
        //    // Set Button Text as ???
        //    newButton.GetComponent<ButtonListButton>().SetText("???");

        //    // Set Button as NON interactable
        //    newButton.GetComponentInChildren<Button>().interactable = false;
        //}

        // Set new Button's parent to Button List Content
        newButton.transform.SetParent(m_ButtonPrefab.transform.parent, false);

        // Add new Button to Button list
        m_ButtonList.Add(newButton);
    }

    public void ButtonClicked(string buttonName)
    {
        // if Teleporter Menu exists
        if (transform.parent.GetComponent<TeleportMenu>() != null)
        {
            // Call TeleportMenu's Teleport (Teleports Player to buttonName)
            transform.parent.GetComponent<TeleportMenu>().Teleport(buttonName);
        }
    }

    public void RemoveButtons()
    {
        // For each Button is the list
        foreach (GameObject button in m_ButtonList)
        {
            // Destroy Button's gameobject
            Destroy(button.gameObject);
        }


        // Clear Button List
        m_ButtonList.Clear();
    }
}
