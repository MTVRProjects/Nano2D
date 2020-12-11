//================================================
//描述 ： 
//作者 ：
//创建时间 ：2018/05/28 09:50:07  
//版本： 
// ================================================
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HMLFramwork.Mono
{
    #region special
    public enum AutoSearchFlag
    {
        Local,
        Global
    }
    /// <summary>
    /// Common description for class 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutoSearch : Attribute
    {
        public AutoSearchFlag autoSearchFlag = AutoSearchFlag.Local;
        public string path = string.Empty;
        /// <summary>
        /// Search in children
        /// </summary>
        public AutoSearch()
        {
        }
        /// <summary>
        /// Search in child node
        /// </summary>
        /// <param name="findPath"></param>
        public AutoSearch(string findPath)
        {
            path = findPath;
        }
        /// <summary>
        /// Set to Search in local or world
        /// </summary>
        /// <param name="flag"></param>
        public AutoSearch(AutoSearchFlag flag)
        {
            autoSearchFlag = flag;
        }
        /// <summary>
        /// Swarch in local child/world node
        /// </summary>
        /// <param name="findPath"></param>
        /// <param name="flag"></param>
        public AutoSearch(string findPath, AutoSearchFlag flag)
        {
            path = findPath;
            autoSearchFlag = flag;
        }
    }
    #endregion

    public static class AttributeHelper
    {
        #region Help Funcs
        public static object[] GetAttributes(Type type, string name)
        {
            var memInfo = type.GetMember(name.ToString());
            if(memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(false);
                return attrs;
            }
            return null;
        }

        public static T AttributeFilter<T>(MemberInfo info) where T : Attribute
        {
            var attributeInfos = info.GetCustomAttributes(typeof(T), false);
            if(attributeInfos != null && attributeInfos.Length > 0)
            {
                return attributeInfos[0] as T;
            }
            return null;
        }

        public static Dictionary<FieldInfo, T> GetAttributeTarget_Fields<T>(Type type, BindingFlags flags, Dictionary<FieldInfo, T> caches = null) where T : Attribute
        {
            var memInfo = type.GetFields(flags);
            var retVal = caches != null ? caches : new Dictionary<FieldInfo, T>();
            if(memInfo != null && memInfo.Length > 0)
            {
                for(int i = 0; i < memInfo.Length; i++)
                {
                    var info = memInfo[i];
                    var attribute = AttributeFilter<T>(info);
                    if(attribute != null)
                    {
                        retVal[info] = attribute;
                    }
                }
            }
            return retVal;
        }

        public static Dictionary<PropertyInfo, T> GetAttributeTarget_Properties<T>(Type type, BindingFlags flags, Dictionary<PropertyInfo, T> caches = null) where T : Attribute
        {
            var memInfo = type.GetProperties(flags);
            var retVal = caches != null ? caches : new Dictionary<PropertyInfo, T>();
            if(memInfo != null && memInfo.Length > 0)
            {
                for(int i = 0; i < memInfo.Length; i++)
                {
                    var info = memInfo[i];
                    var attribute = AttributeFilter<T>(info);
                    if(attribute != null)
                    {
                        retVal[info] = attribute;
                    }
                }
            }
            return retVal;
        }
        #endregion
    }
}

