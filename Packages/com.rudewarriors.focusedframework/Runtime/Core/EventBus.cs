using System;
using System.Collections.Generic;
using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> listener);
        void Unsubscribe<T>(Action<T> listener);
        void UnsubscribeAll(object owner);
        void Publish<T>(T message);
        void Clear();
    }

    /// <summary>
    /// Simple EventBus with low allocations and owner-based unsubscribe support.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _listeners = new();

        public void Subscribe<T>(Action<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _listeners[type] = list;
            }

            if (!list.Contains(listener))
                list.Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null) return;
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list)) return;
            list.Remove(listener);
            if (list.Count == 0)
                _listeners.Remove(type);
        }

        /// <summary>
        /// Remove all delegates whose target matches the given owner instance.
        /// Useful to call from MonoBehaviour.OnDestroy to avoid leaks.
        /// </summary>
        public void UnsubscribeAll(object owner)
        {
            if (owner == null) return;

            foreach (var kv in _listeners)
            {
                var list = kv.Value;
                // Remove all delegates whose target equals the owner
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var d = list[i];
                    if (d.Target == owner)
                        list.RemoveAt(i);
                }
            }

            // Optionally prune empty keys
            var emptyKeys = new List<Type>();
            foreach (var kv in _listeners)
                if (kv.Value.Count == 0) emptyKeys.Add(kv.Key);
            foreach (var k in emptyKeys) _listeners.Remove(k);
        }

        /// <summary>
        /// Publish without creating snapshots/arrays each time.
        /// Iterates safely and tolerates listeners being removed during invocation.
        /// </summary>
        public void Publish<T>(T message)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list) || list.Count == 0)
                return;

            // Iterate using index; if listeners are removed during iteration,
            // ensure index remains consistent by adjusting end index.
            for (int i = 0; i < list.Count; i++)
            {
                if (i >= list.Count) break; // defensive

                if (list[i] is Action<T> action)
                {
                    try
                    {
                        action.Invoke(message);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[EventBus] Error invoking listener for {type.Name}: {ex}");
                    }
                }
            }
        }

        public void Clear()
        {
            _listeners.Clear();
        }
    }
}
