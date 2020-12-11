using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMLFramwork;

namespace HMLFramwork
{
    public class Pool<T> where T : Object
    {
        /// <summary>
        /// 存储暂时不用的对象池
        /// </summary>
        protected Stack<T> _push_Pool;

        /// <summary>
        /// 池中对象的数量
        /// </summary>
        public int Count { get { if (_push_Pool != null) return _push_Pool.Count; else { Debug.Log("警告：请实例化对象池..."); return 0; } } }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        public virtual void InitPool()
        {
        }

        /// <summary>
        /// 入池
        /// </summary>
        public virtual void Push(T _target)
        {
            if (_target)
            {
                _push_Pool.Push(_target);
            }
        }
        /// <summary>
        /// 出池
        /// </summary>
        /// <returns></returns>
        public virtual T Pop()
        {
            if (Count < 1)
            {
                T _target = default(T) ;
                return _target;
            }
            else
            {
                T _target = _push_Pool.Pop();
                return _target;
            }
        }
    }
    /// <summary>
    /// 对象池（通过实例化来创建）
    /// 适合做子弹等自有生命周期的对象的对象池
    /// </summary>
    public class ObjectPool1: Pool<GameObject>
    {
        /// <summary>
        /// 预制体存储路径
        /// </summary>
        private string _prefabPath = "";
        /// <summary>
        /// 对象池初始大小
        /// </summary>
        private int _initialValue = 0;
        /// <summary>
        /// 对象池的目标对象（加载起来的预制体）
        /// </summary>
        private GameObject _target;
        /// <summary>
        /// 池中对象的父节点
        /// </summary>
        private Transform _root;

        public ObjectPool1() { }

        /// <summary>
        /// 构造函数（传入对象预制体路径、对象池初始大小和池中对象的父节点）
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="initialValue"></param>
        /// <param name="root"></param>
        public ObjectPool1(string prefabPath, int initialValue, Transform root = null)
        {
            _push_Pool = new Stack<GameObject>();
            _prefabPath = prefabPath;
            _initialValue = initialValue;
            if (root) _root = root;
            else _root = new GameObject("GO_Pool").transform;

            if (!string.IsNullOrEmpty(prefabPath))
            {
                try
                {
                    _target = Resources.Load<GameObject>(_prefabPath);
                    if (_target == null) Debug.Log("警告：对象池预制体加载失败...");
                }
                catch
                {
                    Debug.Log("警告：对象池预制体加载失败...");
                }
            }
            else Debug.Log("警告：对象池预制体路径为空...");

            InitPool();
        }

        public ObjectPool1(GameObject target, int initialValue, Transform root = null)
        {
            _push_Pool = new Stack<GameObject>();
            _target = target;
            _initialValue = initialValue;

            if (root) _root = root;
            else _root = new GameObject("GO_Pool").transform;

            InitPool();
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        public override void InitPool()
        {
            if (_target != null && _initialValue > 0)
            {
                for (int i = 0; i < _initialValue; i++)
                {
                    GameObject go = GameObject.Instantiate(_target, _root.transform);
                    _push_Pool.Push(go);
                    go.SetActive(false);
                }
            }
            else Debug.Log("警告：目标对象为空或者初始数量为0...");
        }

        /// <summary>
        /// 入池
        /// </summary>
        public override void Push(GameObject go)
        {
            if (go)
            {
                go.transform.SetParent(_root);
                _push_Pool.Push(go);
                go.SetActive(false);
            }
        }
        /// <summary>
        /// 出池
        /// </summary>
        /// <returns></returns>
        public override GameObject Pop()
        {
            GameObject go;
            if (Count < 1)
            {
                go = GameObject.Instantiate(_target, _root.transform);
                if (!go.activeInHierarchy) go.SetActive(true);
            }
            else
            {
                go = _push_Pool.Pop();
                go.SetActive(true);
            }
            return go;
        }
    }

    /// <summary>
    /// 对象池（通过实例化来创建）
    /// 适合做没有固定生命周期的，需要人为控制其消失的对象的对象池
    /// </summary>
    public class ObjectPool2
    {
        /// <summary>
        /// 存储已经出池的对象的对象池
        /// </summary>
        private List<GameObject> _pop_Pool;
        /// <summary>
        /// 池中对象的数量
        /// </summary>
        private ObjectPool1 _pool;
        public int Count { get { if (_pool != null) return _pool.Count; else { Debug.Log("警告：请实例化对象池..."); return 0; } } }

        public ObjectPool2() { }

        /// <summary>
        /// 构造函数（传入对象预制体路径、对象池初始大小和池中对象的父节点）
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="initialValue"></param>
        /// <param name="root"></param>
        public ObjectPool2(string prefabPath, int initialValue, Transform root = null)
        {
            _pop_Pool = new List<GameObject>();
            _pool = new ObjectPool1(prefabPath, initialValue, root);
        }

        public ObjectPool2(GameObject target, int initialValue, Transform root = null)
        {
            _pop_Pool = new List<GameObject>();
            _pool = new ObjectPool1(target, initialValue, root);
        }

        /// <summary>
        /// 指定对象入池
        /// </summary>
        public void Push(GameObject go)
        {
            if (_pop_Pool.Contains(go))
            {
                _pool.Push(go);
                _pop_Pool.Remove(go);
            }
        }
        /// <summary>
        /// 所有对象入池
        /// </summary>
        public void PushAll()
        {
            for (int i = 0; i < _pop_Pool.Count; i++)
            {
                _pool.Push(_pop_Pool[i]);
                _pop_Pool.RemoveAt(i);
                i--;
            }
        }
        /// <summary>
        /// 出池
        /// </summary>
        /// <returns></returns>
        public GameObject Pop()
        {
            GameObject go = _pool.Pop();
            _pop_Pool.Add(go);
            return go;
        }
    }
}

