using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLQStartup.全局类;
using XLQStartup.操作类;
using XLQStartup.日志;

namespace XLQStartup.记录数据文件类
{
    /// <summary>
    /// 根据用户的需求重新组织返显的数据，将文件操作模拟成数据库操作
    /// </summary>
    internal class RedvelopRecord
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
        /// 程序Base目录
        /// </summary>
        private string baseDir = Directory.GetCurrentDirectory();
        /// <summary>
        /// 
        /// </summary>
        byte[] pReadByte = new byte[3000];
        /// <summary>
        /// 创建文件夹及文件
        /// </summary>
        private const string path = @"\userdata";
        private const string usersettingpath = @"\usersetting";
        private const string doorlocksettingpath = @"\doorlocksetting";
        private const string doorlocktypesettingpath = @"\doorlocktypesetting";
        private const string userfile = "userrecord.dat";
        private const string warningfile = "warningrecord.dat";
        private const string usersettingfile = "usersetting.dat";
        private const string doorlocksettingfile = "locksetting.dat";
        private const string doorlocktypesettingfile = "locktypesetting.dat";
        /// <summary>
        /// 单例
        /// </summary>
        private static RedvelopRecord unique = new RedvelopRecord();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static RedvelopRecord GetInstance()
        {
            if (unique == null)
            {
                unique = new RedvelopRecord();
            }
            return unique;
        }

