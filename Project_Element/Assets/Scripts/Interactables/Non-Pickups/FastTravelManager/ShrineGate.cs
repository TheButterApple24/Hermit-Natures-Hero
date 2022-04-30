/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ShrineGate
Description:        Handles the Shrine Gate Objects
Date Created:       10/12/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/02/2022
        - [Max] Created base class
    12/02/2022
        - [Max] Created public struct for materials
    16/02/2022
        - [Max] Set Shrine Lantern material dynamically

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;

[System.Serializable]
public struct ShrineGateMatList
{
    public Material m_NeutralMat;
    public Material m_FireMat;
    public Material m_WaterMat;
    public Material m_PlantMat;
}

public class ShrineGate : MonoBehaviour
{
    public ShrineGateMatList m_MaterialList;

    private void Start()
    {
        // Set up Lanterns if they exist and are enabled
        if (GetComponentInChildren<JapaneseLantern>() != null && GetComponentInChildren<JapaneseLantern>().isActiveAndEnabled)
        {
            GetComponentInChildren<JapaneseLantern>().Start();
        }
    }

    public void Activate(Element element)
    {
        // If this object's renderer exists, apply activated material to this object based on its element
        if (transform.GetComponent<Renderer>() != null)
        {
            switch (element)
            {
                case Element.None:
                    transform.GetComponent<Renderer>().material = m_MaterialList.m_NeutralMat;
                    break;
                case Element.Fire:
                    transform.GetComponent<Renderer>().material = m_MaterialList.m_FireMat;
                    break;
                case Element.Water:
                    transform.GetComponent<Renderer>().material = m_MaterialList.m_WaterMat;
                    break;
                case Element.Plant:
                    transform.GetComponent<Renderer>().material = m_MaterialList.m_PlantMat;
                    break;
                default:
                    transform.GetComponent<Renderer>().material = m_MaterialList.m_NeutralMat;
                    break;
            }
        }
    }
}
