using UnityEngine;
using System;
using System.Linq;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Automatically registers and clears scene-level services with the ServiceLocator.
    /// Attach this to a GameObject in each scene to manage contextual services.
    /// </summary>
    [DefaultExecutionOrder(-100)] // ensures it runs before normal scripts
    public class SceneContext : MonoBehaviour
    {
        [SerializeField] private bool clearOnUnload = true;

        private void Awake()
        {
            RegisterSceneServices();
        }

        private void OnDestroy()
        {
            if (clearOnUnload)
                ServiceLocator.Clear();
        }

        private void RegisterSceneServices()
        {
            // Find all MonoBehaviours with [AutoRegister] attribute
            var allBehaviours = FindObjectsOfType<MonoBehaviour>(true);
            var autoRegisterTypes = allBehaviours
                .Select(b => new { Behaviour = b, Attr = b.GetType().GetCustomAttributes(typeof(AutoRegisterAttribute), true).FirstOrDefault() })
                .Where(x => x.Attr != null)
                .ToList();

            foreach (var entry in autoRegisterTypes)
            {
                Type type = entry.Behaviour.GetType().GetInterfaces().FirstOrDefault();
                if (type == null)
                {
                    Debug.LogWarning($"[SceneContext] {entry.Behaviour.name} has [AutoRegister] but no interface found. Skipping.");
                    continue;
                }

                ServiceLocator.Register(Convert.ChangeType(entry.Behaviour, type));
                Debug.Log($"[SceneContext] Registered {type.Name} from {entry.Behaviour.name}");
            }
        }
    }

    /// <summary>
    /// Marks a MonoBehaviour as automatically registered by SceneContext.
    /// Must implement at least one interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoRegisterAttribute : Attribute { }
}
