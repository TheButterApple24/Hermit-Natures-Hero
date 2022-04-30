/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              MapMenu
Description:        Displays a map to the player with various icons represented landmarks
Date Created:       03/09/2022
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/09/2022
        - [Zoe] Created Map Menu
    03/10/2022
        - [Zoe] Added Hermit map icon
    03/16/2022
        - [Aaron] Added in tutorial functionality for this menu

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapMenu : MonoBehaviour
{
    [Header("Tutorial")]
    public string MapTutorialText = "";
    private bool m_MapUsed = false;

    private PointerEventData m_PointerEventData;
    private Image m_LastHoveredIcon;
    [SerializeField] private GraphicRaycaster m_Raycaster;
    [SerializeField] private EventSystem m_EventSystem = null;
    [SerializeField] private GameObject m_LocationBox;
    [SerializeField] private Text m_LocationName;
    [SerializeField] private Image m_PlayerIcon;
    [SerializeField] private Camera m_MapCamera;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_OpenSFX;

    // Update is called once per frame
    void Update()
    {
        // Convert the player's world position to screen position
        Vector3 playerPos = m_MapCamera.WorldToScreenPoint(PlayerManager.Instance.Player.transform.position);

        // Set the Player icon to the player's screen position
        m_PlayerIcon.gameObject.transform.position = playerPos;

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        // Send out a ray from the mouse position
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        
        // Update the box with the marker name to the mouse's position after it's turned on
        if (m_LocationBox.activeSelf == true)
        {
            m_LocationBox.transform.position = Input.mousePosition;
        }

        foreach (RaycastResult result in results)
        {
            // If the mouse is hovered over a map marker
            if (result.gameObject.tag == "MapMarker")
            {
                // Store the last icon that was hovered over
                m_LastHoveredIcon = result.gameObject.GetComponent<Image>();

                // Scale the icon up slightly (from 0.55)
                m_LastHoveredIcon.rectTransform.localScale = new Vector3(0.65f, 0.65f, 1);

                // Enable the location text box and set it's location to the mouse
                m_LocationBox.SetActive(true);
                m_LocationBox.transform.position = Input.mousePosition;

                // Set the location name to the map marker's name
                m_LocationName.text = result.gameObject.name;
            }
        }

        // This checks if you are still hovering over the current map marker
        if (m_LastHoveredIcon != null && results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                // If you are still hovering over the icon, leave this function and skip the rest
                if (result.gameObject == m_LastHoveredIcon.gameObject)
                {
                    return;
                }
            }

            // If you loop through everything and didn't find the map marker, you aren't hovered over it anymore
            // Set the box inactive and reset the icon scale
            m_LocationBox.SetActive(false);
            m_LastHoveredIcon.rectTransform.localScale = new Vector3(0.55f, 0.55f, 1);
            m_LastHoveredIcon = null;
        }
    }

    public void OpenMapMenu()
    {
        if (m_MapUsed == false)
        {
            m_MapUsed = true;
            string title = "Map of Plantis";
            PlayerManager.Instance.Player.PlayerUI.DisplayTutorialOnFirstRun(title, MapTutorialText);
        }

        PlayerManager.Instance.Player.PlayerUI.DisableBackpackMenuButtons();
        gameObject.SetActive(true);
        m_AudioSource.PlayOneShot(m_OpenSFX);
    }

    public void CloseMapMenu()
    {
        PlayerManager.Instance.Player.PlayerUI.EnableBackpackMenuButtons();
        m_AudioSource.PlayOneShot(m_OpenSFX);
        gameObject.SetActive(false);
    }
}
