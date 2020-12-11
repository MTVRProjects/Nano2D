//================================================
//描述 ： Excel表读取
//作者 ：HML
//创建时间 ：2018/07/14 11:35:07  
//版本： 1.0
//================================================

using Excel;
using HMLFramwork.Log;
using System.Data;
using System.IO;

namespace HMLFramwork.Helpers
{
    public class ExcelReader
    {
        private string _excelPath = string.Empty;

        private int _rowCount = 0;
        /// <summary>
        /// 总行数（默认为第一张表的数据）
        /// </summary>
        public int rowCount { get { return _rowCount; } }

        private int _coluCount = 0;
        /// <summary>
        /// 总列数（默认为第一张表的数据）
        /// </summary>
        public int coluCount { get { return _coluCount; } }

        private DataRowCollection _rows;
        /// <summary>
        /// 所有行数据（默认为第一张表的数据）
        /// </summary>
        public DataRowCollection Rows { get { return _rows; } }
        private DataColumnCollection _colus;
        /// <summary>
        /// 所有列数据（默认为第一张表的数据）
        /// </summary>
        public DataColumnCollection Colus { get { return _colus; } }

        private DataTableCollection _dataTable;
        /// <summary>
        /// 表格集合
        /// </summary>
        public DataTableCollection Tables { get { return _dataTable; } }

        private DataSet _excelData;
        /// <summary>
        /// Excel数据容器
        /// </summary>
        public DataSet ExcelData { get { return _excelData; } }

        public ExcelReader(string excelPath)
        {
            _excelPath = excelPath;
            if (!string.IsNullOrEmpty(_excelPath))
            {
                FileStream stream = File.Open(_excelPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                if (excelReader.ResultsCount > 0)
                {
                    _excelData = excelReader.AsDataSet();
                    _dataTable = _excelData.Tables;
                    _rows = _dataTable[0].Rows;
                    _colus = _dataTable[0].Columns;
                    _rowCount = _rows.Count;
                    _coluCount = _colus.Count;

                    if (excelReader != null) excelReader.Close();
                    if (stream != null) stream.Close();
                }
                else
                {
                    LogQueue.Add("表格读取错误...");
                }
            }
            else LogQueue.Add("表格存储路径错误...");
        }

    }
}
