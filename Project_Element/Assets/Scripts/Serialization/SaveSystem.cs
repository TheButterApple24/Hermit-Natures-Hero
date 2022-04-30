/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SaveSystem
Description:        Handles saving and loading individual files
Date Created:       21/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/10/2021
        - [Jeffrey] Created base class

 ===================================================*/

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    public static void SaveGame(Player player, string saveName)
    {
        // If directory doesn't exist, create it
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        // Create binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        // Grab the path the file will save in
        string path = Application.persistentDataPath + "/saves/" + saveName + ".element";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Create a player data to store all the data
        PlayerData data = new PlayerData(player);

        // Save the file
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadGame(string path)
    {
        //string path = Application.persistentDataPath + "/player.element";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static SettingsData LoadSettings(string path)
    {
        //string path = Application.persistentDataPath + "/player.element";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

}
