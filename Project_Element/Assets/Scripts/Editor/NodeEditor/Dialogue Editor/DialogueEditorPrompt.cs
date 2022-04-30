/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeEditorPrompt
Description:        Handles prompt data for the node
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
using UnityEditor;
using System;
using System.Xml.Serialization;

public class DialogueEditorPrompt
{
    [XmlIgnore] public DialogueEditorNode m_ParentNode;
    [XmlIgnore] public Rect m_Rect;
    [XmlIgnore] public Rect m_UpdatedRect;
    [XmlIgnore] public GUIStyle m_Style;
    public DialogueEditorConnectionPoint m_OutPoint;
    public int m_PromptIndex = 0;
    public string m_PromptText;

    [XmlIgnore] bool m_IsSelected;

    [XmlIgnore] public Action<DialogueEditorPrompt> OnRemovePrompt;

    public DialogueEditorPrompt() { }

    /// <summary>
    /// Initializes the Editor Prompt
    /// </summary>
    /// <param name="node">Parent Node</param>
    /// <param name="rect">Parent Rect</param>
    /// <param name="outPointStyle">Style for the out connection</param>
    /// <param name="OnClickOutPoint">Method used when connection is pressed</param>
    /// <param name="index">Current index of prompt</param>
    /// <param name="OnClickRemovePrompt">Method for when Remove Prompt is selected</param>
    public DialogueEditorPrompt(DialogueEditorNode node, Rect rect, GUIStyle outPointStyle, Action<DialogueEditorConnectionPoint> OnClickOutPoint, int index, Action<DialogueEditorPrompt> OnClickRemovePrompt)
    {
        m_ParentNode = node;
        m_Rect = rect;
        m_OutPoint = new DialogueEditorConnectionPoint(node, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        m_Style = new GUIStyle();
        m_Style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_Style.border = new RectOffset(12, 12, 12, 12);
        m_Style.alignment = TextAnchor.MiddleCenter;

        m_PromptIndex = index;

        this.OnRemovePrompt = OnClickRemovePrompt;
    }

    /// <summary>
    /// Draws the GUI for the Prompt
    /// </summary>
    public void Draw()
    {
        Rect rect = new Rect();
        Vector2 pos = m_ParentNode.m_Rect.position;
        pos.y += (m_Rect.height * (4 + m_PromptIndex)) - 15;
        rect.position = pos;
        rect.width = m_ParentNode.m_Rect.width;
        rect.height = m_ParentNode.m_Rect.height + 15;
        m_UpdatedRect = rect;
        m_OutPoint.Draw(rect);

        GUI.Box(rect, "", m_Style);
        pos.y += 10;
        rect.position = pos;
        rect.width = m_ParentNode.m_Rect.width - 12;
        rect.height = m_ParentNode.m_Rect.height - 12;
        EditorGUIUtility.labelWidth = 60;
        m_PromptText = EditorGUI.TextField(rect, "Prompt: ", m_PromptText);
    }

    /// <summary>
    /// Process all keyboard events for prompts
    /// </summary>
    /// <param name="e">Event being passed in</param>
    /// <returns></returns>
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (m_UpdatedRect.Contains(e.mousePosition))
                    {
                        GUI.changed = true;
                        m_IsSelected = true;
                    }
                    else
                    {
                        GUI.changed = true;
                        m_IsSelected = false;
                    }
                }
                if (e.button == 1 && m_IsSelected && m_UpdatedRect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
        }

        return false;
    }

    /// <summary>
    /// Called when prompt is right-clicked
    /// </summary>
    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Prompt"), false, OnClickRemovePrompt);
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Called when Prompt is removed
    /// </summary>
    void OnClickRemovePrompt()
    {
        if (OnRemovePrompt != null)
        {
            OnRemovePrompt(this);
        }
    }
}
