using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using wclBluetooth;
using XLQStartup.全局类;
using XLQStartup.操作类;
using XLQStartup.数据导出;
using XLQStartup.日志;
using XLQStartup.记录数据文件类;

namespace XLQStartup
{
    /// <summary>
    /// 实体框架类
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]//COM+组件可见
    public partial class Form1 : Form
    {
        /// <summary>
        /// 窗体构造函数
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            EventClass.GetInstance().RecordLogFunc = ConfigurationManager.AppSettings["RecordLogFunc"].ToString();
            //init
            EventClass.GetInstance()._gloabledataTable = DataAnalysis.GetInstance().CreatNewDataTable("gloabledataTable");
            EventClass.GetInstance()._warningdataTable = DataAnalysis.GetInstance().CreatNewDataTable("warningdataTable");
            EventClass.GetInstance()._processFlags = 0;
            EventClass.GetInstance()._manager = new wclBluetoothManager();
            EventClass.GetInstance()._client = new wclGattClient();
            EventClass.GetInstance()._eventDelegate = new EventDelegate();
            EventClass.GetInstance()._manager.OnDeviceFound += EventClass.GetInstance().Manager_OnDeviceFound;
            EventClass.GetInstance()._manager.OnDiscoveringCompleted += EventClass.GetInstance().Manager_OnDiscoveringCompleted;
            EventClass.GetInstance()._manager.OnDiscoveringStarted += EventClass.GetInstance().Manager_OnDiscoveringStarted;
            EventClass.GetInstance()._client.OnCharacteristicChanged += EventClass.GetInstance().Client_OnCharacteristicChanged;
            EventClass.GetInstance()._client.OnConnect += EventClass.GetInstance().Client_OnConnect;
            EventClass.GetInstance()._client.OnDisconnect += EventClass.GetInstance().Client_OnDisconnect;
            EventClass.GetInstance()._manager.Open();
            EventClass.GetInstance().Cleanup();
            //按钮事件 - 搜索
            EventClass.GetInstance()._eventDelegate.OnBleSearchBtn += EventClass.GetInstance().BleSearchBtn_Event;
            //按钮事件 - 连接
            //EventClass.GetInstance()._eventDelegate.OnBleConncetBtn += EventClass.GetInstance().BleConnect_Click;            
        }

        /// <summary>
        /// 窗体运行时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string strPage = ConfigurationManager.AppSettings["IndexPage"].ToString();
#if DEBUG
                webBrowser1.ScriptErrorsSuppressed = false;
#else
                webBrowser1.ScriptErrorsSuppressed = true;
