using System;
using System.Collections.Generic;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Centralized event system for decoupled communication between systems.
    /// </summary>
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> listener);
        void Unsubscribe<T>(Action<T> listener);
        void Publish<T>(T message);
        void Clear();
    }

    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _listeners = new();

        /// <summary>
        /// Register a listener for a specific message type.
        /// </summary>
        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                _listeners[type] = list = new List<Delegate>();

            if (!list.Contains(listener))
                list.Add(listener);
        }

        /// <summary>
        /// Remove a listener for a specific message type.
        /// </summary>
        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_listeners.TryGetValue(type, out var list))
                list.Remove(listener);
        }

        /// <summary>
        /// Broadcast an event to all listeners of that message type.
        /// </summary>
        public void Publish<T>(T message)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                return;

            foreach (var del in list.ToArray())
                if (del is Action<T> action)
                    action.Invoke(message);
        }

        /// <summary>
        /// Clears all listeners. Call when unloading scenes or resetting systems.
        /// </summary>
        public void Clear() => _listeners.Clear();
    }
}
