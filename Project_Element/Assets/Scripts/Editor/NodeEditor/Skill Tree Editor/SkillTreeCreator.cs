/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SkillTreeCreator
Description:        Editor window used for creating Skill Trees
Date Created:       10/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class SkillTreeCreator : EditorWindow
{
    private List<SkillTreeEditorNode> m_Nodes;
    private List<SkillTreeEditorConnection> m_Connections;

    private GUIStyle m_NodeStyle;
    private GUIStyle m_SelectedNodeStyle;
    private GUIStyle m_InPointStyle;
    private GUIStyle m_OutPointStyle;

    private SkillTreeEditorConnectionPoint m_SelectedInPoint;
    private SkillTreeEditorConnectionPoint m_SelectedOutPoint;

    private Vector2 m_Offset;
    private Vector2 m_Drag;

    private Rect m_MenuBar;

    [MenuItem("Project Element/Skill Tree Creator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SkillTreeCreator window = (SkillTreeCreator)EditorWindow.GetWindow(typeof(SkillTreeCreator));
        window.Show();
    }

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

                m_Nodes[i].ProcessBranchEvents(e);
            }
        }
    }

    void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    void OnClickAddNode(Vector2 mousePosition)
    {
        if (m_Nodes == null)
        {
            m_Nodes = new List<SkillTreeEditorNode>();
        }

        m_Nodes.Add(new SkillTreeEditorNode(mousePosition, 200, 50, m_NodeStyle, m_SelectedNodeStyle, m_InPointStyle, m_OutPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, null, null));
    }

    void OnClickInPoint(SkillTreeEditorConnectionPoint inPoint)
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

    void OnClickOutPoint(SkillTreeEditorConnectionPoint outPoint)
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

    void OnClickRemoveConnection(SkillTreeEditorConnection connection)
    {
        m_Connections.Remove(connection);
    }

    void CreateConnection()
    {
        if (m_Connections == null)
        {
            m_Connections = new List<SkillTreeEditorConnection>();
        }

        m_Connections.Add(new SkillTreeEditorConnection(m_SelectedInPoint, m_SelectedOutPoint, OnClickRemoveConnection));
    }

    void ClearConnectionSelection()
    {
        m_SelectedInPoint = null;
        m_SelectedOutPoint = null;
    }

    void OnClickRemoveNode(SkillTreeEditorNode node)
    {
        if (m_Connections != null)
        {
            List<SkillTreeEditorConnection> connectionsToRemove = new List<SkillTreeEditorConnection>();

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

    void Save(string path)
    {
        XMLOp.Serialize(m_Nodes, path + "/nodes.xml");
        for (int i = 0; i < m_Nodes.Count; i++)
        {
            if (m_Nodes[i].m_Branches.Count > 0)
            {
                XMLOp.Serialize(m_Nodes[i].m_Branches, path + "/branches" + i + ".xml");
            }
        }

        if (m_Connections != null)
        {
            XMLOp.Serialize(m_Connections, path + "/connections.xml");
        }
    }

    void SaveAsScriptable(string path)
    {
        SkillTreeObject obj = ScriptableObject.CreateInstance<SkillTreeObject>();
    
        int totalPromptCount = 0;
    
        for (int i = 0; i < m_Nodes.Count; i++)
        {
            totalPromptCount += m_Nodes[i].m_Branches.Count;
        }
    
        obj.Nodes = new SkillTreeNode[m_Nodes.Count];
        obj.Branches = new SkillTreeBranch[totalPromptCount];
    
        if (m_Connections != null)
            obj.Connections = new SkillTreeConnection[m_Connections.Count];
    
        int totalPromptIndex = 0;
        for (int i = 0; i < m_Nodes.Count; i++)
        {
            SkillTreeNode nodeObj = new SkillTreeNode();
            nodeObj.PointCost = m_Nodes[i].PointCost;
            nodeObj.SkillProperty = m_Nodes[i].SkillProperty;
            nodeObj.SkillAmount = m_Nodes[i].SkillAmount;
            nodeObj.HasBranches = m_Nodes[i].m_HasBranches;
            nodeObj.InID = m_Nodes[i].m_InPoint.m_ID;
            if (!m_Nodes[i].m_HasBranches)
            {
                nodeObj.OutID = m_Nodes[i].m_OutPoint.m_ID;
            }
            else
            {
                nodeObj.BranchNumber = m_Nodes[i].m_Branches.Count;
                nodeObj.BranchStartNumber = totalPromptIndex;
            }
            obj.Nodes[i] = nodeObj;
    
            if (m_Nodes[i].m_HasBranches)
            {
                for (int j = 0; j < m_Nodes[i].m_Branches.Count; j++)
                {
                    SkillTreeBranch promptObject = new SkillTreeBranch();
    
                    promptObject.OutID = m_Nodes[i].m_Branches[j].m_OutPoint.m_ID;
                    obj.Branches[totalPromptIndex] = promptObject;
                    totalPromptIndex++;
                }
    
            }
        }
    
        if (m_Connections != null)
        {
            for (int i = 0; i < m_Connections.Count; i++)
            {
                SkillTreeConnection connectionObj = new SkillTreeConnection();
                connectionObj.InID = m_Connections[i].m_InPoint.m_ID;
                connectionObj.OutID = m_Connections[i].m_OutPoint.m_ID;
                obj.Connections[i] = connectionObj;
            }
        }
    
        string fileName = Path.GetFileNameWithoutExtension(path);
    
        AssetDatabase.CreateAsset(obj, path + ".asset");
    }

    void Load(string path)
    {
        var nodesDeserialized = XMLOp.Deserialize<List<SkillTreeEditorNode>>(path + "/nodes.xml");
        var connectionsDeserialized = XMLOp.Deserialize<List<SkillTreeEditorConnection>>(path + "/connections.xml");

        m_Nodes = new List<SkillTreeEditorNode>();
        m_Connections = new List<SkillTreeEditorConnection>();

        int index = 0;
        foreach (var nodeDeserialized in nodesDeserialized)
        {
            m_Nodes.Add(new SkillTreeEditorNode(
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

            m_Nodes[index].PointCost = nodeDeserialized.PointCost;
            m_Nodes[index].SkillProperty = nodeDeserialized.SkillProperty;
            m_Nodes[index].SkillAmount = nodeDeserialized.SkillAmount;
            m_Nodes[index].m_HasBranches = nodeDeserialized.m_HasBranches;
            m_Nodes[index].m_Branches = new List<SkillTreeEditorBranchNode>();

            if (m_Nodes[index].m_HasBranches)
            {
                int promptIndex = 0;
                var promptsDeserialized = XMLOp.Deserialize<List<SkillTreeEditorBranchNode>>(path + "/branches" + index + ".xml");
                foreach (SkillTreeEditorBranchNode prompt in promptsDeserialized)
                {
                    m_Nodes[index].m_Branches.Add(new SkillTreeEditorBranchNode(m_Nodes[index], m_Nodes[index].m_Rect, m_Nodes[index].m_OutPointStyle, m_Nodes[index].OnClickConnectionPoint, prompt.m_PromptIndex, m_Nodes[index].OnClickRemoveBranch));
                    m_Nodes[index].m_Branches[promptIndex].m_PromptText = prompt.m_PromptText;
                    m_Nodes[index].m_Branches[promptIndex].m_PromptIndex = prompt.m_PromptIndex;
                    m_Nodes[index].m_Branches[promptIndex].m_OutPoint.m_ID = prompt.m_OutPoint.m_ID;
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
                var inPoint = new SkillTreeEditorConnectionPoint();
                var outPoint = new SkillTreeEditorConnectionPoint();
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
                        for (int j = 0; j < m_Nodes[i].m_Branches.Count; j++)
                        {
                            if (m_Nodes[i].m_Branches[j].m_OutPoint.m_ID == connectionDeserialized.m_OutPoint.m_ID)
                            {
                                outPoint = m_Nodes[i].m_Branches[j].m_OutPoint;
                            }
                        }
                    }
                }

                m_Connections.Add(new SkillTreeEditorConnection(inPoint, outPoint, OnClickRemoveConnection));

                nodeIndex++;
            }
        }

    }
}
