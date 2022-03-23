using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;

public interface DictQueueElement<T> where T : Enum {
    public T GetID();
}

/// <summary>
/// A dictionary of lists.
/// </summary>
/// <typeparam name="K">Key type</typeparam>
/// <typeparam name="V">Value type</typeparam>
public abstract class DictQueue<K, V>
    where K : Enum
    where V : DictQueueElement<K> {
    public Dictionary<K, Queue<V>> _backer { get; }

    public DictQueue() {
        _backer = new Dictionary<K, Queue<V>>();
    }

    public void Add(V add) {
        K type = add.GetID();
        if (!ContainsKey(type)) {
            Queue<V> newQueue = new Queue<V>();
            _backer.Add(type, newQueue);
        }
        _backer[type].Enqueue(add);
    }

    public bool ContainsKey(K type) {
        foreach (K key in _backer.Keys) {
            if (CompareKeys(key, type)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// This will throw an exception if type is not in the list.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public V Dequeue(K type) {
        return _backer[type].Dequeue();
    }

    public int Count(K type) {
        if (!ContainsKey(type)) {
            return 0;
        }
        return _backer[type].Count;
    }

    public List<V> ToList() {
        List<V> ret = new List<V>();
        foreach(KeyValuePair<K, Queue<V>> entry in _backer) {
            ret.AddRange(entry.Value);
        }
        return ret;
    }

    public void ForEach(Action<V> e) {
        foreach(KeyValuePair<K, Queue<V>> entry in _backer) {
            foreach (V v in entry.Value) {
                e(v);
            }
        }
    }

    public void Clear() {
        _backer.Clear();
    }

    public abstract bool CompareKeys(K k1, K k2);

    public R ForEachForgivable<R>(K type, Func<Queue<V>, R> e) {
        foreach(K key in _backer.Keys) {
            if (CompareKeys(key, type)) {
                return e(_backer[key]);
            }
        }

        return default;
    }

    public bool HasEnough(K type, int num) {
        foreach(K key in _backer.Keys) {
            if (CompareKeys(key, type) && _backer[key].Count >= num) return true;
        }
        return false;
    }

    public int CountForgivable(K type) {
        int ret = 0;
        foreach(K key in _backer.Keys) {
            if (CompareKeys(key, type)) {
                ret += _backer[key].Count;
            }
        }
        return ret;
    }
}