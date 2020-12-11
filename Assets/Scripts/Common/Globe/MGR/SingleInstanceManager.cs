using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 单例管理
/// </summary>
namespace HMLFramwork.Singleton
{
    /// <summary>
    /// 普通全局单例（需要继承此父类）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleInstance<T> where T : new()
    {
        private static T instance = default(T);

        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins
        {
            get
            {
                if (instance == null) instance = new T();
                return instance;
            }
        }
    }

    /// <summary>
    /// MONO场景单例（需要继承此父类）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleInstance<T> : MonoBehaviour
    {
        private static T instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins { get { return instance; } }
        protected void Awake()
        {
            instance = GetComponent<T>();
        }
    }

    /// <summary>
    /// MONO场景单例（需要继承此父类）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
    {
        private static T instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins { get { if (instance == null) instance = FindObjectOfType<T>(); return instance; } }

    }

    /// <summary>
    /// MONO全局单例（需要继承此父类）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoGlobalSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins { get { return instance; } }
        protected void Awake()
        {
            if (instance != null) DestroyImmediate(gameObject);
            else instance = GetComponent<T>();
        }

    }

    public class QSingletonComponent<T> where T : class
    {
        protected static T m_Instance = null;

        public static T Ins
        {
            get
            {
                if (m_Instance == null)
                {
                    // 先获取所有非public的构造方法
                    ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    // 从ctors中获取无参的构造方法
                    ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                    if (ctor == null)
                        throw new Exception("没有公共的构造方法！");
                    // 调用构造方法
                    m_Instance = ctor.Invoke(null) as T;
                }

                return m_Instance;
            }
        }
        /// <summary>
        /// 清除单例
        /// </summary>
        public static void Dispose()
        {
            m_Instance = null;
        }
    }

    /// <summary>
    /// 生成单利对象（无需继承）
    /// 使用方法：return SingletonCenter.GetInstance<xxxxxx>()
    /// </summary>
    public static class SingletonCenter
    {
        /// <summary>
        /// 用来存储创建的各种单例
        /// </summary>
        private static Dictionary<Type, object> singleIns = new Dictionary<Type, object>();

        /// <summary>
        /// 从字典表里面获取单利实例
        /// </summary>
        /// <typeparam name="T">需要创建单例的类</typeparam>
        /// <returns>返回所需类的单例</returns>
        public static T GetInstance<T>() where T : class
        {
            Type type = typeof(T);//现获取要创建的类的类型
            if (!singleIns.ContainsKey(type))//先判断单例字典中有没有，没有则创建，创建后则添加进单例字典中
            {
                T t = Activator.CreateInstance<T>();
                singleIns.Add(type, t);
            }
            return (T)singleIns[type];
        }
    }

}

