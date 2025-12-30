using UnityEngine;

namespace HelloDev.Utils
{
    /// <summary>
    /// Base class for all HelloDev locator ScriptableObjects.
    /// Provides a common interface for locating managers and bootstrap integration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Locators act as decoupled access points to manager instances.
    /// They enable systems to communicate without direct references.
    /// </para>
    /// <para>
    /// Inherit from this class when creating a new locator:
    /// </para>
    /// <code>
    /// public class MyLocator_SO : LocatorBase_SO
    /// {
    ///     public override string LocatorId => "HelloDev.MySystem";
    ///     public override bool IsAvailable => _manager != null;
    ///
    ///     private MyManager _manager;
    ///
    ///     public void Register(MyManager manager) { _manager = manager; }
    ///     public void Unregister(MyManager manager) { if (_manager == manager) _manager = null; }
    /// }
    /// </code>
    /// </remarks>
    public abstract class LocatorBase_SO : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for this locator type.
        /// Convention: "HelloDev.{Package}.{LocatorName}"
        /// </summary>
        /// <example>
        /// "HelloDev.Conditions.WorldFlags"
        /// "HelloDev.QuestSystem.Save"
        /// </example>
        public abstract string LocatorId { get; }

        /// <summary>
        /// Whether the manager for this locator is registered and ready.
        /// </summary>
        /// <remarks>
        /// Check this before calling locator methods to avoid null reference exceptions.
        /// In bootstrap mode, this becomes true after the manager's InitializeAsync completes.
        /// </remarks>
        public abstract bool IsAvailable { get; }

        /// <summary>
        /// Called when a bootstrap system is about to initialize all systems.
        /// Override to prepare for controlled initialization.
        /// </summary>
        /// <remarks>
        /// Default implementation does nothing. Override if your locator needs
        /// to clear state or prepare for a fresh initialization cycle.
        /// </remarks>
        public virtual void PrepareForBootstrap()
        {
            // Override in derived class if needed
        }

        /// <summary>
        /// Called after all bootstrap initialization completes successfully.
        /// Override to perform post-initialization setup.
        /// </summary>
        /// <remarks>
        /// At this point, all locators should be available and ready.
        /// Safe to make cross-locator calls here.
        /// </remarks>
        public virtual void OnBootstrapComplete()
        {
            // Override in derived class if needed
        }

        /// <summary>
        /// Called when the bootstrap system shuts down.
        /// Override to clean up any bootstrap-specific state.
        /// </summary>
        public virtual void OnBootstrapShutdown()
        {
            // Override in derived class if needed
        }
    }
}
