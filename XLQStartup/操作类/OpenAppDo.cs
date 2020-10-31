using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLQStartup.日志;

namespace XLQStartup.操作类
{
    /// <summary>
    /// 这个用来一开始，哈哈哈哈
    /// </summary>
    internal class OpenAppDo
    {
        /// <summary>
        /// 日志文件数据流
        /// </summary>
        private FileStream fs = null;
        /// <summary>
        /// 用于向日志数据流中写入字符
        /// </summary>
        private StreamWriter sw = null;
        private string baseDir = Directory.GetCurrentDirectory();
        /// <summary>
        /// 单例
        /// </summary>
        private static OpenAppDo unique = new OpenAppDo();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static OpenAppDo GetInstance()
        {
            if (unique == null)
            {
                unique = new OpenAppDo();
            }
            return unique;
        }
        /// <summary>
        /// 门锁对应信息文件
        /// </summary>
        private const string infopath = "lkconnect.lkct";
        /// <summary>
        /// 首先判断是否存在已知门锁对应信息
        /// </summary>
        /// <param name="status">返回的内容信息</param>
        /// <returns>0为返回，其他为失败</returns>
        public int judelkconnect(out int status)
        {
            status = 0;
            string LocalPath = baseDir + @"\" + infopath;
            try
            {
                fs = new FileStream(LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (fs.Length > 8)
                {
                    StreamReader read = new StreamReader(fs);
                    //循环读取每一行
                    string strReadline = string.Empty;
                    while ((strReadline = read.ReadLine()) != null)
                    {
                        //name
                        string temp = strReadline.Split(',')[0].ToString();
                        temp += ",";
                        //mac
                        temp += strReadline.Split(',')[1].ToString();
                        temp += ",";
                        //lastConnecttime
                        temp += strReadline.Split(',')[2].ToString();
                        //
                        EventClass.GetInstance().knowsLock.Add(temp);
                    }
                    RecordLog.GetInstance().WriteLog(Level.Info, "读取对应门锁信息文件成功");
                    status = 1;
                }                
                return 0;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("Exception:{0}", ex.Message));
                return -1;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }

        /// <summary>
        /// 修改已经存在的配对文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mac"></param>
        /// <param name="timedate"></param>
        public void ModifyLockTime(string name ,string mac,string timedate)
        {
            try
            {
                string LocalPath = baseDir + @"\" + infopath;
                //先删除指定行
                List<string> lines = new List<string>(File.ReadAllLines(LocalPath));//先读取到内存变量
                if (lines.Count != 0)
                {
                    string param = string.Empty;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (mac.Equals(lines[i].Split(',')[1].ToString()))
                        {
                            lines.RemoveAt(i);//指定删除的行
                            param = name + "," + mac + "," + timedate;
                            lines.Insert(i, param);
                        }
                    }
                    File.WriteAllLines(LocalPath, lines.ToArray());//在写回硬盤
                }
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("Exception:{0}", ex.Message));
            }
        }

        /// <summary>
        /// 记录对应门锁信息
        /// </summary>
        /// <param name="lockInfo"></param>
        public void AddLockMap(string name,string mac,string timedate)
        {
            string LocalPath = baseDir + @"\" + infopath;
            try
            {              
                string param = name + "," + mac + "," + timedate;
                fs = new FileStream(LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Seek(0, System.IO.SeekOrigin.End);
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(param);
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("Exception:{0}", ex.Message));
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }

        /// <summary>
        /// 删除lkconnect.lkct文件
        /// </summary>
        public void DeletInfoFile()
        {
            string LocalPath = baseDir + @"\" + infopath;
            try
            {
                if (File.Exists(LocalPath))
                {
                    File.Delete(LocalPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception" + ex.Message);
                return;
            }
        }
    }
}
