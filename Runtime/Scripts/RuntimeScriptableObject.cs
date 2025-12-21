using System.Collections.Generic;
using UnityEngine;

namespace HelloDev.Utils
{
    /// <summary>
    /// Abstract base class for ScriptableObjects that need their runtime state reset
    /// when entering play mode. Tracks all instances and calls OnScriptableObjectReset()
    /// on each one before the first scene loads.
    /// </summary>
    /// <remarks>
    /// Inherit from this class instead of ScriptableObject when your SO accumulates
    /// runtime state (cached values, listeners, etc.) that should be cleared between
    /// play sessions.
    /// </remarks>
    public abstract class RuntimeScriptableObject : ScriptableObject
    {
        [SerializeField, TextArea] private string _description = "Describe what this scriptable object represents";

        /// <summary>
        /// Optional description field visible in the Inspector.
        /// </summary>
        public string Description => _description;

        // Using HashSet for O(1) add/remove instead of List's O(n)
        static readonly HashSet<RuntimeScriptableObject> Instances = new();

        /// <summary>
        /// Called when the ScriptableObject is loaded. Registers this instance for reset tracking.
        /// </summary>
        protected virtual void OnEnable()
        {
            Instances.Add(this);
        }

        /// <summary>
        /// Called when the ScriptableObject is unloaded. Unregisters this instance from reset tracking.
        /// </summary>
        protected virtual void OnDisable()
        {
            Instances.Remove(this);
        }

        /// <summary>
        /// Override this method to reset any runtime state accumulated during play mode.
        /// Called automatically before the first scene loads via RuntimeInitializeOnLoadMethod.
        /// </summary>
        protected abstract void OnScriptableObjectReset();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetInstances()
        {
            foreach (var instance in Instances)
            {
                // Null check in case instance was destroyed
                if (instance != null)
                    instance.OnScriptableObjectReset();
            }
        }
    }
}
