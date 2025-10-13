namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Static entry point for the Rude Warriors Framework.
    /// Provides a fluent API for manual initialization and service linking.
    /// </summary>
    public static class Framework
    {
        /// <summary>
        /// Begin building a custom framework setup.
        /// Example:
        /// Framework.Init().UseEventBus().UseTimeService().Build();
        /// </summary>
        public static FrameworkBuilder Init() => new FrameworkBuilder();
    }
}
