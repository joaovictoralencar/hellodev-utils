using System.Threading.Tasks;

namespace HelloDev.Utils
{
    /// <summary>
    /// Interface for systems that can be initialized by an external bootstrap.
    /// Implementing this interface is OPTIONAL - managers work standalone without it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface enables a dual-mode initialization pattern:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <term>Standalone Mode</term>
    /// <description>Manager self-initializes in Unity lifecycle (default behavior)</description>
    /// </item>
    /// <item>
    /// <term>Bootstrap Mode</term>
    /// <description>GameBootstrap controls initialization order</description>
    /// </item>
    /// </list>
    /// <para>
    /// Initialization order is controlled by the GameBootstrap group configuration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyManager : MonoBehaviour, IBootstrapInitializable
    /// {
    ///     [SerializeField] private bool selfInitialize = true;
    ///
    ///     public bool SelfInitialize => selfInitialize;
    ///     public bool IsInitialized => _isInitialized;
    ///     private bool _isInitialized;
    ///
    ///     private void OnEnable()
    ///     {
    ///         if (selfInitialize && !_isInitialized)
    ///             _ = InitializeAsync();
    ///     }
    ///
    ///     public Task InitializeAsync()
    ///     {
    ///         if (_isInitialized) return Task.CompletedTask;
    ///         // ... initialization logic ...
    ///         _isInitialized = true;
    ///         return Task.CompletedTask;
    ///     }
    ///
    ///     public void Shutdown()
    ///     {
    ///         _isInitialized = false;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IBootstrapInitializable
    {
        /// <summary>
        /// Called by GameBootstrap before InitializeAsync.
        /// Store the context if you need to register services or access other managers.
        /// </summary>
        /// <remarks>
        /// Systems that register themselves (like UpdateManagerBehaviour) or need to access
        /// other services (like QuestManager accessing SaveManager) should implement this.
        /// Systems that don't need the context can provide an empty implementation.
        /// </remarks>
        /// <param name="context">The game context for service registration and lookup.</param>
        void ReceiveContext(GameContext context);

        /// <summary>
        /// Whether this system should self-initialize in Unity lifecycle (OnEnable/Start).
        /// Set to false when using GameBootstrap for coordinated initialization.
        /// </summary>
        /// <remarks>
        /// When true (default), the system initializes itself during Unity's lifecycle.
        /// When false, the system waits for GameBootstrap to call <see cref="InitializeAsync"/>.
        /// GameBootstrap automatically sets this to false for systems in its list.
        /// </remarks>
        bool SelfInitialize { get; set; }

        /// <summary>
        /// Called by bootstrap to initialize. Should return when initialization is complete.
        /// </summary>
        /// <remarks>
        /// This method should be idempotent - safe to call multiple times.
        /// Check <see cref="IsInitialized"/> and return early if already initialized.
        /// </remarks>
        /// <returns>A task that completes when initialization is done.</returns>
        Task InitializeAsync();

        /// <summary>
        /// Called by bootstrap during shutdown for cleanup.
        /// </summary>
        /// <remarks>
        /// Should unregister from services, clear state, and prepare for re-initialization.
        /// </remarks>
        void Shutdown();

        /// <summary>
        /// Whether this system has completed initialization.
        /// </summary>
        bool IsInitialized { get; }
    }
}
