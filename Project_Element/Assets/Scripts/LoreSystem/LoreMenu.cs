/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoreMenu
Description:        Handles all the UI for the Lore Menu/System
Date Created:       10/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Jeffrey] Created base implementation
    11/03/2022
        - [Zoe] Added Audio Source and SFX for Menu Opening/Closing

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreMenu : MonoBehaviour
{
    public LoreSystem m_LoreSystem;
    public GameObject m_LoreSocketPrefab;
    [HideInInspector] public int m_SelectedCategory = 0;
    public Sprite[] m_LoreImages;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_OpenSFX;
    [SerializeField] private AudioClip m_CloseSFX;
    [SerializeField] private AudioClip m_SwitchTabSFX;

    [Header("Tutorial")]
    public string LoreTutorialText = "";

    List<GameObject> m_LoreBoxList;
    bool m_LoreBookUsed = false;

    void Start()
    {
        m_LoreBoxList = new List<GameObject>();
        m_LoreSystem.LoadDatabase();
        gameObject.SetActive(false);
    }

    public void OpenLoreMenu()
    {
        if (m_LoreBookUsed == false)
        {
            m_LoreBookUsed = true;
            string titleText = "Lore Menu";
            PlayerManager.Instance.Player.PlayerUI.DisplayTutorialOnFirstRun(titleText, LoreTutorialText);
        }

        PlayerManager.Instance.Player.PlayerUI.DisableBackpackMenuButtons();
        gameObject.SetActive(true);
        m_AudioSource.PlayOneShot(m_OpenSFX);
        CreateLoreBoxes();
    }

    public void CloseLoreMenu()
    {
        ClearLoreBoxes();
        PlayerManager.Instance.Player.PlayerUI.EnableBackpackMenuButtons();
        m_AudioSource.PlayOneShot(m_CloseSFX);
        gameObject.SetActive(false);
    }

    public void CreateLoreBoxes()
    {
        // Loop through each index of the Lore Database
        for (int i = 0; i < m_LoreSystem.m_LoreDatabase.Count; i++)
        {
            GameObject box = Instantiate(m_LoreSocketPrefab, m_LoreSystem.gameObject.transform);

            LoreMenuBox loreBox = box.GetComponent<LoreMenuBox>();

            // Check if the index category is the same as the selected category. If it isn't, destroy the created box and move to the next index
            if (m_LoreSystem.m_LoreDatabase[i].category == m_SelectedCategory)
            {
                GameManager manager = GameManager.Instance;

                // Check if the index has been unlocked. If it isn't, destroy the created box and move to the next index
                if (manager.m_LoreUnlockedIDs[m_LoreSystem.m_LoreDatabase[i].id])
                {
                    // Set the box image and title to the associated data from the database
                    loreBox.m_Title.text = m_LoreSystem.m_LoreDatabase[i].title;
                    loreBox.m_Description.text = m_LoreSystem.m_LoreDatabase[i].description;
                    loreBox.m_Image.sprite = m_LoreImages[m_LoreSystem.m_LoreDatabase[i].imageIndex];

                    // Add the lore box to the list
                    m_LoreBoxList.Add(box);
                }
                else
                {
                    Destroy(box);
                }
            }
            else
            {
                Destroy(box);
            }

        }
    }

    public void ClearLoreBoxes()
    {
        // Destroy all lore boxes and clear the list.
        foreach (GameObject obj in m_LoreBoxList)
        {
            Destroy(obj);
        }

        m_LoreBoxList.Clear();
    }

    public void ChangeSelectedCategory(int index)
    {
        // Change the category and re-create all Lore Boxes.
        m_SelectedCategory = index;
        ClearLoreBoxes();
        CreateLoreBoxes();
        m_AudioSource.PlayOneShot(m_SwitchTabSFX);
    }
}
