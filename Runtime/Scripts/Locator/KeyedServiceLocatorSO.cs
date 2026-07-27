using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Logger = HelloDev.Logging.Logger;

namespace HelloDev.Utils.Locator.Locator
{
    public abstract class KeyedServiceLocatorSO<TKey, TValue> : ScriptableObject, IResettableLocator where TValue : class
    {
        private readonly Dictionary<TKey, (TValue service, GameObject owner)> _services = new();

        void IResettableLocator.ResetLocator() => _services.Clear();

        public bool Has(TKey key) => _services.ContainsKey(key);

        public bool Register(TKey key, TValue service, GameObject owner = null, bool overrideExisting = true)
        {
            if (service == null)
            {
                Logger.LogError("ServiceLocator", $"Tried to register null {typeof(TValue).Name} for key {key}.");
                return false;
            }

            if (Has(key) && !overrideExisting && !Equals(_services[key].service, service))
            {
                Logger.LogWarning("ServiceLocator", $"{typeof(TValue).Name} for key {key} already registered, ignoring.");
                return false;
            }

            _services[key] = (service, owner);
            return true;
        }

        public bool Unregister(TKey key, TValue service)
        {
            if (!_services.TryGetValue(key, out var entry) || entry.service != service)
                return false;
            _services.Remove(key);
            return true;
        }

        public TValue Get(TKey key)
        {
            if (_services.TryGetValue(key, out var entry)) return entry.service;
            Logger.LogError("ServiceLocator", $"{typeof(TValue).Name} not registered for key {key}.");
            return null;
        }

        public GameObject GetOwner(TKey key) => _services.TryGetValue(key, out var entry) ? entry.owner : null;

        public async UniTask<TValue> WaitFor(TKey key, CancellationToken ct = default)
        {
            while (!Has(key)) await UniTask.Yield(ct);
            return _services[key].service;
        }
    }
}