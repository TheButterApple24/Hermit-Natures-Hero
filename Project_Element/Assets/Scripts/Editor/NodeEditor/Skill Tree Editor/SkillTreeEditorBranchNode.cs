/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeBranchNode
Description:        Handles different branch nodes for Skill Tree
Date Created:       10/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml.Serialization;

public class SkillTreeEditorBranchNode
{
    [XmlIgnore] public SkillTreeEditorNode m_ParentNode;
    [XmlIgnore] public Rect m_Rect;
    [XmlIgnore] public Rect m_UpdatedRect;
    [XmlIgnore] public GUIStyle m_Style;
    public SkillTreeEditorConnectionPoint m_OutPoint;
    public int m_PromptIndex = 0;
    public string m_PromptText;

    [XmlIgnore] bool m_IsSelected;

    [XmlIgnore] public Action<SkillTreeEditorBranchNode> OnRemovePrompt;

    public SkillTreeEditorBranchNode() { }

    public SkillTreeEditorBranchNode(SkillTreeEditorNode node, Rect rect, GUIStyle outPointStyle, Action<SkillTreeEditorConnectionPoint> OnClickOutPoint, int index, Action<SkillTreeEditorBranchNode> OnClickRemovePrompt)
    {
        m_ParentNode = node;
        m_Rect = rect;
        m_OutPoint = new SkillTreeEditorConnectionPoint(node, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        m_Style = new GUIStyle();
        m_Style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_Style.border = new RectOffset(12, 12, 12, 12);
        m_Style.alignment = TextAnchor.MiddleCenter;

        m_PromptIndex = index;

        this.OnRemovePrompt = OnClickRemovePrompt;
    }

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
    }

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

    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Prompt"), false, OnClickRemovePrompt);
        genericMenu.ShowAsContext();
    }

    void OnClickRemovePrompt()
    {
        if (OnRemovePrompt != null)
        {
            OnRemovePrompt(this);
        }
    }
}
