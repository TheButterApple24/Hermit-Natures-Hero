/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoadMenuManager
Description:        Creates a list of buttons for all the savegames and when clicked will load the selected savegame.
Date Created:       21/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/10/2021
        - [Jeffrey] Created base class
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadMenuManager : MonoBehaviour
{
    public GameObject m_PrefabButton;
    public RectTransform m_ParentPanel;
    public SaveManager m_Manager;

    Navigation CancelButtonNav;

    private string[] m_SaveNames;
    private Button[] m_LoadButtons;

    // Start is called before the first frame update
    void Start()
    {
        // Get all save files
        m_Manager.GetLoadFiles();
        m_SaveNames = new string[m_Manager.m_SaveFiles.Length];
        m_LoadButtons = new Button[m_Manager.m_SaveFiles.Length];

        CancelButtonNav = MainMenuManager.Instance.LoadGameCancelButton.navigation;
        CancelButtonNav.mode = Navigation.Mode.None;
        CancelButtonNav.mode = Navigation.Mode.Explicit;

        // Loop through and create a button for each save file
        for (int i = 0; i < m_Manager.m_SaveFiles.Length; i++)
        {
            GameObject goButton = (GameObject)Instantiate(m_PrefabButton);
            goButton.transform.SetParent(m_ParentPanel, false);
            goButton.transform.localScale = new Vector3(1, 1, 1);
            goButton.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(m_Manager.m_SaveFiles[i]);

            m_SaveNames[i] = Path.GetFileNameWithoutExtension(m_Manager.m_SaveFiles[i]);

            Button tempButton = goButton.GetComponent<Button>();
            int tempInt = i;
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt));

            m_LoadButtons[i] = tempButton;

            if (i == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(goButton);
                CancelButtonNav.selectOnRight = tempButton;
            }

            Navigation tempNav = tempButton.navigation;
            tempNav.mode = Navigation.Mode.Explicit;
            tempNav.selectOnLeft = MainMenuManager.Instance.LoadGameCancelButton;
            tempButton.navigation = tempNav;
        }

        MainMenuManager.Instance.LoadGameCancelButton.navigation = CancelButtonNav;

        if (m_Manager.m_SaveFiles.Length > 1)
        {
            for (int i = 0; i < m_Manager.m_SaveFiles.Length; i++)
            {
                Navigation currentButtonNav = m_LoadButtons[i].navigation;
                if (i > 0)
                {
                    currentButtonNav.selectOnUp = m_LoadButtons[i - 1];
                }
                if (i != m_Manager.m_SaveFiles.Length - 1)
                {
                    currentButtonNav.selectOnDown = m_LoadButtons[i + 1];
                }
                m_LoadButtons[i].navigation = currentButtonNav;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ButtonClicked(int buttonID)
    {
        PlayerManager.Instance.Player.SaveName = m_SaveNames[buttonID];
        MainMenuManager.Instance.MainMenu.LoadPlayer();
    }
}
