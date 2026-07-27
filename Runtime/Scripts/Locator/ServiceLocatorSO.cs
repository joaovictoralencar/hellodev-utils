using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Logger = HelloDev.Logging.Logger;

namespace HelloDev.Utils.Locator.Locator
{
    public abstract class ServiceLocatorSO<T> : ScriptableObject, IResettableLocator where T : class
    {
        private T _service;
        private GameObject _owner;

        public bool IsRegistered => _service != null;
        public GameObject Owner => _owner;

        void IResettableLocator.ResetLocator()
        {
            _service = null;
            _owner = null;
        }

        public bool Register(T service, GameObject owner = null, bool overrideExisting = true)
        {
            if (service == null)
            {
                Logger.LogError("ServiceLocator", $"Tried to register null {typeof(T).Name}.");
                return false;
            }

            if (IsRegistered && _service != service && !overrideExisting)
            {
                Logger.LogWarning("ServiceLocator", $"{typeof(T).Name} already registered, ignoring.");
                return false;
            }

            _service = service;
            _owner = owner;
            return true;
        }

        public bool Unregister(T service)
        {
            if (_service != service) return false;
            _service = null;
            _owner = null;
            return true;
        }

        public T Get()
        {
            if (!IsRegistered)
                Logger.LogError("ServiceLocator", $"{typeof(T).Name} not registered.");
            return _service;
        }

        public async UniTask<T> WaitFor(CancellationToken ct = default, bool waitForOwner = false)
        {
            while (!IsRegistered || (waitForOwner && _owner == null))
                await UniTask.Yield(ct);
            return _service;
        }
    }
}