using UnityEngine;

namespace HelloDev.Utils.Locator.Locator
{
    internal static class ServiceLocatorResetter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            foreach (ScriptableObject obj in Resources.FindObjectsOfTypeAll<ScriptableObject>())
            {
                if (obj is IResettableLocator resettable)
                    resettable.ResetLocator();
            }
        }
    }
}