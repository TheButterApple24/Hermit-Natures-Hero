/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LoadingScreenManager
Description:        Handles loading screens
Date Created:       25/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/01/2022
        - [Jeffrey] Created base implementation
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject m_ScreenObject;
    public GameObject m_ScreenContinue;
    public GameObject m_LoadingCircle;
    public GameObject m_LoadingCircleBG;

    public Image m_ScreenImage;

    public Sprite[] m_ScreenImages;
    public int m_SpriteIndex = 0;

    bool m_FullyLoaded = false;
    bool m_SpinCirlce = false;

    float m_Angle = 1.0f;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;

        if (MenuManager.Instance.LoadingScreen == null)
        {
            MenuManager.Instance.SetLoadingScreen(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ScreenObject.activeSelf)
        {
            if (Input.anyKey && m_FullyLoaded)
            {
                m_ScreenObject.SetActive(false);
            }
            AnimateLoadingCircle();
        }
    }

    public void EnableLoadingScreen()
    {
        m_ScreenObject.SetActive(true);
        m_FullyLoaded = false;
        m_SpinCirlce = true;
        m_SpriteIndex = 0;
        m_ScreenImage.sprite = m_ScreenImages[m_SpriteIndex];
        StartCoroutine(SwapImageTimer());
    }

    //// called second
    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    m_ScreenContinue.SetActive(true);
    //    m_LoadingCircle.SetActive(false);
    //    m_LoadingCircleBG.SetActive(false);
    //    m_FullyLoaded = true;
    //}

    void AnimateLoadingCircle()
    {
        if (m_SpinCirlce == true)
        {
            m_LoadingCircle.GetComponent<RectTransform>().Rotate(Vector3.forward, m_Angle);
        }


        //if (m_LoadingCircle.GetComponent<RectTransform>().localRotation.z > 0 && m_LoadingCircle.GetComponent<RectTransform>().rotation.z < 0.1f)
        //{
        //    m_SpinCirlce = false;
        //    StartCoroutine(PauseCircleSpin());
        //}
    }

    //IEnumerator PauseCircleSpin()
    //{
    //    yield return new WaitForSeconds(0.35f);
    //    m_SpinCirlce = true;
    //}

    IEnumerator SwapImageTimer()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(ChangeImage());
    }

    IEnumerator ChangeImage()
    {
        // Fade out current image
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            m_ScreenImage.color = new Color(m_ScreenImage.color.r, m_ScreenImage.color.g, m_ScreenImage.color.b, i);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        // FChange image and fade in
        m_SpriteIndex++;
        if (m_SpriteIndex < m_ScreenImages.Length)
        {
            m_ScreenImage.sprite = m_ScreenImages[m_SpriteIndex];
        }

        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            m_ScreenImage.color = new Color(m_ScreenImage.color.r, m_ScreenImage.color.g, m_ScreenImage.color.b, i);
            yield return null;
        }

    }

}
