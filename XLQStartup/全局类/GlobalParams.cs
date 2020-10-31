using System.Collections.Generic;
using System.Data;
using wclBluetooth;

namespace XLQStartup.全局类
{
    /// <summary>
    /// BLE Use GlobalParams
    /// </summary>
    public class GlobalParams
    {
        /// <summary>
        /// 保存设备对应的相关信息
        /// </summary>
        public readonly Dictionary<string, wclBluetoothRadio> _devices = new Dictionary<string, wclBluetoothRadio>();
        /// <summary>
        /// 声明wclGattClient
        /// </summary>
        public wclGattClient _client;
        /// <summary>
        /// 声明wclGattCharacteristic[]
        /// </summary>
        public wclGattCharacteristic[] _fCharacteristics;
        /// <summary>
        /// 声明wclGattCharacteristic
        /// </summary>
        public wclGattCharacteristic _txCharacteristic;
        /// <summary>
        /// 声明wclGattDescriptor[]
        /// </summary>
        public wclGattDescriptor[] _fDescriptors;
        /// <summary>
        /// 声明wclGattService[]
        /// </summary>
        public wclGattService[] _fServices;
        /// <summary>
        /// 声明wclBluetoothManager
        /// </summary>
        public wclBluetoothManager _manager;
        /// <summary>
        /// 声明EventDelegate
        /// </summary>
        public EventDelegate _eventDelegate;
        /// <summary>
        /// 声明wclGattUuid
        /// </summary>
        public readonly wclGattUuid _uuid;
        /// <summary>
        /// 初始化变量_txUuid
        /// </summary>
        public ushort _txUuid = 65281;
        /// <summary>
        /// 初始化变量_rxUuid
        /// </summary>
        public ushort _rxUuid = 65282;
        /// <summary>
        /// 初始化字节数组val_GetRecord
        /// </summary>
        public byte[] val_GetRecord;
        /// <summary>
        /// 保存搜索到的地址集合
        /// </summary>
        public List<string> _deviceAddress = new List<string>();
        /// <summary>
        /// 保存搜索到的地址集合,字典
        /// </summary>
        public Dictionary<string,string> _dicdeviceAddress = new Dictionary<string, string>();
        /// <summary>
        /// 保存上送的本次所有信息集合
        /// </summary>
        public List<string> _bleResult = new List<string>();
        public List<string> _bleSettingResult = new List<string>();
        public List<string> _bleWarningResult = new List<string>();
        /// <summary>
        /// 需要连接（适配）的BLE蓝牙设备地址
        /// </summary>
        public string _ConnectBleAddress { get; set; } = string.Empty;
        /// <summary>
        /// 过程标记为
        /// </summary>
        public int _processFlags { get; set; }
        /// <summary>
        /// 结果标记为
        /// </summary>
        public int _resultFlags { get; set; }
        /// <summary>
        /// 获取记录报文
        /// </summary>
        public const string _getRecordMsg = "AA55000133FFFFFFFFFFFFFFFFFFFFFFFFFFFF4F";
        /// <summary>
        /// 获取配置记录报文，不包含CRC校验位
        /// </summary>
        public const string _getSettingDataBody = "AA55000134FFFFFFFFFFFFFFFFFFFFFFFFFFFF";//A2
        /// <summary>
        /// 获取报警记录报文，不包含CRC校验位
        /// </summary>
        public const string _getWarningDataBody = "AA55000135FFFFFFFFFFFFFFFFFFFFFFFFFFFF";//64
        /// <summary>
        /// 全局数据表
        /// </summary>
        public DataTable _gloabledataTable = new DataTable();
        public DataTable _warningdataTable = new DataTable();
        public DataTable tb = new DataTable();
        public DataTable _temptb = new DataTable();
        /// <summary>
        /// 当前操作标记.
        ///  0 读取操作记录
        ///  1 读取配置记录
        ///  2 读取报警记录
        /// </summary>
        public int _processName { get; set; } = 0;
        /// <summary>
        /// 电量
        /// </summary>
        public int _battery { get; set; } = 0;
        /// <summary>
        /// 记录存在的门锁信息并用于连接
        /// </summary>
        public List<string> knowsLock = new List<string>();
        public string Eopen { get; set; } = "M";
        public string lastEopen { get; set; } = "M";
        /// <summary>
        /// 是否记录日志
        /// </summary>
        public string RecordLogFunc { get; set; } = string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        public GlobalParams()
        {
            this._uuid.ShortUuid = 65280;
        }
    }
}
