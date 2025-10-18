using UnityEngine;
using RudeWarriors.Framework.Core;

public class MinimalFrameworkBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Register core services once.
        ServiceLocator.Register<IEventBus>(new EventBus());
        ServiceLocator.Register<IDataService>(new DataService());

        // Add your own: ServiceLocator.Register<ISomeService>(new SomeService());
    }
}
