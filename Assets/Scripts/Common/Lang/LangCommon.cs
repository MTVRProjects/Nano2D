//================================================
//描述 ： 
//作者 ：
//创建时间 ：2020/09/18 16:56:42  
//版本： 
//================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HMLFramwork.Lang
{
    public enum LangType
    {
        CH,
        EN,
    }

    /// <summary>
    /// 单国语言
    /// </summary>
    public class Lang
    {
        /// <summary>
        /// 语言内类型
        /// </summary>
        public LangType type;
        /// <summary>
        /// 语言内容
        /// </summary>
        public string content;
        public Lang()
        {

        }
        public Lang(LangType type, string content)
        {
            this.type = type;
            this.content = content;
        }
        public Lang(string type, string content)
        {
            Enum.TryParse<LangType>(type, out this.type);
            this.content = content;
        }
    }

    /// <summary>
    /// 每一个Text对应的多国语言
    /// </summary>
    public class LangsData
    {
        public string ID;

        public Dictionary<LangType, Lang> langs;

        public LangsData()
        {

        }
        public LangsData(string ID, Dictionary<LangType, Lang> langs)
        {
            this.ID = ID;
            langs = this.langs;
        }
    }
}

