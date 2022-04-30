/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeEditorNode
Description:        Handles the node. This is where the user will put the data (lines and names and such)
Date Created:       25/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class DialogueEditorNode
{
    public Rect m_Rect;
    [XmlIgnore] public string m_Title;
    [XmlIgnore] public bool m_IsDragged;
    [XmlIgnore] public bool m_IsSelected;
    public bool m_HasPrompts;

    public DialogueEditorConnectionPoint m_InPoint;
    public DialogueEditorConnectionPoint m_OutPoint;

    [XmlIgnore] public GUIStyle m_Style;
    [XmlIgnore] public GUIStyle m_DefaultNodeStyle;
    [XmlIgnore] public GUIStyle m_SelectedNodeStyle;

    [XmlIgnore] private GUIStyle m_NodeContentStyle;
    [XmlIgnore] private GUIStyle m_BGStyle;
    [XmlIgnore] public GUIStyle m_OutPointStyle;

    [XmlIgnore] public Action<DialogueEditorNode> OnRemoveNode;

    [XmlIgnore] public string m_BoxTitle = "";
    public string m_Name = "Name";
    public string m_Line = "This is a line of Dialogue";

    public int Emotion;

    [XmlIgnore] GUIContent m_TitleContent;
    [XmlIgnore] public GUIContent EmotionContent;

    [XmlIgnore] public List<DialogueEditorPrompt> m_Prompts;

    [XmlIgnore] public Action<DialogueEditorConnectionPoint> OnClickConnectionPoint;

    public DialogueEditorNode() { }

    /// <summary>
    /// Node constructor
    /// </summary>
    public DialogueEditorNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<DialogueEditorConnectionPoint> OnClickInPoint,
        Action<DialogueEditorConnectionPoint> OnClickOutPoint, Action<DialogueEditorNode> OnClickRemoveNode, string inPointID, string outPointID)
    {
        m_Rect = new Rect(position.x, position.y, width, height);
        m_Style = nodeStyle;
        m_Style.alignment = TextAnchor.MiddleLeft;

        m_InPoint = new DialogueEditorConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);
        m_OutPoint = new DialogueEditorConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, outPointID);

        OnClickConnectionPoint = OnClickOutPoint;

        m_OutPointStyle = outPointStyle;

        m_DefaultNodeStyle = nodeStyle;
        m_DefaultNodeStyle.alignment = TextAnchor.MiddleLeft;

        m_SelectedNodeStyle = selectedStyle;
        m_SelectedNodeStyle.alignment = TextAnchor.MiddleLeft;

        this.OnRemoveNode = OnClickRemoveNode;

        m_TitleContent = new GUIContent(m_BoxTitle);
        EmotionContent = new GUIContent("Neutral");

        m_NodeContentStyle = new GUIStyle();
        m_NodeContentStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_NodeContentStyle.border = new RectOffset(12, 12, 12, 12);
        m_NodeContentStyle.alignment = TextAnchor.MiddleCenter;

        m_BGStyle = new GUIStyle();
        m_BGStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        m_BGStyle.border = new RectOffset(12, 12, 12, 12);
        m_BGStyle.alignment = TextAnchor.MiddleCenter;
        m_BGStyle.fixedHeight = 200;
        m_BGStyle.fixedWidth = 200;

        m_Prompts = new List<DialogueEditorPrompt>();
    }

    /// <summary>
    /// Called when node is being dragged
    /// </summary>
    /// <param name="delta">The mouse moving position at time of drag (direction)</param>
    public void Drag(Vector2 delta)
    {
        m_Rect.position += delta;
    }

    /// <summary>
    /// Draws all GUI elements for Nodes
    /// </summary>
    public void Draw()
    {
        m_InPoint.Draw(m_Rect);
        if (!m_HasPrompts)
        {
            if (m_Prompts.Count > 0)
            {
                for (int i = 0; i < m_Prompts.Count; i++)
                {
                    OnClickRemovePrompt(m_Prompts[i]);
                }
            }
            m_OutPoint.Draw(m_Rect);
        }

        EditorGUI.indentLevel = 1;

        if (m_HasPrompts)
        {
            Rect rectButton = new Rect();
            Vector2 posButton = m_Rect.position;
            posButton.x += m_Rect.width - 10;
            posButton.y += 6;
            rectButton.position = posButton;
            rectButton.width = m_Rect.height;
            rectButton.height = m_Rect.height + 15;

            if (GUI.Button(rectButton, "Add \nPrompt", m_OutPointStyle))
                CreatePrompt();
        }

        GUI.Box(m_Rect, "", m_BGStyle);
        GUI.Box(m_Rect, m_TitleContent, m_Style);

        EditorGUIUtility.labelWidth = 90;
        EditorStyles.textField.wordWrap = true;

        Rect rectToggle = new Rect();
        Vector2 posToggle = m_Rect.position;
        posToggle.x += m_Rect.width / 3;
        posToggle.y += 15;
        rectToggle.position = posToggle;
        rectToggle.width = m_Rect.height;
        rectToggle.height = 20;
        m_HasPrompts = EditorGUI.Toggle(rectToggle, "Has Prompt: ", m_HasPrompts);

        Rect rect = new Rect();
        Vector2 pos = m_Rect.position;
        pos.y += m_Rect.height;
        rect.position = pos;
        rect.width = m_Rect.width - 12;
        rect.height = m_Rect.height - 12;

        EditorGUIUtility.labelWidth = 60;
        m_Name = EditorGUI.TextField(rect, "Name: ", m_Name);

        rect = new Rect();
        pos = m_Rect.position;
        pos.y += m_Rect.height * 2;
        rect.position = pos;
        rect.width = m_Rect.width - 12;
        rect.height = m_Rect.height - 12;

        EditorGUIUtility.labelWidth = 60;
        m_Line = EditorGUI.TextField(rect, "Line: ", m_Line);

        Rect rectEmotionText = new Rect();
        Vector2 posEmotionText = m_Rect.position;
        posEmotionText.y += m_Rect.height * 2.5f;
        rectEmotionText.position = posEmotionText;
        rectEmotionText.width = m_Rect.height + 20;
        rectEmotionText.height = m_Rect.height + 15;
        EditorGUI.LabelField(rectEmotionText, "Emotion: ");

        Rect rectEmotionButton = new Rect();
        Vector2 posEmotionButton = m_Rect.position;
        posEmotionButton.x += 75;
        posEmotionButton.y += m_Rect.height * 3;
        rectEmotionButton.position = posEmotionButton;
        rectEmotionButton.width = m_Rect.height + 20;
        rectEmotionButton.height = m_Rect.height + 15;

        if (EditorGUI.DropdownButton(rectEmotionButton, EmotionContent, FocusType.Passive))
        {
            var genericMenu = new GenericMenu();

            string[] EmotionTypeNames = System.Enum.GetNames(typeof(DialogueEmotion));

            foreach (var option in EmotionTypeNames)
            {
                bool selected = false;
                genericMenu.AddItem(new GUIContent(option), selected, () =>
                {
                    EmotionContent.text = option;
                    DialogueEmotion emotion = (DialogueEmotion)System.Enum.Parse(typeof(DialogueEmotion), option);
                    Emotion = (int)emotion;
                });
            }
            genericMenu.DropDown(m_Rect);
        }

        for (int i = 0; i < m_Prompts.Count; i++)
        {
            m_Prompts[i].Draw();
        }
    }

    /// <summary>
    /// Processes all events for Nodes
    /// </summary>
    /// <param name="e">Event being checked</param>
    /// <returns></returns>
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (m_Rect.Contains(e.mousePosition))
                    {
                        m_IsDragged = true;
                        GUI.changed = true;
                        m_IsSelected = true;
                        m_Style = m_SelectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        m_IsSelected = false;
                        m_Style = m_DefaultNodeStyle;
                    }
                }

                if (e.button == 1 && m_IsSelected && m_Rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                m_IsDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && m_IsDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }

                break;
        }

        return false;
    }

    /// <summary>
    /// Called when Prompt is selected
    /// </summary>
    void CreatePrompt()
    {
        DialogueEditorPrompt prompt = new DialogueEditorPrompt(this, m_Rect, m_OutPointStyle, OnClickConnectionPoint, m_Prompts.Count, OnClickRemovePrompt);
        m_Prompts.Add(prompt);
    }

    /// <summary>
    /// Called when Node is right-clicked
    /// </summary>
    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Called when remove prompt is called. Will remove the prompt
    /// </summary>
    /// <param name="prompt">Selected prompt</param>
    public void OnClickRemovePrompt(DialogueEditorPrompt prompt)
    {
        m_Prompts.Remove(prompt);
    }

    /// <summary>
    /// Called when RemoveNode is selected. Will delete the current node
    /// </summary>
    void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    /// <summary>
    /// Process all events for the node
    /// </summary>
    /// <param name="e">Event being checked</param>
    public void ProcessPromptEvents(Event e)
    {
        if (m_Prompts != null)
        {
            for (int i = 0; i < m_Prompts.Count; i++)
            {
                bool guiChanged = m_Prompts[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }
}
