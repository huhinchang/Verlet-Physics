using System.Collections;
using System.Collections.Generic;//List
using UnityEngine;
using System;
using System.Collections.ObjectModel;

public enum ObjectPoolerMaxSizeReachedBehavior { ReuseOldest, CancelSpawn }

public class ObjectPooler<T> where T: Component
{
    public Action OnMaxSizeReached;

    private Transform _parent = default;
    private GameObject _prefab = default;
    private int? _maxSize = default;
    private ObjectPoolerMaxSizeReachedBehavior _maxSizeReachedBehavior = default;

    private List<T> _activePool = new List<T>();
    private Queue<T> _disabledPool = new Queue<T>();

    public ReadOnlyCollection<T> ActivePool => _activePool.AsReadOnly();

    public ObjectPooler(GameObject prefab, Transform parent, int initialSize = 0, int? maxSize = null, ObjectPoolerMaxSizeReachedBehavior maxSizeReachedBehavior = ObjectPoolerMaxSizeReachedBehavior.ReuseOldest)
    {
        _prefab = prefab;
        _parent = parent;

        if (maxSize != null && maxSize <= 0)
        {
            Debug.LogWarning($"Max size mus be positive. Setting it to be equal to initial size {initialSize}");
            _maxSize = initialSize;
        } else
        {
            _maxSize = maxSize;
        }

        _maxSizeReachedBehavior = maxSizeReachedBehavior;

        if (initialSize < 0)
        {
            Debug.LogWarning("Initial size must be npn-negative. Setting it to be 0");
            initialSize = 0;
        }
        for (int i = 0; i < initialSize; i++)
        {
            SpawnFromPool();
        }
    }

    public bool CanSpawn() {
        return _maxSize == null || _activePool.Count < _maxSize;
    }

    public T SpawnFromPool()
    {
        T instance = null;

        // special logic for when max size reached
        if (!CanSpawn()) {
            if (_maxSizeReachedBehavior == ObjectPoolerMaxSizeReachedBehavior.CancelSpawn) {
                return null;
            } else {
                return _activePool[0];
            }
        }

        // reuse disabled object if available
        if (_disabledPool.Count > 0)
        {
            instance = _disabledPool.Dequeue();
        } else
        {
            instance = GameObject.Instantiate(_prefab, _parent).GetComponent<T>();
        }
        _activePool.Add(instance);

        return instance;
    }

    public void ReturnToPool(T toDisable)
    {
        if (_activePool.Contains(toDisable))
        {
            toDisable.gameObject.SetActive(false);
            _activePool.Remove(toDisable);
            _disabledPool.Enqueue(toDisable);
        } else
        {
            Debug.LogWarning("Object was not spawned from this pool!");
        }
    }

    public void ReturnToPoolAt(int index)
    {
        if (_activePool.Count > index)
        {
            ReturnToPool(_activePool[index]);
        } else
        {
            Debug.LogWarning($"Index {index} exceeds active pool count");
        }
    }

    public void ClearPool() {
        for (int i = _activePool.Count - 1 ; i >=0 ; i--)
        {
            ReturnToPool(_activePool[i]);
        }
    }
}
