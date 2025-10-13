# Minimal Setup

1. Drop `FrameworkBootstrapper` in your startup scene.
2. Access services anywhere:
   ```csharp
   var bus = ServiceLocator.Get<IEventBus>();
   bus.Publish(new PlayerSpawned { Id = 1 });
csharp```
