using System;
using System.Collections.Generic;

namespace RudeWarriors.Framework.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = null;
            return false;
        }

        public static T Get<T>() where T : class
        {
            return (T)(_services.TryGetValue(typeof(T), out var obj)
                ? obj
                : throw new InvalidOperationException($"{typeof(T).Name} not registered in ServiceLocator."));
        }

        public static void Clear() => _services.Clear();
    }
}
