//================================================
//描述 ：多语言工具 
//作者 ：HML
//创建时间 ：2020/09/18 16:53:30  
//版本：1.0 
//================================================
using HMLFramwork.Res;
using HMLFramwork.Singleton;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HMLFramwork.Lang
{
    public class LanguageMgr : MonoSingleton<LanguageMgr>
    {
        void Awake()
        {
            ReadLangFile();
        }
        string file_name = "/Configs/Lang/Language.csv";

        Dictionary<string, LangsData> langsDataDic = new Dictionary<string, LangsData>();
        public Dictionary<string, LangsData> getLangDataDic
        {
            get { return langsDataDic; }
        }

        void ReadLangFile()
        {
            DataLoaderCenter.Add(Application.streamingAssetsPath + file_name, loadFileDone);
        }

        LangsData langData_temp;
        Dictionary<LangType, Lang> langs_temp;
        void loadFileDone(string content)
        {
            string content_temp = content.Replace(System.Environment.NewLine, " ");
            string[] langs = content_temp.Split(' ');
            string[] lang_title = langs[0].Split(',');//数组最后一个无数据
            for (int i = 1; i < langs.Length - 1; i++)
            {
                langData_temp = new LangsData();
                langs_temp = new Dictionary<LangType, Lang>();

                string[] row_temp = langs[i].Split(',');
                langData_temp.ID = row_temp[0];

                for (int j = 2; j < row_temp.Length; j++)
                {
                    Lang lang_temp = new Lang(lang_title[j], row_temp[j]);
                    langs_temp.Add(lang_temp.type, lang_temp);
                }

                langData_temp.langs = langs_temp;
                langsDataDic.Add(langData_temp.ID, langData_temp);
            }
        }

        public Lang getLangContent(string ID,string lang_type)
        {
            LangType langTypeTemp;
            if (Enum.TryParse<LangType>(lang_type, out langTypeTemp))
            {
                return getLangContent(ID, langTypeTemp);
            }
            return null;
        }

        public Lang getLangContent(string ID, LangType lang_type)
        {
            LangsData langData;
            Lang lang = default(Lang);
            if (langsDataDic.TryGetValue(ID, out langData))
            {
                if (langData.langs.ContainsKey(lang_type))
                {
                    langData.langs.TryGetValue(lang_type, out lang);
                }
            }
            return lang;
        }

    }
}
