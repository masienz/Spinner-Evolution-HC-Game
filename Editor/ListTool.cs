using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class ListTool : EditorWindow
{
    const int TOP_PADDING = 2;
    const string HELP_TEXT = "Can not find 'EndGame' component on any GameObject in the Scene.";

    static Vector2 s_WindowsMinSize = Vector2.one * 300.0f;
    static Rect s_HelpRect = new Rect(0.0f, 0.0f, 300.0f, 100.0f);
    static Rect s_ListRect = new Rect(Vector2.zero, s_WindowsMinSize);

    [MenuItem("Window/BeybladeList/Open Window")]
    static void Initialize()
    {
        ListTool window = EditorWindow.GetWindow<ListTool>(true, "Make Beyblade List");
        window.minSize = s_WindowsMinSize;
    }

    SerializedObject m_AnimalsSO = null;
    ReorderableList m_ReorderableList = null;

    void OnEnable()
    {
        EndGame endGame = FindObjectOfType<EndGame>();
        if (endGame)
        {
            m_AnimalsSO = new SerializedObject(endGame);
            m_ReorderableList = new ReorderableList(m_AnimalsSO, m_AnimalsSO.FindProperty("enemyBeyblades"), true, true, true, true);

            m_ReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Beyblade");
            m_ReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += TOP_PADDING;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index));
            };
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (m_AnimalsSO != null)
        {
            m_AnimalsSO.Update();
            m_ReorderableList.DoList(s_ListRect);
            m_AnimalsSO.ApplyModifiedProperties();
        }
        else
        {
            EditorGUI.HelpBox(s_HelpRect, HELP_TEXT, MessageType.Warning);
        }
    }
}
