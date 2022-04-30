/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              XMLOp
Description:        Helper class that allows user to save to XML data
Date Created:       02/25/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/25/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.IO;
using System.Xml.Serialization;

public class XMLOp
{
    public static void Serialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    public static T Deserialize<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        if (System.IO.File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            T deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }
        else
        {
            return default(T);
        }

    }
}