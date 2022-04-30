/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeEditorConnectionPoint
Description:        Handles Skill Tree Connection Points
Date Created:       10/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Xml.Serialization;
using UnityEngine;

public class SkillTreeEditorConnectionPoint
{
    public string m_ID;
    [XmlIgnore] public Rect m_Rect;
    [XmlIgnore] public ConnectionPointType m_Type;
    [XmlIgnore] public SkillTreeEditorNode m_Node;
    [XmlIgnore] public GUIStyle m_Style;
    [XmlIgnore] public Action<SkillTreeEditorConnectionPoint> OnClickConnectionPoint;

    public SkillTreeEditorConnectionPoint() { }
    public SkillTreeEditorConnectionPoint(SkillTreeEditorNode node, ConnectionPointType type, GUIStyle style, Action<SkillTreeEditorConnectionPoint> OnClickConnectionPoint, string id = null)
    {
        m_Node = node;
        m_Type = type;
        m_Style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        m_Rect = new Rect(0, 0, 10f, 20f);

        m_ID = id ?? Guid.NewGuid().ToString();
    }

    public void Draw(Rect rect)
    {
        m_Rect.y = rect.y + (rect.height * 0.5f) - 10;

        switch(m_Type)
        {
            case ConnectionPointType.In:
                m_Rect.x = rect.x - m_Rect.width + 8.0f;
                break;
            case ConnectionPointType.Out:
                m_Rect.x = rect.x + rect.width - 8.0f;
                break;
        }

        if (GUI.Button(m_Rect, "", m_Style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}
