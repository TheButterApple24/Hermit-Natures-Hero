/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DialougeCreator
Description:        Handles the window for creating dialogue (node-based)
Date Created:       25/02/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    25/02/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class DialogueCreator : EditorWindow
{
    private List<DialogueEditorNode> m_Nodes;
    private List<DialogueEditorConnection> m_Connections;

    private GUIStyle m_NodeStyle;
    private GUIStyle m_SelectedNodeStyle;
    private GUIStyle m_InPointStyle;
    private GUIStyle m_OutPointStyle;

    private DialogueEditorConnectionPoint m_SelectedInPoint;
    private DialogueEditorConnectionPoint m_SelectedOutPoint;

    private Vector2 m_Offset;
    private Vector2 m_Drag;

    private Rect m_MenuBar;

    /// <summary>
    /// Called to initalize window. Called by Unity when window is created
    /// </summary>
    [MenuItem("Project Element/Dialogue Creator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DialogueCreator window = (DialogueCreator)EditorWindow.GetWindow(typeof(DialogueCreator));
        window.Show();
    }

    /// <summary>
    /// Built-In: Called when Window is enabled
    /// </summary>
    private void OnEnable()
    {
        m_NodeStyle = new GUIStyle();
        m_NodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_NodeStyle.border = new RectOffset(12, 12, 12, 12);
        m_NodeStyle.alignment = TextAnchor.MiddleCenter;

        m_SelectedNodeStyle = new GUIStyle();
        m_SelectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        m_SelectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        m_SelectedNodeStyle.alignment = TextAnchor.MiddleCenter;

        m_InPointStyle = new GUIStyle();
        m_InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        m_InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        m_InPointStyle.border = new RectOffset(4, 4, 12, 12);
        m_InPointStyle.alignment = TextAnchor.MiddleCenter;

        m_OutPointStyle = new GUIStyle();
        m_OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        m_OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        m_OutPointStyle.border = new RectOffset(4, 4, 12, 12);
        m_OutPointStyle.alignment = TextAnchor.MiddleCenter;

    }

    /// <summary>
    /// Build-In: Called when GUI is being drawn
    /// </summary>
    void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        DrawMenuBar();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    /// <summary>
    /// Draws a menu bar at the top of the window
    /// </summary>
    void DrawMenuBar()
    {
        m_MenuBar = new Rect(0, 0, position.width, position.height);

        GUILayout.BeginArea(m_MenuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Save as XML"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            string path = EditorUtility.SaveFolderPanel("Select a save location", "", "");
            Save(path);
        }
        GUILayout.Space(5);

        if (GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            string path = EditorUtility.OpenFolderPanel("Select a dialogue folder", "", "");
            Load(path);
        }

        GUILayout.Space(5);

        if (GUILayout.Button(new GUIContent("Save as Scriptable"), EditorStyles.toolbarButton, GUILayout.Width(150)))
        {
            string path = EditorUtility.SaveFilePanel("Select a save location", "", "", "");

            path = path.Replace(Application.dataPath, "Assets");
            SaveAsScriptable(path);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws a grid
    /// </summary>
    /// <param name="gridSpacing">Spacing for the grid</param>
    /// <param name="gridOpacity">How see-through the grid shoukd be</param>
    /// <param name="gridColor">What color the grid is</param>
    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        m_Offset += m_Drag * 0.5f;
        Vector3 newOffset = new Vector3(m_Offset.x % gridSpacing, m_Offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int i = 0; i < heightDivs; i++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3(position.width, gridSpacing * i, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    /// <summary>
    /// Will draw all the nodes that are created
    /// </summary>
    void DrawNodes()
    {
        if (m_Nodes != null)
        {
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                m_Nodes[i].Draw();
            }
        }
    }

    /// <summary>
    /// Will draw all connections created
    /// </summary>
    void DrawConnections()
    {
        if (m_Connections != null)
        {
            for (int i = 0; i < m_Connections.Count; i++)
            {
                m_Connections[i].Draw();
            }
        }
    }

    /// <summary>
    /// Draws the curve between connection points
    /// </summary>
    /// <param name="e"></param>
    void DrawConnectionLine(Event e)
    {
        if (m_SelectedInPoint != null && m_SelectedOutPoint == null)
        {
            Handles.DrawBezier(m_SelectedInPoint.m_Rect.center, e.mousePosition,
            m_SelectedInPoint.m_Rect.center + Vector2.left * 50.0f, e.mousePosition - Vector2.left * 50.0f, Color.white, null, 2.0f);

            GUI.changed = true;
        }

        if (m_SelectedOutPoint != null && m_SelectedInPoint == null)
        {
            Handles.DrawBezier(m_SelectedOutPoint.m_Rect.center, e.mousePosition,
            m_SelectedOutPoint.m_Rect.center - Vector2.left * 50.0f, e.mousePosition + Vector2.left * 50.0f, Color.white, null, 2.0f);

            GUI.changed = true;
        }
    }

    /// <summary>
    /// Handles processing events for window
    /// </summary>
    /// <param name="e"></param>
    void ProcessEvents(Event e)
    {
        m_Drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    /// <summary>
    /// Handles Node Events
    /// </summary>
    /// <param name="e"></param>
    void ProcessNodeEvents(Event e)
    {
        if (m_Nodes != null)
        {
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                bool guiChanged = m_Nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }

                m_Nodes[i].ProcessPromptEvents(e);
            }
        }
    }

    /// <summary>
    /// Called when user right-clicks the window
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Adds a new node
    /// </summary>
    /// <param name="mousePosition"></param>
    void OnClickAddNode(Vector2 mousePosition)
    {
        if (m_Nodes == null)
        {
            m_Nodes = new List<DialogueEditorNode>();
        }

        m_Nodes.Add(new DialogueEditorNode(mousePosition, 200, 50, m_NodeStyle, m_SelectedNodeStyle, m_InPointStyle, m_OutPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, null, null));
    }

    /// <summary>
    /// Called when InPoint is clicked
    /// </summary>
    /// <param name="inPoint"></param>
    void OnClickInPoint(DialogueEditorConnectionPoint inPoint)
    {
        m_SelectedInPoint = inPoint;

        if (m_SelectedOutPoint != null)
        {
            if (m_SelectedOutPoint.m_Node != m_SelectedInPoint.m_Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    /// <summary>
    /// Called when OutPoint is clicked
    /// </summary>
    /// <param name="outPoint"></param>
    void OnClickOutPoint(DialogueEditorConnectionPoint outPoint)
    {
        m_SelectedOutPoint = outPoint;

        if (m_SelectedInPoint != null)
        {
            if (m_SelectedOutPoint.m_Node != m_SelectedInPoint.m_Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    /// <summary>
    /// Called when connection is to be removed
    /// </summary>
    /// <param name="connection"></param>
    void OnClickRemoveConnection(DialogueEditorConnection connection)
    {
        m_Connections.Remove(connection);
    }

    /// <summary>
    /// Called when connection is created
    /// </summary>
    void CreateConnection()
    {
        if (m_Connections == null)
        {
            m_Connections = new List<DialogueEditorConnection>();
        }

        m_Connections.Add(new DialogueEditorConnection(m_SelectedInPoint, m_SelectedOutPoint, OnClickRemoveConnection));
    }

    /// <summary>
    /// Called when user wants to clear the current connection selection
    /// </summary>
    void ClearConnectionSelection()
    {
        m_SelectedInPoint = null;
        m_SelectedOutPoint = null;
    }

    /// <summary>
    /// Called when User deletes a node
    /// </summary>
    /// <param name="node">Node being deleted</param>
    void OnClickRemoveNode(DialogueEditorNode node)
    {
        if (m_Connections != null)
        {
            List<DialogueEditorConnection> connectionsToRemove = new List<DialogueEditorConnection>();

            for (int i = 0; i < m_Connections.Count; i++)
            {
                if (m_Connections[i].m_InPoint == node.m_InPoint || m_Connections[i].m_OutPoint == node.m_OutPoint)
                {
                    connectionsToRemove.Add(m_Connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                m_Connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        m_Nodes.Remove(node);
    }

    /// <summary>
    /// Called when user drags the screen
    /// </summary>
    /// <param name="delta">Mouse velocity when dragged</param>
    void OnDrag(Vector2 delta)
    {
        m_Drag = delta;

        if (m_Nodes != null)
        {
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                m_Nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Called when user Saves the nodes as XML
    /// </summary>
    /// <param name="path">Where to save</param>
    void Save(string path)
    {
        XMLOp.Serialize(m_Nodes, path + "/nodes.xml");
        for (int i = 0; i < m_Nodes.Count; i++)
        {
            if (m_Nodes[i].m_Prompts.Count > 0)
            {
                XMLOp.Serialize(m_Nodes[i].m_Prompts, path + "/prompts" + i + ".xml");
            }
        }

        if (m_Connections != null)
        {
            XMLOp.Serialize(m_Connections, path + "/connections.xml");
        }
    }

    /// <summary>
    /// Called when user wants to save as scriptable object
    /// </summary>
    /// <param name="path">Path to save</param>
    void SaveAsScriptable(string path)
    {
        DialogueObject obj = ScriptableObject.CreateInstance<DialogueObject>();

        int totalPromptCount = 0;

        for (int i = 0; i < m_Nodes.Count; i++)
        {
            totalPromptCount += m_Nodes[i].m_Prompts.Count;
        }

        obj.Nodes = new DialogueNode[m_Nodes.Count];
        obj.Prompts = new DialoguePrompt[totalPromptCount];

        if (m_Connections != null)
            obj.Connections = new DialogueConnection[m_Connections.Count];

        int totalPromptIndex = 0;
        for (int i = 0; i < m_Nodes.Count; i++)
        {
            DialogueNode nodeObj = new DialogueNode();
            nodeObj.m_SpeakerName = m_Nodes[i].m_Name;
            nodeObj.m_Line = m_Nodes[i].m_Line;
            nodeObj.EmotionIndex = m_Nodes[i].Emotion;
            nodeObj.m_HasPrompts = m_Nodes[i].m_HasPrompts;
            nodeObj.m_InID = m_Nodes[i].m_InPoint.m_ID;
            if (!m_Nodes[i].m_HasPrompts)
            {
                nodeObj.m_OutID = m_Nodes[i].m_OutPoint.m_ID;
            }
            else
            {
                nodeObj.m_PromptNumber = m_Nodes[i].m_Prompts.Count;
                nodeObj.m_PromptStartNumber = totalPromptIndex;
            }
            obj.Nodes[i] = nodeObj;

            if (m_Nodes[i].m_HasPrompts)
            {

                for (int j = 0; j < m_Nodes[i].m_Prompts.Count; j++)
                {
                    DialoguePrompt promptObject = new DialoguePrompt();

                    promptObject.m_Line = m_Nodes[i].m_Prompts[j].m_PromptText;
                    promptObject.m_OutID = m_Nodes[i].m_Prompts[j].m_OutPoint.m_ID;
                    promptObject.m_PromptIndex = totalPromptIndex;
                    obj.Prompts[totalPromptIndex] = promptObject;
                    totalPromptIndex++;
                }

            }
        }

        if (m_Connections != null)
        {
            for (int i = 0; i < m_Connections.Count; i++)
            {
                DialogueConnection connectionObj = new DialogueConnection();
                connectionObj.m_InID = m_Connections[i].m_InPoint.m_ID;
                connectionObj.m_OutID = m_Connections[i].m_OutPoint.m_ID;
                obj.Connections[i] = connectionObj;
            }
        }

        string fileName = Path.GetFileNameWithoutExtension(path);

        AssetDatabase.CreateAsset(obj, path + ".asset");
    }

    /// <summary>
    /// Called when user Loads a saved node tree
    /// </summary>
    /// <param name="path"></param>
    void Load(string path)
    {
        var nodesDeserialized = XMLOp.Deserialize<List<DialogueEditorNode>>(path + "/nodes.xml");
        var connectionsDeserialized = XMLOp.Deserialize<List<DialogueEditorConnection>>(path + "/connections.xml");

        m_Nodes = new List<DialogueEditorNode>();
        m_Connections = new List<DialogueEditorConnection>();

        int index = 0;
        foreach (var nodeDeserialized in nodesDeserialized)
        {
            m_Nodes.Add(new DialogueEditorNode(
                nodeDeserialized.m_Rect.position,
                nodeDeserialized.m_Rect.width,
                nodeDeserialized.m_Rect.height,
                m_NodeStyle,
                m_SelectedNodeStyle,
                m_InPointStyle,
                m_OutPointStyle,
                OnClickInPoint,
                OnClickOutPoint,
                OnClickRemoveNode,
                nodeDeserialized.m_InPoint.m_ID,
                nodeDeserialized.m_OutPoint.m_ID));

            m_Nodes[index].m_Name = nodeDeserialized.m_Name;
            m_Nodes[index].m_Line = nodeDeserialized.m_Line;
            m_Nodes[index].Emotion = nodeDeserialized.Emotion;
            m_Nodes[index].EmotionContent.text = System.Enum.GetName(typeof(DialogueEmotion), nodeDeserialized.Emotion);
            m_Nodes[index].m_HasPrompts = nodeDeserialized.m_HasPrompts;
            m_Nodes[index].m_Prompts = new List<DialogueEditorPrompt>();

            if (m_Nodes[index].m_HasPrompts)
            {
                int promptIndex = 0;
                var promptsDeserialized = XMLOp.Deserialize<List<DialogueEditorPrompt>>(path + "/prompts" + index + ".xml");
                foreach (DialogueEditorPrompt prompt in promptsDeserialized)
                {
                    m_Nodes[index].m_Prompts.Add(new DialogueEditorPrompt(m_Nodes[index], m_Nodes[index].m_Rect, m_Nodes[index].m_OutPointStyle, m_Nodes[index].OnClickConnectionPoint, prompt.m_PromptIndex, m_Nodes[index].OnClickRemovePrompt));
                    m_Nodes[index].m_Prompts[promptIndex].m_PromptText = prompt.m_PromptText;
                    m_Nodes[index].m_Prompts[promptIndex].m_PromptIndex = prompt.m_PromptIndex;
                    m_Nodes[index].m_Prompts[promptIndex].m_OutPoint.m_ID = prompt.m_OutPoint.m_ID;
                    promptIndex++;
                }
            }

            index++;
        }

        if (connectionsDeserialized != null)
        {

            int nodeIndex = 0;
            foreach (var connectionDeserialized in connectionsDeserialized)
            {
                var inPoint = new DialogueEditorConnectionPoint();
                var outPoint = new DialogueEditorConnectionPoint();
                for (int i = 0; i < m_Nodes.Count; i++)
                {
                    if (m_Nodes[i].m_InPoint.m_ID == connectionDeserialized.m_InPoint.m_ID)
                    {
                        inPoint = m_Nodes[i].m_InPoint;
                    }

                    if (m_Nodes[i].m_OutPoint.m_ID == connectionDeserialized.m_OutPoint.m_ID)
                    {
                        outPoint = m_Nodes[i].m_OutPoint;
                    }

                    if (outPoint.m_ID == null)
                    {
                        for (int j = 0; j < m_Nodes[i].m_Prompts.Count; j++)
                        {
                            if (m_Nodes[i].m_Prompts[j].m_OutPoint.m_ID == connectionDeserialized.m_OutPoint.m_ID)
                            {
                                outPoint = m_Nodes[i].m_Prompts[j].m_OutPoint;
                            }
                        }
                    }
                }

                m_Connections.Add(new DialogueEditorConnection(inPoint, outPoint, OnClickRemoveConnection));

                nodeIndex++;
            }
        }

    }
}
