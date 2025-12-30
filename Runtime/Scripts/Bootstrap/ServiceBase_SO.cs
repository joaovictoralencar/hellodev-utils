using UnityEngine;

namespace HelloDev.Utils
{
    /// <summary>
    /// Base class for all HelloDev service ScriptableObjects.
    /// Provides a common interface for service discovery and bootstrap integration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Services act as decoupled access points to manager instances.
    /// They enable systems to communicate without direct references.
    /// </para>
    /// <para>
    /// Inherit from this class when creating a new service:
    /// </para>
    /// <code>
    /// public class MyService_SO : ServiceBase_SO
    /// {
    ///     public override string ServiceId => "HelloDev.MySystem";
    ///     public override bool IsAvailable => _manager != null;
    ///
    ///     private MyManager _manager;
    ///
    ///     public void Register(MyManager manager) { _manager = manager; }
    ///     public void Unregister(MyManager manager) { if (_manager == manager) _manager = null; }
    /// }
    /// </code>
    /// </remarks>
    public abstract class ServiceBase_SO : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for this service type.
        /// Convention: "HelloDev.{Package}.{ServiceName}"
        /// </summary>
        /// <example>
        /// "HelloDev.Conditions.WorldFlags"
        /// "HelloDev.QuestSystem.Save"
        /// </example>
        public abstract string ServiceId { get; }

        /// <summary>
        /// Whether the manager for this service is registered and ready.
        /// </summary>
        /// <remarks>
        /// Check this before calling service methods to avoid null reference exceptions.
        /// In bootstrap mode, this becomes true after the manager's InitializeAsync completes.
        /// </remarks>
        public abstract bool IsAvailable { get; }

        /// <summary>
        /// Called when a bootstrap system is about to initialize all services.
        /// Override to prepare for controlled initialization.
        /// </summary>
        /// <remarks>
        /// Default implementation does nothing. Override if your service needs
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
        /// At this point, all services should be available and ready.
        /// Safe to make cross-service calls here.
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
