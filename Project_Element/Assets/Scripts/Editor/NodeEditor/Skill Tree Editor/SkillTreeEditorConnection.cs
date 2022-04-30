/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeEditorConnection
Description:        Handles Skill Tree Connections
Date Created:       10/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class SkillTreeEditorConnection
{
    public SkillTreeEditorConnectionPoint m_InPoint;
    public SkillTreeEditorConnectionPoint m_OutPoint;
    [XmlIgnore] public Action<SkillTreeEditorConnection> OnClickRemoveConnection;

    public SkillTreeEditorConnection() { }

    public SkillTreeEditorConnection(SkillTreeEditorConnectionPoint inPoint, SkillTreeEditorConnectionPoint outPoint, Action<SkillTreeEditorConnection> OnClickRemoveConnection)
    {
        m_InPoint = inPoint;
        m_OutPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(m_InPoint.m_Rect.center, m_OutPoint.m_Rect.center,
            m_InPoint.m_Rect.center + Vector2.left * 50.0f, m_OutPoint.m_Rect.center - Vector2.left * 50.0f, Color.white, null, 2.0f);

        if (Handles.Button((m_InPoint.m_Rect.center + m_OutPoint.m_Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}
