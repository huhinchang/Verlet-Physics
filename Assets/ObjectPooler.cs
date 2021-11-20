using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPooler<TKey, TVal> where TVal : Component
{
    private Transform _parent = default;
    private GameObject _prefab = default;

    private Dictionary<TKey, TVal> _activePool = new Dictionary<TKey, TVal>();
    private Queue<TVal> _disabledPool = new Queue<TVal>();

    public ObjectPooler(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public TVal SpawnFromPool(TKey key)
    {
        TVal instance = null;

        // special logic for when max size reached
        if (_activePool.ContainsKey(key))
        {
            Debug.LogWarning($"Key {key} was already in the pool");
            return null;
        }

        // reuse disabled object if available
        if (_disabledPool.Count > 0)
        {
            instance = _disabledPool.Dequeue();
        } else
        {
            instance = GameObject.Instantiate(_prefab, _parent).GetComponent<TVal>();
        }
        _activePool.Add(key, instance);

        return instance;
    }

    public void ReturnToPool(TKey toDisable)
    {
        if (_activePool.ContainsKey(toDisable))
        {
            _activePool[toDisable].gameObject.SetActive(false);
            _disabledPool.Enqueue(_activePool[toDisable]);
            _activePool.Remove(toDisable);
        } else
        {
            Debug.LogWarning($"Key {toDisable} was not spawned from this pool!");
        }
    }

    public void ClearPool()
    {
        foreach (var key in _activePool.Keys)
        {
            ReturnToPool(key);
        }
    }
}
