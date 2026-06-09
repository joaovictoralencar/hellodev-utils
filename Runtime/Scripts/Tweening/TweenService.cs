using HelloDev.Logging;
using UnityEngine;
using UnityEngine.UI;
using Logger = HelloDev.Logging.Logger;

namespace HelloDev.Tweening
{
    /// <summary>
    /// Static service providing access to the current tween provider.
    /// Set the provider once at application startup.
    /// </summary>
    public static class TweenService
    {
        private static ITweenProvider _provider;

        /// <summary>
        /// Gets the current tween provider. Returns a NullTweenProvider if none is set.
        /// </summary>
        public static ITweenProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    Logger.LogError("Tween", "No tween provider configured! Returning NullTweenProvider.");
                    return new NullTweenProvider();
                }
                return _provider;
            }
        }

        /// <summary>
        /// Returns true if a tween provider has been configured.
        /// </summary>
        public static bool IsConfigured => _provider != null;

        /// <summary>
        /// Sets the tween provider to use throughout the application.
        /// Call this once at application startup (e.g., in a bootstrap script).
        /// </summary>
        /// <param name="provider">The tween provider implementation to use.</param>
        public static void SetProvider(ITweenProvider provider)
        {
            _provider = provider;
            Logger.LogVerbose("Tween", $"Provider set: {provider?.GetType().Name ?? "null"}");
        }

        /// <summary>
        /// Clears the current provider (useful for testing or shutdown).
        /// </summary>
        public static void ClearProvider()
        {
            _provider = null;
            Logger.LogVerbose("Tween", $"Provider cleared");
        }
    }

    /// <summary>
    /// A no-op tween provider that does nothing. Used as fallback when no provider is configured.
    /// </summary>
    internal class NullTweenProvider : ITweenProvider
    {
        public static readonly NullTweenProvider Instance = new();

        private NullTweenProvider() { }

        public ITweenHandle Scale(Transform target, Vector3 endValue, float duration) => NullTweenHandle.Instance;
        public ITweenHandle Scale(Transform target, float endValue, float duration) => NullTweenHandle.Instance;
        public ITweenHandle Fade(Graphic target, float endValue, float duration) => NullTweenHandle.Instance;
        public ITweenHandle Fade(CanvasGroup target, float endValue, float duration) => NullTweenHandle.Instance;
        public ITweenHandle FillAmount(Image target, float endValue, float duration) => NullTweenHandle.Instance;
        public void Kill(Transform target) { }
        public void Kill(Component target) { }
        public void KillAll() { }
    }

    /// <summary>
    /// A no-op tween handle that does nothing.
    /// </summary>
    internal class NullTweenHandle : ITweenHandle
    {
        public static readonly NullTweenHandle Instance = new();

        private NullTweenHandle() { }

        public ITweenHandle From(float value) => this;
        public ITweenHandle SetEase(EaseType ease) => this;
        public ITweenHandle OnComplete(System.Action callback) => this;
        public ITweenHandle SetUpdate(bool useUnscaledTime) => this;
        public void Kill() { }
    }
}
