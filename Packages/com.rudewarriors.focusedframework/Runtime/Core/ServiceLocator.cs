using System;
using System.Collections.Generic;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Global service locator for Rude Warriors Framework.
    /// Thread-safe.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();
        private static readonly object _lock = new object();

        /// <summary>
        /// Register a service instance for type T.
        /// </summary>
        public static void Register<T>(T instance) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            lock (_lock)
            {
                _services[typeof(T)] = instance;

                if (instance is IServiceLifecycle lifecycle)
                    lifecycle.OnServiceRegistered();
            }
        }

        /// <summary>
        /// Register by runtime Type.
        /// </summary>
        public static void Register(Type type, object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            lock (_lock)
            {
                _services[type] = instance;
                if (instance is IServiceLifecycle lifecycle)
                    lifecycle.OnServiceRegistered();
            }
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            lock (_lock)
            {
                if (_services.TryGetValue(typeof(T), out var obj))
                {
                    service = (T)obj;
                    return true;
                }
                service = null;
                return false;
            }
        }

        public static T Get<T>() where T : class
        {
            lock (_lock)
            {
                if (_services.TryGetValue(typeof(T), out var obj))
                    return (T)obj;
                throw new InvalidOperationException($"{typeof(T).Name} not registered in ServiceLocator.");
            }
        }

        public static T GetOrDefault<T>(T fallback = default) where T : class
        {
            lock (_lock)
            {
                return TryGet<T>(out var s) ? s : fallback;
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                // Call OnServiceUnregistered for lifecycle services
                foreach (var kv in new Dictionary<Type, object>(_services))
                {
                    if (kv.Value is IServiceLifecycle lifecycle)
                        lifecycle.OnServiceUnregistered();
                }

                _services.Clear();
            }
        }

        public static void Unregister<T>() where T : class
        {
            lock (_lock)
            {
                if (_services.TryGetValue(typeof(T), out var instance))
                {
                    if (instance is IServiceLifecycle lifecycle)
                        lifecycle.OnServiceUnregistered();

                    _services.Remove(typeof(T));
                }
            }
        }

        public static void Unregister(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            lock (_lock)
            {
                if (_services.TryGetValue(type, out var instance))
                {
                    if (instance is IServiceLifecycle lifecycle)
                        lifecycle.OnServiceUnregistered();

                    _services.Remove(type);
                }
            }
        }
    }
}
