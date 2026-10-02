using System.Collections.Generic;
using UnityEngine;

namespace HoldfastAR.Pooling
{
    public class ObjectPool<T> where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _available;
        private readonly LinkedList<T> _active = new LinkedList<T>();
        private readonly Dictionary<T, LinkedListNode<T>> _activeNodes = new Dictionary<T, LinkedListNode<T>>();

        public int CountAvailable => _available.Count;
        public int CountActive => _active.Count;
        public int Capacity { get; private set; }

        public ObjectPool(T prefab, int size, Transform container)
        {
            _prefab = prefab;
            _container = container;
            _available = new Stack<T>(size);
            Prewarm(size);
        }

        private void Prewarm(int size)
        {
            for (int i = 0; i < size; i++)
            {
                T item = Object.Instantiate(_prefab, _container);
                item.name = $"{_prefab.name}_{i:00}";
                item.gameObject.SetActive(false);
                _available.Push(item);
            }
            Capacity = size;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T item;
            if (_available.Count > 0)
            {
                item = _available.Pop();
            }
            else
            {
                item = _active.First.Value;
                RemoveActive(item);
                item.OnReturnedToPool();
            }

            item.transform.SetPositionAndRotation(position, rotation);
            _activeNodes[item] = _active.AddLast(item);
            item.gameObject.SetActive(true);
            item.OnTakenFromPool();
            return item;
        }

        public void Release(T item)
        {
            if (item == null || !_activeNodes.ContainsKey(item)) return;
            RemoveActive(item);
            item.OnReturnedToPool();
            item.gameObject.SetActive(false);
            item.transform.SetParent(_container, false);
            _available.Push(item);
        }

        public void ReleaseAll()
        {
            while (_active.Count > 0) Release(_active.First.Value);
        }

        private void RemoveActive(T item)
        {
            if (_activeNodes.TryGetValue(item, out LinkedListNode<T> node))
            {
                _active.Remove(node);
                _activeNodes.Remove(item);
            }
        }
    }
}
