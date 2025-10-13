using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Bootstraps the Rude Warriors Framework.
    /// Initializes all core services and auto-registers
    /// any classes marked with [AutoRegister].
    /// </summary>
    [DefaultExecutionOrder(-300)]
    public class FrameworkBootstrapper : MonoBehaviour
    {
        [Header("Optional Prefabs")]
        [SerializeField] private AudioService audioServicePrefab;
        [SerializeField] private UIService uiServicePrefab;
        [SerializeField] private bool includeDebugConsole = true;

        private void Awake()
        {
            // prevent duplicates
            var existing = FindObjectsByType<FrameworkBootstrapper>(FindObjectsSortMode.None);
            if (existing.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            InitializeCore();
            AutoRegisterServices();
        }

        private void InitializeCore()
        {
            RWDebug.System("Initializing Rude Warriors Framework...");

            // Event Bus
            var eventBus = new EventBus();
            ServiceLocator.Register<IEventBus>(eventBus);
            RWDebug.System("EventBus registered.");

            // Data Service
            var dataService = new DataService();
            ServiceLocator.Register<IDataService>(dataService);
            RWDebug.System("DataService registered.");

            // Time Service
            var timeService = new GameObject("TimeService").AddComponent<TimeService>();
            DontDestroyOnLoad(timeService.gameObject);
            ServiceLocator.Register<ITimeService>(timeService);
            RWDebug.System("TimeService registered.");

            // Save Profile Manager
            var saveManager = new GameObject("SaveProfileManager").AddComponent<SaveProfileManager>();
            DontDestroyOnLoad(saveManager.gameObject);
            ServiceLocator.Register<ISaveProfileManager>(saveManager);
            RWDebug.System("SaveProfileManager registered.");

            // Optional Audio & UI prefabs
            if (audioServicePrefab != null)
            {
                var audio = Instantiate(audioServicePrefab);
                DontDestroyOnLoad(audio.gameObject);
                ServiceLocator.Register<IAudioService>(audio);
                RWDebug.System("AudioService registered.");
            }

            if (uiServicePrefab != null)
            {
                var ui = Instantiate(uiServicePrefab);
                DontDestroyOnLoad(ui.gameObject);
                ServiceLocator.Register<IUIService>(ui);
                RWDebug.System("UIService registered.");
            }

            // Optional Debug Console
            if (includeDebugConsole)
            {
                var console = new GameObject("DebugConsole").AddComponent<DebugConsole>();
                DontDestroyOnLoad(console.gameObject);
                RWDebug.System("DebugConsole spawned.");
            }

            RWDebug.System("Core initialization complete.");
        }

        /// <summary>
        /// Automatically registers all classes marked with [AutoRegister]
        /// that implement IService. Instantiates MonoBehaviours as needed.
        /// </summary>
        private void AutoRegisterServices()
        {
            RWDebug.System("Scanning for [AutoRegister] services...");

            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t =>
                    t.GetCustomAttribute<AutoRegisterAttribute>() != null &&
                    typeof(IService).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<AutoRegisterAttribute>();
                UnityEngine.Object instance = null;

                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    var go = new GameObject(type.Name);
                    instance = (MonoBehaviour)go.AddComponent(type);
                    if (attr.Persistent)
                        DontDestroyOnLoad(go);
                }
                else
                {
                    instance = Activator.CreateInstance(type) as UnityEngine.Object;
                }

                if (instance != null)
                {
                    ServiceLocator.Register(type, instance);
                    RWDebug.System($"Auto-registered service: {type.Name}");
                }
            }

            RWDebug.System("Auto-registration complete.");
        }
    }
}
