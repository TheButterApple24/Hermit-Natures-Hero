/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoreSystem
Description:        Handles the internal system and loading the database
Date Created:       10/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreSystem : MonoBehaviour
{
    [HideInInspector] public List<LoreData> m_LoreDatabase;

    public TextAsset m_JsonFile;

    public void LoadDatabase()
    {
        // Load all the pickups avaiable
        LoreDatas loreDataInJson = JsonUtility.FromJson<LoreDatas>(m_JsonFile.text);

        // Add pickup data from the .json
        foreach (LoreData data in loreDataInJson.m_LoreDatas)
        {
            m_LoreDatabase.Add(data);
        }
    }
}

/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoreData
Description:        Stores variables that are used with the Lore System
Date Created:       10/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/
[System.Serializable]
public class LoreData
{
    public int category;
    public int id;
    public int imageIndex;
    public string title;
    public string description;
    public bool m_Unlocked;
}

/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoreDatas
Description:        Stores an array of LoreData objects to help the database .json
Date Created:       10/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/
[System.Serializable]
public class LoreDatas
{
    public LoreData[] m_LoreDatas;
}
