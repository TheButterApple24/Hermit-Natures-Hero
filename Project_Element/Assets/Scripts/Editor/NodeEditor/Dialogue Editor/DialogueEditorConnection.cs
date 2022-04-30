/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeEditorConnection
Description:        Handles the connection data
Date Created:       25/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class DialogueEditorConnection
{
    public DialogueEditorConnectionPoint m_InPoint;
    public DialogueEditorConnectionPoint m_OutPoint;
    [XmlIgnore] public Action<DialogueEditorConnection> OnClickRemoveConnection;

    public DialogueEditorConnection() { }

    /// <summary>
    /// Connection Constructor
    /// </summary>
    public DialogueEditorConnection(DialogueEditorConnectionPoint inPoint, DialogueEditorConnectionPoint outPoint, Action<DialogueEditorConnection> OnClickRemoveConnection)
    {
        m_InPoint = inPoint;
        m_OutPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    /// <summary>
    /// Draw the connection line
    /// </summary>
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