        #region 将对象序列化到文件
        /// <summary>
        /// 将对象序列化到文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pathfile"></param>
        public void BinaryFileSerialize(object obj, string pathfile)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, obj);
                    pReadByte = new byte[ms.GetBuffer().Length];
                    pReadByte = ms.GetBuffer();
                }
                string str = string.Empty;
                for (int i = 0; i < pReadByte.Length; i++)
                {
                    str += pReadByte[i].ToString("X2");
                }
                fs = new FileStream(pathfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Seek(0, System.IO.SeekOrigin.End);
                //文件节流点
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(str);
            }
            catch(Exception ex)
            {

                MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  对象序列化有误!\n 异常信息:"+ex.Message, "错误!");
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
        #endregion

        #region 将二进制文件反序列化到对象
        /// <summary>
        /// 将二进制文件反序列化到对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public object BinaryFileDeserialize(string path)
        {
            object obj = new object();
            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader read = new StreamReader(fs);
                //循环读取每一行
                string strReadline = string.Empty;
                while ((strReadline = read.ReadLine()) != null)
                {
                    //对每一行数据做反序列化处理
                    if (strReadline.Length % 2 != 0)
                    {
                        strReadline = "0" + strReadline;
                    }
                    byte[] binReadline = new byte[strReadline.Length / 2];
                    for (int i = 0; i < binReadline.Length; i++)
                    {
                        string b = strReadline.Substring(i * 2, 2);
                        binReadline[i] = Convert.ToByte(b, 16);
                    }
                    using (MemoryStream ms = new MemoryStream(binReadline))
                    {
                        IFormatter iFormatter = new BinaryFormatter();
                        obj = iFormatter.Deserialize(ms);
                    }
                }
                return obj;
            }
            catch
            {
                MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  反序列化信息异常!", "错误!");
                return obj;
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
        #endregion

        #region 获取当前门锁配置信息
        /// <summary>
        /// 获取当前门锁配置信息
        /// </summary>
        /// <returns></returns>
        public List<lockDetail> GetlockSetting(out string msg)
        {
            msg = string.Empty;
            List<lockDetail> lockDetailsList = new List<lockDetail>();
            lockDetail lockDetail = new lockDetail();
            try
            {
                string dataDir = baseDir + path + doorlocksettingpath;
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles("*.dat").Count() == 0)
                {
                    msg = string.Format("门锁配置文件不存在");
                    return null;
                }
                foreach (FileInfo file in folder.GetFiles("*.dat"))
                {
                    string savefile = dataDir + "\\" + file;
                    lockDetail = (lockDetail)BinaryFileDeserialize(savefile);
                    lockDetailsList.Add(lockDetail);
                }
                return lockDetailsList;
            }
            catch(Exception ex)
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Exception:" + ex.Message;
                return null;
            }
        }
        #endregion

        #region 删除指定的门锁
        /// <summary>
        /// 删除指定的门锁
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="macid"></param>
        public void DeletlockSetting(out string msg,string macid)
        {
            msg = string.Empty;
            try
            {
                string dataDir = baseDir + path + doorlocksettingpath;
                string file = macid + "_"+ doorlocksettingfile;
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles(file).Count() < 1)
                {
                    msg = string.Format("门锁配置文件不存在");
                    return;
                }
                string savefile = dataDir + "\\" + file;
                File.Delete(savefile);
                msg = "Success";
            }
            catch(Exception ex)
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Exception:" + ex.Message;
                return;
            }
        }
        #endregion


        public void DeletSettingFile(string mac)
        {
            try
            {
                string dataDir = baseDir + path + doorlocksettingpath;
                string savefile = dataDir + "\\" + mac + "_" + doorlocksettingfile;

                if(File.Exists(savefile))
                {
                    File.Delete(savefile);
                    RecordLog.GetInstance().WriteLog(Level.Info,"已删除门锁配置文件");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("删除配置文件异常:" + ex.Message);
                return;
            }
        }

        #region 添加门锁设置
        /// <summary>
        /// 添加门锁设置
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="lockid">门锁唯一的mac地址</param>
        /// <param name="lockName"></param>
        public void Regedit2DoorSetting(out string msg, string lockid, string lockName = "")
        {
            lock (this)
            {
                msg = string.Empty;
                try
                {
                    string dataDir = baseDir + path + doorlocksettingpath;
                    string savefile = dataDir + "\\" + lockid + "_" + doorlocksettingfile;

                    if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(dataDir);
                    }
                    lockDetail lockDetail = new lockDetail
                    {
                        _id = lockid,
                        _modifyName = lockName
                    };
                    OpenAppDo.GetInstance().ModifyLockTime(lockName, lockid, "");
                    BinaryFileSerialize(lockDetail, savefile);
                    msg = "Success";
                }
                catch
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  门锁设置信息保存有误!", "错误!");
                    return;
                }
            }
        }
        #endregion

        #region 添加门锁设置
        /// <summary>
        /// 添加门锁设置
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="lockid">门锁唯一的mac地址</param>
        /// <param name="lockName"></param>
        public void RegeditDoorSetting(out string msg, string lockid, string lockName = "")
        {
            lock (this)
            {
                msg = string.Empty;
                try
                {
                    string dataDir = baseDir + path + doorlocksettingpath;
                    string savefile = dataDir + "\\" + lockid + "_" + doorlocksettingfile;

                    if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(dataDir);
                    }                   
                    lockDetail lockDetail = new lockDetail
                    {
                        _id = lockid,
                        _modifyName = lockName
                    };
                    OpenAppDo.GetInstance().AddLockMap(lockName, lockid, "");
                    BinaryFileSerialize(lockDetail, savefile);
                    msg = "Success";
                }
                catch
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  门锁设置信息保存有误!", "错误!");
                    return;
                }
            }
        }
        #endregion

        #region 删除指定的用户信息
        /// <summary>
        /// 删除指定的用户信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="macid"></param>
        public void DeletUserInfoSetting(out string msg, string name)
        {
            msg = string.Empty;
            try
            {
                string dataDir = baseDir + path + usersettingpath;
                string file = name + "_" + usersettingfile;
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles(file).Count() < 1)
                {
                    msg = string.Format("用户信息文件不存在");
                    return;
                }
                string savefile = dataDir + "\\" + file;
                File.Delete(savefile);
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Exception:" + ex.Message;
                return;
            }
        }
        #endregion

        #region 从用户信息配置文件获取用户信息对象
        /// <summary>
        /// 从用户信息配置文件获取用户信息对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public List<userDetails> getuserDetails(out string msg)
        {
            msg = string.Empty;
            userDetails userDetails = new userDetails() { };
            List<userDetails> userDetailsList = new List<userDetails>();
            try
            {
                string dataDir = baseDir + path + usersettingpath;
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles("*.dat").Count() == 0)
                {
                    msg = string.Format("用户信息配置文件不存在");
                    return null;
                }
                foreach (FileInfo file in folder.GetFiles("*.dat"))
                {
                    string savefile = dataDir + "\\" + file;
                    userDetails = (userDetails)BinaryFileDeserialize(savefile);
                    userDetailsList.Add(userDetails);
                }
                return userDetailsList;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
        #endregion

        #region 用户列表中添加用户
        /// <summary>
        /// 用户列表中添加用户
        /// </summary>
        /// <param name="msg">返回的消息结果</param>
        /// <param name="id">唯一的用户编号</param>
        /// <param name="username"></param>
        /// <param name="usersex"></param>
        /// <param name="userjob"></param>
        /// <param name="lockMacid">门锁的编号信息</param>
        public void RegeditUser(out string msg, string id,string username, string usersex, string userjob,string lockMacid)
        {
            lock (this)
            {
                msg = string.Empty;
                try
                {
                    string dataDir = baseDir + path + usersettingpath;
                    string savefile = dataDir + "\\" + username + "_" + usersettingfile;

                    if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(dataDir);
                    }
                    //遍历文件名称
                    DirectoryInfo folder = new DirectoryInfo(dataDir);
                    foreach (FileInfo file in folder.GetFiles("*.dat"))
                    {
                        if (username == file.Name.ToString().Split('_')[0].ToString())
                        {
                            msg = string.Format("当前用户已经存在，username：{0}", username);
                            return;
                        }
                    }
                    //写配置文件:用户信息加对应的上门锁信息
                    userDetails userDetails = new userDetails
                    {        
                        _useId = id,
                        _useName = username,
                        _useSex = usersex,
                        _userjob = userjob,
                        _lockDetail_id = lockMacid
                    };
                    BinaryFileSerialize(userDetails, savefile);

                    //this.RegeditDoorLockType(out msg, "", "", username,"", 0, "", 0);

                    msg = "Success";
                }
                catch
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  用户设置信息保存有误!", "错误!");
                    return;
                }
            }
        }
        #endregion
        #region 用户列表中修改用户
        /// <summary>
        /// 用户列表中修改用户
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="oldname"></param>
        /// <param name="usersex"></param>
        /// <param name="userjob"></param>
        /// <param name="lockMacid"></param>
        public void RegeditUser(out string msg,string newname, string username , 
            string usersex, string userjob)
        {
            lock (this)
            {
                msg = string.Empty;
                try
                {
                    string dataDir = baseDir + path + usersettingpath;     
                    string oldfile = dataDir + "\\" + username + "_" + usersettingfile;
                    if (!File.Exists(oldfile))// 目录不存在,新建目录
                    {
                        msg = "原用户设置不存在";
                        MessageBox.Show("原用户设置文件不存在", "系统提示");
                        return;
                    }
                    userDetails oldDetails = (userDetails)BinaryFileDeserialize(oldfile);
                    string savefile = dataDir + "\\" + newname + "_" + usersettingfile;
                    System.IO.File.Move(oldfile, savefile);
                    //写配置文件:用户信息加对应的上门锁信息
                    userDetails newDetails = new userDetails();
                    newDetails = oldDetails;
                    newDetails._useName = newname;
                    newDetails._useSex = usersex;
                    newDetails._userjob = userjob;

                    List<string> lines = new List<string>(File.ReadAllLines(savefile));//先读取到内存变量
                    if(lines.Count() > 0)
                    {
                        lines.RemoveAt(0);//指定删除的行
                        File.WriteAllLines(savefile, lines.ToArray());//在写回硬盤
                    }                    
                    BinaryFileSerialize(newDetails, savefile);
                    msg = "Success";
                }
                catch
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  用户设置信息保存有误!", "错误!");
                    return;
                }
            }
        }
        #endregion

        /// <summary>
        /// 用于设置用户门锁开锁方式时对应设置用户信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="usersex"></param>
        /// <param name="userjob"></param>
        /// <param name="lockMacid"></param>
        public void InsterUserInfoType(out string msg, string id, string username, string usersex, string userjob, string lockMacid)
        {
            lock (this)
            {
                msg = string.Empty;
                try
                {
                    string dataDir = baseDir + path + usersettingpath;
                    string savefile = dataDir + "\\" + username + "_" + usersettingfile;
                    if (!File.Exists(savefile))// 目录不存在,新建目录
                    {
                        msg = "原用户设置不存在";
                        MessageBox.Show("原用户设置文件不存在", "系统提示");
                        return;
                    }
                    //写配置文件:用户信息加对应的上门锁信息
                    userDetails newDetails = new userDetails
                    {
                        _useId = id,
                        _useName = username,
                        _useSex = usersex,
                        _userjob = userjob,
                        _lockDetail_id = lockMacid
                    };

                    List<string> lines = new List<string>(File.ReadAllLines(savefile));//先读取到内存变量
                    if (lines.Count() > 0)
                    {
                        lines.RemoveAt(0);//指定删除的行
                        File.WriteAllLines(savefile, lines.ToArray());//在写回硬盤
                    }
                    BinaryFileSerialize(newDetails, savefile);
                    msg = "Success";
                }
                catch
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  用户设置信息保存有误!", "错误!");
                    return;
                }
            }
        }

        #region 删除指定的用户配置开锁方式
        /// <summary>
        /// 删除指定的用户配置开锁方式
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="id">已设置的用户Id</param>
        public void DeletLocalSettingType(out string msg, string id ,string opentype)
        {
            msg = string.Empty;
            try
            {
                string lockmac = EventClass.GetInstance()._ConnectBleAddress;
                string dataDir = baseDir + path + doorlocktypesettingpath;
                string file = lockmac + "_" + id + "_" + opentype + "_" + doorlocktypesettingfile;
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles(file).Count() < 1)
                {
                    msg = string.Format("开锁方式文件不存在");
                    return;
                }
                string savefile = dataDir + "\\" + file;
                File.Delete(savefile);
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Exception:" + ex.Message;
                return;
            }
        }
        #endregion


        #region 初始化开锁方式文件
        /// <summary>
        /// 初始化开锁方式文件，先将门锁关系关联起来
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="lockid"></param>
        /// <param name="opentype"></param>
        public void InitDoorLockType(out string msg, string lockid, string opentype)
        {
            lock (this)
            {
                msg = string.Empty;
                lockSettingType lockSettingType = new lockSettingType();
                try
                {
                    string lockmac = EventClass.GetInstance()._ConnectBleAddress;
                    string dataDir = baseDir + path + doorlocktypesettingpath;
                    string savefile = dataDir + "\\" + lockmac + "_" + lockid + "_" + opentype + "_" + doorlocktypesettingfile;
                    if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(dataDir);
                    }
                    lockSettingType.userDetails._useId = lockid;                //用户ID
                    lockSettingType.userDetails._lockDetail_id = lockmac;       //蓝牙mac
                    //获取门锁名称 start
                    string msg1 = string.Empty;
                    string lockmname = string.Empty;
                    List<lockDetail> lockDetaillist = GetInstance().GetlockSetting(out msg1);
                    foreach (lockDetail lockDetail in lockDetaillist)
                    {
                        if (lockDetail._id.Equals(lockmac))
                        {
                            lockmname = lockDetail._modifyName;                 
                            break;
                        }
                    }
                    if (lockmname == "")
                    {
                        msg = "在配置开锁方式时," + msg1;
                        RecordLog.GetInstance().WriteLog(Level.Error, msg);
                        return;
                    }
                    //获取门锁名称 end
                    lockSettingType._lockModifyName = lockmname;            //蓝牙名称
                    lockSettingType._openLockType = opentype;               //开锁方式
                    BinaryFileSerialize(lockSettingType, savefile);
                    RecordLog.GetInstance().WriteLog(Level.Info, string.Format("绑定门锁关系,mac:{0},id:{1}", lockmac, lockid));
                }
                catch (Exception ex)
                {
                    RecordLog.GetInstance().WriteLog(Level.Info, string.Format("绑定门锁关系异常,异常信息:{0}", ex.Message));
                }
            }
        }
        #endregion

        #region 获取开锁方式配置文件
        /// <summary>
        /// 获取开锁方式配置文件
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public List<lockSettingType> lockSettingTypeList(out string msg)
        {
            msg = string.Empty;
            List<lockSettingType> lockSettingTypesList = new List<lockSettingType>();
            string savefile = string.Empty;
            try
            {               
                lockSettingType lockSettingType = new lockSettingType();
                string dataDir = baseDir + path + doorlocktypesettingpath;
                if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                {
                    Directory.CreateDirectory(dataDir);
                }
                DirectoryInfo folder = new DirectoryInfo(dataDir);
                if (folder.GetFiles("*.dat").Count() == 0)
                {
                    msg = "开锁方式配置文件不存在";
                    RecordLog.GetInstance().WriteLog(Level.Error, msg);
                    return null;
                }

                foreach (FileInfo file in folder.GetFiles("*.dat"))
                {
                    savefile = dataDir + "\\" + file;
                    lockSettingType = (lockSettingType)BinaryFileDeserialize(savefile);
                    lockSettingTypesList.Add(lockSettingType);
                }
                return lockSettingTypesList;
            }
            catch (Exception ex)
            {
                msg = string.Format("获取开锁方式配置文件异常,{0}!", ex.Message);
                RecordLog.GetInstance().WriteLog(Level.Error, msg);
                return null;
            }
        }
        #endregion

        

        #region 设置用户门锁开锁方式
        /// <summary>
        /// 设置用户门锁开锁方式
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userid"></param>
        /// <param name="openlocktype"></param>
        /// <param name="morningTime"></param>
        /// <param name="afternoonTime"></param>
        /// <param name="openlimit"></param>
        /// <param name="timetype"></param>
        /// <param name="opentimes"></param>
        public void RegeditDoorLockType(out string msg, string lockmac,string lockid, string opentype, string username,
            string opentime, int openlimit, string timetype, int opentimes)
        {
            lock (this)
            {
                msg = string.Empty;
                string msg1 = string.Empty;
                lockSettingType lockSettingType = new lockSettingType();
                List<lockSettingType> m_lockSettingTypeList = new List<lockSettingType>();
                try
                {                    
                    m_lockSettingTypeList = this.lockSettingTypeList(out msg1);
                    if(m_lockSettingTypeList == null)
                    {
                        msg = "设置用户门锁开锁方式异常：" + msg1;
                        RecordLog.GetInstance().WriteLog(Level.Error, msg);
                        return;
                    }                    
                    foreach (lockSettingType lst in m_lockSettingTypeList)
                    {
                        //if(lst.userDetails._useId == lockid && lst._openLockType == opentype && lst.userDetails._lockDetail_id == lockmac)
                        //{
                        //    lockSettingType = lst;
                        //    lockSettingType.recordDetail = null;
                        //    lockSettingType._aollowTime = opentime;
                        //    lockSettingType._totalTime = timetype;
                        //    lockSettingType._openlimitTimes = opentimes;
                        // };
                        if ( lst._openLockType == opentype && lst.userDetails._lockDetail_id == lockmac)
                        {
                            lockSettingType = lst;
                        };

                    }
                    List<userDetails> userDetailslist = this.getuserDetails(out msg1);
                    {
                        foreach (userDetails userDetails in userDetailslist)
                        {
                            if (userDetails._useName == username)
                            {
                                lockSettingType.userDetails._useName = userDetails._useName;
                                lockSettingType.userDetails._useSex = userDetails._useSex;
                                lockSettingType.userDetails._userjob = userDetails._userjob;
                                //add
                                lockSettingType.recordDetail = null;
                                lockSettingType._aollowTime = opentime;
                                lockSettingType._totalTime = timetype;
                                lockSettingType._openlimitTimes = opentimes;
                                lockSettingType.userDetails._useId = lockid;
                                //add
                                break;
                            }
                        }
                    }
                    InsterUserInfoType(out msg1, lockSettingType.userDetails._useId,
                        lockSettingType.userDetails._useName, lockSettingType.userDetails._useSex,
                        lockSettingType.userDetails._userjob, lockSettingType.userDetails._lockDetail_id);
                    if (msg1 != "Success")
                    {
                        msg = "更新用户设置异常：" + msg1;
                        RecordLog.GetInstance().WriteLog(Level.Error, msg);
                        return;
                    }

                    string dataDir = baseDir + path + doorlocktypesettingpath;
                    string savefile = dataDir + "\\" + lockSettingType.userDetails._lockDetail_id + "_" + lockid + "_" + opentype + "_" + doorlocktypesettingfile;
                    if (!File.Exists(savefile))
                    {
                        //Directory.CreateDirectory(dataDir);
                        msg = "错误:开锁配置初始化文件不存在!";
                        return;
                    }

                    List<string> lines = new List<string>(File.ReadAllLines(savefile));//先读取到内存变量
                    if (lines.Count() > 0)
                    {
                        lines.RemoveAt(0);//指定删除的行
                        File.WriteAllLines(savefile, lines.ToArray());//在写回硬盤
                    }
                    BinaryFileSerialize(lockSettingType, savefile);
                    msg = "Success";
                }
                catch(Exception ex)
                {
                    MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  用户设置信息保存有误!", "错误!");
                    msg = "用户设置信息保存有误!Exception:" + ex.Message;
                    return;
                }
            }
        }
        #endregion

        #region 将日志保存到文本文件中
        /// <summary>
        /// 将日志保存到文本文件中
        /// 注意:指定蓝牙连接设备读取数据可以获取到
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userid"></param>
        /// <param name="recordDetail"></param>
        /// <param name="warningRecord"></param>
        /// <param name="wrType"></param>
        public void WriteData(out string msg, string userid, RecordDetail recordDetail, WarningRecordDetails warningRecord, string wrType = "")
        {
            lock (this)
            {
                bool IsHaveinfo = false;
                msg = string.Empty;
                RecordLogsDataDetails recordLogsDataDetails = new RecordLogsDataDetails();
                string resultStatus = string.Empty;
                try
                {
                    if (wrType == "record")
                    {
                        List<userDetails> getuserDetailsList = new List<userDetails>();
                        string msg1 = string.Empty;

                        getuserDetailsList = getuserDetails(out msg1);
                        if (getuserDetailsList == null)
                        {
                            msg = "记录操作日志时错误：" + msg1;
                            return;
                        }

                        if (userid == "0")
                        {
                            //这是一个要是开锁的方式
                            recordLogsDataDetails = new RecordLogsDataDetails();
                            recordLogsDataDetails.openlocktime = recordDetail._opendatetime;
                            recordLogsDataDetails.lockid = userid;
                            string bleMac = EventClass.GetInstance()._ConnectBleAddress;
                            foreach (lockDetail lockinfo in GetlockSetting(out msg1))
                            {
                                if (lockinfo._id.Equals(bleMac))
                                {
                                    recordLogsDataDetails.lockmodifyname = lockinfo._modifyName.ToString();
                                }
                            }
                            recordLogsDataDetails.openlocktype = recordDetail._userOpenType;
                            if (recordDetail._userOpenType == "钥匙")
                            {
                                resultStatus = "钥匙开锁";
                                recordLogsDataDetails.userstatus = "异常";
                                recordLogsDataDetails.openexception += resultStatus;
                            }
                        }
                        else
                        {//userid存在，要对比一下
                            foreach (lockSettingType lockSettingType in lockSettingTypeList(out msg1))
                            {
                                if (lockSettingType.userDetails._useId == userid)
                                {
                                    foreach (var selectuser in getuserDetailsList)
                                    {
                                        if (lockSettingType.userDetails._useName.Equals(selectuser._useName) && //门锁设置文件中的用户名称和用户配置文件中的名称一致存在
                                            lockSettingType._openLockType.Equals(recordDetail._userOpenType) &&  //门锁设置文件中的开锁方式和传入的开锁方式一致存在
                                            lockSettingType.userDetails._lockDetail_id == EventClass.GetInstance()._ConnectBleAddress) //门锁设置中门锁mac地址和已连接的门锁一致存在
                                        {
                                            recordLogsDataDetails = new RecordLogsDataDetails();
                                            recordLogsDataDetails.openlocktime = recordDetail._opendatetime;
                                            //recordLogsDataDetails.lockmacid = selectuser._lockDetail_id;
                                            recordLogsDataDetails.lockmacid = EventClass.GetInstance()._ConnectBleAddress;//指定蓝牙门锁下的对象
                                            recordLogsDataDetails.lockid = userid;
                                            foreach (lockDetail lockinfo in GetlockSetting(out msg1))
                                            {
                                                if (lockinfo._id.Equals(EventClass.GetInstance()._ConnectBleAddress))
                                                {
                                                    recordLogsDataDetails.lockmodifyname = lockinfo._modifyName.ToString();
                                                }
                                            }
                                            if (string.IsNullOrEmpty(recordLogsDataDetails.lockmodifyname))
                                            {
                                                msg = "未知的蓝牙门锁MAC，请重新连接";
                                                MessageBox.Show("未知的蓝牙门锁MAC，请重新连接");
                                                return;
                                            }
                                            IsHaveinfo = true;
                                            recordLogsDataDetails.openlocktype = recordDetail._userOpenType;
                                            recordLogsDataDetails.userjob = selectuser._userjob;
                                            recordLogsDataDetails.userlevel = selectuser._uselevel;
                                            recordLogsDataDetails.username = selectuser._useName;
                                            recordLogsDataDetails.opentime = lockSettingType._aollowTime;
                                            recordLogsDataDetails.opentotalTime = lockSettingType._totalTime;//天
                                            recordLogsDataDetails.openlimitTimes = lockSettingType._openlimitTimes;//次

                                            if ((recordLogsDataDetails.opentotalTime != "0" || !string.IsNullOrEmpty(recordLogsDataDetails.opentotalTime)) &&
                                                recordLogsDataDetails.openlimitTimes != 0)
                                            {

                                                RecordGolable recordGolable = ReadDataToMemory("record");
                                                int ons = recordGolable._recordlogsdatadetailsList.Count();
                                                if (ons != 0 && ons - recordLogsDataDetails.openlimitTimes - 1 >= 0)
                                                {
                                                    string lastopentime = recordGolable._recordlogsdatadetailsList[ons - recordLogsDataDetails.openlimitTimes - 1].openlocktime;
                                                    if (lastopentime != "")
                                                    {
                                                        TimeSpan ts = DateTime.Parse(recordLogsDataDetails.openlocktime) - DateTime.Parse(lastopentime);
                                                        //if ((ts.Minutes * 60 + ts.Seconds) <= Int32.Parse(recordLogsDataDetails.opentotalTime) * 60) //按分钟计算
                                                        if (ts.Days == Int32.Parse(recordLogsDataDetails.opentotalTime) - 1)    //按天计算
                                                        {
                                                            resultStatus = "次数频繁";
                                                            recordLogsDataDetails.userstatus = "异常";
                                                            if (recordLogsDataDetails.openexception != "")
                                                            {
                                                                recordLogsDataDetails.openexception += @"\";
                                                            }
                                                            recordLogsDataDetails.openexception += resultStatus;
                                                        }
                                                    }
                                                }
                                            }
                                            //判断用户操作状态 1, 时间  2，次数
                                            TimeSpan dt = DateTime.Parse(recordLogsDataDetails.openlocktime.Split(' ')[1].ToString()).TimeOfDay;
                                            bool ExceptionStatus = false;

                                            int Timecounts = (recordLogsDataDetails.opentime.Split(';').Count() == 0) ? 1 :
                                                recordLogsDataDetails.opentime.Split(';').Count();
                                            for (int i = 0; i < Timecounts; i++)
                                            {
                                                TimeSpan time1 = DateTime.Parse(recordLogsDataDetails.opentime.Split(';')[i].ToString().Split('-')[0].ToString()).TimeOfDay;
                                                TimeSpan time2 = DateTime.Parse(recordLogsDataDetails.opentime.Split(';')[i].ToString().Split('-')[1].ToString()).TimeOfDay;

                                                if (((dt < time1) || (time2 < dt)) &&
                                                (dt < DateTime.Parse("12:00:00").TimeOfDay) && (dt > DateTime.Parse("00:00:00").TimeOfDay))
                                                {
                                                    ExceptionStatus = true;
                                                }

                                                if ((dt > DateTime.Parse("12:00:00").TimeOfDay) && (dt < DateTime.Parse("23:59:59").TimeOfDay) &&
                                                ((dt < time1) || (time2 < dt)))
                                                {
                                                    ExceptionStatus = true;
                                                }
                                            }
                                            if (ExceptionStatus == true)
                                            {
                                                resultStatus = "异常时段";
                                                recordLogsDataDetails.userstatus = "异常";

                                                if (recordLogsDataDetails.openexception != "")
                                                {
                                                    recordLogsDataDetails.openexception += @"\";
                                                }
                                                recordLogsDataDetails.openexception += resultStatus;
                                            }
                                        }                                        
                                    }                                    
                                }
                            }
                            if (IsHaveinfo == false)
                            {
                                recordLogsDataDetails = new RecordLogsDataDetails();
                                recordLogsDataDetails.openlocktime = recordDetail._opendatetime;
                                recordLogsDataDetails.lockmacid = EventClass.GetInstance()._ConnectBleAddress;
                                recordLogsDataDetails.lockid = userid;
                                foreach (lockDetail lockinfo in GetlockSetting(out msg1))
                                {
                                    if (lockinfo._id.Equals(EventClass.GetInstance()._ConnectBleAddress))
                                    {
                                        recordLogsDataDetails.lockmodifyname = lockinfo._modifyName.ToString();
                                    }
                                }
                                if (string.IsNullOrEmpty(recordLogsDataDetails.lockmodifyname))
                                {
                                    msg = "未知的蓝牙门锁MAC，请重新连接";
                                    MessageBox.Show("未知的蓝牙门锁MAC，请重新连接");
                                    return;
                                }
                                recordLogsDataDetails.openlocktype = recordDetail._userOpenType;
                                recordLogsDataDetails.userjob = "未配置";
                                recordLogsDataDetails.userlevel = 0;
                                recordLogsDataDetails.username = "未配置用户";
                                recordLogsDataDetails.opentime = "00:00:01-23:59:59";
                                recordLogsDataDetails.opentotalTime = "";//天
                                recordLogsDataDetails.openlimitTimes = 0;//次
                                resultStatus = "";
                                recordLogsDataDetails.userstatus = "正常";
                                recordLogsDataDetails.openexception += resultStatus;
                            }
                        }
                    }                 
                    //创建文件夹
                    string dataDir = baseDir + path;
                    if (!Directory.Exists(dataDir))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(dataDir);
                    }
                    if (wrType == "record")
                    {
                        string savefile = dataDir + "\\" + userfile;
                        BinaryFileSerialize(recordLogsDataDetails, savefile);
                    }
                    else if (wrType == "warning")
                    {
                        string savefile = dataDir + "\\" + warningfile;
                        BinaryFileSerialize(warningRecord, savefile);
                    }
                    else
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  记录信息保存有误!";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Exception:" + ex.Message;
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
        #endregion

        #region 删除指定的日志行数
        /// <summary>
        /// 删除指定的日志行数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="indexrow"></param>
        public void DelRecordLog(out string msg ,string indexrow)
        {
            msg = string.Empty;
            try
            {
                string dataDir = baseDir + path + "\\" + userfile;
                List<string> lines = new List<string>(File.ReadAllLines(dataDir));//先读取到内存变量
                lines.RemoveAt(Int32.Parse(indexrow)-1);//指定删除的行
                File.WriteAllLines(dataDir, lines.ToArray());//在写回硬盤
                msg = string.Format("Success");
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("UserRecord Delete Success:{0},第{1}行", dataDir, indexrow));
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("UserRecord Delete Exception:{0}", ex.Message));
            }           
        }
        #endregion
        #region 删除指定的报警日志行数
        /// <summary>
        /// 删除指定的日志行数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="indexrow"></param>
        public void DelWarningLog(out string msg, string indexrow)
        {
            msg = string.Empty;
            try
            {
                string dataDir = baseDir + path + "\\" + warningfile;
                List<string> lines = new List<string>(File.ReadAllLines(dataDir));//先读取到内存变量
                lines.RemoveAt(Int32.Parse(indexrow) - 1);//指定删除的行
                File.WriteAllLines(dataDir, lines.ToArray());//在写回硬盤
                msg = string.Format("Success");
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("WarningRecord Delete Success:{0},第{1}行", dataDir, indexrow));
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, string.Format("WarningRecord Delete Exception:{0}", ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// 读取teble文件添加到List列表中
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public List<RecordLogsDataDetails> ReadDataTableToMemory(DataTable dataTable)
        {
            RecordGolable recordGolable = new RecordGolable();
            RecordLogsDataDetails recordLogsDataDetails = new RecordLogsDataDetails();
            List<RecordLogsDataDetails> recordLogsDataDetailsList = new List<RecordLogsDataDetails>();
            try
            {
                int irows = dataTable.Rows.Count;
                if(irows > 0)
                {
                    for(int i = 0;i<irows;i++)
                    {
                        recordLogsDataDetails = new RecordLogsDataDetails();
                        {
                            recordLogsDataDetails.openlocktime = DateTime.Parse(dataTable.Rows[i][0].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                            recordLogsDataDetails.lockmacid = dataTable.Rows[i][1].ToString();
                            recordLogsDataDetails.lockid = dataTable.Rows[i][2].ToString();
                            recordLogsDataDetails.lockmodifyname = dataTable.Rows[i][3].ToString();
                            recordLogsDataDetails.openlocktype = dataTable.Rows[i][4].ToString();
                            recordLogsDataDetails.userid = dataTable.Rows[i][5].ToString();
                            recordLogsDataDetails.username = dataTable.Rows[i][6].ToString();
                            recordLogsDataDetails.userjob = dataTable.Rows[i][7].ToString();
                            recordLogsDataDetails.userlevel = Int32.Parse(dataTable.Rows[i][8].ToString());
                            recordLogsDataDetails.opentime = dataTable.Rows[i][9].ToString();
                            recordLogsDataDetails.opentotalTime = dataTable.Rows[i][10].ToString();
                            recordLogsDataDetails.openlimitTimes = Int32.Parse(dataTable.Rows[i][11].ToString() == "" ? "0" : dataTable.Rows[i][11].ToString());
                            recordLogsDataDetails.openexception = dataTable.Rows[i][12].ToString();
                            recordLogsDataDetails.userstatus = dataTable.Rows[i][13].ToString();
                        }

                        recordLogsDataDetailsList.Add(recordLogsDataDetails);
                    }
                }
                //recordGolable._recordlogsdatadetailsList = recordLogsDataDetailsList;
                return recordLogsDataDetailsList.OrderByDescending(c => DateTime.Parse(c.openlocktime)).ToList();
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "ReadDataTableToMemory Exception: " + ex.Message);
                return recordLogsDataDetailsList;
            }
        }

        #region 将日志记录从文本中读取到内存
        /// <summary>
        /// 将日志记录从文本中读取到内存
        /// </summary>
        /// <param name="readtype"></param>
        /// <returns></returns>
        public RecordGolable ReadDataToMemory(string readtype = "")
        {
            RecordGolable recordGolable = new RecordGolable();

            WarningRecordDetails warningRecordDetail = new WarningRecordDetails();
            List<WarningRecordDetails> warningRecordDetailList = new List<WarningRecordDetails>();
            RecordLogsDataDetails recordLogsDataDetails = new RecordLogsDataDetails();
            List<RecordLogsDataDetails> recordLogsDataDetailsList = new List<RecordLogsDataDetails>();

            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                if (readtype == "record")
                {
                    //获取用户记录信息到内存字典中
                    string dataDir = baseDir + path + "\\" + userfile;
                    if (!Directory.Exists(baseDir + path))// 目录不存在,新建目录
                    {
                        Directory.CreateDirectory(baseDir + path);
                    }
                    fs = new FileStream(dataDir, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader read = new StreamReader(fs);
                    //循环读取每一行
                    string strReadline = string.Empty;
                    while ((strReadline = read.ReadLine()) != null)
                    {
                        //对每一行数据做反序列化处理
                        if (strReadline.Length % 2 != 0)
                        {
                            strReadline = "0" + strReadline;
                        }
                        byte[] binReadline = new byte[strReadline.Length / 2];
                        for (int i = 0; i < binReadline.Length; i++)
                        {
                            string b = strReadline.Substring(i * 2, 2);
                            binReadline[i] = Convert.ToByte(b, 16);
                        }
                        using (MemoryStream ms = new MemoryStream(binReadline))
                        {
                            IFormatter iFormatter = new BinaryFormatter();
                            recordLogsDataDetails = (RecordLogsDataDetails)iFormatter.Deserialize(ms);
                        }
                        recordLogsDataDetailsList.Add(recordLogsDataDetails);
                    }

                    //判断EventClass.GetInstance().tb是否存在，存在的话将数据进行追加 begin
                    if(EventClass.GetInstance()._temptb.Rows.Count > 0)
                    {
                        List<RecordLogsDataDetails> tempAdd = new List<RecordLogsDataDetails>();
                        foreach(RecordLogsDataDetails s in ReadDataTableToMemory(EventClass.GetInstance().tb))
                        {
                            recordLogsDataDetailsList.Add(s);
                            tempAdd.Add(s);
                        }

                        if (fs != null)
                        {
                            fs.Close();
                            fs = null;
                        }
                        RecordLog.GetInstance().WriteLog(Level.Info, "有新数据导入，追加日志文件");                        
                        foreach (RecordLogsDataDetails indata in tempAdd)
                        {
                            BinaryFileSerialize(indata, dataDir);
                        }
                        tempAdd.Clear();
                        EventClass.GetInstance()._temptb.Clear();
                    }
                    //判断EventClass.GetInstance().tb是否存在，存在的话将数据进行追加 end

                    //得到指定ID下用户的信息，配置，及操作
                    //recordGolable._dicrecordlogsdatadetails = new Dictionary<string, List<RecordLogsDataDetails>>();
                    //recordGolable._dicrecordlogsdatadetails.Add(recordLogsDataDetails.userid, recordLogsDataDetailsList);
                    recordGolable._recordlogsdatadetailsList = 
                        recordLogsDataDetailsList.OrderByDescending(c=>DateTime.Parse(c.openlocktime)).ToList();

                    //测试数据
                    EventClass.GetInstance()._gloabledataTable = 
                        DataAnalysis.GetInstance().AddTableData(EventClass.GetInstance()._gloabledataTable,
                        recordGolable._recordlogsdatadetailsList, null, readtype);
                    //recordGolable._objlist = DataAnalysis.GetInstance().QueryDataRecordView(EventClass.GetInstance()._gloabledataTable, "", "2018-9-1", "2018-9-4 23:59:59", "", "");
                    
                }
                if (readtype == "warning")
                {
                    //获取用户记录信息到内存字典中
                    string dataDir = baseDir + path + "\\" + warningfile;
                    fs = new FileStream(dataDir, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader read = new StreamReader(fs);
                    //循环读取每一行
                    string strReadline = string.Empty;
                    while ((strReadline = read.ReadLine()) != null)
                    {
                        //对每一行数据做反序列化处理
                        if (strReadline.Length % 2 != 0)
                        {
                            strReadline = "0" + strReadline;
                        }
                        byte[] binReadline = new byte[strReadline.Length / 2];
                        for (int i = 0; i < binReadline.Length; i++)
                        {
                            string b = strReadline.Substring(i * 2, 2);
                            binReadline[i] = Convert.ToByte(b, 16);
                        }
                        using (MemoryStream ms = new MemoryStream(binReadline))
                        {
                            IFormatter iFormatter = new BinaryFormatter();
                            warningRecordDetail = (WarningRecordDetails)iFormatter.Deserialize(ms);
                        }
                        warningRecordDetailList.Add(warningRecordDetail);
                    }
                    //得到指定时间下报警的类型和状态                    
                    recordGolable._warningRecordList = warningRecordDetailList;
                    EventClass.GetInstance()._warningdataTable = DataAnalysis.GetInstance().AddTableData(EventClass.GetInstance()._warningdataTable,
                        null, warningRecordDetailList, readtype);
                }
                return recordGolable;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Write RecordData Exception: " + ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                //if(readtype == "record")
                //{
                //    RecordLog.GetInstance().WriteLog(Level.Info, "重写日志文件");
                //    string dataDir = baseDir + path + "\\" + userfile;
                //    //重写文件
                //    if (File.Exists(dataDir))
                //    {
                //        File.Delete(dataDir);
                //    }
                //    foreach (RecordLogsDataDetails indata in recordGolable._recordlogsdatadetailsList)
                //    {
                //        BinaryFileSerialize(indata, dataDir);
                //    }
                //}
            }
        }
        #endregion
    }
}
