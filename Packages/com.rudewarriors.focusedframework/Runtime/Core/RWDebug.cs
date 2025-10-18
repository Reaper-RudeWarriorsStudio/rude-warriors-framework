using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Central debug utility for Rude Warriors.
    /// Mirrors to EventBus if available.
    /// </summary>
    public static class RWDebug
    {
        private static IEventBus _bus;
        private static bool _mirrorToEventBus = true;
        private static readonly string _prefix = "<b><color=#FF4F4F>[RudeWarriors]</color></b>";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Try to get the event bus at startup. If not present yet, some services may register it later.
            if (ServiceLocator.TryGet<IEventBus>(out var bus))
                _bus = bus;
        }

        public static void System(string message)
        {
            Log(message, "System");
        }

        public static void Log(string message, string category = "General")
        {
            Debug.Log($"{_prefix} <color=#BBBBBB>[{category}]</color> {message}");
            if (_mirrorToEventBus)
                MirrorToBus("INFO", category, message);
        }

        public static void Error(string message)
        {
            Debug.LogError($"{_prefix} <color=#FFAAAA>[ERROR]</color> {message}");
            if (_mirrorToEventBus)
                MirrorToBus("ERROR", "General", message);
        }

        private static void MirrorToBus(string level, string category, string message)
        {
            if (_bus == null)
            {
                // lazy attempt: maybe bus registered after startup
                ServiceLocator.TryGet<IEventBus>(out _bus);
                if (_bus == null) return;
            }

            // Using a simple log message struct that EventBus subscribers can handle
            var payload = new DebugLogEvent
            {
                Level = level,
                Category = category,
                Message = message
            };

            try
            {
                _bus.Publish(payload);
            }
            catch
            {
                // never throw from logging
            }
        }

        public struct DebugLogEvent
        {
            public string Level;
            public string Category;
            public string Message;
        }
    }
}
