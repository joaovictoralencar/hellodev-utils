using System;
using UnityEngine;
using UnityEngine.Events;

namespace HelloDev.Utils
{
    /// <summary>
    /// Extension methods for Transform and GameObject components.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroys all children of this transform.
        /// Uses reverse iteration to avoid allocation.
        /// </summary>
        /// <param name="parent">The transform whose children will be destroyed.</param>
        /// <param name="immediate">If true, uses DestroyImmediate instead of Destroy.</param>
        public static void DestroyAllChildren(this Transform parent, bool immediate = false)
        {
            // Reverse iteration avoids allocation and handles index shifting
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                if (immediate)
                    UnityEngine.Object.DestroyImmediate(parent.GetChild(i).gameObject);
                else
                    UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Destroys all children of this transform that match the given condition.
        /// </summary>
        /// <param name="parent">The transform whose children will be destroyed.</param>
        /// <param name="condition">Predicate to filter which children to destroy.</param>
        /// <param name="immediate">If true, uses DestroyImmediate instead of Destroy.</param>
        public static void DestroyAllChildren(this Transform parent, Predicate<Transform> condition, bool immediate = false)
        {
            // Reverse iteration avoids allocation and handles index shifting
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (!condition(child)) continue;

                if (immediate)
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                else
                    UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Destroys all children of this game object.
        /// </summary>
        /// <param name="parent">The game object whose children will be destroyed.</param>
        /// <param name="immediate">If true, uses DestroyImmediate instead of Destroy.</param>
        public static void DestroyAllChildren(this GameObject parent, bool immediate = false)
        {
            parent.transform.DestroyAllChildren(immediate);
        }

        /// <summary>
        /// Destroys all children of this game object that match the given condition.
        /// </summary>
        /// <param name="parent">The game object whose children will be destroyed.</param>
        /// <param name="condition">Predicate to filter which children to destroy.</param>
        /// <param name="immediate">If true, uses DestroyImmediate instead of Destroy.</param>
        public static void DestroyAllChildren(this GameObject parent, Predicate<Transform> condition, bool immediate = false)
        {
            parent.transform.DestroyAllChildren(condition, immediate);
        }
    }

    /// <summary>
    /// Extension methods for UnityEvent providing null-safe operations
    /// and duplicate-prevention for subscriptions.
    /// </summary>
    /// <remarks>
    /// All methods are null-safe:
    /// - SafeInvoke: Only invokes if the event is not null
    /// - SafeSubscribe: Removes existing subscription before adding (prevents duplicates)
    /// - SafeUnsubscribe: Only removes if event and listener are not null
    /// </remarks>
    public static class UnityEventExtensions
    {
        // Zero Parameters

        /// <summary>
        /// Invokes the event if it is not null.
        /// </summary>
        public static void SafeInvoke(this UnityEvent unityEvent)
        {
            unityEvent?.Invoke();
        }

        /// <summary>
        /// Subscribes to the event, removing any existing subscription first to prevent duplicates.
        /// </summary>
        public static void SafeSubscribe(this UnityEvent unityEvent, UnityAction newListener)
        {
            if (unityEvent == null || newListener == null) return;

            unityEvent.RemoveListener(newListener);
            unityEvent.AddListener(newListener);
        }

        /// <summary>
        /// Unsubscribes from the event if both event and listener are not null.
        /// </summary>
        public static void SafeUnsubscribe(this UnityEvent unityEvent, UnityAction listener)
        {
            if (unityEvent == null || listener == null) return;

            unityEvent.RemoveListener(listener);
        }

        // One Parameter

        /// <summary>
        /// Invokes the event with one parameter if it is not null.
        /// </summary>
        public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T arg1)
        {
            unityEvent?.Invoke(arg1);
        }

        /// <summary>
        /// Subscribes to the event, removing any existing subscription first to prevent duplicates.
        /// </summary>
        public static void SafeSubscribe<T>(this UnityEvent<T> unityEvent, UnityAction<T> newListener)
        {
            if (unityEvent == null || newListener == null) return;

            unityEvent.RemoveListener(newListener);
            unityEvent.AddListener(newListener);
        }

        /// <summary>
        /// Unsubscribes from the event if both event and listener are not null.
        /// </summary>
        public static void SafeUnsubscribe<T>(this UnityEvent<T> unityEvent, UnityAction<T> listener)
        {
            if (unityEvent == null || listener == null) return;

            unityEvent.RemoveListener(listener);
        }

        // Two Parameters

        /// <summary>
        /// Invokes the event with two parameters if it is not null.
        /// </summary>
        public static void SafeInvoke<T1, T2>(this UnityEvent<T1, T2> unityEvent, T1 arg1, T2 arg2)
        {
            unityEvent?.Invoke(arg1, arg2);
        }

        /// <summary>
        /// Subscribes to the event, removing any existing subscription first to prevent duplicates.
        /// </summary>
        public static void SafeSubscribe<T1, T2>(this UnityEvent<T1, T2> unityEvent, UnityAction<T1, T2> newListener)
        {
            if (unityEvent == null || newListener == null) return;

            unityEvent.RemoveListener(newListener);
            unityEvent.AddListener(newListener);
        }

        /// <summary>
        /// Unsubscribes from the event if both event and listener are not null.
        /// </summary>
        public static void SafeUnsubscribe<T1, T2>(this UnityEvent<T1, T2> unityEvent, UnityAction<T1, T2> listener)
        {
            if (unityEvent == null || listener == null) return;

            unityEvent.RemoveListener(listener);
        }

        // Three Parameters

        /// <summary>
        /// Invokes the event with three parameters if it is not null.
        /// </summary>
        public static void SafeInvoke<T1, T2, T3>(this UnityEvent<T1, T2, T3> unityEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            unityEvent?.Invoke(arg1, arg2, arg3);
        }

        /// <summary>
        /// Subscribes to the event, removing any existing subscription first to prevent duplicates.
        /// </summary>
        public static void SafeSubscribe<T1, T2, T3>(this UnityEvent<T1, T2, T3> unityEvent, UnityAction<T1, T2, T3> newListener)
        {
            if (unityEvent == null || newListener == null) return;

            unityEvent.RemoveListener(newListener);
            unityEvent.AddListener(newListener);
        }

        /// <summary>
        /// Unsubscribes from the event if both event and listener are not null.
        /// </summary>
        public static void SafeUnsubscribe<T1, T2, T3>(this UnityEvent<T1, T2, T3> unityEvent, UnityAction<T1, T2, T3> listener)
        {
            if (unityEvent == null || listener == null) return;

            unityEvent.RemoveListener(listener);
        }

        // Four Parameters

        /// <summary>
        /// Invokes the event with four parameters if it is not null.
        /// </summary>
        public static void SafeInvoke<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            unityEvent?.Invoke(arg0, arg1, arg2, arg3);
        }

        /// <summary>
        /// Subscribes to the event, removing any existing subscription first to prevent duplicates.
        /// </summary>
        public static void SafeSubscribe<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> newListener)
        {
            if (unityEvent == null || newListener == null) return;

            unityEvent.RemoveListener(newListener);
            unityEvent.AddListener(newListener);
        }

        /// <summary>
        /// Unsubscribes from the event if both event and listener are not null.
        /// </summary>
        public static void SafeUnsubscribe<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> listener)
        {
            if (unityEvent == null || listener == null) return;

            unityEvent.RemoveListener(listener);
        }
    }
}
