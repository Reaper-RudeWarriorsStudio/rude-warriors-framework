using UnityEngine;
using RudeWarriors.Framework.Core;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Initializes and registers all core services for the Rude Warriors Framework.
    /// Add this to your startup scene once. It persists across scene loads.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class FrameworkBootstrapper : MonoBehaviour
    {
        [Header("Optional Services")]
        [SerializeField] private AudioService audioServicePrefab;
        [SerializeField] private bool registerDebugConsole = true;

        private void Awake()
        {
            if (FindObjectsOfType<FrameworkBootstrapper>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            RegisterCoreServices();
        }

        private void RegisterCoreServices()
        {
            // Core event system
            var eventBus = new EventBus();
            ServiceLocator.Register<IEventBus>(eventBus);

            // Data handling
            var dataService = new DataService();
            ServiceLocator.Register<IDataService>(dataService);

            // Optional: Audio
            if (audioServicePrefab)
            {
                var audio = Instantiate(audioServicePrefab);
                DontDestroyOnLoad(audio.gameObject);
                ServiceLocator.Register<IAudioService>(audio);
            }

            // Debug Console
            if (registerDebugConsole)
            {
                var console = new GameObject("DebugConsole").AddComponent<DebugConsole>();
                DontDestroyOnLoad(console.gameObject);
            }

            Debug.Log("[RudeWarriors] FrameworkBootstrapper initialized core services.");
        }
    }
}