#endif
                webBrowser1.ObjectForScripting = this;//具体公开的对象,这里可以公开自定义对象
                webBrowser1.Navigate(Application.StartupPath + "\\" + strPage);

            }
            catch(Exception ex)
            {
                MessageBox.Show("WebBrowser框架加载异常: "+ ex.Message, "错误！", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        /// <summary>
        /// 结束窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            EventClass.GetInstance()._client.Disconnect();
            EventClass.GetInstance()._client = null;
            EventClass.GetInstance()._manager.Close();
            EventClass.GetInstance()._manager = null;
            EventClass.GetInstance().Cleanup();
            webBrowser1.Dispose();
        }
        /// <summary>
        /// 对外测试接口
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string ShowMsg(string msg)
        {
            //MessageBox.Show(msg);
            return msg;
        }
        /// <summary>
        /// 清除内存变量
        /// </summary>
        /// <returns></returns>
        private string ClearDevice()
        {
            EventClass.GetInstance().Cleanup();
            RecordLog.GetInstance().WriteLog(Level.Info, "初始化参数");
            return "初始化参数";
        }
        /// <summary>
        /// 1，对外接口：搜索BLE设备，触发搜索事件，保存到全局中
        /// </summary>
        public void SearchDevice()
        {
            //防止前端忘记断开连接
            this.DisConnectDevice();
            this.ClearDevice();
            EventClass.GetInstance()._eventDelegate.OnHandler();     
        }

        /// <summary>
        /// 蓝牙名称显示
        /// </summary>
        internal class outParam
        {
            public string Name { get; set; }
            public string Mac { get; set; }
            public string SetTime { get; set; }
            public outParam()
            {
                Name = string.Empty;
                Mac = string.Empty;
                SetTime = string.Empty;
            }
        }
        /// <summary>
        /// 2，对外接口：查看已经搜索到的所有BLE设备
        /// </summary>
        /// <returns></returns>
        public string ViewDevice()
        {
            List<outParam> outdata = new List<outParam>();
            //string outParam = string.Empty;
            if (EventClass.GetInstance()._processFlags == 1)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "查找结束");
            }
            if(EventClass.GetInstance()._deviceAddress.Count <= 0)
            {
                return "";
            }
            RecordDeviceInfo.GetInstance().DeletDeviceInfoFile();            

            string lastmac = string.Empty;
            string lastTemp = string.Empty;
            int status = 0;
            int ret = OpenAppDo.GetInstance().judelkconnect(out status);
            if(ret == 0)
            {
                if(status != 0)
                {
                    RecordLog.GetInstance().WriteLog(Level.Info, "发现已经存在蓝牙信息");
                    List<string> outStrList = EventClass.GetInstance().knowsLock;
                    foreach (string splitparam in EventClass.GetInstance()._deviceAddress)
                    {
                        foreach (string lockInfo in outStrList)
                        {
                            if (lockInfo.Split(',')[1].ToString().Equals(splitparam.Split('|')[1].ToString()))
                            {
                                //mac相同才会显示
                                outParam temp = new outParam()
                                {
                                    Name = lockInfo.Split(',')[0].ToString(),
                                    Mac = lockInfo.Split(',')[1].ToString(),
                                    SetTime = lockInfo.Split(',')[2].ToString(),
                                };
                                outdata.Add(temp);
                                if (lastmac == lockInfo.Split(',')[1].ToString())
                                {
                                    continue;
                                }
                                lastmac = temp.Mac;
                                //判断上次地址和当前地址是否一致                            
                                EventClass.GetInstance()._dicdeviceAddress.Add(splitparam.Split('|')[0].ToString(), splitparam.Split('|')[1].ToString());
                                RecordDeviceInfo.GetInstance().WriteLocalInfo(splitparam.Split('|')[0].ToString(), splitparam.Split('|')[1].ToString(), FingerPrint.Value().ToString());
                            }
                        }
                    }
                    EventClass.GetInstance().knowsLock.Clear();
                }
                else
                {
                    foreach (string splitparam in EventClass.GetInstance()._deviceAddress)
                    {
                        //记录本次地址 start
                        string msg = string.Empty;
                        if (lastTemp != splitparam.Split('|')[1].ToString())
                        {
                            RedvelopRecord.GetInstance().RegeditDoorSetting(out msg, splitparam.Split('|')[1].ToString());
                            lastTemp = splitparam.Split('|')[1].ToString();
                        }
                        //记录本次地址 end
                        EventClass.GetInstance()._dicdeviceAddress.Add(splitparam.Split('|')[0].ToString(), splitparam.Split('|')[1].ToString());
                        RecordDeviceInfo.GetInstance().WriteLocalInfo(splitparam.Split('|')[0].ToString(), splitparam.Split('|')[1].ToString(), FingerPrint.Value().ToString());
                        outParam temp = new outParam()
                        {
                            Name = "",
                            Mac = splitparam.Split('|')[1].ToString(),
                            SetTime = "",
                        };
                        outdata.Add(temp);
                    }                    
                }
            }
            //[{"Name":"前门","Mac":"0CAE7DAB5F72","SetTime":"2018-10-16 03:00:45"}]
            string json = JsonConvert.SerializeObject(outdata);
            return json;
        }
        /// <summary>
        /// 3，对外接口：连接BLE设备
        /// </summary>
        /// <param name="MacAddress"></param>
        /// <returns></returns>
        public string ConnectDevice(string MacAddress)
        {
            RecordLog.GetInstance().WriteLog(Level.Info, string.Format("开始配对蓝牙地址,Mac:{0}", MacAddress));
            LicenseHelper licenseHelper = new LicenseHelper();
            if (!licenseHelper.IsHaveListenersFile())
            {
                MessageBox.Show("授权文件不存在，请联系系统管理员","系统提示");
                return "Error";
            }
            List<string> CheckData = licenseHelper.ReturnCheckData();
            if(CheckData.Count == 0)
            {
                MessageBox.Show("授权文件信息不存在，请联系系统管理员", "系统提示");
                return "Error";
            }

            string name = string.Empty;
            string mac = string.Empty;
            string outCheckValue = string.Empty;
            //连接地址的时候判断授权文件
            foreach (var s in EventClass.GetInstance()._dicdeviceAddress)
            {
                if (s.Value.ToString().Equals(MacAddress))
                {
                    bool res = licenseHelper.EncrptyData(s.Key.ToString(), s.Value.ToString(), out outCheckValue);
                    if (res == true)
                    {
                        bool res2 = licenseHelper.CheckLockDecValue(outCheckValue, CheckData);
                        if (res2 == true)
                        {
                            RecordLog.GetInstance().WriteLog(Level.Info, "开始连接设备: " + MacAddress.ToString());
                            EventClass.GetInstance()._ConnectBleAddress = MacAddress;
                            //EventClass.GetInstance()._eventDelegate.OnHandlerConnect();
                            //return "Success";
                            string msg = string.Empty;
                            List<lockDetail> objlist = RedvelopRecord.GetInstance().GetlockSetting(out msg);
                            if (objlist == null)
                            {
                                MessageBox.Show("系统尚未配置门锁文件", "系统提示");
                                return "Error";
                            }
                            foreach (lockDetail lockDetail in objlist)
                            {
                                if (lockDetail._id.Equals(EventClass.GetInstance()._ConnectBleAddress))
                                {
                                    OpenAppDo.GetInstance().ModifyLockTime(lockDetail._modifyName, lockDetail._id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    //OpenAppDo.GetInstance().AddLockMap(lockDetail._modifyName, lockDetail._id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }

                            EventClass.GetInstance()._client.Address = Convert.ToInt64(MacAddress, 16);
                            wclBluetoothRadio radio = EventClass.GetInstance()._devices[MacAddress];
                            if (EventClass.GetInstance()._client.Connect(radio) != 0)
                            {
                                RecordLog.GetInstance().WriteLog(Level.Info, "设备连接失败");
                                MessageBox.Show("设备连接失败", "提示");
                                return "Error";
                            }
                            RecordLog.GetInstance().WriteLog(Level.Info, "设备连接成功");
                            EventClass.GetInstance()._dicdeviceAddress.Clear();
                            return "Success";
                        }
                        else
                        {
                            MessageBox.Show(string.Format("{0}门锁未授权，请联系系统管理员", s.Key.ToString()), "系统提示");
                            return "Error";
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("处理授权文件失败，请联系系统管理员"), "系统提示");
                        return "Error";
                    }
                }
            }
            MessageBox.Show("未找到适配的蓝牙设备", "系统提示");
            return "Error";
        }
        /// <summary>
        /// 4，对外接口：断开BLE设备
        /// </summary>
        public void DisConnectDevice()
        {           
            EventClass.GetInstance()._client.Disconnect();
        }

        /// <summary>
        /// 设置系统时间
        /// </summary>
        public static void SetCurrentTime()
        {
            byte[] package = BleProtocol.GetUpdateTimePackage();
            EventClass.GetInstance().WriteCommand(package);
            RecordLog.GetInstance().WriteLog(Level.Info, "设置时间行为结束");
        }
        /// <summary>
        /// 6，对外接口：获取记录
        /// </summary>
        public void GetHistoryRecord()
        {
            if (EventClass.GetInstance()._ConnectBleAddress == "")
            {
                MessageBox.Show("尚未连接蓝牙门锁", "系统提示");
                return;
            }
            string str = GlobalParams._getRecordMsg;
            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] val = new byte[str.Length / 2];
            for (int i = 0; i < val.Length; i++)
            {
                string b = str.Substring(i * 2, 2);
                val[i] = Convert.ToByte(b, 16);
            }
            EventClass.GetInstance().val_GetRecord = val;
            EventClass.GetInstance().WriteCommand(val);
        }
        /// <summary>
        /// 7，对外接口：获取设置
        /// </summary>
        public void GetSettingRecord()
        {
            if (EventClass.GetInstance()._ConnectBleAddress == "")
            {
                MessageBox.Show("尚未连接蓝牙门锁","系统提示");
                return;
            }
            string str = GlobalParams._getSettingDataBody;
            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] val = new byte[str.Length / 2];
            for (int i = 0; i < val.Length; i++)
            {
                string b = str.Substring(i * 2, 2);
                val[i] = Convert.ToByte(b, 16);
            }
            //CRC
            byte[] send = new byte[val.Length + 1];
            byte[] crcBytes = new byte[val.Length - 2];
            for (int i = 0; i < send.Length; i++)
            {
                send[i] = byte.MaxValue;
            }
            for (int k = 0; k < val.Length; k++)
            {
                send[k] = val[k];
            }
            for (int j = 0; j < crcBytes.Length; j++)
            {
                crcBytes[j] = val[j + 2];
            }
            send[val.Length] = CRC.Crc8(crcBytes);
            EventClass.GetInstance().val_GetRecord = send;
            EventClass.GetInstance().WriteCommand(send);
        }
        /// <summary>
        /// 8，对外接口：获取报警记录
        /// </summary>
        public static void GetWarningRecord()
        {
            if (EventClass.GetInstance()._ConnectBleAddress == "")
            {
                MessageBox.Show("尚未连接蓝牙门锁", "系统提示");
                return;
            }
            string str = GlobalParams._getWarningDataBody;
            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] val = new byte[str.Length / 2];
            for (int i = 0; i < val.Length; i++)
            {
                string b = str.Substring(i * 2, 2);
                val[i] = Convert.ToByte(b, 16);
            }
            //CRC
            byte[] send = new byte[val.Length + 1];
            byte[] crcBytes = new byte[val.Length - 2];
            for (int i = 0; i < send.Length; i++)
            {
                send[i] = byte.MaxValue;
            }
            for (int k = 0; k < val.Length; k++)
            {
                send[k] = val[k];
            }
            for (int j = 0; j < crcBytes.Length; j++)
            {
                crcBytes[j] = val[j + 2];
            }
            send[val.Length] = CRC.Crc8(crcBytes);
            EventClass.GetInstance().val_GetRecord = send;
            EventClass.GetInstance().WriteCommand(send);
        }
        /// <summary>
        /// 9，对外接口：查看记录信息
        /// </summary>
        /// <returns></returns>
        public string ViewHistroyRecord()
        {
            if (EventClass.GetInstance()._ConnectBleAddress == "")
            {
                return "Error";
            }
            string outParam = string.Empty;
            List<string> Tmplist = new List<string>();
            if(EventClass.GetInstance()._processName == 0)
            {
                Tmplist = EventClass.GetInstance()._bleResult;
            }
            else if(EventClass.GetInstance()._processName == 1)
            {
                Tmplist = EventClass.GetInstance()._bleSettingResult;
            }
            else
            {
                Tmplist = EventClass.GetInstance()._bleWarningResult;
            }
            foreach (var spiltparam in Tmplist)
            {
                outParam += spiltparam + ",";
            }            
            return outParam;
        }
       
        /// <summary>
        /// 10，对外接口：获取记录对象
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="counts">条目数</param>
        /// <returns></returns>
        public string GetRecordObject(string page,string counts)
        {
            RecordGolable goableData = new RecordGolable();
            List<RecordLogsDataDetails> tempList = new List<RecordLogsDataDetails>();

            goableData = RedvelopRecord.GetInstance().ReadDataToMemory("record");
            int iPage = Int32.Parse(page);
            int iCounts = Int32.Parse(counts);

            try
            {
                int totalcounts = goableData._recordlogsdatadetailsList.Count;
                int totalpages = 0;

                if (totalcounts % Int32.Parse(counts) > 0)
                {
                    totalpages = totalcounts / Int32.Parse(counts) + 1;
                }
                else
                {
                    totalpages = totalcounts / Int32.Parse(counts);
                }
                if (Int32.Parse(page) > totalpages)
                {
                    //MessageBox.Show("当前输入页数大于总页数", "系统提示");
                    return "[]";
                }

                for (int i = 0; i < iCounts; i++)
                {
                    if (((iPage - 1) * iCounts + i) == totalcounts)
                    {
                        break;
                    }
                    if (goableData._recordlogsdatadetailsList[(iPage - 1) * iCounts + i].openlocktime != "")
                    {
                        tempList.Add(goableData._recordlogsdatadetailsList[(iPage - 1) * iCounts + i]);
                    }
                    else
                    {                        
                        break;
                    }
                    //RecordLog.GetInstance().WriteLog(Level.Info, string.Format("查询分页返回数据完成,page:[{0}],count:[{1}]", iPage, (iPage - 1) * iCounts + i + 1));
                }

                object obj = new
                {
                    totalcounts = totalcounts,
                    pagedata = tempList
                };

                string json = JsonConvert.SerializeObject(obj);
                //string json = JsonConvert.SerializeObject(goableData._recordlogsdatadetailsList);
                return json;
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "分页时异常：" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// 报警对象数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="counts"></param>
        /// <returns></returns>
        public string GetWarningObject(string page, string counts)
        {
            RecordGolable goableData = new RecordGolable();
            List<WarningRecordDetails> tempList = new List<WarningRecordDetails>();

            goableData = RedvelopRecord.GetInstance().ReadDataToMemory("warning");
            int iPage = Int32.Parse(page);
            int iCounts = Int32.Parse(counts);

            try
            {
                int totalcounts = goableData._warningRecordList.Count;
                int totalpages = 0;

                if (totalcounts % Int32.Parse(counts) > 0)
                {
                    totalpages = totalcounts / Int32.Parse(counts) + 1;
                }
                else
                {
                    totalpages = totalcounts / Int32.Parse(counts);
                }
                if (Int32.Parse(page) > totalpages)
                {
                    //MessageBox.Show("当前输入页数大于总页数", "系统提示");
                    return "[]";
                }

                for (int i = 0; i < iCounts; i++)
                {
                    if (((iPage - 1) * iCounts + i) == totalcounts)
                    {
                        break;
                    }
                    if (goableData._warningRecordList[(iPage - 1) * iCounts + i]._warningdatetime != "")
                    {
                        tempList.Add(goableData._warningRecordList[(iPage - 1) * iCounts + i]);
                    }
                    else
                    {
                        break;
                    }
                    RecordLog.GetInstance().WriteLog(Level.Info, string.Format("报警分页返回数据完成,page:[{0}],count:[{1}]", iPage, (iPage - 1) * iCounts + i + 1));
                }

                object obj = new
                {
                    totalcounts = totalcounts,
                    pagedata = tempList
                };

                string json = JsonConvert.SerializeObject(obj);
                //string json = JsonConvert.SerializeObject(goableData._recordlogsdatadetailsList);
                return json;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "分页时异常：" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// 删除指定记录对象行数
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public object DelRecordObject(string row)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().DelRecordLog(out msg, row);
            object obj = new
            {
                Status = msg
            };
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }
        /// <summary>
        /// 删除指定报警记录对象行数
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public object DelWarningObject(string row)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().DelWarningLog(out msg, row);
            object obj = new
            {
                Status = msg
            };
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }
        /// <summary>
        /// 11，对外接口：获取门锁配置文件信息
        /// </summary>
        public string GetLockSettingInfo()
        {
            string msg = string.Empty;            
            List<lockDetail> objlist = RedvelopRecord.GetInstance().GetlockSetting(out msg);
            if(msg != "")
            {
                return "";
            }
            string json = JsonConvert.SerializeObject(objlist);
            return json;
        }
        /// <summary>
        /// 12，对外接口：设置门锁的信息
        /// </summary>
        /// <param name="macid"></param>
        /// <param name="modifyname"></param>
        /// <returns></returns>
        public string RegeditLockSetting(string macid,string modifyname)
        {
            RedvelopRecord.GetInstance().DeletSettingFile(macid);
            string msg = string.Empty;
            RedvelopRecord.GetInstance().Regedit2DoorSetting(out msg, macid, modifyname);
            return msg;
        }
        /// <summary>
        /// 13，对外接口：删除指定的门锁
        /// </summary>
        /// <param name="macid"></param>
        /// <returns></returns>
        public string DeletLockSetting(string macid)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().DeletlockSetting(out msg, macid);
            return msg;
        }
        /// <summary>
        /// 14，对外接口：获取用户信息列表
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            string msg = string.Empty;
            List<userDetails> objlist = RedvelopRecord.GetInstance().getuserDetails(out msg);
            if (msg != "")
            {
                return "";
            }
            string json = JsonConvert.SerializeObject(objlist);
            return json;
        }
        /// <summary>
        /// 15，对外接口：设置用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="usersex"></param>
        /// <param name="userjob"></param>
        /// <param name="lockMacid"></param>
        /// <returns></returns>
        public string RegeditUserInfo(string id, string username, string usersex, string userjob, string lockMacid)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().RegeditUser(out msg, id, username, usersex, userjob, lockMacid);
            return msg;
        }
        /// <summary>
        /// 15，对外接口：修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="usersex"></param>
        /// <param name="userjob"></param>
        /// <param name="lockMacid"></param>
        /// <returns></returns>
        public string ModifyUserInfo(string newname, string username, string usersex, string userjob)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().RegeditUser(out msg, newname, username, usersex, userjob);
            return msg;
        }
        /// <summary>
        /// 16，对外接口：删除指定的门锁
        /// </summary>
        /// <param name="macid"></param>
        /// <returns></returns>
        public string DeletUserSetting(string name)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().DeletUserInfoSetting(out msg, name);
            return msg;
        }
        /// <summary>
        /// 获取开锁方式设置信息
        /// </summary>
        /// <returns></returns>
        public string GetLockSettingType()
        {
            string msg = string.Empty;
            List<lockSettingType> objlist = RedvelopRecord.GetInstance().lockSettingTypeList(out msg);
            if (msg != "")
            {
                return "";
            }
            string json = JsonConvert.SerializeObject(objlist);
            return json;
        }
        /// <summary>
        /// 新增开锁方式设置信息配置文件
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="openlocktype"></param>
        /// <param name="morningTime"></param>
        /// <param name="afternoonTime"></param>
        /// <param name="openlimit"></param>
        /// <param name="timetype"></param>
        /// <param name="opentimes"></param>
        /// <returns></returns>
        public string RegeditDoorLockType(string loackmac,string userid, string openlocktype,string username,string opentime,
            string openlimit, string timetype, string opentimes)
        {
            int iopenlimit = 0;
            if(!Int32.TryParse(openlimit,out iopenlimit))
            {
                return "参数openlimit不是标准Int类型数据";
            }
            int iopentimes = 0;
            if(!Int32.TryParse(opentimes,out iopentimes))
            {
                return "参数iopentimes不是标准Int类型数据";
            }
            if(string.IsNullOrEmpty(openlocktype))
            {
                return "开门类型不能为空";
            }
            if (string.IsNullOrEmpty(loackmac))
            {
                return "门锁MAC不能为空";
            }
            if (string.IsNullOrEmpty(username))
            {
                return "用户姓名不能为空";
            }
            if (string.IsNullOrEmpty(userid))
            {
                return "ID不能为空";
            }
            string msg = string.Empty;
            RedvelopRecord.GetInstance().RegeditDoorLockType(out msg, loackmac,userid,openlocktype, username, opentime,
                iopenlimit, timetype, iopentimes);
            return msg;
        }
        /// <summary>
        /// 删除指定的配置文件
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string DeletLocalSettingType(string userid,string opentype)
        {
            if (EventClass.GetInstance()._ConnectBleAddress == "")
            {
                MessageBox.Show("尚未连接蓝牙门锁", "系统提示");
                return "Error";
            }
            string msg = string.Empty;
            RedvelopRecord.GetInstance().DeletLocalSettingType(out msg, userid , opentype);
            return msg;
        }
        /// <summary>
        /// 根据条件进行搜索
        /// </summary>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="userjob"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="counts"></param>
        /// <returns></returns>
        public string SearchRocordResult(string lockname,string starttime, string endtime,string userjob, string status,string opentype,
            string tabletype ,string page, string counts)
        {
            string msg = string.Empty;
            List<RecordLogsDataDetails> QueryDataRecordList = new List<RecordLogsDataDetails>();

            if (tabletype == "0")
            {
                QueryDataRecordList = DataAnalysis.GetInstance().QueryDataRecordView(
                   out msg, EventClass.GetInstance()._gloabledataTable, lockname, starttime, endtime, userjob, status, opentype);
                if (msg != "Success")
                {
                    return msg;
                }
            }
            if(tabletype == "1")
            {
                QueryDataRecordList = DataAnalysis.GetInstance().QueryDataRecordView(
                out msg, EventClass.GetInstance().tb, lockname, starttime, endtime, userjob, status, opentype);
                if (msg != "Success")
                {
                    return msg;
                }
            }
            

            List<RecordLogsDataDetails> tempList = new List<RecordLogsDataDetails>();
           
            int iPage = Int32.Parse(page);
            int iCounts = Int32.Parse(counts);

            try
            {
                int totalcounts = QueryDataRecordList.Count;

                if (iPage == 1 && iCounts == 0)
                {
                    iCounts = totalcounts;
                    iPage = 1;
                }
                else
                {
                    int totalpages = 0;

                    if (totalcounts % Int32.Parse(counts) > 0)
                    {
                        totalpages = totalcounts / Int32.Parse(counts) + 1;
                    }
                    else
                    {
                        totalpages = totalcounts / Int32.Parse(counts);
                    }
                    if (totalcounts == 0)
                    {
                        //MessageBox.Show("未搜索到数据", "系统提示");
                        return "[]";
                    }
                    if (Int32.Parse(page) > totalpages)
                    {
                        MessageBox.Show("当前输入页数大于总页数", "系统提示");
                        return "[]";
                    }
                }  

                for (int i = 0; i < iCounts; i++)
                {
                    if(((iPage - 1) * iCounts + i) == totalcounts)
                    {
                        break;
                    }

                    if (QueryDataRecordList[(iPage - 1) * iCounts + i].openlocktime != "")
                    {
                        tempList.Add(QueryDataRecordList[(iPage - 1) * iCounts + i]);
                    }
                    else
                    {
                        break;
                    }
                    //RecordLog.GetInstance().WriteLog(Level.Info, string.Format("搜索分页返回数据完成,page:[{0}],count:[{1}]",iPage, (iPage - 1) * iCounts + i + 1));
                }

                object obj = new
                {
                    totalcounts = totalcounts,
                    pagedata = tempList
                };
                string json = JsonConvert.SerializeObject(obj);
                //string json = JsonConvert.SerializeObject(goableData._recordlogsdatadetailsList);
                return json;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "分页时异常：" + ex.Message);
                return "";
            }
        }

        public string SearchWarningResult(string starttime, string endtime, string page, string counts)
        {
            string msg = string.Empty;
            List<WarningRecordDetails> QueryDataRecordList = DataAnalysis.GetInstance().QueryWarningDataView(
                out msg, EventClass.GetInstance()._warningdataTable, starttime, endtime);
            if (msg != "Success")
            {
                return msg;
            }

            List<WarningRecordDetails> tempList = new List<WarningRecordDetails>();

            int iPage = Int32.Parse(page);
            int iCounts = Int32.Parse(counts);

            try
            {
                int totalcounts = QueryDataRecordList.Count;
                int totalpages = 0;

                if (totalcounts % Int32.Parse(counts) > 0)
                {
                    totalpages = totalcounts / Int32.Parse(counts) + 1;
                }
                else
                {
                    totalpages = totalcounts / Int32.Parse(counts);
                }
                if (totalcounts == 0)
                {
                    //MessageBox.Show("未搜索到数据", "系统提示");
                    return "[]";
                }
                if (Int32.Parse(page) > totalpages)
                {
                    //MessageBox.Show("当前输入页数大于总页数", "系统提示");
                    return "[]";
                }

                for (int i = 0; i < iCounts; i++)
                {
                    if (((iPage - 1) * iCounts + i) == totalcounts)
                    {
                        break;
                    }

                    if (QueryDataRecordList[(iPage - 1) * iCounts + i]._warningdatetime != "")
                    {
                        tempList.Add(QueryDataRecordList[(iPage - 1) * iCounts + i]);
                    }
                    else
                    {
                        break;
                    }
                    RecordLog.GetInstance().WriteLog(Level.Info, string.Format("搜索报警分页返回数据完成,page:[{0}],count:[{1}]", iPage, (iPage - 1) * iCounts + i + 1));
                }

                object obj = new
                {
                    totalcounts = totalcounts,
                    pagedata = tempList
                };
                string json = JsonConvert.SerializeObject(obj);
                //string json = JsonConvert.SerializeObject(goableData._recordlogsdatadetailsList);
                return json;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "搜索报警分页时异常：" + ex.Message);
                return "";
            }
        }
            

        /// <summary>
        /// 数据分析接口
        /// </summary>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string StatisticalAnalysis(string lockname, string starttime, string endtime, string username)
        {
            string msg = string.Empty;
            RecordGolable obj = DataAnalysis.GetInstance().StatisticalAnalysis(out msg, EventClass.GetInstance()._gloabledataTable, lockname, starttime, endtime, username);
            if (msg != "Success")
            {
                return msg;
            }
            else
            {
                string json = JsonConvert.SerializeObject(obj._monthlydisplaysList);
                return json;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string TotalDataAnalysis(string lockname, string starttime, string endtime)
        {
            string msg = string.Empty;
            RecordGolable obj = DataAnalysis.GetInstance().totalAnalysis(out msg, EventClass.GetInstance()._gloabledataTable, lockname, starttime, endtime);
            if (msg != "Success")
            {
                return msg;
            }
            else
            {
                string json = JsonConvert.SerializeObject(obj._analysisData);
                return json;
            }
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        public void DataTableToExcel()
        {
            string path = string.Empty;
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
                return;
            }
            if (saveFileDialog.FileName == "")
                return;
            Task ta = Task.Factory.StartNew(delegate { DoExcelHelper.GetInstance().DataTableToExcel(EventClass.GetInstance()._gloabledataTable, saveFileDialog.FileName, "Table1"); });
            Task.WaitAny(ta);

            //DoExcelHelper.GetInstance().DataGridtoExcelModify(EventClass.GetInstance()._gloabledataTable);

        }
        /// <summary>
        /// 数据导入
        /// </summary>
        public void ImportExcelToDataTable()
        {
            EventClass.GetInstance().tb = EventClass.GetInstance()._temptb
                = DoExcelHelper.GetInstance().ImportExcelToDataTable("readdataTable");
            //if(EventClass.GetInstance()._temptb.Rows.Count != 0)
            //{
            //    MessageBox.Show("导入数据成功");
            //}
            
        }
        /// <summary>
        /// 导入数据分页展示
        /// </summary>
        /// <param name="page"></param>
        /// <param name="counts"></param>
        /// <returns></returns>
        public string GetRecordImportEcxel(string page, string counts)
        {
            RecordGolable goableData = new RecordGolable();
            List<RecordLogsDataDetails> tempList = new List<RecordLogsDataDetails>();

            Task ta = Task.Factory.StartNew(delegate { goableData._recordlogsdatadetailsList = RedvelopRecord.GetInstance().ReadDataTableToMemory(EventClass.GetInstance().tb); });
            Task.WaitAny(ta);
            int iPage = Int32.Parse(page);
            int iCounts = Int32.Parse(counts);

            try
            {
                int totalcounts = goableData._recordlogsdatadetailsList.Count;
                int totalpages = 0;

                if (totalcounts % Int32.Parse(counts) > 0)
                {
                    totalpages = totalcounts / Int32.Parse(counts) + 1;
                }
                else
                {
                    totalpages = totalcounts / Int32.Parse(counts);
                }
                if (Int32.Parse(page) > totalpages)
                {
                    //MessageBox.Show("当前输入页数大于总页数", "系统提示");
                    return "[]";
                }

                for (int i = 0; i < iCounts; i++)
                {
                    if (((iPage - 1) * iCounts + i) == totalcounts)
                    {
                        break;
                    }
                    if (goableData._recordlogsdatadetailsList[(iPage - 1) * iCounts + i].openlocktime != "")
                    {
                        tempList.Add(goableData._recordlogsdatadetailsList[(iPage - 1) * iCounts + i]);
                    }
                    else
                    {
                        break;
                    }
                    //RecordLog.GetInstance().WriteLog(Level.Info, string.Format("查询分页返回数据完成,page:[{0}],count:[{1}]", iPage, (iPage - 1) * iCounts + i + 1));
                }

                object obj = new
                {
                    totalcounts = totalcounts,
                    pagedata = tempList
                };

                string json = JsonConvert.SerializeObject(obj);
                //string json = JsonConvert.SerializeObject(goableData._recordlogsdatadetailsList);
                return json;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "分页时异常：" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 模拟门锁连接
        /// </summary>
        /// <param name="simMac"></param>
        public void SimConnect(string simMac)
        {
            EventClass.GetInstance()._ConnectBleAddress = simMac;
        }

        /// <summary>
        /// 模拟设置开锁方式
        /// </summary>
        public void SimSettingType(string id,string locktype)
        {
            string msg = string.Empty;
            RedvelopRecord.GetInstance().InitDoorLockType(out msg, id.ToString(), locktype);
        }

        /// <summary>
        /// 外部测试数据，模拟开锁数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inData"></param>
        public void SimDateInTest(string simMac ,string id,string inData)
        {
            EventClass.GetInstance()._ConnectBleAddress = simMac;           

            RecordDetail recordDetail = new RecordDetail()
            {
                _opendatetime = DateTime.Parse(inData.Split(',')[0]).ToString("yyyy/MM/dd HH:mm:ss"),
                _userOpenType = inData.Split(',')[1]
            };

            string msg = string.Empty;
            RedvelopRecord.GetInstance().WriteData(out msg, id.ToString(), recordDetail, null, "record");
        }
    }
}
