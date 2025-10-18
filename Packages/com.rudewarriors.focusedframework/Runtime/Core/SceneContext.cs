using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// SceneContext manages scene-level service registration.
    /// When a new scene loads, it automatically discovers any
    /// components that implement IService and registers them
    /// to the global ServiceLocator.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class SceneContext : MonoBehaviour
    {
        private readonly List<IService> _registered = new();
        private readonly HashSet<System.Type> _registeredTypes = new();

        private void Awake()
        {
            // Prevent duplicate SceneContexts
            var existing = FindObjectsByType<SceneContext>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            if (existing.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            RegisterSceneServices();
        }

        /// <summary>
        /// Discovers and registers all IService implementations in the scene.
        /// Registers both interfaces and concrete types for maximum flexibility.
        /// </summary>
        private void RegisterSceneServices()
        {
            _registered.Clear();
            _registeredTypes.Clear();

            // Find all services in the scene
            var services = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var mb in services)
            {
                if (mb is IService service)
                {
                    // Register all IService-derived interfaces
                    var interfaces = mb.GetType().GetInterfaces()
                        .Where(i => i != typeof(IService) && typeof(IService).IsAssignableFrom(i))
                        .ToList();

                    bool registered = false;

                    foreach (var iface in interfaces)
                    {
                        ServiceLocator.Register(iface, service);
                        _registeredTypes.Add(iface);
                        registered = true;
                        RWDebug.System($"[SceneContext] Registered {mb.GetType().Name} as {iface.Name}");
                    }

                    // Also register concrete type if no interfaces found
                    if (!registered)
                    {
                        ServiceLocator.Register(mb.GetType(), service);
                        _registeredTypes.Add(mb.GetType());
                        RWDebug.System($"[SceneContext] Registered service: {mb.GetType().Name}");
                    }

                    _registered.Add(service);
                }
            }
        }

        private void OnDestroy()
        {
            // Unregister all types that were registered
            foreach (var type in _registeredTypes)
            {
                ServiceLocator.Unregister(type);
                RWDebug.System($"[SceneContext] Unregistered: {type.Name}");
            }

            _registered.Clear();
            _registeredTypes.Clear();
        }
    }
}
