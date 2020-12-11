﻿//================================================//描述 ： 版本管理器//作者 ：HML//创建时间 ：2018/12/24 11:07:26  //版本： 1.0//================================================using HMLFramwork.Helpers;using System;using System.IO;using UnityEngine;namespace HMLFramwork{    public class VersionMgr : HMLFramwork.Singleton.SingleInstance<VersionMgr>    {        private string _versionPath = Helpers.PathHelper.getVersionPath;        private Version _cur_Version;        public Version Cur_Version        {            set            {                _cur_Version = value;                string ver_str = _cur_Version.ToString();                PlayerPrefs.SetString("version", ver_str);                FileHelper.Save(String.Format("{0}:{1}", "version", ver_str), _versionPath, "version", ".ver", true, true);            }            get            {                if (!PlayerPrefs.HasKey("version"))                {                    string filePath = Path.Combine(_versionPath, "version.ver");                    if (!File.Exists(filePath))                    {                        _cur_Version = new Version(1, 0, 0);                    }                    else _cur_Version = new Version(FileHelper.GetStrFromFile(filePath).Remove(0, 8));                }                else _cur_Version = new Version(PlayerPrefs.GetString("version"));                return _cur_Version;            }        }        public string Cur_VersionStr        {            set            {                Cur_Version = new Version(value);            }            get            {                return Cur_Version.ToString();            }        }    }}