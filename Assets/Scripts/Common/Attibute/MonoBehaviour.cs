//================================================
//描述 ： 
//作者 ：
//创建时间 ：2018/05/25 10:22:24  
//版本： 
// ================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HMLFramwork.Mono
{
    public static partial class Extent
    {
        public static V TryGetNullableValue<K, V>(this Dictionary<K, V> dic, K key)
        {
            V value = default(V);
            if (dic != null) dic.TryGetValue(key, out value);
            return value;
        }

        public static Component FindFirstComponentInChild(this Transform trans, Type type, string childName)
        {
            Component comp = null;
            if (trans.Find(childName)) comp = trans.Find(childName).GetComponent(type);
            return comp;
        }
    }
    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        private static readonly Dictionary<Type, Dictionary<Type, object>> ms_attributedFieldCache
            = new Dictionary<Type, Dictionary<Type, object>>();
        private static readonly Dictionary<Type, Dictionary<Type, object>> ms_attributedPropertyCache
            = new Dictionary<Type, Dictionary<Type, object>>();

        #region Mono Funcs
        /// <summary>
        /// recommand call base.Awake first in inherit class
        /// </summary>
        protected virtual void Awake()
        {
            InitAutoSearchFields();
            InitAutoSearchProperties();
        }
        #endregion

        #region Init Funcs
        void InitAutoSearchFields()
        {
            var dict = ms_attributedFieldCache.TryGetNullableValue(this.GetType());
            var fields = dict != null ? dict.TryGetNullableValue(typeof(HMLFramwork.Mono.AutoSearch)) as Dictionary<FieldInfo, HMLFramwork.Mono.AutoSearch> : null;
            if (fields == null)
            {
                fields = HMLFramwork.Mono.AttributeHelper.GetAttributeTarget_Fields<HMLFramwork.Mono.AutoSearch>(this.GetType(),
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    new Dictionary<FieldInfo, HMLFramwork.Mono.AutoSearch>());
            }
            if (fields != null)
            {
                MemberInfoAccess(fields,
                    (_field) => { return _field.GetValue(this) as UnityEngine.Object; },
                    (_field) => { return _field.FieldType; },
                    (_field, _attr, _comp) => { _field.SetValue(this, _comp); });
            }
        }
        void InitAutoSearchProperties()
        {
            var dict = ms_attributedPropertyCache.TryGetNullableValue(this.GetType());
            var properties = dict != null ? dict.TryGetNullableValue(typeof(HMLFramwork.Mono.AutoSearch)) as Dictionary<PropertyInfo, HMLFramwork.Mono.AutoSearch> : null;
            if (properties == null)
            {
                properties = HMLFramwork.Mono.AttributeHelper.GetAttributeTarget_Properties<HMLFramwork.Mono.AutoSearch>(this.GetType(),
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    new Dictionary<PropertyInfo, HMLFramwork.Mono.AutoSearch>());
            }
            if (properties != null)
            {
                MemberInfoAccess(properties, (_property) => { return _property.GetValue(this, null) as UnityEngine.Object; },
                     (_property) => { return _property.PropertyType; },
                     (_property, _attr, _comp) => { _property.SetValue(this, _comp, null); });
            }
        }
        #endregion

        #region Help Funcs
        protected Component SearchComponent(Type type, AutoSearch search)
        {
            var searchPath = search != null ? search.path : string.Empty;
            var flag = search != null ? search.autoSearchFlag : AutoSearchFlag.Local;
            if (flag == HMLFramwork.Mono.AutoSearchFlag.Local)
            {
                if (string.IsNullOrEmpty(searchPath))
                {
                    return transform.GetComponentInChildren(type);
                }
                return transform.FindFirstComponentInChild(type, searchPath);
            }
            else
            {
                if (string.IsNullOrEmpty(searchPath))
                {
                    return FindObjectOfType(type) as Component;
                }
                var go = GameObject.Find(searchPath);
                return go ? go.GetComponent(type) as Component : null;
            }
        }
        /// <summary>
        /// 寻找GameObject
        /// </summary>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        protected GameObject SearchGameObject(Type type, AutoSearch search)
        {
            var searchPath = search != null ? search.path : string.Empty;
            var flag = search != null ? search.autoSearchFlag : AutoSearchFlag.Local;
            if (flag == AutoSearchFlag.Local)
            {
                return transform.Find(searchPath).gameObject;
            }
            else
            {
                if (string.IsNullOrEmpty(searchPath))
                {
                    return FindObjectOfType(type) as GameObject;
                }
                var go = GameObject.Find(searchPath);
                return go;
            }
        }

        void MemberInfoAccess<T, V>(Dictionary<T, V> dict,
            System.Func<T, UnityEngine.Object> getValue,
            System.Func<T, System.Type> getType,
            System.Action<T, V, UnityEngine.Object> access) where T : MemberInfo where V : AutoSearch
        {
            foreach (var pair in dict)
            {
                var key = pair.Key as T;
                var value = pair.Value;
                if (key != null)
                {
                    var val = getValue(key);
                    if (val == null || val == false)
                    {
                        var type = getType(key);
                        if (type.IsSubclassOf(typeof(Component)))
                        {
                            var comp = SearchComponent(type, value);
                            access(key, value, comp);
                        }
                        else
                        {
                            var comp = SearchGameObject(type, value);
                            access(key, value, comp);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

