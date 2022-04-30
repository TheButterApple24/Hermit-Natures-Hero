/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeObject
Description:        Handles different structures for storing dialogue objects
Date Created:       25/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueNode
{
    public string m_SpeakerName;
    public string m_Line;
    public string m_InID;
    public string m_OutID;
    public bool m_HasPrompts;
    public int m_PromptNumber;
    public int m_PromptStartNumber;
    public int EmotionIndex;
    public int PitchIndex;
}

[System.Serializable]
public struct DialoguePrompt
{
    public int m_PromptIndex;
    public string m_Line;
    public string m_OutID;
}

[System.Serializable]
public struct DialogueConnection
{
    public string m_InID;
    public string m_OutID;
}
