using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using XLQStartup.操作类;

namespace XLQStartup.日志
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// 只记录异常信息
        /// </summary>
        Error = 1,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 正常的日志流水(一些敏感信息不记录)
        /// </summary>
        Info,
        /// <summary>
        /// 调试信息(最为详细)
        /// </summary>
        Trace,
    }
    internal class RecordLog
    {
        /// <summary>
        /// 日志文件数据流
        /// </summary>
        private FileStream fs = null;
        /// <summary>
        /// 用于向日志数据流中写入字符
        /// </summary>
        private StreamWriter sw = null;

        private string baseDir = Directory.GetCurrentDirectory() /*+ @"\Log"*/;
        /// <summary>
        /// 配置文件中的日志级别
        /// </summary>
        public Level ConfigLogLevel = Level.Info;
        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private RecordLog()
        {

        }
        /// <summary>
        /// 单例
        /// </summary>
        private static RecordLog unique = new RecordLog();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static RecordLog GetInstance()
        {
            if (unique == null)
            {
                unique = new RecordLog();
            }
            return unique;
        }        

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="info">需要记录的日志信息</param>
        /// <returns></returns>
        public void WriteLog(Level level, string info)
        {
            lock (this)
            {
                if(EventClass.GetInstance().RecordLogFunc == "false")
                {
                    return;
                }
                if (level > ConfigLogLevel)
                {
                    return;
                }
                try
                {
                    string logDir = baseDir + @"\Log" + DateTime.Now.ToString("yyyyMM");
                    if (!Directory.Exists(logDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    fs = new FileStream(logDir + "\\" + getFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Seek(0, System.IO.SeekOrigin.End);
                    sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    DateTime dt = DateTime.Now;
                    string tempTime = "[" + dt.ToString("HH:mm:ss:fff") + "]";
                    string tempLevel = "[" + level.ToString() + "]";
                    sw.WriteLine("{0}{1}{2}", tempTime, tempLevel, info);
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
        /// 返回根据当前日期所创建的文件名
        /// </summary>
        /// <returns></returns>
        private string getFileName()
        {
            DateTime dt = DateTime.Now;
            string date = "XLQ" + dt.ToString("yyyyMMdd");
            return date + ".log";
        }
    }
}
