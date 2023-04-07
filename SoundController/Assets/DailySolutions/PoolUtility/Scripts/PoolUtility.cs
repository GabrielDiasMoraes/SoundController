using System;
using System.Collections.Generic;
using DailySolutions.PoolUtility.Scripts.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DailySolutions.PoolUtility.Scripts
{
    public class PoolUtility : IPoolableUtility
    {
        private readonly Stack<PoolableItem> _poolStack;
        private readonly PoolableItem _referenceItem;
        private readonly Transform _parent;
        private readonly Func<PoolableItem, Transform, PoolableItem> _custonInstantiator;

        public PoolUtility(PoolableItem referenceItem, int amount = 0, Transform parent = null, Func<PoolableItem, Transform, PoolableItem> customInstantiator = null)
        {
            _poolStack = new Stack<PoolableItem>();
            _referenceItem = referenceItem;
            _parent = parent;
            _custonInstantiator = customInstantiator;
            PreLoadPrefab(amount);
        }
        
        public T GetFromPool<T>() where T : PoolableItem
        {
            return (T)GetFromPool();
        }

        public void ReturnToPool(PoolableItem item)
        {
            if (item.IsOnPool)
                return;
            item.IsOnPool = true;
            _poolStack.Push(item);
        }

        private void PreLoadPrefab(int amount)
        {
            while (_poolStack.Count < amount)
            {
                ReturnToPool(Instantiate());
            }
        }

        private PoolableItem Instantiate()
        {
            PoolableItem newItem = null;
            newItem = _custonInstantiator == null ? Object.Instantiate(_referenceItem, _parent) : _custonInstantiator.Invoke(_referenceItem, _parent);
            newItem.SetupPool(this);
            return newItem;
        }

        private PoolableItem GetFromPool()
        {
            if (!_poolStack.TryPop(out var item))
            {
                item = Instantiate();
            }
            item.Init();
            item.transform.SetParent(_parent);
            return item;
        }

        

    }
}
