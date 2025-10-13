using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Initializes the Rude Warriors Framework via the new builder pattern.
    /// This acts as a drop-in legacy bridge for users who prefer prefab setup.
    /// </summary>
    [DefaultExecutionOrder(-300)]
    public class FrameworkBootstrapper : MonoBehaviour
    {
        [Header("Optional Prefabs / Instances")]
        [Tooltip("Optional AudioService instance to register.")]
        [SerializeField] private AudioService audioService;

        [Tooltip("Optional UIService instance to register.")]
        [SerializeField] private UIService uiService;

        [Tooltip("Include the runtime Debug Console on launch.")]
        [SerializeField] private bool includeDebugConsole = true;

        private void Awake()
        {
            // Prevent duplicate bootstrappers
            if (FindObjectsOfType<FrameworkBootstrapper>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            InitializeFramework();
        }

        private void InitializeFramework()
        {
            RWDebug.System("Initializing Rude Warriors Framework via Bootstrapper...");

            Framework.Init()
                .UseEventBus()
                .UseDataService()
                .UseTimeService()
                .UseSaveProfiles()
                .UseUI(uiService)
                .UseAudio(audioService)
                .UseDebugConsole()
                .Build();

            RWDebug.System("FrameworkBootstrapper setup complete.");
        }
    }
}
