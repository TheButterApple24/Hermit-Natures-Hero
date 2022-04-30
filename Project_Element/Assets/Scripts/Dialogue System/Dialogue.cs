/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeBox
Description:        Handles lines and names and prompts for dialogue system
Date Created:       26/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/01/2022
        - [Jeffrey] Created base implementation
    27/01/2022
        - [Jeffrey] Implemented multiple prompts
    25/02/2022
        - [Jeffrey] Refactored Dialogue System

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public enum DialogueEmotion
{
    Neutral = 0,
    Joy = 1,
    Fear = 2,
    Sadness = 3,
    Anger = 4,
    None = 5
}

public enum DialoguePitch
{
    Med = 0,
    Low= 1,
    High = 2
}

public class Dialogue : InteractableBase
{
    public DialogueObject m_DialogueObject;

    [HideInInspector] public bool m_IsComplete = false;
    public bool IsQuestDialogue = false;
    public Animator DialogueAnimator;

    private AudioSource m_MumbleAudioOutput;
    private GameObject m_PromptButton;

    int m_NodeIndex;
    int m_PromptIndex;
    List<GameObject> m_ButtonObjects;

    private int m_AnimationIndex = 0;

    DialogueEmotion Emotion = DialogueEmotion.None;
    DialoguePitch DialoguePitch = DialoguePitch.Med;

    bool m_IsActive = false;

    bool m_SheathStateOnStart = false;

    // Called on Start
    public override void Start()
    {
        base.Start();
        m_RemoveFromListOnTrigger = true;
    }

    /// <summary>
    /// Called when Dialogue Object is interacted with
    /// </summary>
    public override void Activate()
    {
        StartDialogue();
    }

    /// <summary>
    /// Starts the Dialogue sequence
    /// </summary>
    public void StartDialogue()
    {
        m_Player.m_HealthComp.IsInvulnerable = true;
        m_Player.m_Controller.ResetVelocity();
        if (!m_Player.IsWeaponInSheath)
        {
            m_SheathStateOnStart = true;
            m_Player.SheathWeapon(true);
        }
        else
        {
            m_SheathStateOnStart = false;
        }

        m_MumbleAudioOutput = MenuManager.Instance.MenuAudioSource;
        m_PromptButton = GameManager.Instance.DialoguePromptPrefab;

        m_IsActive = true;
        HUDManager.Instance.DialogueUIObject.SetActive(true);
        m_NodeIndex = 0;
        m_IsComplete = false;
        HUDManager.Instance.DialogueNameText.text = "";
        HUDManager.Instance.DialogueLineText.text = "";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_Player.m_IsInputDisabled = true;
        m_Player.m_FollowCamera.m_IsInputDisabled = true;
        m_Player.m_Controller.m_Inputs = Vector3.zero;

        HUDManager.Instance.DialogueNameText.text = m_DialogueObject.Nodes[m_NodeIndex].m_SpeakerName;
        HUDManager.Instance.DialogueLineText.text = m_DialogueObject.Nodes[m_NodeIndex].m_Line;
        Emotion = (DialogueEmotion)m_DialogueObject.Nodes[m_NodeIndex].EmotionIndex;
        DialoguePitch = (DialoguePitch)m_DialogueObject.Nodes[m_NodeIndex].PitchIndex;

        m_ButtonObjects = new List<GameObject>();
        SetupPromptButtons();
        PlayMumbleSounds();
        if (DialogueAnimator != null)
        {
            PlayAnimations();
        }
    }

