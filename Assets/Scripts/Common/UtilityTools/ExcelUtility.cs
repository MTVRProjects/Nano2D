//================================================
//描述 ： Excel表通用工具（Excel表的路径默认为流资源文件夹）
//作者 ：HML
//创建时间 ：2018/07/14 09:15:12  
//版本： 1.0
//================================================
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System.IO;
using System.Data;

namespace HMLFramwork.Helpers
{
    /// <summary>
    /// Excel表通用工具（Excel表的路径默认为流资源文件夹）
    /// </summary>
    public class ExcelUtility
    {
        /// <summary>
        /// 获取Excel表数据对象（Excel表默认存储路径为流资源文件夹）
        /// 读Excel的文件流需要传出去，以便在使用后关闭
        /// </summary>
        /// <param name="excel_fullName">Excel表全名（带扩展）</param>
        /// <param name="_fileStream">读Excel的文件流，需要传出去，以便在使用后关闭</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string excel_fullName)
        {
            DataSet excelData = null;
            FileStream stream = File.Open(Path.Combine(Application.streamingAssetsPath, excel_fullName), FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelData = excelReader.AsDataSet();
            if (stream != null) stream.Close();
            return excelData;
        }

        /// <summary>
        /// 读某一行或者列的数据
        /// </summary>
        /// <param name="excel_fullName">Excel表全名（带扩展）</param>
        /// <param name="targetIndex">目标行或者列下标</param>
        /// <param name="start">起始列或者行的下标(包含此列或行)</param>
        /// <param name="end">终止列或者行的下标(包含此列或行)</param>
        /// <returns></returns>
        public static List<string> ReadData(string excel_fullName, int targetIndex, int start, int end, ExcelStruc es)
        {
            List<string> _dataList = new List<string>();
            DataSet excelData = GetDataSet(excel_fullName);
            //总列数
            int colus = excelData.Tables[0].Columns.Count;
            //总行数
            int rows = excelData.Tables[0].Rows.Count;

            if (es == ExcelStruc.ROW) //读取某一行数据
            {
                //限制start范围
                start = start > 0 && start <= colus ? start : 1;
                //当终止列下标大于总列数时，读到最后一列即止
                end = end > start && end <= colus ? end : colus - 1;
                //当目标行下标大于总行数时，即读最后一行
                targetIndex = targetIndex < rows ? targetIndex : rows - 1;

                for (int i = start - 1; i <= end; i++)
                {
                    string value = excelData.Tables[0].Rows[targetIndex][i].ToString();
                    _dataList.Add(value);
                }
            }
            else if (es == ExcelStruc.COLU)
            {
                //限制start范围
                start = start > 0 && start <= rows ? start : 1;
                //当终止行下标大于总行数时，读到最后行即止
                end = end > start && end <= rows ? end : rows - 1;
                //当目标列下标大于总列数时，即读最后一列
                targetIndex = targetIndex < colus ? targetIndex : colus - 1;
                start = start == 0 ? 1 : start;
                for (int i = start - 1; i <= end; i++)
                {
                    string value = excelData.Tables[0].Rows[i][targetIndex].ToString();
                    _dataList.Add(value);
                }
            }
            return _dataList;
        }

        /// <summary>
        /// 读某一行的数据
        /// </summary>
        /// <param name="excel_fullName">Excel表全名（带扩展）</param>
        /// <param name="targetIndex">目标行下标</param>
        /// <param name="start">起始列的下标(包含此列)</param>
        /// <param name="end">终止列的下标(包含此列)</param>
        public static List<string> ReadRow(string excel_fullName, int targetIndex, int start, int end)
        {
            return ReadData(excel_fullName, targetIndex, start, end, ExcelStruc.ROW);
        }

        /// <summary>
        /// 读某一列的数据
        /// </summary>
        /// <param name="excel_fullName">Excel表全名（带扩展）</param>
        /// <param name="targetIndex">目标列下标</param>
        /// <param name="start">起始行的下标(包含此行)</param>
        /// <param name="end">终止行的下标(包含此行)</param>
        public static List<string> ReadColu(string excel_fullName, int targetIndex, int start, int end)
        {
            return ReadData(excel_fullName, targetIndex, start, end, ExcelStruc.COLU);
        }

    }

    /// <summary>
    /// Excel表结构（行？列）
    /// </summary>
    public enum ExcelStruc
    {
        /// <summary>
        /// 行
        /// </summary>
        ROW,
        /// <summary>
        /// 列
        /// </summary>
        COLU,
    }
}
