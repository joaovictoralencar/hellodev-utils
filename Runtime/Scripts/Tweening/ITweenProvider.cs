using UnityEngine;
using UnityEngine.UI;

namespace HelloDev.Tweening
{
    /// <summary>
    /// Interface for a tween provider that can be implemented by different tween libraries
    /// (DOTween, PrimeTween, LeanTween, etc.).
    /// </summary>
    public interface ITweenProvider
    {
        #region Transform Tweens

        /// <summary>
        /// Tweens the transform's local scale to the target value.
        /// </summary>
        ITweenHandle Scale(Transform target, Vector3 endValue, float duration, float delay = 0);

        /// <summary>
        /// Tweens the transform's local scale uniformly to the target value.
        /// </summary>
        ITweenHandle Scale(Transform target, float endValue, float duration, float delay = 0);

        #endregion

        #region Graphic/UI Tweens

        /// <summary>
        /// Tweens a Graphic's (Image, Text, etc.) alpha to the target value.
        /// </summary>
        ITweenHandle Fade(Graphic target, float endValue, float duration, float delay = 0);

        /// <summary>
        /// Tweens a CanvasGroup's alpha to the target value.
        /// </summary>
        ITweenHandle Fade(CanvasGroup target, float endValue, float duration, float delay = 0);

        /// <summary>
        /// Tweens an Image's fillAmount to the target value.
        /// </summary>
        ITweenHandle FillAmount(Image target, float endValue, float duration, float delay = 0);

        #endregion

        #region Kill Tweens

        /// <summary>
        /// Kills all tweens on the target Transform.
        /// </summary>
        void Kill(Transform target);

        /// <summary>
        /// Kills all tweens on the target Component.
        /// </summary>
        void Kill(Component target);

        /// <summary>
        /// Kills all active tweens.
        /// </summary>
        void KillAll();

        #endregion
    }
}