using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Editor
{
    public class FrameworkExplorerWindow : EditorWindow
    {
        private Vector2 _scroll;
        private Dictionary<Type, object> _services;

        [MenuItem("Rude Warriors/Framework Explorer %#e")] // Ctrl/Cmd + Shift + E
        public static void ShowWindow()
        {
            var window = GetWindow<FrameworkExplorerWindow>("Framework Explorer");
            window.minSize = new Vector2(350, 250);
            window.RefreshServices();
            window.Show();
        }

        private void OnFocus() => RefreshServices();

        private void RefreshServices()
        {
            var field = typeof(ServiceLocator).GetField("_services",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);

            _services = field?.GetValue(null) as Dictionary<Type, object>;
            Repaint();
        }

        private void OnGUI()
        {
            DrawHeader();

            if (_services == null || _services.Count == 0)
            {
                EditorGUILayout.HelpBox("No services registered.", MessageType.Info);
                if (GUILayout.Button("Refresh")) RefreshServices();
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var kvp in _services)
            {
                DrawServiceBox(kvp.Key, kvp.Value);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh")) RefreshServices();
            if (GUILayout.Button("Clear All")) ServiceLocator.Clear();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHeader()
        {
            GUILayout.Space(5);
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("🧠 Rude Warriors Framework Explorer", titleStyle);
            GUILayout.Space(8);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void DrawServiceBox(Type type, object instance)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

            if (instance != null)
            {
                EditorGUILayout.LabelField("Instance Type:", instance.GetType().FullName);
                if (instance is UnityEngine.Object unityObj)
                    EditorGUILayout.ObjectField("Reference", unityObj, typeof(UnityEngine.Object), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Instance is null or unassigned.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(3);
        }
    }
}
