using System;
using System.Collections.Generic;

namespace HelloDev.Utils
{
    /// <summary>
    /// Generic service container. Systems self-register during initialization.
    /// GameBootstrap passes this to all systems without knowing what's inside.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This enables a decoupled initialization pattern:
    /// </para>
    /// <list type="bullet">
    /// <item>GameBootstrap creates an empty GameContext</item>
    /// <item>Each system receives the context via ReceiveContext()</item>
    /// <item>Systems register themselves during InitializeAsync()</item>
    /// <item>Later systems can access earlier systems via Get&lt;T&gt;()</item>
    /// </list>
    /// <para>
    /// Benefits:
    /// </para>
    /// <list type="bullet">
    /// <item>GameBootstrap is completely decoupled from manager types</item>
    /// <item>Type-safe access via generics</item>
    /// <item>Testable - create mock context with mock services</item>
    /// <item>Scalable - add new managers without changing GameBootstrap</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // In UpdateManagerBehaviour.InitializeAsync():
    /// _context?.Register&lt;IUpdateManager&gt;(_manager);
    ///
    /// // In QuestManager.InitializeAsync():
    /// if (_context.TryGet&lt;IUnifiedSaveManager&gt;(out var saveManager))
    /// {
    ///     saveManager.RegisterSystem(_snapshotProvider);
    /// }
    /// </code>
    /// </example>
    public class GameContext
    {
        private readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Register a service. Called by managers during their InitializeAsync.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <exception cref="ArgumentNullException">Thrown if service is null.</exception>
        public void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services[typeof(T)] = service;
        }

        /// <summary>
        /// Get a registered service. Throws if not found.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if service is not registered.</exception>
        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            throw new InvalidOperationException(
                $"Service {typeof(T).Name} not registered. Check initialization order.");
        }

        /// <summary>
        /// Try to get a service. Returns false if not found.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <param name="service">The service instance if found, null otherwise.</param>
        /// <returns>True if the service was found, false otherwise.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        /// <summary>
        /// Check if a service is registered.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        /// <returns>True if the service is registered.</returns>
        public bool Has<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregister a service.
        /// </summary>
        /// <typeparam name="T">The service interface type.</typeparam>
        public void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// Clear all services. Called on shutdown.
        /// </summary>
        public void Clear()
        {
            _services.Clear();
        }
    }
}
