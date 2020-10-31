using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using XLQStartup.日志;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace XLQStartup.数据导出
{
    class DoExcelHelper
    {
        /// <summary>
        /// 函数原型；DWORD GetWindowThreadProcessld(HWND hwnd，LPDWORD lpdwProcessld);
        /// 参数：hWnd:窗口句柄
        /// 参数：lpdwProcessld:接收进程标识的32位值的地址。如果这个参数不为NULL，GetWindwThreadProcessld将进程标识拷贝到这个32位值中，否则不拷贝
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pid"></param>
        /// <returns>返回值为创建窗口的线程标识</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);
        

        private static DoExcelHelper instance;

        /// <summary>
        /// 单例
        /// </summary>
        public static DoExcelHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new DoExcelHelper();
            }
            return instance;
        }

        /// <summary>
        /// 保存为excel表格，并对表格进行同步
        /// </summary>
        /// <param name="dt">从数据库读取的数据</param>
        /// <param name="file_name">保存路径</param>
        /// <param name="sheet_name">表单名称</param>
        public void DataTableToExcel(System.Data.DataTable dt, string file_name, string sheet_name)
        {
            Microsoft.Office.Interop.Excel.Application Myxls = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook Mywkb = Myxls.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet MySht = Mywkb.ActiveSheet;
            MySht.Name = sheet_name;
            Myxls.Visible = false;
            Myxls.DisplayAlerts = false;
            try
            {
                //写入表头
                object[] arrHeader = new object[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    arrHeader[i] = dt.Columns[i].ColumnName;
                }
                MySht.Range[MySht.Cells[1, 1], MySht.Cells[1, dt.Columns.Count]].Value2 = arrHeader;
                //写入表体数据
                object[,] arrBody = new object[dt.Rows.Count, dt.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        arrBody[i, j] = dt.Rows[i][j].ToString();
                    }
                }
                MySht.Range[MySht.Cells[2, 1], MySht.Cells[dt.Rows.Count + 1, dt.Columns.Count]].Value2 = arrBody;
                //设置格式 begin
                Range range = MySht.Range[MySht.Cells[2, 1], MySht.Cells[dt.Rows.Count + 1, 1]];
                range.NumberFormat = @"yyyy-MM-dd HH:mm:ss"; //日期格式
                range.EntireColumn.AutoFit();//自动调整列宽
                //设置格式 end
                if (Mywkb != null)
                {
                    Mywkb.SaveAs(file_name, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7); //Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7 保证excel一致性
                    Mywkb.Close(Type.Missing, Type.Missing, Type.Missing);
                    Mywkb = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");
            }
            finally
            {
                //彻底关闭Excel进程
                if (Myxls != null)
                {
                    Myxls.Quit();
                    try
                    {
                        if (Myxls != null)
                        {
                            int pid;
                            GetWindowThreadProcessId(new IntPtr(Myxls.Hwnd), out pid);
                            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                            p.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("结束当前EXCEL进程失败：" + ex.Message);
                    }
                    Myxls = null;
                }
                GC.Collect();
            }
        }

        /// <summary>
        /// 保存为excel表格，并对表格进行同步
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool DataGridtoExcelModify(System.Data.DataTable dt)
        {
            #region
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl 表格文件 (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.Title = "导出Excel文件";
            //设置默认文件名称
            saveFileDialog.FileName = "报表-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xls";
            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }
            if (saveFileDialog.FileName == "")
                return false;
            Stream myStream;
            myStream = saveFileDialog.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            object str = string.Empty;
            try
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += dt.Columns[i].ColumnName;
                }
                sw.WriteLine(str);
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    object tempStr = string.Empty;
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        tempStr += dt.Rows[j][k].ToString();
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
                return true;
            }

            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                RecordLog.GetInstance().WriteLog(Level.Error, "DataGridtoExcel:" + ex.Message);
                return true;
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
            #endregion
        }
        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public System.Data.DataTable ImportExcelToDataTable(string tablename)
        {
            try
            {
                string path = string.Empty;
                OpenFileDialog pOpenFileDialog = new OpenFileDialog();
                pOpenFileDialog.Filter = "Execl 表格文件 (*.xls)| *.xls";//若打开指定类型的文件只需修改Filter，如打开txt文件，改为*.txt即可
                pOpenFileDialog.Multiselect = false;
                pOpenFileDialog.Title = "打开Execl 表格文件";
                if (pOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = pOpenFileDialog.FileName;
                }
                string conStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data source={0}; Extended Properties=Excel 12.0;", path);
                using (OleDbConnection conn = new OleDbConnection(conStr))
                {
                    conn.Open();
                    //获取所有Sheet的相关信息
                    System.Data.DataTable dtSheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                    //获取第一个 Sheet的名称
                    string sheetName = dtSheet.Rows[0]["Table_Name"].ToString();
                    string sql = string.Format("select * from [{0}]", sheetName);
                    using (OleDbDataAdapter oda = new OleDbDataAdapter(sql, conn))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable(tablename);
                        Task ta = Task.Factory.StartNew(delegate { oda.Fill(dt); });
                        Task.WaitAny(ta);
                        return dt;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("选择导入文件异常", "系统提示");
                RecordLog.GetInstance().WriteLog(Level.Error, "导入错误：" + ex.Message);
                return null;
            }
        }
    }
}
