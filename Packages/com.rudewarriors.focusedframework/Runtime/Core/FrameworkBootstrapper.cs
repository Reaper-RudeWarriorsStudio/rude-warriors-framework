using UnityEngine;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Initializes and registers all core systems for the Rude Warriors Framework.
    /// Place this in your startup scene once; it persists across scene loads.
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
            // Ensure singleton instance
            if (FindObjectsOfType<FrameworkBootstrapper>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            InitializeCore();
        }

        private void InitializeCore()
        {
            // 1. Logging system
            RWDebug.System("Initializing Rude Warriors Framework...");

            // 2. Core event system
            var eventBus = new EventBus();
            ServiceLocator.Register<IEventBus>(eventBus);
            RWDebug.System("EventBus registered.");

            // 3. Data service
            var dataService = new DataService();
            ServiceLocator.Register<IDataService>(dataService);
            RWDebug.System("DataService registered.");

            // 4. Time control
            var timeService = new GameObject("TimeService").AddComponent<TimeService>();
            DontDestroyOnLoad(timeService.gameObject);
            ServiceLocator.Register<ITimeService>(timeService);
            RWDebug.System("TimeService registered.");

            // 5. Save profiles
            var saveManager = new GameObject("SaveProfileManager").AddComponent<SaveProfileManager>();
            DontDestroyOnLoad(saveManager.gameObject);
            ServiceLocator.Register<ISaveProfileManager>(saveManager);
            RWDebug.System("SaveProfileManager registered.");

            // 6. Audio (optional prefab)
            if (audioServicePrefab)
            {
                var audio = Instantiate(audioServicePrefab);
                DontDestroyOnLoad(audio.gameObject);
                ServiceLocator.Register<IAudioService>(audio);
                RWDebug.System("AudioService registered.");
            }

            // 7. UI (optional prefab)
            if (uiServicePrefab)
            {
                var ui = Instantiate(uiServicePrefab);
                DontDestroyOnLoad(ui.gameObject);
                ServiceLocator.Register<IUIService>(ui);
                RWDebug.System("UIService registered.");
            }

            // 8. Debug console (optional)
            if (includeDebugConsole)
            {
                var console = new GameObject("DebugConsole").AddComponent<DebugConsole>();
                DontDestroyOnLoad(console.gameObject);
                RWDebug.System("DebugConsole spawned.");
            }

            RWDebug.System("FrameworkBootstrapper complete.");
        }
    }
}
