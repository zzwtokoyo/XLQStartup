using System;
using System.Threading;
using System.Windows.Forms;
using wclBluetooth;
using XLQStartup.日志;
using XLQStartup.记录数据文件类;

namespace XLQStartup.操作类
{
    /// <summary>
    /// 事件类，继承于wcl操作协议函数类
    /// </summary>
    class EventClass : WclOperatedFunction
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static EventClass unique = new EventClass();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static EventClass GetInstance()
        {
            if (unique == null)
            {
                unique = new EventClass();
            }
            return unique;
        }
        /// <summary>
        /// 搜索开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="radio"></param>
        public void Manager_OnDiscoveringStarted(object sender, wclBluetoothRadio radio)
        {
            try
            {
                this.TraceEvent(0L, "Discovering started", "", "");
                //RecordLog.GetInstance().WriteLog(Level.Info, string.Format("开始搜索设备"));
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 已经搜索到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="radio"></param>
        /// <param name="address"></param>
        public void Manager_OnDeviceFound(object sender, wclBluetoothRadio radio, long address)
        {
            try
            {
                wclBluetoothDeviceType devType = wclBluetoothDeviceType.dtMixed;
                radio.GetRemoteDeviceType(address, out devType);
                this._devices.Add(address.ToString("X12"), radio);
                this.TraceEvent(address, "Device found", "", "");
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
            
        }
        /// <summary>
        /// 搜索完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="radio"></param>
        /// <param name="error"></param>
        public void Manager_OnDiscoveringCompleted(object sender, wclBluetoothRadio radio, int error)
        {
            try
            {
                this._processFlags = 1;
                if (this._devices.Count == 0)
                {
                    RecordLog.GetInstance().WriteLog(Level.Warning, "未发现可用的蓝牙设备");
                }
                else
                {
                    foreach (string value in this._devices.Keys)
                    {
                        long address = Convert.ToInt64(value, 16);
                        string devName = string.Empty;
                        int res = radio.GetRemoteName(address, out devName);
                        if (devName.Contains("SCH"))
                        {
                            if (res != 0)
                            {
                                RecordLog.GetInstance().WriteLog(Level.Error, "Error: 0x" + res.ToString("X8"));
                            }
                            else
                            {
                                //添加到主界面
                                this._deviceAddress.Add(devName + "|" + address.ToString("X12"));
                                RecordLog.GetInstance().WriteLog(Level.Info, "设备名称: " + devName.ToString() + ", 设备地址: " + address.ToString("X12"));
                            }
                        }
                        else
                        {
                            //RecordLog.GetInstance().WriteLog(Level.Warning, "Other Devices: " + devName.ToString());
                        }
                    }
                }
                this.TraceEvent(0L, "找到蓝牙设备", "", "");
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }

        //开锁方式是否存在        
        /// <summary>
        /// 发生数据变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="handle"></param>
        /// <param name="value"></param>
        public void Client_OnCharacteristicChanged(object sender, ushort handle, byte[] value)
        {
            try
            {
                this.TraceEvent(((wclGattClient)sender).Address, "ValueChanged", "Handle", handle.ToString("X4"));
                if (value == null)
                {
                    this.TraceEvent(0L, "", "Value", "");
                    return;
                }
                if (value.Length == 0)
                {
                    this.TraceEvent(0L, "", "Value", "");
                    return;
                }
                string str = string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    str += value[i].ToString("X2");
                }
                this.TraceEvent(((wclGattClient)sender).Address, "RX", str, "Success");
                BleProtocol.PackageAnalyze(value);
                if (GetInstance()._processName == 0)
                {
                    if (BleProtocol.Record != null)
                    {//操作记录
                        OperationRecord record = BleProtocol.Record;
                        string sOperType = string.Empty;
                        switch (record.UserType)
                        {
                            case 1:
                                Eopen = "E";
                                sOperType = "密码";
                                break;
                            case 2:
                                Eopen = "E";
                                sOperType = "指纹";
                                break;
                            case 3:
                                Eopen = "E";
                                sOperType = "刷卡";
                                break;
                            default:
                                Eopen = "M";
                                sOperType = "钥匙";
                                break;
                        }
                        //若是开锁方式，记录
                        if (Eopen == "E")
                        {
                            _bleResult.Add(record.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + record.UserId.ToString() + "|" + sOperType + "|" + record.UserType.ToString() + "|" + EventClass.GetInstance()._battery.ToString());
                            RecordLog.GetInstance().WriteLog(Level.Info, record.Date.ToString("yyyy/MM/dd HH:mm:ss") + " | " + record.UserId.ToString() + " | " + sOperType + " | " + record.UserType.ToString() + "|电量:" + EventClass.GetInstance()._battery.ToString());

                            //增加记录 begin                        
                            string msg = string.Empty;
                            RecordDetail _obj_Record_Details = new RecordDetail
                            {
                                _opendatetime = record.Date.ToString("yyyy/MM/dd HH:mm:ss"),
                                _userOpenType = sOperType
                            };
                            RedvelopRecord.GetInstance().WriteData(out msg, record.UserId.ToString(), _obj_Record_Details, null, "record");
                            if (!string.IsNullOrEmpty(msg))
                            {
                                _bleResult.Clear();
                                _bleResult.Add(msg);
                            }                            
                        }
                        //若不是开锁方式，那么判断之前的操作是不是开锁方式，如果是则不记录，不是的话则记录
                        else
                        {
                            if ( Eopen == lastEopen )
                            {
                                _bleResult.Add(record.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + record.UserId.ToString() + "|" + sOperType + "|" + record.UserType.ToString() + "|" + EventClass.GetInstance()._battery.ToString());
                                RecordLog.GetInstance().WriteLog(Level.Info, record.Date.ToString("yyyy/MM/dd HH:mm:ss") + " | " + record.UserId.ToString() + " | " + sOperType + " | " + record.UserType.ToString() + "|电量:" + EventClass.GetInstance()._battery.ToString());

                                //增加记录 begin                        
                                string msg = string.Empty;
                                RecordDetail _obj_Record_Details = new RecordDetail
                                {
                                    _opendatetime = record.Date.ToString("yyyy/MM/dd HH:mm:ss"),
                                    _userOpenType = sOperType
                                };
                                RedvelopRecord.GetInstance().WriteData(out msg, record.UserId.ToString(), _obj_Record_Details, null, "record");
                                if (!string.IsNullOrEmpty(msg))
                                {
                                    _bleResult.Clear();
                                    _bleResult.Add(msg);
                                }                                
                            }
                        }
                        //增加记录 end
                        BleProtocol.Record = null;
                        this.WriteCommand(this.val_GetRecord);
                        lastEopen = Eopen;
                    }
                    GetInstance()._processName = 0;
                }
                else if (GetInstance()._processName == 1)
                {
                    if (BleProtocol.settingRecord != null)
                    {//配置记录
                        SettingRecord settingrecord = BleProtocol.settingRecord;
                        string sSettingType = string.Empty;
                        switch (settingrecord.SettingType)
                        {
                            case 1:
                                sSettingType = "添加密码用户";
                                break;
                            case 2:
                                sSettingType = "删除密码用户";
                                break;
                            case 3:
                                sSettingType = "删除所有密码用户";
                                break;
                            case 4:
                                sSettingType = "添加卡用户";
                                break;
                            case 5:
                                sSettingType = "删除卡用户";
                                break;
                            case 6:
                                sSettingType = "删除所有卡用户";
                                break;
                            case 7:
                                sSettingType = "添加指纹用户";
                                break;
                            case 8:
                                sSettingType = "删除指纹用户";
                                break;
                            case 9:
                                sSettingType = "删除所有指纹用户";
                                break;
                            case 10:
                                sSettingType = "通道锁模式";
                                break;
                            case 11:
                                sSettingType = "高安全模式";
                                break;
                            case 12:
                                sSettingType = "音量 +";
                                break;
                            case 13:
                                sSettingType = "语言";
                                break;
                            case 14:
                                sSettingType = "恢复工厂模式";
                                break;
                            default:
                                sSettingType = "";
                                break;
                        }
                        _bleSettingResult.Add(settingrecord.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + sSettingType.ToString() + "|" + settingrecord.SettingDetils.ToString());
                        RecordLog.GetInstance().WriteLog(Level.Info, settingrecord.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + sSettingType.ToString() + "|" + settingrecord.SettingDetils.ToString());
                        if (settingrecord.SettingType == 1 || settingrecord.SettingType == 4 || settingrecord.SettingType == 7)
                        {
                            string locktype = string.Empty;
                            switch(settingrecord.SettingType)
                            {
                                case 1:
                                    locktype = "密码";
                                    break;
                                case 4:
                                    locktype = "刷卡";
                                    break;
                                case 7:
                                    locktype = "指纹";
                                    break;
                            }

                            string msg = string.Empty;
                            RedvelopRecord.GetInstance().InitDoorLockType(out msg, settingrecord.SettingDetils.ToString(), locktype);
                        }
                        BleProtocol.settingRecord = null;
                        this.WriteCommand(this.val_GetRecord);
                    }
                    GetInstance()._processName = 1;
                }
                else if (GetInstance()._processName == 2)
                {
                    if (BleProtocol.warningRecord != null)
                    {//配置记录
                        WarningRecord warningrecord = BleProtocol.warningRecord;
                        string sWarningType = string.Empty;
                        switch (warningrecord.WarningType)
                        {
                            case 1:
                                sWarningType = "防拆报警";
                                break;
                            case 2:
                                sWarningType = "高温报警";
                                break;
                            case 3:
                                sWarningType = "劫持报警";
                                break;
                            case 7:
                                sWarningType = "错误输入多次报警";
                                break;
                            default:
                                sWarningType = "";
                                break;
                        }
                        string sWarningStatus = string.Empty;
                        switch (warningrecord.WarningStatus)
                        {
                            case 0:
                                sWarningStatus = "报警触发";
                                break;
                            case 1:
                                sWarningStatus = "报警解除";
                                break;
                            default:
                                sWarningStatus = "报警触发";
                                break;
                        }
                        _bleWarningResult.Add(warningrecord.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + sWarningType + "|" + sWarningStatus);
                        RecordLog.GetInstance().WriteLog(Level.Info, warningrecord.Date.ToString("yyyy/MM/dd HH:mm:ss") + "|" + sWarningType + "|" + sWarningStatus);
                        string msg = string.Empty;
                        WarningRecordDetails _obj_WarningRecordDetails = new WarningRecordDetails
                        {
                            _warningdatetime = warningrecord.Date.ToString("yyyy/MM/dd HH:mm:ss"),
                            _warningType = sWarningType,
                            _warningStatus = sWarningStatus
                        };
                        RedvelopRecord.GetInstance().WriteData(out msg, "", null, _obj_WarningRecordDetails, "warning");
                        if (!string.IsNullOrEmpty(msg))
                        {
                            _bleWarningResult.Clear();
                            _bleWarningResult.Add(msg);
                        }
                        BleProtocol.warningRecord = null;
                        this.WriteCommand(this.val_GetRecord);
                    }
                    GetInstance()._processName = 2;
                }
                this.TraceEvent(0L, "", "Value", str);
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 连接（适配）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        public void Client_OnConnect(object sender, int error)
        {
            try
            {
                RecordLog.GetInstance().WriteLog(Level.Info, "Address:" + ((wclGattClient)sender).Address.ToString("X12") + 
                    ",TX:" + "Connected" + ",param:" + "0x" + ",value:" + error.ToString("X8"));
                //this.TraceEvent(((wclGattClient)sender).Address, "Connected", "Error",  + error.ToString("X8"));
                this.GetService();
                Thread.Sleep(200);
                Form1.SetCurrentTime();
                //防止前端忘记获取硬件设备及时信息        
                //Form1.GetWarningRecord();                
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        public void Client_OnDisconnect(object sender, int reason)
        {
            try
            {
                this.TraceEvent(((wclGattClient)sender).Address, "Disconnected", "Reason", "0x" + reason.ToString("X8"));
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 搜索BLE设备事件
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        public void BleSearchBtn_Event(object Sender, EventArgs e)
        {
            try
            {
                EventClass.GetInstance()._processFlags = 0;
                EventClass.GetInstance().Cleanup();
                wclBluetoothRadio radio = EventClass.GetInstance().GetRadio();
                if (radio != null)
                {
                    int res = radio.Discover(10, wclBluetoothDiscoverKind.dkBle);
                    if (res != 0)
                    {
                        RecordLog.GetInstance().WriteLog(Level.Info, "Error starting discovering: 0x" + res.ToString("X8"));
                    }
                }
                RecordLog.GetInstance().WriteLog(Level.Info, "Start Find");
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 按钮，连接事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BleConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this._client.Address = Convert.ToInt64(this._ConnectBleAddress, 16);
                wclBluetoothRadio radio = this._devices[this._ConnectBleAddress];
                if (this._client.Connect(radio) != 0)
                {
                    RecordLog.GetInstance().WriteLog(Level.Info, "Connect Failed");
                    MessageBox.Show("设备链接失败","提示");
                    return;
                }
                RecordLog.GetInstance().WriteLog(Level.Info, "Connect Success");                
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
    }
}
