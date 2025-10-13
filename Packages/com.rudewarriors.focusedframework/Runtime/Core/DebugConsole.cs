using UnityEngine;
using System;
using System.Collections.Generic;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Lightweight runtime console for internal debugging and framework control.
    /// Type commands at runtime; integrates with EventBus for custom handlers.
    /// </summary>
    [DefaultExecutionOrder(9999)]
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;
        [SerializeField] private int maxLogCount = 100;

        private bool _visible;
        private string _input = string.Empty;
        private readonly List<string> _log = new();

        private Vector2 _scroll;
        private IEventBus _bus;

        private void Awake()
        {
            if (FindObjectsOfType<DebugConsole>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _bus = ServiceLocator.TryGet<IEventBus>(out var bus) ? bus : null;
            Log("DebugConsole initialized.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                _visible = !_visible;
        }

        private void OnGUI()
        {
            if (!_visible) return;

            const int margin = 10;
            var width = Screen.width - margin * 2;
            var height = Mathf.Min(Screen.height / 2, 400);

            GUI.Box(new Rect(margin, margin, width, height), "Rude Warriors Debug Console");

            GUILayout.BeginArea(new Rect(margin + 10, margin + 25, width - 20, height - 35));
            _scroll = GUILayout.BeginScrollView(_scroll);

            foreach (var entry in _log)
                GUILayout.Label(entry);

            GUILayout.EndScrollView();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("ConsoleInput");
            _input = GUILayout.TextField(_input);
            if (GUILayout.Button("Run", GUILayout.Width(60)))
                ExecuteCommand();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUI.FocusControl("ConsoleInput");
        }

        private void ExecuteCommand()
        {
            string cmd = _input.Trim();
            _input = string.Empty;

            if (string.IsNullOrEmpty(cmd))
                return;

            Log($"> {cmd}");

            string[] parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string command = parts[0].ToLower();

            switch (command)
            {
                case "help":
                    Log("Commands: help, clear, services, publish <EventName>, quit");
                    break;

                case "clear":
                    _log.Clear();
                    break;

                case "services":
                    var field = typeof(ServiceLocator).GetField("_services",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    var services = field?.GetValue(null) as Dictionary<Type, object>;
                    if (services == null || services.Count == 0)
                    {
                        Log("No services registered.");
                        break;
                    }
                    foreach (var s in services)
                        Log($"• {s.Key.Name} → {(s.Value != null ? s.Value.GetType().Name : "null")}");
                    break;

                case "publish":
                    if (parts.Length > 1 && _bus != null)
                    {
                        string evt = parts[1];
                        _bus.Publish(evt);
                        Log($"Published event: {evt}");
                    }
                    else Log("Usage: publish <EventName>");
                    break;

                case "quit":
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    break;

                default:
                    Log($"Unknown command: {command}");
                    break;
            }
        }

        private void Log(string message)
        {
            if (_log.Count >= maxLogCount)
                _log.RemoveAt(0);
            _log.Add(message);
        }
    }
}