    /// <summary>
    /// Plays a random mumble sound based on Pitch index
    /// </summary>
    void PlayMumbleSounds()
    {
        m_MumbleAudioOutput.Stop();
        if (Emotion != DialogueEmotion.None)
        {
            switch (Emotion)
            {
                case DialogueEmotion.Neutral:
                    switch (DialoguePitch)
                    {
                        case DialoguePitch.Med:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.NeutralMedMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.NeutralMedMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.Low:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.NeutralLowMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.NeutralLowMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.High:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.NeutralHighMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.NeutralHighMumbleSounds.Length)]);
                            break;
                        default:
                            break;
                    }
                    break;
                case DialogueEmotion.Joy:
                    switch (DialoguePitch)
                    {
                        case DialoguePitch.Med:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.JoyMedMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.JoyMedMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.Low:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.JoyLowMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.JoyLowMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.High:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.JoyHighMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.JoyHighMumbleSounds.Length)]);
                            break;
                        default:
                            break;
                    }
                    break;
                case DialogueEmotion.Fear:
                    switch (DialoguePitch)
                    {
                        case DialoguePitch.Med:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.FearMedMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.FearMedMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.Low:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.FearLowMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.FearLowMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.High:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.FearHighMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.FearHighMumbleSounds.Length)]);
                            break;
                        default:
                            break;
                    }
                    break;
                case DialogueEmotion.Sadness:
                    switch (DialoguePitch)
                    {
                        case DialoguePitch.Med:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.SadMedMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.SadMedMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.Low:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.SadLowMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.SadLowMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.High:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.SadHighMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.SadHighMumbleSounds.Length)]);
                            break;
                        default:
                            break;
                    }
                    break;
                case DialogueEmotion.Anger:
                    switch (DialoguePitch)
                    {
                        case DialoguePitch.Med:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.AngerMedMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.AngerMedMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.Low:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.AngerLowMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.AngerLowMumbleSounds.Length)]);
                            break;
                        case DialoguePitch.High:
                            m_MumbleAudioOutput.PlayOneShot(MumbleSoundManager.Instance.AngerHighMumbleSounds[Random.Range(0, MumbleSoundManager.Instance.AngerHighMumbleSounds.Length)]);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Plays dialogue animations
    /// </summary>
    void PlayAnimations()
    {
        ResetAnimations();
        m_AnimationIndex = Random.Range(0, 5);
        DialogueAnimator.SetInteger("TalkIndex", m_AnimationIndex);
        DialogueAnimator.SetBool("Talking", true);
        DialogueAnimator.Play("Talk", 1);
    }

    /// <summary>
    /// Resets the dialogue animations
    /// </summary>
    void ResetAnimations()
    {
        DialogueAnimator.SetBool("Talking", false);
        DialogueAnimator.SetInteger("TalkIndex", 0);
        DialogueAnimator.Play("Idle", 0);
    }

    // Called every frame
    public override void Update()
    {
        base.Update();
        if (m_IsActive && m_NodeIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_DialogueObject.Nodes[m_NodeIndex].m_HasPrompts == false
             || Input.GetMouseButtonDown(0) && m_DialogueObject.Nodes[m_NodeIndex].m_HasPrompts == false)
            {
                SwitchNode();
            }

            if (DialogueAnimator != null)
            {
                if (DialogueAnimator.GetCurrentAnimatorStateInfo(1).length > DialogueAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime)
                {
                    ResetAnimations();
                }
            }
        }
    }

    /// <summary>
    /// Switches to the next node in the list
    /// </summary>
    void SwitchNode()
    {
        DialogueNode currentNode = m_DialogueObject.Nodes[m_NodeIndex];
        string desiredInID = "";

        for (int i = 0; i < m_DialogueObject.Connections.Length; i++)
        {
            if (!currentNode.m_HasPrompts)
            {
                if (currentNode.m_OutID == m_DialogueObject.Connections[i].m_OutID)
                {
                    desiredInID = m_DialogueObject.Connections[i].m_InID;
                }
            }
            else
            {
                DialoguePrompt selectedPrompt = m_DialogueObject.Prompts[m_PromptIndex];

                if (selectedPrompt.m_OutID == m_DialogueObject.Connections[i].m_OutID)
                {
                    desiredInID = m_DialogueObject.Connections[i].m_InID;
                }

            }
        }

        m_NodeIndex = -1;
        for (int i = 0; i < m_DialogueObject.Nodes.Length; i++)
        {
            if (desiredInID == m_DialogueObject.Nodes[i].m_InID)
            {
                m_NodeIndex = i;
            }
        }

        if (m_NodeIndex != -1)
        {
            HUDManager.Instance.DialogueNameText.text = m_DialogueObject.Nodes[m_NodeIndex].m_SpeakerName;
            HUDManager.Instance.DialogueLineText.text = m_DialogueObject.Nodes[m_NodeIndex].m_Line;
            Emotion = (DialogueEmotion)m_DialogueObject.Nodes[m_NodeIndex].EmotionIndex;
            DialoguePitch = (DialoguePitch)m_DialogueObject.Nodes[m_NodeIndex].PitchIndex;

            if (m_DialogueObject.Nodes[m_NodeIndex].m_HasPrompts == false)
            {
                m_PromptButton.SetActive(false);
                m_PromptButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                m_PromptButton.SetActive(true);
                m_PromptButton.transform.GetChild(0).gameObject.SetActive(true);
            }

            if (m_DialogueObject.Nodes[m_NodeIndex].m_HasPrompts)
            {
                SetupPromptButtons();
            }

            if (DialogueAnimator != null)
            {
                PlayAnimations();
            }

            PlayMumbleSounds();
        }
        else
        {
            if (m_SheathStateOnStart)
            {
                m_Player.SheathWeapon(false);
            }

            if (DialogueAnimator != null)
            {
                ResetAnimations();
            }

            m_IsComplete = true;
            m_IsActive = false;
            m_Player.m_IsInputDisabled = false;
            m_Player.m_FollowCamera.m_IsInputDisabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            m_Player.m_HealthComp.IsInvulnerable = false;
            HUDManager.Instance.DialogueUIObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when Prompt is clicked
    /// </summary>
    /// <param name="index"></param>
    public void PromptClick(int index)
    {
        for (int i = 0; i < m_ButtonObjects.Count; i++)
        {
            Destroy(m_ButtonObjects[i]);
        }
        m_PromptIndex = index;
        SwitchNode();
    }

    /// <summary>
    /// Setups up prompts if needed
    /// </summary>
    void SetupPromptButtons()
    {
        if (m_DialogueObject.Nodes[m_NodeIndex].m_HasPrompts)
        {
            int promptStartNum = m_DialogueObject.Nodes[m_NodeIndex].m_PromptStartNumber;

            for (int i = 0; i < m_DialogueObject.Nodes[m_NodeIndex].m_PromptNumber; i++)
            {
                GameObject goButton = (GameObject)Instantiate(m_PromptButton);

                goButton.transform.SetParent(HUDManager.Instance.DialogueButtonPanel, false);

                goButton.transform.localScale = new Vector3(1, 1, 1);

                goButton.transform.GetChild(0).GetComponent<Text>().text = m_DialogueObject.Prompts[promptStartNum].m_Line;

                Button tempButton = goButton.GetComponent<Button>();

                int tempInt = promptStartNum;

                tempButton.onClick.AddListener(() => PromptClick(tempInt));

                goButton.SetActive(true);
                goButton.transform.GetChild(0).gameObject.SetActive(true);
                m_ButtonObjects.Add(goButton);
                promptStartNum++;
            }
        }
    }
}

[CreateAssetMenu(fileName = "DialogueObject", menuName = "Project Element/Dialogue Object")]
public class DialogueObject : ScriptableObject
{
    public DialogueNode[] Nodes;
    public DialogueConnection[] Connections;
    public DialoguePrompt[] Prompts;
}