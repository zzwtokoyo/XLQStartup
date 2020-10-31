using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLQStartup.操作类;
using XLQStartup.日志;

namespace XLQStartup.记录数据文件类
{
    /// <summary>
    /// 用户操作记录类
    /// </summary>
    [Serializable]
    internal class RecordDetail
    {
        #region 用户操作记录类
        /// <summary>
        /// 用户开锁时间
        /// </summary>
        public string _opendatetime { set; get; } = string.Empty;
        /// <summary>
        /// 用户设置时间
        /// </summary>
        public string _settingdatetime { get; set; } = string.Empty;        
        /// <summary>
        /// 开锁的方式
        /// </summary>
        public string _userOpenType { get; set; } = string.Empty;
        /// <summary>
        /// 用户设置类型
        /// </summary>
        public string _userSettingType { get; set; } = string.Empty;
        /// <summary>
        /// 用户设置的细节
        /// </summary>
        public string _userSettingDetails { get; set; } = string.Empty;
        #endregion
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    [Serializable]
    internal class userDetails
    {
        #region 用户信息
        /// <summary>
        /// 用户Id
        /// </summary>
        public string _useId { get; set; } = string.Empty;
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string _useName { get; set; } = string.Empty;
        /// <summary>
        /// 用户性别
        /// </summary>
        public string _useSex { get; set; } = string.Empty;
        /// <summary>
        /// 用户职位
        /// </summary>
        public string _userjob { get; set; } = string.Empty;
        /// <summary>
        /// 用户权限级别
        /// </summary>
        public int _uselevel { get; set; } = 0;
        /// <summary>
        /// 添加可用的门锁设备信息
        /// </summary>
        public string _lockDetail_id { get; set; } = string.Empty;
        #endregion
    }
    /// <summary>
    /// 门锁设置信息
    /// </summary>
    [Serializable]
    internal class lockDetail
    {
        #region 门锁设置信息
        /// <summary>
        /// 门锁编号即门锁的mac地址
        /// </summary>
        public string _id { get; set; } = string.Empty;
        /// <summary>
        /// 自定义名称
        /// </summary>
        public string _modifyName { get; set; } = string.Empty;
        #endregion
    }
    /// <summary>
    /// 开锁方式设置
    /// </summary>
    [Serializable]
    internal class lockSettingType
    {
        #region 开锁方式设置
        /// <summary>
        /// 门锁名称
        /// </summary>
        public string _lockModifyName { get; set; } = string.Empty;
        /// <summary>
        /// 开门方式
        /// </summary>
        public string _openLockType { get; set; } = string.Empty;
        /// <summary>
        /// 用户信息
        /// </summary>
        public userDetails userDetails = new userDetails();
        /// <summary>
        /// 用户操作记录
        /// </summary>
        public RecordDetail recordDetail = new RecordDetail();
        /// <summary>
        /// 用户授权时间：上班时间段
        /// </summary>
        public string _aollowTime { get; set; } = string.Empty;
        /// <summary>
        /// 天， 分， 时 当前未定义
        /// </summary>
        public string _totalTime { get; set; } = string.Empty;
        /// <summary>
        /// 次数限制.5次/分钟 这样类似
        /// </summary>
        public int _openlimitTimes { get; set; } = 0;
        #endregion
    }

    /// <summary>
    /// 报警操作记录类
    /// </summary>
    [Serializable]
    internal class WarningRecordDetails
    {
        #region 报警操作记录类
        /// <summary>
        /// 报警的时间
        /// </summary>
        public string _warningdatetime { get; set; } = string.Empty;
        /// <summary>
        /// 报警的类型
        /// </summary>
        public string _warningType { get; set; } = string.Empty;
        /// <summary>
        /// 报警的状态
        /// </summary>
        public string _warningStatus { get; set; } = string.Empty;
        #endregion
    }

    /// <summary>
    /// 全局集合类
    /// </summary>
    [Serializable]
    internal class RecordGolable
    {
        #region 全局集合类
        /// <summary>
        /// 通用类集合
        /// </summary>
        public List<object> _objlist = new List<object>();
        /// <summary>
        /// 用户集合
        /// </summary>
        public List<lockSettingType> _lockSettingTypeList = new List<lockSettingType>();
        /// <summary>
        /// 报警集合
        /// </summary>
        public List<WarningRecordDetails> _warningRecordList = new List<WarningRecordDetails>();
        /// <summary>
        /// 操作日志
        /// </summary>
        public List<RecordLogsDataDetails> _recordlogsdatadetailsList = new List<RecordLogsDataDetails>();
        /// <summary>
        /// 用户信息字段，全局变量，方便查询
        /// </summary>
        public Dictionary<string, List<lockSettingType>> _diclockSettingType = new Dictionary<string, List<lockSettingType>>();
        /// <summary>
        /// 报警信息字典，全局变量，方便查询
        /// </summary>
        public Dictionary<string, List<WarningRecordDetails>> _dicWarningRecord = new Dictionary<string, List<WarningRecordDetails>>();
        /// <summary>
        /// 操作信息字典，全局变量，方便查询
        /// </summary>
        public Dictionary<string, List<RecordLogsDataDetails>> _dicrecordlogsdatadetails = new Dictionary<string, List<RecordLogsDataDetails>>();
        /// <summary>
        /// 统计分析
        /// </summary>
        public List<Monthlydisplay> _monthlydisplaysList = new List<Monthlydisplay>();
        /// <summary>
        /// 统计分析,全盘分析
        /// </summary>
        public AnalysisData _analysisData = new AnalysisData();
        #endregion
    }

