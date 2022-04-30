/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SettingsMenu
Description:        Handles settings buttons/UI elements
Date Created:       20/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/2021
        - [Jeffrey] Created base class
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    09/03/2022
        - [Aaron] Added toggle sprint functionality 

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    public GameObject m_AudioMenu;
    public GameObject m_DisplayMenu;
    public GameObject m_ControlsMenu;
    public GameObject m_OptionsMenu;

    public Button m_AudioButton;
    public Button m_DisplayButton;
    public Button m_ControlsButton;
    public Button m_SaveSettingsButton;
    Navigation m_AudioButtonNav;
    Navigation m_DisplayButtonNav;
    Navigation m_ControlsButtonNav;
    Navigation m_SaveSettingsButtonNav;

    public Button m_SettingsBackButton;
    public Button m_AudioBackButton;
    public Button m_DisplayBackButton;
    public Button m_ControlsBackButton;
    Navigation m_SettingsBackButtonNav;
    Navigation m_AudioBackButtonNav;
    Navigation m_DisplayBackButtonNav;
    Navigation m_ControlsBackButtonNav;

    public AudioMixer m_AudioMixer;

    public Slider m_MasterSlider;
    public Slider m_MusicSlider;
    public Slider m_EffectsSlider;
    Navigation m_MasterSliderNav;
    Navigation m_MusicSliderNav;
    Navigation m_EffectsSliderNav;

    public Slider m_SensitivitySlider;
    Navigation m_SensitivitySliderNav;

    public Toggle m_FullscreenToggle;
    public Toggle m_VSyncToggle;
    public Toggle m_ToggleSprint;
    Navigation m_FullscreenToggleNav;
    Navigation m_ToggleSprintNav;

    public Toggle m_ToggleTutorial;

    public Toggle m_InvertXAxisToggle;
    public Toggle m_InvertYAxisToggle;
    Navigation m_InvertXAxisToggleNav;
    Navigation m_InvertYAxisToggleNav;

    public Dropdown m_GraphicsDropdown;
    public Dropdown m_ResolutionDropdown;
    Navigation m_GraphicsDropdownNav;
    Navigation m_ResolutionDropdownNav;

    public GameObject m_BrightnessCanvas;
    public Image m_BrightnessOverlay;
    public Slider m_BrightnessSlider;
    Navigation m_BrightnessSliderNav;

    public PlayerProperties m_Properties;

    public string m_PlayerSaveName;

    public Text m_MasterVolumeValue;
    public Text m_MusicVolumeValue;
    public Text m_EffectsVolumeValue;

    public Text m_SensitivityValue;
    public MainMenu m_Mainmenu;

    public bool m_CloseAfterLoad = false;

    Resolution[] m_Resolutions;


    private void Start()
    {
        m_AudioButtonNav = m_AudioButton.navigation;
        m_AudioButtonNav.mode = Navigation.Mode.None;
        m_AudioButtonNav.mode = Navigation.Mode.Explicit;
        m_AudioButtonNav.selectOnUp = m_SettingsBackButton;
        m_AudioButtonNav.selectOnDown = m_DisplayButton;
        m_AudioButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_AudioButtonNav.selectOnRight = m_SaveSettingsButton;
        m_AudioButton.navigation = m_AudioButtonNav;

        m_DisplayButtonNav = m_DisplayButton.navigation;
        m_DisplayButtonNav.mode = Navigation.Mode.None;
        m_DisplayButtonNav.mode = Navigation.Mode.Explicit;
        m_DisplayButtonNav.selectOnUp = m_AudioButton;
        m_DisplayButtonNav.selectOnDown = m_ControlsButton;
        m_DisplayButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_DisplayButtonNav.selectOnRight = m_SaveSettingsButton;
        m_DisplayButton.navigation = m_DisplayButtonNav;

        m_ControlsButtonNav = m_ControlsButton.navigation;
        m_ControlsButtonNav.mode = Navigation.Mode.None;
        m_ControlsButtonNav.mode = Navigation.Mode.Explicit; 
        m_ControlsButtonNav.selectOnUp = m_DisplayButton;
        m_ControlsButtonNav.selectOnDown = m_SettingsBackButton;
        m_ControlsButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_ControlsButtonNav.selectOnRight = m_SaveSettingsButton;
        m_ControlsButton.navigation = m_ControlsButtonNav;

        m_SaveSettingsButtonNav = m_SaveSettingsButton.navigation;
        m_SaveSettingsButtonNav.mode = Navigation.Mode.None;
        m_SaveSettingsButtonNav.mode = Navigation.Mode.Explicit;
        m_SaveSettingsButtonNav.selectOnUp = m_ControlsButton;
        m_SaveSettingsButtonNav.selectOnDown = m_AudioButton;
        m_SaveSettingsButtonNav.selectOnLeft = m_SettingsBackButton;
        m_SaveSettingsButtonNav.selectOnRight = m_SettingsBackButton;
        m_SaveSettingsButton.navigation = m_SaveSettingsButtonNav;

        m_SettingsBackButtonNav = m_SettingsBackButton.navigation;
        m_SettingsBackButtonNav.mode = Navigation.Mode.None;
        m_SettingsBackButtonNav.mode = Navigation.Mode.Explicit;
        m_SettingsBackButtonNav.selectOnUp = m_ControlsButton;
        m_SettingsBackButtonNav.selectOnDown = m_AudioButton;
        m_SettingsBackButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_SettingsBackButtonNav.selectOnRight = m_SaveSettingsButton;
        m_SettingsBackButton.navigation = m_SettingsBackButtonNav;

        m_AudioBackButtonNav = m_AudioBackButton.navigation;
        m_AudioBackButtonNav.mode = Navigation.Mode.None;
        m_AudioBackButtonNav.mode = Navigation.Mode.Explicit;
        m_AudioBackButtonNav.selectOnUp = m_ControlsButton;
        m_AudioBackButtonNav.selectOnDown = m_AudioButton;
        m_AudioBackButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_AudioBackButtonNav.selectOnRight = m_SaveSettingsButton;
        m_AudioBackButton.navigation = m_AudioBackButtonNav;

        m_DisplayBackButtonNav = m_DisplayBackButton.navigation;
        m_DisplayBackButtonNav.mode = Navigation.Mode.None;
        m_DisplayBackButtonNav.mode = Navigation.Mode.Explicit; 
        m_DisplayBackButtonNav.selectOnUp = m_ControlsButton;
        m_DisplayBackButtonNav.selectOnDown = m_AudioButton;
        m_DisplayBackButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_DisplayBackButtonNav.selectOnRight = m_SaveSettingsButton;
        m_DisplayBackButton.navigation = m_DisplayBackButtonNav;

        m_ControlsBackButtonNav = m_ControlsBackButton.navigation;
        m_ControlsBackButtonNav.mode = Navigation.Mode.None;
        m_ControlsBackButtonNav.mode = Navigation.Mode.Explicit;
        m_ControlsBackButtonNav.selectOnUp = m_ControlsButton;
        m_ControlsBackButtonNav.selectOnDown = m_AudioButton;
        m_ControlsBackButtonNav.selectOnLeft = m_SaveSettingsButton;
        m_ControlsBackButtonNav.selectOnRight = m_SaveSettingsButton;
        m_ControlsBackButton.navigation = m_ControlsBackButtonNav;

        m_MasterSliderNav = m_MasterSlider.navigation;
        m_MasterSliderNav.mode = Navigation.Mode.None;
        m_MasterSliderNav.mode = Navigation.Mode.Explicit;
        m_MasterSliderNav.selectOnUp = m_SaveSettingsButton;
        m_MasterSliderNav.selectOnDown = m_MusicSlider;
        m_MasterSlider.navigation = m_MasterSliderNav;

        m_MusicSliderNav = m_MusicSlider.navigation;
        m_MusicSliderNav.mode = Navigation.Mode.None;
        m_MusicSliderNav.mode = Navigation.Mode.Explicit;
        m_MusicSliderNav.selectOnUp = m_MasterSlider;
        m_MusicSliderNav.selectOnDown = m_EffectsSlider;
        m_MusicSlider.navigation = m_MusicSliderNav;

        m_EffectsSliderNav = m_EffectsSlider.navigation;
        m_EffectsSliderNav.mode = Navigation.Mode.None;
        m_EffectsSliderNav.mode = Navigation.Mode.Explicit;
        m_EffectsSliderNav.selectOnUp = m_MusicSlider;
        m_EffectsSliderNav.selectOnDown = m_SaveSettingsButton;
        m_EffectsSlider.navigation = m_EffectsSliderNav;

        m_GraphicsDropdownNav = m_GraphicsDropdown.navigation;
        m_GraphicsDropdownNav.mode = Navigation.Mode.None;
        m_GraphicsDropdownNav.mode = Navigation.Mode.Explicit;
        m_GraphicsDropdownNav.selectOnUp = m_SaveSettingsButton;
        m_GraphicsDropdownNav.selectOnDown = m_ResolutionDropdown;
        m_GraphicsDropdownNav.selectOnLeft = m_DisplayButton;
        m_GraphicsDropdownNav.selectOnRight = m_DisplayButton;
        m_GraphicsDropdown.navigation = m_GraphicsDropdownNav;

        m_ResolutionDropdownNav = m_ResolutionDropdown.navigation;
        m_ResolutionDropdownNav.mode = Navigation.Mode.None;
        m_ResolutionDropdownNav.mode = Navigation.Mode.Explicit;
        m_ResolutionDropdownNav.selectOnUp = m_GraphicsDropdown;
        m_ResolutionDropdownNav.selectOnDown = m_FullscreenToggle;
        m_ResolutionDropdownNav.selectOnLeft = m_DisplayButton;
        m_ResolutionDropdownNav.selectOnRight = m_DisplayButton;
        m_ResolutionDropdown.navigation = m_ResolutionDropdownNav;

        m_FullscreenToggleNav = m_FullscreenToggle.navigation;
        m_FullscreenToggleNav.mode = Navigation.Mode.None;
        m_FullscreenToggleNav.mode = Navigation.Mode.Explicit;
        m_FullscreenToggleNav.selectOnUp = m_ResolutionDropdown;
        m_FullscreenToggleNav.selectOnDown = m_BrightnessSlider;
        m_FullscreenToggleNav.selectOnLeft = m_DisplayButton;
        m_FullscreenToggleNav.selectOnRight = m_DisplayButton;
        m_FullscreenToggle.navigation = m_FullscreenToggleNav;

        m_BrightnessSliderNav = m_BrightnessSlider.navigation;
        m_BrightnessSliderNav.mode = Navigation.Mode.None;
        m_BrightnessSliderNav.mode = Navigation.Mode.Explicit;
        m_BrightnessSliderNav.selectOnUp = m_FullscreenToggle;
        m_BrightnessSliderNav.selectOnDown = m_SaveSettingsButton;
        m_BrightnessSlider.navigation = m_BrightnessSliderNav;

        m_SensitivitySliderNav = m_SensitivitySlider.navigation;
        m_SensitivitySliderNav.mode = Navigation.Mode.None;
        m_SensitivitySliderNav.mode = Navigation.Mode.Explicit;
        m_SensitivitySliderNav.selectOnUp = m_SaveSettingsButton;
        m_SensitivitySliderNav.selectOnDown = m_InvertXAxisToggle;
        m_SensitivitySlider.navigation = m_SensitivitySliderNav;

        m_InvertXAxisToggleNav = m_InvertXAxisToggle.navigation;
        m_InvertXAxisToggleNav.mode = Navigation.Mode.None;
        m_InvertXAxisToggleNav.mode = Navigation.Mode.Explicit;
        m_InvertXAxisToggleNav.selectOnUp = m_SensitivitySlider;
        m_InvertXAxisToggleNav.selectOnDown = m_InvertYAxisToggle;
        m_InvertXAxisToggleNav.selectOnLeft = m_ControlsButton;
        m_InvertXAxisToggleNav.selectOnRight = m_ControlsButton;
        m_InvertXAxisToggle.navigation = m_InvertXAxisToggleNav;

        m_InvertYAxisToggleNav = m_InvertYAxisToggle.navigation;
        m_InvertYAxisToggleNav.mode = Navigation.Mode.None;
        m_InvertYAxisToggleNav.mode = Navigation.Mode.Explicit;
        m_InvertYAxisToggleNav.selectOnUp = m_InvertXAxisToggle;
        m_InvertYAxisToggleNav.selectOnDown = m_ToggleSprint;
        m_InvertYAxisToggleNav.selectOnLeft = m_ControlsButton;
        m_InvertYAxisToggleNav.selectOnRight = m_ControlsButton;
        m_InvertYAxisToggle.navigation = m_InvertYAxisToggleNav;

        m_ToggleSprintNav = m_ToggleSprint.navigation;
        m_ToggleSprintNav.mode = Navigation.Mode.None;
        m_ToggleSprintNav.mode = Navigation.Mode.Explicit;
        m_ToggleSprintNav.selectOnUp = m_InvertYAxisToggle;
        m_ToggleSprintNav.selectOnDown = m_SaveSettingsButton;
        m_ToggleSprintNav.selectOnLeft = m_ControlsButton;
        m_ToggleSprintNav.selectOnRight = m_ControlsButton;
        m_ToggleSprint.navigation = m_ToggleSprintNav;

        if (GameObject.Find("BrightnessCanvas"))
        {
            m_BrightnessCanvas = GameObject.Find("BrightnessCanvas");
            m_BrightnessOverlay = m_BrightnessCanvas.transform.GetChild(0).GetComponent<Image>();
        }

        m_Resolutions = Screen.resolutions;

        m_ResolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        // Grab and store all resolutions the user can have
        int currentResolutionIndex = 0;
        for (int i = 0; i < m_Resolutions.Length; i++)
        {
            string option = m_Resolutions[i].width + "x" + m_Resolutions[i].height + " @" + m_Resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (m_Resolutions[i].width == Screen.width && m_Resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_ResolutionDropdown.AddOptions(options);

        m_ResolutionDropdown.value = currentResolutionIndex;
        m_ResolutionDropdown.RefreshShownValue();

        //if (m_Properties != null)
        //{
        //    LoadSettings(m_Properties.m_SaveName);
        //}
        //else
        //{
        //    LoadSettings("Placeholder");
        //}


        //if (m_BrightnessCanvas != null)
        //{
        //    DontDestroyOnLoad(m_BrightnessCanvas);
        //}

        if (m_CloseAfterLoad)
        {
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR

        SetMasterVolume(-20);
#endif
    }

    public void OpenAudioMenu()
    {
        m_AudioMenu.SetActive(true);
        m_DisplayMenu.SetActive(false);
        //m_OptionsMenu.SetActive(false);
        m_ControlsMenu.SetActive(false);

        m_SettingsBackButton.gameObject.SetActive(false);

        m_AudioButtonNav.selectOnUp = m_AudioBackButton;
        m_AudioButtonNav.selectOnLeft = m_MasterSlider;
        m_AudioButtonNav.selectOnRight = m_MasterSlider;
        m_AudioButton.navigation = m_AudioButtonNav;

        m_DisplayButtonNav.selectOnLeft = m_MasterSlider;
        m_DisplayButtonNav.selectOnRight = m_MasterSlider;
        m_DisplayButton.navigation = m_DisplayButtonNav;

        m_ControlsButtonNav.selectOnDown = m_AudioBackButton;
        m_ControlsButtonNav.selectOnRight = m_MasterSlider;
        m_ControlsButtonNav.selectOnLeft = m_MasterSlider;
        m_ControlsButton.navigation = m_ControlsButtonNav;

        m_SaveSettingsButtonNav.selectOnUp = m_EffectsSlider;
        m_SaveSettingsButtonNav.selectOnDown = m_MasterSlider;
        m_SaveSettingsButtonNav.selectOnLeft = m_AudioBackButton;
        m_SaveSettingsButtonNav.selectOnRight = m_AudioBackButton;
        m_SaveSettingsButton.navigation = m_SaveSettingsButtonNav;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(m_MasterSlider.gameObject);
    }

    public void OpenDisplayMenu()
    {
        m_DisplayMenu.SetActive(true);
        m_ControlsMenu.SetActive(false);
        m_AudioMenu.SetActive(false);
        //m_OptionsMenu.SetActive(false);

        m_SettingsBackButton.gameObject.SetActive(false);

        m_AudioButtonNav.selectOnUp = m_DisplayBackButton;
        m_AudioButtonNav.selectOnLeft = m_GraphicsDropdown;
        m_AudioButtonNav.selectOnRight = m_GraphicsDropdown;
        m_AudioButton.navigation = m_AudioButtonNav;

        m_DisplayButtonNav.selectOnLeft = m_GraphicsDropdown;
        m_DisplayButtonNav.selectOnRight = m_GraphicsDropdown;
        m_DisplayButton.navigation = m_DisplayButtonNav;

        m_ControlsButtonNav.selectOnDown = m_DisplayBackButton;
        m_ControlsButtonNav.selectOnRight = m_GraphicsDropdown;
        m_ControlsButtonNav.selectOnLeft = m_GraphicsDropdown;
        m_ControlsButton.navigation = m_ControlsButtonNav;

        m_SaveSettingsButtonNav.selectOnUp = m_BrightnessSlider;
        m_SaveSettingsButtonNav.selectOnDown = m_GraphicsDropdown;
        m_SaveSettingsButtonNav.selectOnLeft = m_DisplayBackButton;
        m_SaveSettingsButtonNav.selectOnRight = m_DisplayBackButton;
        m_SaveSettingsButton.navigation = m_SaveSettingsButtonNav;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(m_GraphicsDropdown.gameObject);
    }
    public void OpenControlsMenu()
    {
        m_ControlsMenu.SetActive(true);
        m_DisplayMenu.SetActive(false);
        m_AudioMenu.SetActive(false);
        //m_OptionsMenu.SetActive(false);

        m_SettingsBackButton.gameObject.SetActive(false);

        m_AudioButtonNav.selectOnUp = m_ControlsBackButton;
        m_AudioButtonNav.selectOnLeft = m_SensitivitySlider;
        m_AudioButtonNav.selectOnRight = m_SensitivitySlider;
        m_AudioButton.navigation = m_AudioButtonNav;

        m_DisplayButtonNav.selectOnLeft = m_SensitivitySlider;
        m_DisplayButtonNav.selectOnRight = m_SensitivitySlider;
        m_DisplayButton.navigation = m_DisplayButtonNav;

        m_ControlsButtonNav.selectOnDown = m_ControlsBackButton;
        m_ControlsButtonNav.selectOnRight = m_SensitivitySlider;
        m_ControlsButtonNav.selectOnLeft = m_SensitivitySlider;
        m_ControlsButton.navigation = m_ControlsButtonNav;

        m_SaveSettingsButtonNav.selectOnUp = m_ToggleSprint;
        m_SaveSettingsButtonNav.selectOnDown = m_SensitivitySlider;
        m_SaveSettingsButtonNav.selectOnLeft = m_ControlsBackButton;
        m_SaveSettingsButtonNav.selectOnRight = m_ControlsBackButton;
        m_SaveSettingsButton.navigation = m_SaveSettingsButtonNav;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(m_SensitivitySlider.gameObject);
    }

    public void CloseAudioMenu()
    {
        m_ControlsMenu.SetActive(false);
        m_AudioMenu.SetActive(false);
        //m_OptionsMenu.SetActive(true);

        m_SettingsBackButton.gameObject.SetActive(true);

        if (m_Mainmenu != null)
        {
            m_Mainmenu.EnableMainMenu();
        }
        else if (PlayerManager.Instance.Player != null)
        {
            PlayerManager.Instance.Player.PlayerUI.CloseSettingsMenu();
        }
    }
    public void CloseDisplayMenu()
    {
        m_ControlsMenu.SetActive(false);
        m_DisplayMenu.SetActive(false);
        //m_OptionsMenu.SetActive(true);

        m_SettingsBackButton.gameObject.SetActive(true);

        if (m_Mainmenu != null)
        {
            m_Mainmenu.EnableMainMenu();
        }
        else if (PlayerManager.Instance.Player != null)
        {
            PlayerManager.Instance.Player.PlayerUI.CloseSettingsMenu();
        }
    }
    public void CloseControlsMenu()
    {
        m_ControlsMenu.SetActive(false);
        m_DisplayMenu.SetActive(false);
        //m_OptionsMenu.SetActive(true);

        m_SettingsBackButton.gameObject.SetActive(true);

        if (m_Mainmenu != null)
        {
            m_Mainmenu.EnableMainMenu();
        }
        else if (PlayerManager.Instance.Player != null)
        {
            PlayerManager.Instance.Player.PlayerUI.CloseSettingsMenu();
        }
    }

    public void SetMasterVolume(float value)
    {
        m_AudioMixer.SetFloat("MasterVolume", value);

        m_MasterVolumeValue.text = value.ToString();
    }

    public void SetMusicVolume(float value)
    {
        m_AudioMixer.SetFloat("MusicVolume", value);
        m_MusicVolumeValue.text = value.ToString();
    }

    public void SetEffectsVolume(float value)
    {
        m_AudioMixer.SetFloat("EffectsVolume", value);
        m_EffectsVolumeValue.text = value.ToString();
    }

    public void SetQualitySettings(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetResolution(int index)
    {
        Screen.SetResolution(m_Resolutions[index].width, m_Resolutions[index].height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetBrightness(float value)
    {
        Color color = new Color(0.0f, 0.0f, 0.0f, value); ;

        m_BrightnessOverlay.color = color;
    }

    public void SetMouseSensitivy(float value)
    {
        if (m_Properties != null)
        {
            m_Properties.m_MouseSensitivity = value * 100;
        }

        m_SensitivityValue.text = value.ToString();
    }
    public void SetInvertXAxis(bool invertXAxis)
    {
        if (m_Properties != null)
        {
            m_Properties.m_InvertXAxis = invertXAxis;
        }
    }
    public void SetInvertYAxis(bool invertYAxis)
    {
        if (m_Properties != null)
        {
            m_Properties.m_InvertYAxis = invertYAxis;
        }
    }

    public void SetToggleSprint(bool toggleSprint)
    {
        if (m_Properties != null)
        {
            m_Properties.m_ToggleSprint = toggleSprint;
        }
    }

    public void SetToggleTutorial(bool toggleTutorial)
    {
        if (m_Properties != null)
        {
            m_Properties.m_ToggleTutorial = toggleTutorial;
        }
    }

    public void SetVSync(bool isVSyncActive)
    {
        if (isVSyncActive)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void SaveSettings()
    {
        m_Properties = PlayerManager.Instance.Player.m_Properties;

        // If directory doesn't exist, create it
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/saves/" + PlayerManager.Instance.Player.SaveName + ".options";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Create settings data to store all the settings data

        SettingsData data = new SettingsData();

        // Store User Settings
        float masterVolume = 0.0f;
        float musicVolume = 0.0f;
        float effectsVolume = 0.0f;

        float resolutionWidth = 0.0f;
        float resolutionHeight = 0.0f;

        bool isFullscreen = false;

        m_AudioMixer.GetFloat("MasterVolume", out masterVolume);
        m_AudioMixer.GetFloat("MusicVolume", out musicVolume);
        m_AudioMixer.GetFloat("EffectsVolume", out effectsVolume);

        resolutionWidth = Screen.currentResolution.width;
        resolutionHeight = Screen.currentResolution.height;

        isFullscreen = Screen.fullScreen;

        data.m_MasterVolume = masterVolume;
        data.m_MusicVolume = musicVolume;
        data.m_EffectsVolume = effectsVolume;

        data.m_ResolutionWidth = resolutionWidth;
        data.m_ResolutionHeight = resolutionHeight;

        data.m_IsFullscreen = isFullscreen;

        data.m_GraphicsIndex = (int)QualitySettings.GetQualityLevel();

        data.m_BrightnessValue = m_BrightnessOverlay.color.a;

        data.m_MouseSensitivityValue = m_Properties.m_MouseSensitivity;

        data.m_InvertXAxis = m_Properties.m_InvertXAxis;
        data.m_InvertYAxis = m_Properties.m_InvertYAxis;

        data.m_ToggleSprint = m_Properties.m_ToggleSprint;
        data.m_ToggleTutorial = m_Properties.m_ToggleTutorial;

        formatter.Serialize(stream, data);
        stream.Close();

        LoadSettings(PlayerManager.Instance.Player.SaveName);
    }

    public void LoadSettings(string name)
    {
        m_Properties = PlayerManager.Instance.Player.m_Properties;

        // If there is no name, use Placeholder
        if (name == null)
        {
            name = "Placeholder";
        }

        // Load settings data at the given path
        SettingsData data = SaveSystem.LoadSettings(Application.persistentDataPath + "/saves/" + name + ".options");

        // If a file as found, set all the variables using the data, if not use default values
        if (data != null)
        {
            // Load settings from data
            SetMasterVolume(data.m_MasterVolume);
            m_MasterSlider.value = data.m_MasterVolume;
            m_MasterVolumeValue.text = data.m_MasterVolume.ToString();

            SetMusicVolume(data.m_MusicVolume);
            m_MusicSlider.value = data.m_MusicVolume;
            m_MusicVolumeValue.text = data.m_MusicVolume.ToString();

            SetEffectsVolume(data.m_EffectsVolume);
            m_EffectsSlider.value = data.m_EffectsVolume;
            m_EffectsVolumeValue.text = data.m_EffectsVolume.ToString();

            SetFullscreen(data.m_IsFullscreen);
            m_FullscreenToggle.isOn = data.m_IsFullscreen;

            Screen.SetResolution((int)data.m_ResolutionWidth, (int)data.m_ResolutionHeight, Screen.fullScreen);

            SetQualitySettings(data.m_GraphicsIndex);
            m_GraphicsDropdown.value = data.m_GraphicsIndex;

            m_BrightnessOverlay.color = new Color(0.0f, 0.0f, 0.0f, data.m_BrightnessValue);
            m_Properties.m_BrightnessValue = new Color(0.0f, 0.0f, 0.0f, data.m_BrightnessValue);
            m_BrightnessSlider.value = data.m_BrightnessValue;

            m_Properties.m_MouseSensitivity = data.m_MouseSensitivityValue;

            m_SensitivitySlider.value = data.m_MouseSensitivityValue / 100.0f;
            m_SensitivityValue.text = (data.m_MouseSensitivityValue / 100.0f).ToString();

            m_Properties.m_InvertXAxis = data.m_InvertXAxis;
            m_Properties.m_InvertYAxis = data.m_InvertYAxis;

            m_InvertXAxisToggle.isOn = data.m_InvertXAxis;
            m_InvertYAxisToggle.isOn = data.m_InvertYAxis;

            m_ToggleSprint.isOn = data.m_ToggleSprint;
            m_ToggleTutorial.isOn = data.m_ToggleTutorial;
        }
        else
        {
            // Set default settings
            SetMasterVolume(-20);
            m_MasterSlider.value = -20;
            m_MasterVolumeValue.text = m_MasterSlider.value.ToString();

            SetMusicVolume(-20);
            m_MusicSlider.value = -20;
            m_MusicVolumeValue.text = m_MusicSlider.value.ToString();

            SetEffectsVolume(-20);
            m_EffectsSlider.value = -20;
            m_EffectsVolumeValue.text = m_EffectsSlider.value.ToString();

            SetFullscreen(true);
            m_FullscreenToggle.isOn = true;

            Screen.SetResolution((int)1920, (int)1080, Screen.fullScreen);

            SetQualitySettings(1);
            m_GraphicsDropdown.value = 1;

            if (m_BrightnessOverlay != null)
            {
                m_BrightnessOverlay.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                m_BrightnessSlider.value = 0.0f;
            }

            if (m_Properties != null)
            {
                m_Properties.m_MouseSensitivity = 500;
            }

            m_SensitivitySlider.value = 500 / 100.0f;
            m_SensitivityValue.text = (500 / 100.0f).ToString();

            if (m_Properties != null)
            {
                m_Properties.m_InvertXAxis = false;
                m_Properties.m_InvertYAxis = false;
            }
            m_InvertXAxisToggle.isOn = false;
            m_InvertYAxisToggle.isOn = false;

            m_ToggleSprint.isOn = false;
            m_ToggleTutorial.isOn = true;
        }
    }
}
