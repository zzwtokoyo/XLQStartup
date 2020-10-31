using System;
using XLQStartup.日志;

namespace XLQStartup.操作类
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    internal static class BleProtocol
    {
        /// <summary>
        /// 操作数据记录
        /// </summary>
        public static OperationRecord Record { get; set; }
        /// <summary>
        /// 操作数据记录
        /// </summary>
        public static SettingRecord settingRecord { get; set; }
        /// <summary>
        /// 操作数据记录
        /// </summary>
        public static WarningRecord warningRecord { get; set; }
        /// <summary>
        /// 拆包函数
        /// </summary>
        /// <param name="data"></param>
        public static void PackageAnalyze(byte[] data)
        {
            try
            {
                byte[] dest = new byte[data.Length - 3];
                for (int i = 2; i < data.Length - 1; i++)
                {
                    dest[i - 2] = data[i];
                }
                if (data[0] == 0xAA && data[1] == 0x55)
                {
                    byte b = data[data.Length - 1];
                    byte crcCalced = CRC.Crc8(dest);
                    if (b == crcCalced)
                    {
                        BleProtocol.CommandHandler(data[4] & 127, data);
                    }
                }
            }
            catch
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "PackageAnalyze Error.");
            }    
        }
        /// <summary>
        /// 命令事件:读取记录
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        private static void CommandHandler(int command, byte[] data)
        {
            if (command == 51)//0x33
            {//操作
                EventClass.GetInstance()._processName = 0;
                BleProtocol.OperationRecordHandler(data);
            }
            if (command == 52)//0x34
            {//配置
                EventClass.GetInstance()._processName = 1;
                BleProtocol.SettingRecordHandler(data);
            }
            if (command == 53)//0x35
            {//报警
                EventClass.GetInstance()._processName = 2;
                BleProtocol.WarningRecordHandler(data);
            }
        }
        /// <summary>
        /// 获取记录操作函数
        /// </summary>
        /// <param name="data"></param>
        private static void OperationRecordHandler(byte[] data)
        {
            if (data[3] == 2)//0x02
            {
                switch (data[5])
                {
                    case 2://0x02
                        //RecordLog.GetInstance().WriteLog(Level.Info, "操作错误.");
                        break;
                    case 3://0x03
                        RecordLog.GetInstance().WriteLog(Level.Info, "所有开门事件记录已经上报.");
                        return;
                    case 4://0x04
                        RecordLog.GetInstance().WriteLog(Level.Info, "所有操作记录等非开门事件已经上报.");
                        return;
                    default:
                        return;
                }
            }
            else
            {
                BleProtocol.Record = new OperationRecord();
                int year = BleProtocol.Bcd2Int(data[10]);
                int month = BleProtocol.Bcd2Int(data[9]);
                int day = BleProtocol.Bcd2Int(data[8]);
                int hour = BleProtocol.Bcd2Int(data[7]);
                int minute = BleProtocol.Bcd2Int(data[6]);
                int second = BleProtocol.Bcd2Int(data[5]);
                BleProtocol.Record.Date = new DateTime(2000 + year, month, day, hour, minute, second); // 日期
                //BleProtocol.Record.UserId = (int)data[11] + ((int)data[12] << 8); //id
                BleProtocol.Record.UserId = (int)data[11]; //id
                BleProtocol.Record.OperationType = (int)data[12] + (int)data[13]; //机械操作方式
                BleProtocol.Record.UserType = (int)data[14] + (int)data[15]; //开锁方式
                EventClass.GetInstance()._battery = (int)data[16];
            }
        }
        /// <summary>
        /// 读取配置记录函数
        /// </summary>
        /// <param name="data"></param>
        private static void SettingRecordHandler(byte[] data)
        {
            if (data[3] == 2)//0x02
            {
                switch (data[5])
                {
                    case 2://0x02
                        //RecordLog.GetInstance().WriteLog(Level.Info, "操作错误.");
                        break;
                    case 3://0x03
                        RecordLog.GetInstance().WriteLog(Level.Info, "配置事件记录已经上报.");
                        return;
                    case 4://0x04
                        RecordLog.GetInstance().WriteLog(Level.Info, "所有操作等配置事件记录已经上报.");
                        return;
                    default:
                        return;
                }
            }
            else
            {
                BleProtocol.settingRecord = new SettingRecord();
                int year = BleProtocol.Bcd2Int(data[10]);
                int month = BleProtocol.Bcd2Int(data[9]);
                int day = BleProtocol.Bcd2Int(data[8]);
                int hour = BleProtocol.Bcd2Int(data[7]);
                int minute = BleProtocol.Bcd2Int(data[6]);
                int second = BleProtocol.Bcd2Int(data[5]);
                BleProtocol.settingRecord.Date = new DateTime(2000 + year, month, day, hour, minute, second);
                BleProtocol.settingRecord.SettingType = (int)data[11];
                BleProtocol.settingRecord.SettingDetils = (int)data[12];
            }
        }
        /// <summary>
        /// 读取报警记录函数
        /// </summary>
        /// <param name="data"></param>
        private static void WarningRecordHandler(byte[] data)
        {
            if (data[3] == 2)//0x02
            {
                switch (data[5])
                {
                    case 2://0x02
                        //RecordLog.GetInstance().WriteLog(Level.Info, "操作错误.");
                        break;
                    case 3://0x03
                        RecordLog.GetInstance().WriteLog(Level.Info, "开门报警事件全部上报.");
                        return;
                    case 4://0x04
                        RecordLog.GetInstance().WriteLog(Level.Info, "配置等其他报警事件已上报.");
                        return;
                    default:
                        return;
                }
            }
            else
            {
                BleProtocol.warningRecord = new WarningRecord();
                int year = BleProtocol.Bcd2Int(data[10]);
                int month = BleProtocol.Bcd2Int(data[9]);
                int day = BleProtocol.Bcd2Int(data[8]);
                int hour = BleProtocol.Bcd2Int(data[7]);
                int minute = BleProtocol.Bcd2Int(data[6]);
                int second = BleProtocol.Bcd2Int(data[5]);
                BleProtocol.warningRecord.Date = new DateTime(2000 + year, month, day, hour, minute, second);
                BleProtocol.warningRecord.WarningType = (int)data[11];
                BleProtocol.warningRecord.WarningStatus = (int)data[12];
            }
        }
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static byte[] GetUpdateTimePackage()
        {
            byte[] data = new byte[20];
            byte[] crcBytes = new byte[17];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = byte.MaxValue;
            }
            data[0] = 0xAA;
            data[1] = 0x55;
            data[2] = 0;
            data[3] = 0x09;
            data[4] = 0x1A;
            data[5] = 0;
            DateTime date = DateTime.Now;
            data[6] = BleProtocol.Int2Bcd(date.Year);
            data[7] = BleProtocol.Int2Bcd(date.Month);
            data[8] = BleProtocol.Int2Bcd(date.Day);
            data[9] = BleProtocol.Int2Bcd(date.Hour);
            data[10] = BleProtocol.Int2Bcd(date.Minute);
            data[11] = BleProtocol.Int2Bcd(date.Second);
            data[12] = BleProtocol.Int2Bcd((int)date.DayOfWeek);
            for (int j = 0; j < crcBytes.Length; j++)
            {
                crcBytes[j] = data[j + 2];
            }
            data[19] = CRC.Crc8(crcBytes);
            return data;
        }
        /// <summary>
        /// BCD码转Int
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int Bcd2Int(byte b)
        {
            return ((b & 0xf0) >> 4) * 10 + (b & 0x0f);
        }
        /// <summary>
        /// Int转BCD
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static byte Int2Bcd(int i)
        {
            i %= 100;
            int temp = ((i / 10 & 0x0f) << 4) + (i % 10 & 0x0f);
            return Convert.ToByte(temp);
        }
    }
    /// <summary>
    /// 操作记录类
    /// </summary>
    public class OperationRecord
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public int OperationType { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; }
    }
    /// <summary>
    /// 操作记录类
    /// </summary>
    public class SettingRecord
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public int SettingType { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public int SettingDetils { get; set; }        
    }
    /// <summary>
    /// 操作记录类
    /// </summary>
    public class WarningRecord
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 报警类型
        /// </summary>
        public int WarningType { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public int WarningStatus { get; set; }
    }
    /// <summary>
    /// CRC校验
    /// </summary>
    internal static class CRC
    {
        /// <summary>
        /// CRC检验函数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte Crc8(byte[] data)
        {
            int crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = (crc >> 1 ^ 0x8c);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return (byte)crc;
        }
    }
}
