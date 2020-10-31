using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLQStartup.操作类;
using XLQStartup.日志;

namespace XLQStartup.记录数据文件类
{
    /// <summary>
    /// 将日志数据进行DataTable处理
    /// </summary>
    class DataAnalysis
    {
        /// <summary>
        /// 将已经获取到的日志放到DataTable环境中保存
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataTable CreatNewDataTable(string tablename = "")
        {
            DataTable dt = new DataTable(tablename);
            try
            {
                if (tablename == "gloabledataTable")
                {
                    dt.Columns.Add("openlocktime", typeof(DateTime));
                    dt.Columns.Add("lockmacid", typeof(System.String));
                    dt.Columns.Add("lockid", typeof(System.String));
                    dt.Columns.Add("lockmodifyname", typeof(System.String));
                    dt.Columns.Add("openlocktype", typeof(System.String));
                    dt.Columns.Add("userid", typeof(System.String));
                    dt.Columns.Add("username", typeof(System.String));
                    dt.Columns.Add("userjob", typeof(System.String));
                    dt.Columns.Add("userlevel", typeof(System.String));
                    dt.Columns.Add("opentime", typeof(System.String));
                    dt.Columns.Add("opentotalTime", typeof(System.String));
                    dt.Columns.Add("openlimitTimes", typeof(System.String));
                    dt.Columns.Add("openexception", typeof(System.String));
                    dt.Columns.Add("userstatus", typeof(System.String));
                }
                if(tablename == "warningdataTable")
                {
                    dt.Columns.Add("warningtime", typeof(DateTime));
                    dt.Columns.Add("warningtype", typeof(System.String));
                    dt.Columns.Add("warningstatus", typeof(System.String));                    
                }
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("创建指定数据表{0}完成", tablename));
                return dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show("创建["+ tablename + "]数据表异常：" + ex.Message);
                return dt;
            }
        }
        /// <summary>
        /// 单例
        /// </summary>
        private static DataAnalysis unique = new DataAnalysis();
        /// <summary>
        /// 返回日志单例对象
        /// </summary>
        /// <returns></returns>
        public static DataAnalysis GetInstance()
        {
            if (unique == null)
            {
                unique = new DataAnalysis();
            }
            return unique;
        }

