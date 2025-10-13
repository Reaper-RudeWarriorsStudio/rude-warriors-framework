using UnityEngine;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Unified logging and diagnostic utility for Rude Warriors Framework.
    /// Adds consistent tags, color-coding, and optional console relay.
    /// </summary>
    public static class RWDebug
    {
        private static IEventBus _bus;
        private static bool _initialized;
        private static bool _mirrorToEventBus = true;
        private static string _prefix = "<b><color=#FF4F4F>[RudeWarriors]</color></b>";

        private static void Init()
        {
            if (_initialized) return;
            if (ServiceLocator.TryGet<IEventBus>(out var bus))
                _bus = bus;
            _initialized = true;
        }

        public static void Log(string message, string category = "General")
        {
            Init();
            Debug.Log($"{_prefix} <color=#BBBBBB>[{category}]</color> {message}");
            MirrorToBus("INFO", category, message);
        }

        public static void Warn(string message, string category = "Warning")
        {
            Init();
            Debug.LogWarning($"{_prefix} <color=#FFD700>[{category}]</color> {message}");
            MirrorToBus("WARN", category, message);
        }

        public static void Error(string message, string category = "Error")
        {
            Init();
            Debug.LogError($"{_prefix} <color=#FF3333>[{category}]</color> {message}");
            MirrorToBus("ERROR", category, message);
        }

        public static void System(string message)
        {
            Init();
            Debug.Log($"{_prefix} <color=#66CCFF>[System]</color> {message}");
            MirrorToBus("SYSTEM", "Core", message);
        }

        private static void MirrorToBus(string level, string category, string message)
        {
            if (!_mirrorToEventBus || _bus == null) return;

            _bus.Publish(new RWDebugEvent
            {
                Level = level,
                Category = category,
                Message = message
            });
        }

        public static void SetMirror(bool enabled)
        {
            _mirrorToEventBus = enabled;
            Log($"EventBus mirror {(enabled ? "enabled" : "disabled")}", "System");
        }
    }

    /// <summary>
    /// Broadcasted through EventBus when RWDebug emits a message.
    /// Useful for routing logs to the in-game DebugConsole.
    /// </summary>
    public struct RWDebugEvent
    {
        public string Level;
        public string Category;
        public string Message;
    }
}
