using System;

namespace HelloDev.Tweening
{
    /// <summary>
    /// Handle for a running tween, allowing chained configuration.
    /// </summary>
    public interface ITweenHandle
    {
        /// <summary>
        /// Sets the starting value for the tween.
        /// </summary>
        ITweenHandle From(float value);

        /// <summary>
        /// Sets the easing function for the tween.
        /// </summary>
        ITweenHandle SetEase(EaseType ease);

        /// <summary>
        /// Sets a callback to invoke when the tween completes.
        /// </summary>
        ITweenHandle OnComplete(Action callback);

        /// <summary>
        /// Sets whether the tween should use unscaled time.
        /// </summary>
        ITweenHandle SetUpdate(bool useUnscaledTime);

        /// <summary>
        /// Kills/stops this tween immediately.
        /// </summary>
        void Kill();
    }
}
