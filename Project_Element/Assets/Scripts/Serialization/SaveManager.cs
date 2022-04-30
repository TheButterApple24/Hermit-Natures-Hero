/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SaveManager
Description:        Handles saving and loading multiple files
Date Created:       21/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/10/2021
        - [Jeffrey] Created base SaveManager class
    26/01/2022
        - [Jeffrey] Added ability to delete save files in a folder

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public string[] m_SaveFiles;

    public void GetLoadFiles()
    {
        // If directory doesn't exist, create it
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        // Get all save files
        m_SaveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/", "*.element");
    }

    public void DeleteAllSaveFiles()
    {
        var files = Directory.GetFiles(Application.persistentDataPath + "/saves/");

        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }

        m_SaveFiles = null;

        GetLoadFiles();

        SceneManager.LoadScene(0);
    }
}
