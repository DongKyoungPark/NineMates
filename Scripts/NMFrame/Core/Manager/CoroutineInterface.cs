using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace NM
{
    public static class CoManager
    {
        static MonoBehaviour _mono;
        public static MonoBehaviour Mono { get { return _mono; } }
        static List<Co> _cos = new List<Co>();
        static Pool<Co> _pool = new Pool<Co>();

        public static void Init(MonoBehaviour mono)
        {
            _mono = mono;
        }

        public static void UnloadCo()
        {
            Queue<Co> queue = new Queue<Co>(_cos);
            while (0 < queue.Count)
            {
                queue.Dequeue().Stop();
            }
            _cos.Clear();
        }

        public static void Release(Co co)
        {
            _cos.Remove(co);
            _pool.Release(co);
        }

        public static Co Take(object owner)
        {
            Co co = _cos.Find((Co match) => { return match._owner == owner; });
            if (null != co)
                return co;

            co = _pool.Take();
            co.Init(owner);
            _cos.Add(co);
            return co;
        }

        public static Coroutine Start(object owner, IEnumerator CoUpdate, int key = 0)
        {
            return Take(owner).Start(CoUpdate, key);
        }
        public static bool IsActive(object owner, int key = 0)
        {
            for (int i = 0; i < _cos.Count; i++)
            {
                if (owner == _cos[i]._owner)
                    return _cos[i].IsActive(key);
            }
            return false;
        }

        public static void Stop(object owner, int key)
        {
            for (int i = 0; i < _cos.Count; i++)
            {
                if (owner == _cos[i]._owner)
                {
                    _cos[i].Stop(key);
                    break;
                }
            }
        }

        public static void Stop(System.Object owner)
        {
            for (int i = 0; i < _cos.Count; i++)
            {
                if (owner == _cos[i]._owner)
                {
                    _cos[i].Stop();
                    break;
                }
            }
        }

        public static IEnumerator Empty() { yield break; }
    }

    public class Co
    {
        public object _owner { get;  set; }
         List<KeyValuePair<int, IEnumerator>> _enumerators = new List<KeyValuePair<int, IEnumerator>>();

        public Co Init(object owner)
        {
            _owner = owner;
            return this;
        }
        void Release()
        {
            _owner = null;
            _enumerators.Clear();
            CoManager.Release(this);
        }

        void KillByKey(int key)
        {
            for (int i = 0; i < _enumerators.Count; i++)
            {
                if (_enumerators[i].Key == key)
                {
                    CoManager.Mono.StopCoroutine(_enumerators[i].Value);
                    _enumerators.RemoveAt(i);
                    return;
                }
            }
        }
        public Coroutine Start(IEnumerator coroutine, int key)
        {
            KillByKey(key);
            return CoManager.Mono.StartCoroutine(Run(coroutine, key));
        }
        IEnumerator Run(IEnumerator coroutine, int key)
        {
            var pair = new KeyValuePair<int, IEnumerator>(key, coroutine);
            _enumerators.Add(pair);
            yield return CoManager.Mono.StartCoroutine(coroutine);

            if (!_enumerators.Contains(pair)) yield break;

            _enumerators.Remove(pair);
            if (_enumerators.Count == 0)
                Release();
        }
        public bool IsActive(int key)
        {
            for (int i = 0; i < _enumerators.Count; i++)
                if (_enumerators[i].Key == key) return true;

            return false;
        }
        public void Stop(int key)
        {
            KillByKey(key);
            if (_enumerators.Count == 0)
                Release();
        }
        public void Stop()
        {
            for (int i = 0; i < _enumerators.Count; ++i)
            {
                if (CoManager.Mono == null) break;
                CoManager.Mono.StopCoroutine(_enumerators[i].Value);
            }
            _enumerators.Clear();
            Release();
        }
    }
    public class Pool<T> where T : class, new()
    {
        List<T> _all = new List<T>();
        Queue<T> _pool = new Queue<T>();

        public int _totalCount { get { return _all.Count; } }
        public int _poolCount { get { return _pool.Count; } }

        public T Take()
        {
            if (0 < _pool.Count)
                return _pool.Dequeue();

            return Create();
        }
        public T Take(System.Func<T, T> func)
        {
            if (0 < _pool.Count)
                return func(_pool.Dequeue());
            return func(Create());
        }
        T Create()
        {
            T obj = new T();
            _all.Add(obj);
            return obj;
        }
        public void Release(T obj)
        {
            _pool.Enqueue(obj);
        }
        public void Clear()
        {
            _all.Clear();
            _pool.Clear();
        }
    }
}
