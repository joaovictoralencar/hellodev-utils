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
    /// Priority ranges:
    /// </para>
    /// <list type="bullet">
    /// <item><term>0-99</term><description>Core services (logging, analytics, input)</description></item>
    /// <item><term>100-149</term><description>Data layer (WorldFlags, EventSystem)</description></item>
    /// <item><term>150-199</term><description>Game systems (Quests, Inventory)</description></item>
    /// <item><term>200-249</term><description>Persistence (SaveManager)</description></item>
    /// <item><term>250-299</term><description>Data loading (load saves, restore state)</description></item>
    /// <item><term>300+</term><description>Gameplay (UI, Audio, Scene-specific)</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyManager : MonoBehaviour, IBootstrapInitializable
    /// {
    ///     [SerializeField] private bool selfInitialize = true;
    ///
    ///     public int InitializationPriority => 150;
    ///     public bool IsInitialized => _isInitialized;
    ///     private bool _isInitialized;
    ///
    ///     private void OnEnable()
    ///     {
    ///         if (selfInitialize)
    ///             _ = InitializeAsync();
    ///     }
    ///
    ///     public async Task InitializeAsync()
    ///     {
    ///         if (_isInitialized) return;
    ///         // ... initialization logic ...
    ///         _isInitialized = true;
    ///         await Task.CompletedTask;
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
        /// Initialization priority. Lower numbers initialize first.
        /// </summary>
        /// <remarks>
        /// Suggested ranges:
        /// <list type="bullet">
        /// <item><term>0-99</term><description>Core services</description></item>
        /// <item><term>100-199</term><description>Data systems</description></item>
        /// <item><term>200-299</term><description>Persistence</description></item>
        /// <item><term>300+</term><description>Gameplay</description></item>
        /// </list>
        /// </remarks>
        int InitializationPriority { get; }

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
