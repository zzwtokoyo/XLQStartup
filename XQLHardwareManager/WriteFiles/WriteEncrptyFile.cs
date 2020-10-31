using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XQLHardwareManager.WriteFiles
{
    internal class WriteEncrptyFile
    {
        /// <summary>
        /// 日志文件数据流
        /// </summary>
        private FileStream fs = null;
        /// <summary>
        /// 用于向日志数据流中写入字符
        /// </summary>
        private StreamWriter sw = null;
        /// <summary>
        /// 基础路径
        /// </summary>
        public string baseDir = Directory.GetCurrentDirectory();

        private WriteEncrptyFile()
        {

        }
        /// <summary>
        /// 单例
        /// </summary>
        private static WriteEncrptyFile unique = new WriteEncrptyFile();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static WriteEncrptyFile GetInstance()
        {
            if (unique == null)
            {
                unique = new WriteEncrptyFile();
            }
            return unique;
        }

        public void DeletFile(string path)
        {
            try
            {               
                if(File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception" + ex.Message);
                return;
            }
        }

        /// <summary>
        /// 写授权文件
        /// </summary>
        /// <param name="info"></param>
        public void WriteFile(string info)
        {
            lock (this)
            {
                try
                {
                    string LocalPath = baseDir + "\\" + getFileName();
                    fs = new FileStream(LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Seek(0, System.IO.SeekOrigin.End);
                    sw = new StreamWriter(fs, System.Text.Encoding.UTF8);                   
                    sw.WriteLine("{0}", info);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception" + ex.Message);
                    return;
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
        }

        /// <summary>
        /// 读取加密数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> ReadEncrptyFile(string path)
        {
            List<string> vsList = new List<string>();
            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader read = new StreamReader(fs);
                //循环读取每一行
                string strReadline = string.Empty;
                while ((strReadline = read.ReadLine()) != null)
                {
                    vsList.Add(strReadline);
                }
                return vsList;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception" + ex.Message);
                return vsList;
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
        /// 返回根据当前日期所创建的文件名
        /// </summary>
        /// <returns></returns>
        public string getFileName()
        {
            DateTime dt = DateTime.Now;
            string date = "LicensesData";
            return date + ".licd";
        }
    }
}