        /// <summary>
        /// 向表中添加数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="recordLogsDataDetailsList"></param>
        /// <returns></returns>
        public DataTable AddTableData(DataTable dt,List<RecordLogsDataDetails> recordLogsDataDetailsList,
            List<WarningRecordDetails> WarningDetailsList, string ReadType)
        {
            try
            {
                if (ReadType == "record")
                {
                    dt.Rows.Clear();
                    foreach (RecordLogsDataDetails _rlds in recordLogsDataDetailsList)
                    {
                        if (_rlds != null)
                        {
                            dt.Rows.Add(
                                DateTime.Parse(_rlds.openlocktime),
                                _rlds.lockmacid,
                                _rlds.lockid,
                                _rlds.lockmodifyname,
                                _rlds.openlocktype,
                                _rlds.userid,
                                _rlds.username,
                                _rlds.userjob,
                                _rlds.userlevel,
                                _rlds.opentime,
                                _rlds.openlimitTimes,
                                _rlds.opentotalTime,
                                _rlds.openexception,
                                _rlds.userstatus
                            );
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                if(ReadType == "warning")
                {
                    dt.Rows.Clear();
                    foreach (WarningRecordDetails _rlds in WarningDetailsList)
                    {
                        if (_rlds != null)
                        {
                            dt.Rows.Add(
                                DateTime.Parse(_rlds._warningdatetime),
                                _rlds._warningType,
                                _rlds._warningStatus
                            );
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("向[{0}]表中添加数据完成", ReadType));
                return dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show("添加到数据表异常：" + ex.Message);
                return dt;
            }
        }

        /// <summary>
        /// 搜索内存数据库
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="dt"></param>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="username"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<RecordLogsDataDetails> QueryDataRecordView(
            out string msg,
            DataTable dt,
            string lockname,
            string starttime,string endtime,
            string userjob, string status ,
            string opentype)
        {
            msg = string.Empty;
            string _OR = " or ";
            string _and = " and ";
            List<RecordLogsDataDetails> list = new List<RecordLogsDataDetails>();
            try
            {     
                string sqlstr = string.Empty;
                if(!string.IsNullOrEmpty(lockname))
                {
                    sqlstr += "lockmodifyname like '%" + lockname + "%' ";  
                    if(!string.IsNullOrEmpty(starttime) | !string.IsNullOrEmpty(endtime) |
                    !string.IsNullOrEmpty(userjob) | !string.IsNullOrEmpty(status) | !string.IsNullOrEmpty(opentype))
                    {
                        sqlstr += _and;
                    }
                }
                if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                {
                    sqlstr += "openlocktime > '" + starttime + "' " + _and + " openlocktime < '" + endtime + "' ";
                    if (!string.IsNullOrEmpty(userjob) | !string.IsNullOrEmpty(status) | !string.IsNullOrEmpty(opentype))
                    {
                        sqlstr += _and;
                    }

                }
                if (!string.IsNullOrEmpty(userjob))
                {
                    //sqlstr += "userjob = '" + userjob + "' ";
                    sqlstr += "username = '" + userjob + "' "; //搜索条件将角色改为人名
                    if (!string.IsNullOrEmpty(status) | !string.IsNullOrEmpty(opentype))
                    {
                        sqlstr += _and;
                    }
                }
                if (!string.IsNullOrEmpty(status))
                {
                    sqlstr += "userstatus = '" + status + "' ";
                    if (!string.IsNullOrEmpty(opentype))
                    {
                        sqlstr += _and;
                    }
                }
                if(!string.IsNullOrEmpty(opentype))
                {
                    sqlstr += "openlocktype = '" + opentype + "' ";
                }
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("执行sql简化语句：{0}", sqlstr));
                DataRow[] drs = dt.Select(sqlstr);
                foreach (DataRow dw in drs)
                {
                    RecordLogsDataDetails ob = new RecordLogsDataDetails
                    {
                        openlocktime = DateTime.Parse(dw[0].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        lockmacid = dw[1].ToString(),
                        lockid = dw[2].ToString(),
                        lockmodifyname = dw[3].ToString(),
                        openlocktype = dw[4].ToString(),
                        userid = dw[5].ToString(),
                        username = dw[6].ToString(),
                        userjob = dw[7].ToString(),
                        userlevel = Int32.Parse(dw[8].ToString() == ""?"0": dw[8].ToString()),
                        opentime = dw[9].ToString(),
                        openlimitTimes = Int32.Parse(dw[10].ToString()==""?"0": dw[10].ToString()),
                        opentotalTime = dw[11].ToString(),
                        openexception = dw[12].ToString(),
                        userstatus = dw[13].ToString()
                    };
                    list.Add(ob);
                }
                msg = "Success";
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("筛选数据完成"));
                return list;
            }
            catch(Exception ex)
            {
                msg = "执行查询数据表异常：" + ex.Message;
                //MessageBox.Show("执行查询数据表异常：" + ex.Message);
                return list;
            }
        }

        public List<WarningRecordDetails> QueryWarningDataView(out string msg, DataTable dt, string starttime, string endtime)
        {
            msg = string.Empty;
            string _and = " and ";
            List<WarningRecordDetails> list = new List<WarningRecordDetails>();
            try
            {
                string sqlstr = string.Empty;                
                if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                {
                    sqlstr += "warningtime > '" + starttime + "' " + _and + " warningtime < '" + endtime + "' ";    
                }                
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("执行sql简化语句：{0}", sqlstr));
                DataRow[] drs = dt.Select(sqlstr);
                foreach (DataRow dw in drs)
                {
                    WarningRecordDetails ob = new WarningRecordDetails
                    {
                        _warningdatetime = DateTime.Parse(dw[0].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        _warningType = dw[1].ToString(),
                        _warningStatus = dw[2].ToString()
                    };
                    list.Add(ob);
                }
                msg = "Success";
                RecordLog.GetInstance().WriteLog(Level.Info, string.Format("筛选数据完成"));
                return list;
            }
            catch (Exception ex)
            {
                msg = "执行查询数据表异常：" + ex.Message;
                return list;
            }
        }

        /// <summary>
        /// 统计分析
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="dt"></param>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="username"></param>
        public RecordGolable StatisticalAnalysis(out string msg, DataTable dt, string lockname, string starttime, string endtime, string username)
        {
            RecordGolable recordGolable = new RecordGolable();
            msg = string.Empty;
            string _and = " and ";
            int _yeat = DateTime.Parse(endtime).Year;
            int _startMonth = DateTime.Parse(starttime).Month;
            int _endMonth = DateTime.Parse(endtime).Month;

            try
            {
                //时间节点
                for (int m = 1; m <= (_endMonth - _startMonth) + 1; m++) 
                {
                    Monthlydisplay monthlydisplay = new Monthlydisplay();
                    StatisticalAnalysis statisticalAnalysis = new StatisticalAnalysis();
                    //门锁名称
                    string sqlstr = string.Empty;
                    if (string.IsNullOrEmpty(lockname))
                    {
                        MessageBox.Show("未选择门锁", "提示");
                        msg = "未选择门锁";
                        return null;
                    }
                    if (string.IsNullOrEmpty(username))
                    {
                        MessageBox.Show( "用户不能为空", "提示");
                        msg = "用户不能为空";
                        return null;
                    }
                    if (string.IsNullOrEmpty(starttime) || string.IsNullOrEmpty(endtime))
                    {
                        MessageBox.Show("时间段选择不正确", "提示");
                        msg = "时间段选择不正确";
                        return null;
                    }
                    if (!string.IsNullOrEmpty(lockname))
                        sqlstr += "lockmodifyname like '%" + lockname + "%' ";
                    sqlstr += _and;
                    if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                    {
                        sqlstr += "openlocktime > '" + DateTime.Parse(_yeat + "-" + (_startMonth + m - 1).ToString()) + "' " + _and +
                             " openlocktime < '" + DateTime.Parse(_yeat + "-" + (_startMonth + m).ToString()) + "' ";
                        if (!string.IsNullOrEmpty(username))
                        {
                            sqlstr += _and;
                        }

                    }
                    if (!string.IsNullOrEmpty(username))
                        sqlstr += "username = '" + username + "' ";
                    

                    RecordLog.GetInstance().WriteLog(Level.Info, string.Format("执行sql简化语句：{0}", sqlstr));
                    DataRow[] drs = dt.Select(sqlstr);

                    if(drs.Count() != 0)
                    {
                        //此人此月开门总次数
                        int totaltimes = drs.Count();
                        //此人此月开门异常总次数
                        int exception = 0;
                        string userjob = string.Empty;
                        if (drs[0][6].ToString() == username)
                        {
                            userjob = drs[0][7].ToString();
                        }
                        foreach (DataRow dw in drs)
                        {
                            if ( "异常"== dw[13].ToString())
                            {
                                exception++;
                            }
                        }
                        statisticalAnalysis = new StatisticalAnalysis
                        {
                            UserName = username,
                            UserJob = userjob,
                            ExcetptionTotalTimes = exception,
                            SuccessTotalTimes = totaltimes - exception,
                            TotalTimes = totaltimes
                        };
                    }
                    monthlydisplay.dicmonth.Add("Month", _startMonth + m - 1);
                    monthlydisplay.dicstatus.Add("Data", statisticalAnalysis);
                    recordGolable._monthlydisplaysList.Add(monthlydisplay);
                }
                msg = "Success";
                return recordGolable;
            }
            catch(Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Ecxeption：" + ex.Message);
                return null; 
            }
        }

        /// <summary>
        /// 全盘分析
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="dt"></param>
        /// <param name="lockname"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public RecordGolable totalAnalysis(out string msg, DataTable dt, string lockname, string starttime, string endtime)
        {
            RecordGolable recordGolable = new RecordGolable();
            List<userinfoAnalysis> userinfolist = new List<userinfoAnalysis>();
            msg = string.Empty;
            string _and = " and ";
            int _yeat = DateTime.Parse(endtime).Year;
            int _startMonth = DateTime.Parse(starttime).Month;
            int _endMonth = DateTime.Parse(endtime).Month;

            try
            {
                //所有用户正常次数总和
                int normal = 0;
                //所有用户异常次数总和
                int exception = 0;
                //所有用户次数总和
                int total = 0;
                //读取用户信息
                List<userDetails> userDetails = RedvelopRecord.GetInstance().getuserDetails(out msg);
                if(msg != "")
                {
                    return recordGolable;
                }
                foreach(userDetails userinfo in userDetails)
                {
                    List<analysisdata> analysisdataslist = new List<analysisdata>();
                    if (userinfo._useName == "")
                    {
                        continue;
                    }
                    string username = userinfo._useName;

                    //该用户所有正常次数
                    int allnormaltimes = 0;
                    //该用户所有异常次数
                    int allexceptiontimes = 0;
                    //该用户所有次数
                    int alltotaltimes = 0;
                    //该用户异常次数情况占比（与所有总异常次数比）
                    string abnormaloccupancyrate = string.Empty;

                    //时间节点
                    for (int m = 1; m <= (_endMonth - _startMonth) + 1; m++)
                    {
                        AnalysisData analysisData = new AnalysisData();
                        //门锁名称
                        string sqlstr = string.Empty;
                        if (string.IsNullOrEmpty(lockname))
                        {
                            MessageBox.Show("未选择门锁", "提示");
                            msg = "未选择门锁";
                            return null;
                        }
                        if (string.IsNullOrEmpty(starttime) || string.IsNullOrEmpty(endtime))
                        {
                            MessageBox.Show("时间段选择不正确", "提示");
                            msg = "时间段选择不正确";
                            return null;
                        }
                        if (!string.IsNullOrEmpty(lockname))
                            sqlstr += "lockmodifyname like '%" + lockname + "%' ";
                        sqlstr += _and;
                        if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                        {
                            sqlstr += "openlocktime > '" + DateTime.Parse(_yeat + "-" + (_startMonth + m - 1).ToString()) + "' " + _and +
                                 " openlocktime < '" + DateTime.Parse(_yeat + "-" + (_startMonth + m).ToString()) + "' ";

                            if (!string.IsNullOrEmpty(username))
                            {
                                sqlstr += _and;
                            }

                        }
                        if (!string.IsNullOrEmpty(username))
                        {
                            sqlstr += "username = '" + username + "' ";
                        }


                        RecordLog.GetInstance().WriteLog(Level.Info, string.Format("执行sql简化语句：{0}", sqlstr));
                        DataRow[] drs = dt.Select(sqlstr);

                        analysisdata analysisdata = new analysisdata();
                        if (drs.Count() != 0)
                        {
                            //开门总次数
                            int totaltimes = drs.Count();
                            //异常总次数
                            int exceptiontimes = 0;
                            foreach (DataRow dw in drs)
                            {
                                if ("异常" == dw[13].ToString())
                                {
                                    exceptiontimes++;
                                }
                            }
                            //正常总次数
                            int normaltimes = totaltimes - exceptiontimes;
                            //异常率
                            double ayt = (double)0;
                            if ((double)exception == 0)
                            {
                                ayt = (double)0;
                            }
                            else
                            {
                                ayt = (double)exceptiontimes / (double)totaltimes;
                            }
                            decimal outtemp = Math.Round(Convert.ToDecimal(ayt), 2, MidpointRounding.AwayFromZero);
                            string abnormalityrate = ((int)(outtemp*100)).ToString() + "%";

                            analysisdata = new analysisdata {
                                normaltimes = normaltimes,
                                exceptiontimes = exceptiontimes,
                                totaltimes = totaltimes,
                                abnormalityrate = abnormalityrate,
                            };

                            allnormaltimes += normaltimes;
                            allexceptiontimes += exceptiontimes;
                            alltotaltimes += totaltimes;
                        }
                        analysisdata.month = _startMonth + m - 1;
                        analysisdataslist.Add(analysisdata);
                    }
                    normal += allnormaltimes;
                    exception += allexceptiontimes;
                    total += alltotaltimes;

                    userinfolist.Add(new userinfoAnalysis
                    {
                        name = username,
                        analysisdata = analysisdataslist,
                        allnormaltimes = allnormaltimes,
                        allexceptiontimes = allexceptiontimes,
                        alltotaltimes = alltotaltimes
                    });

                }
                recordGolable._analysisData.normal = normal;
                recordGolable._analysisData.exception = exception;
                recordGolable._analysisData.total = total;

                string abnormaloccupancyrateTmp = string.Empty;
                foreach (userDetails userinfo in userDetails)
                {
                   foreach(var user in userinfolist)
                    {
                        if(user.name == userinfo._useName)
                        {
                            double ayt = (double)0;
                            if ((double)exception == 0)
                            {
                                ayt = (double)0;
                            }
                            else
                            {
                                ayt = (double)user.allexceptiontimes / (double)exception;
                            }
                            decimal outtemp = Math.Round(Convert.ToDecimal(ayt), 2, MidpointRounding.AwayFromZero);
                            user.abnormaloccupancyrate = ((int)(outtemp * 100)).ToString() + "%";
                            recordGolable._analysisData.userinfo.Add(user);
                        }
                    }
                }
                msg = "Success";
                return recordGolable;
            }
            catch (Exception ex)
            {
                RecordLog.GetInstance().WriteLog(Level.Error, "Ecxeption：" + ex.Message);
                return null;
            }
        }
    }
}
