/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              HUDManager
Description:        Stores all the HUD objects in the game
Date Created:       03/03/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/03/2022
        - [Jeffrey] Created base implementation
    13/03/2022
        - [Aaron] Removed the Main Ability Icon as it is now set using the primary or secondary icon
    17/03/2022
        - [Aaron] Reformatted the HUD after merging and edited the layout. Update variables to match the new layout.
    18/03/2022
        - [Aaron] Reformatted the HUD after merging and edited the layout again. Touched up the anchor points on all HUD elements
    01/04/2022
        - [Max] Added Activated Pop Up GameObject + Text

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager m_Instance = null;
    public static HUDManager Instance { get { return m_Instance; } }

    [SerializeField]
    private GameObject m_PetSwapIcon;
    public GameObject PetSwapIcon { get { return m_PetSwapIcon; } }

    [SerializeField]
    private GameObject m_PrimaryPetIconOutline;
    public GameObject PrimaryPetIconOutline { get { return m_PrimaryPetIconOutline; } }

    [SerializeField]
    private GameObject m_PrimaryPetIconFill;
    public GameObject PrimaryPetIconFill { get { return m_PrimaryPetIconFill; } }

    [SerializeField]
    private GameObject m_MainAbilityBorder;
    public GameObject MainAbilityBorder { get { return m_MainAbilityBorder; } }

    [SerializeField]
    private GameObject m_UltimateAbilityIconOutline;
    public GameObject UltimateAbilityIconOutline { get { return m_UltimateAbilityIconOutline; } }

    [SerializeField]
    private GameObject m_UltimateAbilityIconFill;
    public GameObject UltimateAbilityIconFill { get { return m_UltimateAbilityIconFill; } }

    [SerializeField]
    private GameObject m_UltimateAbilityBorder;
    public GameObject UltimateAbilityBorder { get { return m_UltimateAbilityBorder; } }

    [SerializeField]
    private Image[] m_PotionTimerIcons;
    public Image[] PotionTimerIcons { get { return m_PotionTimerIcons; } }

    [SerializeField]
    private PotionHotbar m_PotionHotbarObject;
    public PotionHotbar PotionHotbarObject { get { return m_PotionHotbarObject; } }

    [SerializeField]
    private Text m_DialogueNameText;
    public Text DialogueNameText { get { return m_DialogueNameText; } }

    [SerializeField]
    private Text m_DialogueLineText;
    public Text DialogueLineText { get { return m_DialogueLineText; } }

    [SerializeField]
    private GameObject m_DialogueUIObject;
    public GameObject DialogueUIObject { get { return m_DialogueUIObject; } }

    [SerializeField]
    private RectTransform m_DialogueButtonPanel;
    public RectTransform DialogueButtonPanel { get { return m_DialogueButtonPanel; } }
	
    [SerializeField]
    private Text m_QuestTipText;
    public Text QuestTipText { get { return m_QuestTipText; } }

    [SerializeField]
    private Text m_QuestTitleText;
    public Text QuestTitleText { get { return m_QuestTitleText; } }

    [SerializeField]
    private Image m_QuestTipBG;
    public Image QuestTipBG { get { return m_QuestTipBG; } }

    [SerializeField]
    private GameObject m_UnlockedPopUp;
    public GameObject UnlockedPopUp { get { return m_UnlockedPopUp; } }

    [SerializeField]
    private Text m_UnlockedPopUpText;
    public Text UnlockedPopUpText { get { return m_UnlockedPopUpText; } }

    [SerializeField]
    private ParticleSystem m_InteractionParticles;
    public ParticleSystem InteractionParticles { get { return m_InteractionParticles; } }

    [SerializeField]
    private Color m_CommonRarityColor;
    public Color CommonRarityColor { get { return m_CommonRarityColor; } }

    [SerializeField]
    private Color m_RareRarityColor;
    public Color RareRarityColor { get { return m_RareRarityColor; } }

    [SerializeField]
    private Color m_HeroicRarityColor;
    public Color HeroicRarityColor { get { return m_HeroicRarityColor; } }

    [SerializeField]
    private Color m_MythicRarityColor;
    public Color MythicRarityColor { get { return m_MythicRarityColor; } }

    [SerializeField]
    private Gradient m_GodlikeColorGradient;
    public Gradient GodlikeColorGradient { get { return m_GodlikeColorGradient; } }

    private float m_PopUpTimer = 0.0f;
    float m_PopUpDuration = 1.75f;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

    private void Update()
    {
        if (m_UnlockedPopUp.activeSelf)
        {
            m_PopUpTimer += Time.deltaTime;

            if (m_PopUpTimer > m_PopUpDuration)
            {
                m_UnlockedPopUp.SetActive(false);
                m_PopUpTimer = 0.0f;
            }
        }
    }
}
