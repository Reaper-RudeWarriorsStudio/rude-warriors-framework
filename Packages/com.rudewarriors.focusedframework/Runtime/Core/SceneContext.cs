using UnityEngine;
using System.Collections.Generic;

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

        private void RegisterSceneServices()
        {
            _registered.Clear();

            // Find all services in the scene
            var services = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var mb in services)
            {
                if (mb is IService service)
                {
                    ServiceLocator.Register(service.GetType(), service);
                    _registered.Add(service);
                    RWDebug.System($"[SceneContext] Registered service: {service.GetType().Name}");
                }
            }
        }

        private void OnDestroy()
        {
            // Unregister scene-bound services when unloading
            foreach (var s in _registered)
            {
                ServiceLocator.Unregister(s.GetType());
                RWDebug.System($"[SceneContext] Unregistered service: {s.GetType().Name}");
            }

            _registered.Clear();
        }
    }
}
