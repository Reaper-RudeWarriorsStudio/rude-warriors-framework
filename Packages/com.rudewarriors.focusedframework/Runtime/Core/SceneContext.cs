using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Scene-scoped auto-registration for IService components.
    /// Registers all IService-derived interfaces implemented by a MonoBehaviour service.
    /// </summary>
    public class SceneContext : MonoBehaviour
    {
        private readonly List<IService> _registered = new();

        private void Awake()
        {
            RegisterSceneServices();
        }

        private void RegisterSceneServices()
        {
            _registered.Clear();

            // Find objects including inactive
            var mbs = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in mbs)
            {
                if (mb is IService svc)
                {
                    var svcType = mb.GetType();

                    // Gather interfaces that extend IService (exclude IService marker interface itself)
                    var interfaces = svcType.GetInterfaces()
                                            .Where(i => i != typeof(IService) && typeof(IService).IsAssignableFrom(i))
                                            .Distinct();

                    bool anyRegistered = false;

                    foreach (var iface in interfaces)
                    {
                        try
                        {
                            ServiceLocator.Register(iface, svc);
                            anyRegistered = true;
                            RWDebug.System($"[SceneContext] Registered {svcType.Name} as {iface.Name}");
                        }
                        catch (Exception ex)
                        {
                            RWDebug.System($"[SceneContext] Failed registering {svcType.Name} as {iface.Name}: {ex.Message}");
                        }
                    }

                    // If no specific interface found, register concrete type so users can request concrete
                    if (!anyRegistered)
                    {
                        try
                        {
                            ServiceLocator.Register(svcType, svc);
                            RWDebug.System($"[SceneContext] Registered concrete service: {svcType.Name}");
                        }
                        catch (Exception ex)
                        {
                            RWDebug.System($"[SceneContext] Failed registering concrete {svcType.Name}: {ex.Message}");
                        }
                    }

                    _registered.Add(svc);
                }
            }
        }

        private void OnDestroy()
        {
            // Unregister all interface registrations that were registered for each service
            if (_registered.Count == 0) return;

            var removed = new HashSet<Type>();

            foreach (var s in _registered)
            {
                var svcType = s.GetType();
                var interfaces = svcType.GetInterfaces()
                    .Where(i => i != typeof(IService) && typeof(IService).IsAssignableFrom(i))
                    .Distinct()
                    .ToArray();

                if (interfaces.Length > 0)
                {
                    foreach (var iface in interfaces)
                    {
                        if (removed.Add(iface))
                            ServiceLocator.Unregister(iface);
                    }
                }
                else
                {
                    // no interface - unregister concrete type once
                    if (removed.Add(svcType))
                        ServiceLocator.Unregister(svcType);
                }
            }

            _registered.Clear();
        }
    }
}