    /// <summary>
    /// 日志合集类
    /// </summary>
    [Serializable]
    internal class RecordLogsDataDetails
    {
        #region 日志合集类
        /// <summary>
        /// 操作门锁时间
        /// </summary>
        public string openlocktime { get; set; } = string.Empty;
        /// <summary>
        /// 门锁mac
        /// </summary>
        public string lockmacid { get; set; } = string.Empty;
        /// <summary>
        /// 门锁Id
        /// </summary>
        public string lockid { get; set; } = string.Empty;
        /// <summary>
        /// 门锁自定义名称
        /// </summary>
        public string lockmodifyname { get; set; } = string.Empty;
        /// <summary>
        /// 门锁开门操作
        /// </summary>
        public string openlocktype { get; set; } = string.Empty;
        /// <summary>
        /// 用户id
        /// </summary>
        public string userid { get { return lockid; } set { lockid = value; } }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string username { get; set; } = string.Empty;
        /// <summary>
        /// 用户职位
        /// </summary>
        public string userjob { get; set; } = string.Empty;
        /// <summary>
        /// 用户等级权限
        /// </summary>
        public int userlevel { get; set; } = 0;
        /// <summary>
        /// 用户开门时间
        /// </summary>
        public string opentime { get; set; } = string.Empty;
        /// <summary>
        /// 天， 分， 时 当前未定义
        /// </summary>
        public string opentotalTime { get; set; } = string.Empty;
        /// <summary>
        /// 次数限制.5次/分钟 这样类似
        /// </summary>
        public int openlimitTimes { get; set; } = 0;
        /// <summary>
        /// 用户开门方式错误
        /// </summary>
        public string openexception { get; set; } = string.Empty;
        /// <summary>
        /// 用户开门状态是否越权
        /// </summary>
        public string userstatus { get; set; } = "正常";
        #endregion
    }

    /// <summary>
    /// 按月显示统计分析内容
    /// </summary>
    [Serializable]
    internal class Monthlydisplay
    {
        public Dictionary<string, int> dicmonth = new Dictionary<string, int>();
        public Dictionary<string, StatisticalAnalysis> dicstatus = new Dictionary<string, StatisticalAnalysis>();
    }

    /// <summary>
    /// 统计分析全量内容
    /// </summary>
    [Serializable]
    internal class AnalysisData
    {
        /// <summary>
        /// 所有用户正常次数总和
        /// </summary>
        public int normal { get; set; } = 0;
        /// <summary>
        /// 所有用户异常次数总和
        /// </summary>
        public int exception { get; set; } = 0;
        /// <summary>
        /// 所有用户次数总和
        /// </summary>
        public int total { get; set; } = 0;
        //用户信息json数组
        public List<userinfoAnalysis> userinfo = new List<userinfoAnalysis>();
    }

    internal class userinfoAnalysis
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string name { get; set; } = string.Empty;
        //当前用户json数组
        public List<analysisdata> analysisdata = new List<analysisdata>();
        /// <summary>
        /// 该用户所有正常次数
        /// </summary>
        public int allnormaltimes { get; set; } = 0;
        /// <summary>
        /// 该用户所有异常次数
        /// </summary>
        public int allexceptiontimes { get; set; } = 0;
        /// <summary>
        /// 该用户所有次数
        /// </summary>
        public int alltotaltimes { get; set; } = 0;
        /// <summary>
        /// 该用户异常次数情况占比（与所有总异常次数比）
        /// </summary>
        public string abnormaloccupancyrate { get; set; } = string.Empty;       
    }

    internal class analysisdata
    {
        /// <summary>
        /// 月份
        /// </summary>
        public int month { get; set; } = 0;
        /// <summary>
        /// 当月正常次数
        /// </summary>
        public int normaltimes { get; set; } = 0;
        /// <summary>
        /// 当月异常
        /// </summary>
        public int exceptiontimes { get; set; } = 0;
        /// <summary>
        /// 当月次数
        /// </summary>
        public int totaltimes { get; set; } = 0;
        /// <summary>
        /// 本月异常率
        /// </summary>
        public string abnormalityrate { get; set; } = string.Empty;
    }

    /// <summary>
    /// 日志统计分析类
    /// </summary>
    [Serializable]
    internal class StatisticalAnalysis
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 用户角色
        /// </summary>
        public string UserJob { get; set; } = string.Empty;
        /// <summary>
        /// 分析周期内的所有异常次数
        /// </summary>
        public int ExcetptionTotalTimes { get; set; } = 0;
        /// <summary>
        /// 分析周期内的所有正常次数
        /// </summary>
        public int SuccessTotalTimes { get; set; } = 0;
        /// <summary>
        /// 分析周期内的所有次数
        /// </summary>
        public int TotalTimes { get; set; } = 0;
    }
}
