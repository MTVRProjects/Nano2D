//================================================
//描述 ： 
//作者 ：
//创建时间 ：2018/09/11 17:34:52  
//版本： 
//================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using HMLFramwork;
using System.IO;
using NAudio.Wave;
using HMLFramwork.Log;

namespace HMLFramwork.Helpers
{
    public class AudioHelper
    {
        /// <summary>
        /// MP3文件转WAV文件
        /// </summary>
        /// <param name="ori_file_path">原MP3文件路径</param>
        /// <param name="_savePath">转成wav文件后的存储路径</param>
        public static void MP3_TO_WAV(string ori_file_path,string _savePath)
        {
            LogQueue.Add("开始转换：" + ori_file_path);
            if (File.Exists(ori_file_path))
            {
                LogQueue.Add("正在转换：" + ori_file_path);
                var stream = File.Open(ori_file_path, FileMode.Open);
                var reader = new Mp3FileReader(stream);
                WaveFileWriter.CreateWaveFile(_savePath, reader);
                
            }
        }
       
    }
}