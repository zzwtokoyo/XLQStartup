using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XLQStartup.日志;

namespace XLQStartup
{
    class LicenseHelper
    {
        //路径
        private string baseDir = Directory.GetCurrentDirectory();
        private const string licName = "LicensesData.licd";
        private string LiceneseDir = string.Empty;
        private const string MasterKey = "ACD34AEE12345678AEEEFDEC";

        /// <summary>
        /// 判断listeners文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsHaveListenersFile()
        {
            try
            {
                LiceneseDir = baseDir + @"\" + licName;
                if (!File.Exists(LiceneseDir))
                {
                    RecordLog.GetInstance().WriteLog(Level.Error, "授权文件不存在");
                    return false;
                }
                RecordLog.GetInstance().WriteLog(Level.Info, "检测到授权文件");
                return true;
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("检查授权文件异常：{0}",ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 解析授权文件，返回存在的加密数据
        /// </summary>
        /// <returns></returns>
        public List<string> ReturnCheckData()
        {
            string file = LiceneseDir;
            /// <summary>
            /// 日志文件数据流
            /// </summary>
            FileStream fs = null;
            List<string> outlist = new List<string>();
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                //fs.Seek(0, System.IO.SeekOrigin.End);
                StreamReader streamReader = new StreamReader(fs);
                string line = string.Empty;
                while ((line = streamReader.ReadLine()) != null)
                {
                    outlist.Add(line);
                }
                return outlist;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("读取授权文件异常:{0}", ex.Message));
                return outlist;
            }
            finally
            {               
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }

        /// <summary>
        /// 判断密钥是否正确
        /// </summary>
        /// <param name="sourdata"></param>
        /// <param name="destdata"></param>
        /// <returns></returns>
        public bool CheckLockDecValue(string sourdata,List<string> destdata)
        {
            try
            {
                foreach (var dest in destdata)
                {
                    if (sourdata.Equals(dest))
                    {
                        RecordLog.GetInstance().WriteLog(Level.Info, string.Format("数据授权成功"));
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("判断授权密钥异常:{0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="loackname"></param>
        /// <param name="loackmac"></param>
        /// <param name="checkvalue"></param>
        /// <returns></returns>
        public bool EncrptyData(string loackname,string loackmac,out string checkvalue)
        {
            checkvalue = string.Empty;
            string readEncdata = loackname + "," + loackmac + "," + FingerPrint.Value().ToString();

            try
            {
                checkvalue = this.Encrypt3Des(readEncdata, MasterKey, CipherMode.CBC);

                if (checkvalue == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        #region 3des加密
        /// <summary>
        /// 3des ecb模式加密
        /// </summary>
        /// <param name="aStrString">待加密的字符串</param>
        /// <param name="aStrKey">密钥</param>
        /// <param name="iv">加密矢量：只有在CBC解密模式下才适用</param>
        /// <param name="mode">运算模式</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.UTF8.GetBytes(aStrKey),
                    Mode = mode
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Encoding.UTF8.GetBytes(iv);
                }
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = Encoding.UTF8.GetBytes(aStrString);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception ex)
            {                
                return string.Empty;
            }
        }
        #endregion

        #region 3des解密
        /// <summary>
        /// des 解密
        /// </summary>
        /// <param name="aStrString">加密的字符串</param>
        /// <param name="aStrKey">密钥</param>
        /// <param name="iv">解密矢量：只有在CBC解密模式下才适用</param>
        /// <param name="mode">运算模式</param>
        /// <returns>解密的字符串</returns>
        public string Decrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.UTF8.GetBytes(aStrKey),
                    Mode = mode,
                    Padding = PaddingMode.PKCS7
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Encoding.UTF8.GetBytes(iv);
                }
                var desDecrypt = des.CreateDecryptor();
                var result = string.Empty;
                byte[] buffer = Convert.FromBase64String(aStrString);
                result = Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                return result;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
