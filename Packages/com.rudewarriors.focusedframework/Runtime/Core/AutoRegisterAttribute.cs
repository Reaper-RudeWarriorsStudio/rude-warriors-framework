using System;

namespace RudeWarriors.Framework.Core
{
    /// <summary>
    /// Marks a class implementing IService to be automatically registered
    /// by the framework during startup or scene load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AutoRegisterAttribute : Attribute
    {
        public bool Persistent { get; }

        /// <param name="persistent">If true, the service persists across scenes.</param>
        public AutoRegisterAttribute(bool persistent = false)
        {
            Persistent = persistent;
        }
    }
}
