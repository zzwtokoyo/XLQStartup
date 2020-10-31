using System;
using System.Windows.Forms;
using wclBluetooth;
using XLQStartup.全局类;
using XLQStartup.日志;

namespace XLQStartup.操作类
{
    /// <summary>
    /// wcl协议操作函数，继承于全局类
    /// </summary>
    public class WclOperatedFunction : GlobalParams
    {
        /// <summary>
        /// 本机蓝牙信息
        /// </summary>
        /// <returns></returns>
        public wclBluetoothRadio GetRadio()
        {
            try
            {
                for (int i = 0; i < this._manager.Count; i++)
                {
                    if (this._manager[i].Available)
                    {
                        return this._manager[i];
                    }
                }
                RecordLog.GetInstance().WriteLog(Level.Warning, "No one Bluetooth Radio found.");
                //MessageBox.Show("No one Bluetooth Radio found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return null;
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "No one Bluetooth Radio found. Exception: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 清空内存
        /// </summary>
        public void Cleanup()
        {
            try
            {
                this._ConnectBleAddress = string.Empty;
                this._fCharacteristics = null;
                this._fDescriptors = null;
                this._fServices = null;
                this._devices.Clear();
                this._deviceAddress.Clear();
                this._dicdeviceAddress.Clear();
                this._bleResult.Clear();
                this._bleSettingResult.Clear();
                this._bleWarningResult.Clear();
                this.knowsLock.Clear();
                this._gloabledataTable.Clear();
                this._warningdataTable.Clear();
                this.tb.Clear();
                this._temptb.Clear();
                this.Eopen = "M";
                this.lastEopen = "M";
            }
            catch(Exception ex)
            {
                //MessageBox.Show("释放内存异常: " + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="package"></param>
        public void WriteCommand(byte[] package)
        {
            try
            {
                string str = string.Empty;
                for (int i = 0; i < package.Length; i++)
                {
                    str += package[i].ToString("X2");
                }
                if (this._client.WriteCharacteristicValue(_txCharacteristic, package, wclGattProtectionLevel.plNone) != 0)
                {
                    this.TraceEvent(_client.Address, "TX", "Status", "Failed");
                    return;
                }
                this.TraceEvent(_client.Address, "TX", "Data", str);
                this.TraceEvent(_client.Address, "TX", "Status", "Success");
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 根据条件写事件日志函数
        /// </summary>
        /// <param name="address"></param>
        /// <param name="Event"></param>
        /// <param name="param"></param>
        /// <param name="value"></param>
        public void TraceEvent(long address, string Event, string param, string value = "")
        {
            try
            {
                string s = string.Empty;
                if (address != 0L)
                {
                    s = address.ToString("X12");
                }
                switch (value)
                {
                    case "Failed":
                        RecordLog.GetInstance().WriteLog(Level.Error, "Address:" + s + ",TX:" + Event + ",param:" + param);
                        break;
                    case "Success":
                        RecordLog.GetInstance().WriteLog(Level.Info, "Address:" + s + ",TX:" + Event + ",param:" + param);
                        break;
                    default:
                        RecordLog.GetInstance().WriteLog(Level.Info, "Address:" + s + ",TX:" + Event + ",param:" + param + ",value:" + value);
                        break;
                }
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
        /// <summary>
        /// 获取GATT服务
        /// </summary>
        public void GetService()
        {
            try
            {
                this._fServices = null;
                int resServices = this._client.ReadServices(wclGattOperationFlag.goNone, out this._fServices);
                if (resServices != 0)
                {
                    RecordLog.GetInstance().WriteLog(Level.Warning, "Error: 0x" + resServices.ToString("X8"));
                    //MessageBox.Show("Error: 0x" + resServices.ToString("X8"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    this.TraceEvent(this._client.Address, "Services", "FF00", "Failed");
                    this._client.Disconnect();
                    return;
                }
                this.TraceEvent(this._client.Address, "Services", "FF00", "Success");
                if (this._fServices == null)
                {
                    return;
                }
                foreach (wclGattService service in this._fServices)
                {
                    if (service.Uuid.IsShortUuid && this._uuid.ShortUuid == service.Uuid.ShortUuid)
                    {
                        if (this._client.ReadCharacteristics(service, wclGattOperationFlag.goNone, out this._fCharacteristics) != 0)
                        {
                            return;
                        }
                        foreach (wclGattCharacteristic character in this._fCharacteristics)
                        {
                            if (character.Uuid.ShortUuid == this._rxUuid)
                            {
                                if (this._client.Subscribe(character) != 0)
                                {
                                    this.TraceEvent(this._client.Address, "Subs", "RX", "Failed");
                                    return;
                                }
                                this.TraceEvent(this._client.Address, "Subs", "RX", "Success");
                                if (this._client.ReadDescriptors(character, wclGattOperationFlag.goNone, out this._fDescriptors) != 0)
                                {
                                    this.TraceEvent(this._client.Address, "Subs", "RX", "Failed to get description");
                                    return;
                                }
                            }
                            else if (character.Uuid.ShortUuid == this._txUuid)
                            {
                                this._txCharacteristic = character;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Exception: " + ex.Message);
            }
        }
    }
}
