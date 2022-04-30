/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeEditorNode
Description:        Handles Skill Tree Nodes
Date Created:       10/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class SkillTreeEditorNode
{
    public Rect m_Rect;
    [XmlIgnore] public string m_Title;
    [XmlIgnore] public bool m_IsDragged;
    [XmlIgnore] public bool m_IsSelected;
    public bool m_HasBranches;

    public SkillTreeEditorConnectionPoint m_InPoint;
    public SkillTreeEditorConnectionPoint m_OutPoint;

    [XmlIgnore] public GUIStyle m_Style;
    [XmlIgnore] public GUIStyle m_DefaultNodeStyle;
    [XmlIgnore] public GUIStyle m_SelectedNodeStyle;

    [XmlIgnore] private GUIStyle m_NodeContentStyle;
    [XmlIgnore] private GUIStyle m_BGStyle;
    [XmlIgnore] public GUIStyle m_OutPointStyle;

    [XmlIgnore] public Action<SkillTreeEditorNode> OnRemoveNode;

    [XmlIgnore] public string m_BoxTitle = "";
    public string PointCost = "Cost";
    public string SkillProperty = "Property";
    public string SkillAmount = "Amount";

    [XmlIgnore] GUIContent m_TitleContent;

    [XmlIgnore] public List<SkillTreeEditorBranchNode> m_Branches;

    [XmlIgnore] public Action<SkillTreeEditorConnectionPoint> OnClickConnectionPoint;

    public SkillTreeEditorNode() { }

    public SkillTreeEditorNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<SkillTreeEditorConnectionPoint> OnClickInPoint,
        Action<SkillTreeEditorConnectionPoint> OnClickOutPoint, Action<SkillTreeEditorNode> OnClickRemoveNode, string inPointID, string outPointID)
    {
        m_Rect = new Rect(position.x, position.y, width, height);
        m_Style = nodeStyle;
        m_Style.alignment = TextAnchor.MiddleLeft;

        m_InPoint = new SkillTreeEditorConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);
        m_OutPoint = new SkillTreeEditorConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, outPointID);

        OnClickConnectionPoint = OnClickOutPoint;

        m_OutPointStyle = outPointStyle;

        m_DefaultNodeStyle = nodeStyle;
        m_DefaultNodeStyle.alignment = TextAnchor.MiddleLeft;

        m_SelectedNodeStyle = selectedStyle;
        m_SelectedNodeStyle.alignment = TextAnchor.MiddleLeft;

        this.OnRemoveNode = OnClickRemoveNode;

        m_TitleContent = new GUIContent(m_BoxTitle);

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

        m_Branches = new List<SkillTreeEditorBranchNode>();
    }

    public void Drag(Vector2 delta)
    {
        m_Rect.position += delta;
    }

    public void Draw()
    {
        m_InPoint.Draw(m_Rect);
        if (!m_HasBranches)
        {
            if (m_Branches.Count > 0)
            {
                for (int i = 0; i < m_Branches.Count; i++)
                {
                    OnClickRemoveBranch(m_Branches[i]);
                }
            }
            m_OutPoint.Draw(m_Rect);
        }

        EditorGUI.indentLevel = 1;

        if (m_HasBranches)
        {
            Rect rectButton = new Rect();
            Vector2 posButton = m_Rect.position;
            posButton.x += m_Rect.width - 10;
            posButton.y += 6;
            rectButton.position = posButton;
            rectButton.width = m_Rect.height;
            rectButton.height = m_Rect.height + 15;

            if (GUI.Button(rectButton, "Add \nBranch", m_OutPointStyle))
                CreateBranch();
        }

        GUI.Box(m_Rect, "", m_BGStyle);
        GUI.Box(m_Rect, m_TitleContent, m_Style);

        EditorGUIUtility.labelWidth = 100;
        EditorStyles.textField.wordWrap = true;

        Rect rectToggle = new Rect();
        Vector2 posToggle = m_Rect.position;
        posToggle.x += m_Rect.width / 4;
        posToggle.y += 15;
        rectToggle.position = posToggle;
        rectToggle.width = m_Rect.height;
        rectToggle.height = 20;
        m_HasBranches = EditorGUI.Toggle(rectToggle, "Has Branches: ", m_HasBranches);

        Rect rect = new Rect();
        Vector2 pos = m_Rect.position;
        pos.y += m_Rect.height;
        rect.position = pos;
        rect.width = m_Rect.width - 12;
        rect.height = m_Rect.height - 12;

        EditorGUIUtility.labelWidth = 60;
        PointCost = EditorGUI.TextField(rect, "Cost: ", PointCost);

        rect = new Rect();
        pos = m_Rect.position;
        pos.y += m_Rect.height * 2;
        rect.position = pos;
        rect.width = m_Rect.width - 12;
        rect.height = m_Rect.height - 12;

        EditorGUIUtility.labelWidth = 80;
        SkillProperty = EditorGUI.TextField(rect, "Property: ", SkillProperty);

        rect = new Rect();
        pos = m_Rect.position;
        pos.y += m_Rect.height * 3;
        rect.position = pos;
        rect.width = m_Rect.width - 12;
        rect.height = m_Rect.height - 12;

        EditorGUIUtility.labelWidth = 80;
        SkillAmount = EditorGUI.TextField(rect, "Amount: ", SkillAmount);

        for (int i = 0; i < m_Branches.Count; i++)
        {
            m_Branches[i].Draw();
        }
    }

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

    void CreateBranch()
    {
        SkillTreeEditorBranchNode prompt = new SkillTreeEditorBranchNode(this, m_Rect, m_OutPointStyle, OnClickConnectionPoint, m_Branches.Count, OnClickRemoveBranch);
        m_Branches.Add(prompt);
    }

    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    public void OnClickRemoveBranch(SkillTreeEditorBranchNode branch)
    {
        m_Branches.Remove(branch);
    }

    void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    public void ProcessBranchEvents(Event e)
    {
        if (m_Branches != null)
        {
            for (int i = 0; i < m_Branches.Count; i++)
            {
                bool guiChanged = m_Branches[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }
}
