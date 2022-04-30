/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Tutorial Trigger
Description:         A trigger script used to update the Small Display Tutorial UI based on interaction with the player and the pop up text desired
Date Created:       03/07/2022
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    07/03/2022
        - [Aaron] Created framework for the class.
        - [Aaron] The tutorial manager text is activated and set when entering the trigger box, and after a specific time will reset and deactivate
    08/03/2022
        - [Aaron] Updated the tutoril manager to be set with the menu manager instance. Added a stop to the timer if player leaves the trigger area.
    16/03/2022
        - [Aaron] Adjusted the trigger and timer to reset the tutorial display if the player remains inside the trigger zone for a certain time or if they leave the trigger zone. Also destroy the gameobject as it won't be needed any longer.

 ===================================================*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TutorialTrigger : MonoBehaviour
{
    public string m_TutorialText = "";
    public float m_DisplayTime = 0.0f;

    TutorialManager m_TutorialManager;
    bool m_HasBeenTriggered = false;

    private void Start()
    {
        m_TutorialManager = MenuManager.Instance.TutorialScreenObject.GetComponent<TutorialManager>();
    }

    // If the player enters the trigger zone, activate and display the tutorial text, set the triggered bool to true to prevent re-trigger and start the reset display timer
    private void OnTriggerEnter(Collider other)
    {
        if (m_HasBeenTriggered == true)
            return;

        Player player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            m_HasBeenTriggered = true;
            m_TutorialManager.ActivateSmallDisplay();
            m_TutorialManager.SetSmallDisplayText(m_TutorialText);

            StartCoroutine(DisplayTimer());
        }
    }

    // If the player leaves the trigger zone, reset the popup text and turn off the window, stop the display if it hasn't finished and destroy the gameobject
    private void OnTriggerExit(Collider other)
    {

        Player player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            m_TutorialManager.SetSmallDisplayText("");
            m_TutorialManager.DeactivateSmallDisplay();

            StopCoroutine(DisplayTimer());
            Destroy(this.gameObject);
        }
    }

    // If the player remains in the trigger zone for the timer duration, turn off the tutorial text
    IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(m_DisplayTime);

        m_TutorialManager.SetSmallDisplayText("");
        m_TutorialManager.DeactivateSmallDisplay();
    }
}
