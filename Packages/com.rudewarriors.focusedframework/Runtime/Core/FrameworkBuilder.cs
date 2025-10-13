using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Fluent builder for initializing and linking Rude Warriors Framework modules.
    /// Keeps initialization code modular and code-driven — no prefabs required.
    /// </summary>
    public class FrameworkBuilder
    {
        private bool _eventBus;
        private bool _dataService;
        private bool _timeService;
        private bool _saveProfiles;
        private bool _uiService;
        private bool _audioService;
        private bool _debugConsole;

        private UIService _uiInstance;
        private AudioService _audioInstance;

        public FrameworkBuilder UseEventBus()
        {
            _eventBus = true;
            return this;
        }

        public FrameworkBuilder UseDataService()
        {
            _dataService = true;
            return this;
        }

        public FrameworkBuilder UseTimeService()
        {
            _timeService = true;
            return this;
        }

        public FrameworkBuilder UseSaveProfiles()
        {
            _saveProfiles = true;
            return this;
        }

        public FrameworkBuilder UseUI(UIService ui)
        {
            _uiService = true;
            _uiInstance = ui;
            return this;
        }

        public FrameworkBuilder UseAudio(AudioService audio)
        {
            _audioService = true;
            _audioInstance = audio;
            return this;
        }

        public FrameworkBuilder UseDebugConsole()
        {
            _debugConsole = true;
            return this;
        }

        public void Build()
        {
            RWDebug.System("Initializing Rude Warriors Framework via Builder...");

            if (_eventBus)
            {
                var bus = new EventBus();
                ServiceLocator.Register<IEventBus>(bus);
                RWDebug.System("EventBus linked.");
            }

            if (_dataService)
            {
                var data = new DataService();
                ServiceLocator.Register<IDataService>(data);
                RWDebug.System("DataService linked.");
            }

            if (_timeService)
            {
                var time = new GameObject("TimeService").AddComponent<TimeService>();
                Object.DontDestroyOnLoad(time.gameObject);
                ServiceLocator.Register<ITimeService>(time);
                RWDebug.System("TimeService linked.");
            }

            if (_saveProfiles)
            {
                var saves = new GameObject("SaveProfileManager").AddComponent<SaveProfileManager>();
                Object.DontDestroyOnLoad(saves.gameObject);
                ServiceLocator.Register<ISaveProfileManager>(saves);
                RWDebug.System("SaveProfileManager linked.");
            }

            if (_uiService && _uiInstance != null)
            {
                Object.DontDestroyOnLoad(_uiInstance.gameObject);
                ServiceLocator.Register<IUIService>(_uiInstance);
                RWDebug.System("UIService linked.");
            }

            if (_audioService && _audioInstance != null)
            {
                Object.DontDestroyOnLoad(_audioInstance.gameObject);
                ServiceLocator.Register<IAudioService>(_audioInstance);
                RWDebug.System("AudioService linked.");
            }

            if (_debugConsole)
            {
                var console = new GameObject("DebugConsole").AddComponent<DebugConsole>();
                Object.DontDestroyOnLoad(console.gameObject);
                RWDebug.System("DebugConsole linked.");
            }

            RWDebug.System("Rude Warriors Framework initialization complete.");
        }
    }
}
