/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:                  Tutorial Manager
Description:         A manager script used to control the different Tutorial UI Displays
Date Created:       03/08/2022
Author:                Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    08/03/2022
        - [Aaron] Set up the framework for the class.
        - [Aaron] Added activate/deactivate and setting functions for the different tutorial displays

 ===================================================*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    GameObject m_SmallDisplayObject;
    GameObject m_LargeDisplayObject;
    float m_DisplayTime;

    // Start is called before the first frame update
    void Start()
    {
        m_SmallDisplayObject = transform.GetChild(0).gameObject;
        m_LargeDisplayObject = transform.GetChild(1).gameObject;
    }

    public void SetupLargeDisplay(string title, string body)
    {
        DeactivateSmallDisplay();
        ActivateLargeDisplay();

        SetLargeDisplayTitleText(title);
        SetLargeDisplayText(body);
    }

    public void SetSmallDisplayText(string text)
    {
        m_SmallDisplayObject.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    void SetLargeDisplayTitleText(string text)
    {
        m_LargeDisplayObject.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    void SetLargeDisplayText(string text)
    {
        m_LargeDisplayObject.transform.GetChild(1).GetComponent<Text>().text = text;
    }

    public void ActivateSmallDisplay()
    {
        m_SmallDisplayObject.SetActive(true);
    }

    public void DeactivateSmallDisplay()
    {
        m_SmallDisplayObject.SetActive(false);
    }

    public void ActivateLargeDisplay()
    {
        m_LargeDisplayObject.SetActive(true);
    }

    public void DeactivateLargeDisplay()
    {
        m_LargeDisplayObject.SetActive(false);
    }

    public void DisplayPopup(string text, float displayTime)
    {
        ActivateSmallDisplay();
        SetSmallDisplayText(text);
        m_DisplayTime = displayTime;

        StartCoroutine(DisplayTimer());
    }

    IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(m_DisplayTime);

        SetSmallDisplayText("");
        DeactivateSmallDisplay();
    }
}
