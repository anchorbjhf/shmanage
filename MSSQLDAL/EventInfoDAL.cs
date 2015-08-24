using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public class EventInfoDAL
    {
        public static readonly SqlConnectionStringBuilder DispatchBuilder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);//取调度库的链接字符串
        public static readonly string DispatchDatabase = DispatchBuilder.InitialCatalog;
        #region
        /// <summary>
        /// 事件信息列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="mainSuit"></param>
        /// <param name="telephoneNumber"></param>
        /// <param name="localAddress"></param>
        /// <param name="patientName"></param>
        /// <param name="sendAddress"></param>
        /// <param name="dispatcher"></param>
        /// <param name="driver"></param>
        /// <param name="doctor"></param>
        /// <param name="nurse"></param>
        /// <param name="eventType"></param>
        /// <param name="illnessJudgment"></param>
        /// <param name="eventCode"></param>
        /// <param name="station"></param>
        /// <param name="ambulanceCode"></param>
        /// <param name="eventSource"></param>
        /// <returns></returns>
        public object GetEventInfoList(int pageSize, int pageIndex, DateTime start, DateTime end, string mainSuit, string telephoneNumber, string localAddress, string patientName, string sendAddress,
                         string dispatcher, string driver, string doctor, string nurse,string stretcher, string eventType, string illnessJudgment, string eventCode, string station, string ambulanceCode,
                         string eventSource, E_StatisticsPermisson em, string selfWorkCode, string selfCenterID, string selfStationID)
        {
            List<EventInfo> list = new List<EventInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select distinct identity(int,1,1) as 行号,tae.事件编码,首次呼救电话,事件名称=现场地址+患者姓名,受理次数,首次调度员编码
                     ,首次受理时刻,事件类型=tzaet.名称,事件来源=tzaeo.名称,派车次数=(select count(*) from dbo.TTask tt where tt.事件编码=tae.事件编码)
                     ,正常完成=(select count(*) from dbo.TTask tt where tt.事件编码=tae.事件编码 and tt.是否正常结束=1) 
                     into #tempa 
                    from TAlarmEvent tae                     
                    left join TZAlarmEventType tzaet on tae.事件类型编码 = tzaet.编码 
                    left join TZAlarmEventOrigin tzaeo on tae.事件来源编码 = tzaeo.编码 
                    left join TTask tt on tae.事件编码 = tt.事件编码
					left join TTaskPersonLink ttpl on tt.任务编码 = ttpl.任务编码
                    left join TStation ts on ts.编码 = tt.分站编码
                    where 是否测试=0 ");
            WhereClauseUtility.AddStringEqual("tae.事件编码", eventCode, sb);
            WhereClauseUtility.AddStringLike("tae.主诉", mainSuit, sb);
            WhereClauseUtility.AddStringLike("tae.首次呼救电话", telephoneNumber, sb);
            WhereClauseUtility.AddStringLike("tae.现场地址", localAddress, sb);
            WhereClauseUtility.AddStringEqual("tae.患者姓名", patientName, sb);
            WhereClauseUtility.AddStringLike("tae.送往地点", sendAddress, sb);
            WhereClauseUtility.AddStringEqual("tae.首次调度员编码", dispatcher, sb);
            //WhereClauseUtility.AddStringEqual("ttpl.人员编码", driver, sb);
            //WhereClauseUtility.AddStringEqual("ttpl.人员编码", doctor, sb);
            //WhereClauseUtility.AddStringEqual("ttpl.人员编码", nurse, sb);
            //WhereClauseUtility.AddStringEqual("ttpl.人员编码", stretcher, sb);
            WhereClauseUtility.AddStringEqual("tae.事件类型编码", eventType, sb);
            WhereClauseUtility.AddStringEqual("tae.病情编码", illnessJudgment, sb);
            WhereClauseUtility.AddStringEqual("tt.车辆编码", ambulanceCode, sb);
            WhereClauseUtility.AddDateTimeGreaterThan("tae.首次受理时刻", start, sb);
            WhereClauseUtility.AddDateTimeLessThan("tae.首次受理时刻", end, sb);
            // WhereClauseUtility.AddStringEqual("ts.名称", station, sb);
            WhereClauseUtility.AddIntEqual("tae.事故类型编码", -1, sb);
            WhereClauseUtility.AddStringEqual("tae.事件来源编码", eventSource, sb);

            if (em == E_StatisticsPermisson.None)
                return null;

            else if (em == E_StatisticsPermisson.ALL)
            {
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", driver, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", doctor, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", nurse, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", stretcher, sb);
                WhereClauseUtility.AddStringEqual("ts.名称", station, sb);
                sb.Append(@" 
                    order by 首次受理时刻 desc 

                    select * into #temp from #tempa where 1=1 ");

                sb.Append(@"
                    select top " + pageSize + " A.*  from #temp A where 行号>" + (pageIndex - 1) * pageSize + " order by 行号 ");
                sb.Append(@"
                    SELECT count(*) FROM #temp t
                    drop table #tempa,#temp ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    EventInfo info = new EventInfo();
                    info.callPhone = dr["首次呼救电话"].ToString();
                    info.acceptTimes = Convert.ToInt32(dr["受理次数"]);
                    info.eventName = dr["事件名称"].ToString();
                    info.firstAcceptTime = dr["首次受理时刻"].ToString();
                    info.firstDispatcher = dr["首次调度员编码"].ToString();
                    info.sendCarTimes = Convert.ToInt32(dr["派车次数"]);
                    info.finishedTimes = Convert.ToInt32(dr["正常完成"]);
                    list.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;        
       
            }
            else if (em == E_StatisticsPermisson.CENTER)
            {
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", driver, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", doctor, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", nurse, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", stretcher, sb);
                WhereClauseUtility.AddStringEqual("tae.中心编码", selfCenterID, sb);
                WhereClauseUtility.AddStringEqual("tt.分站编码", station, sb);               
                
                sb.Append(@" 
                    order by 首次受理时刻 desc 

                    select * into #temp from #tempa where 1=1 ");
 
                sb.Append(@"
                    select top " + pageSize + " A.*  from #temp A where 行号>" + (pageIndex - 1) * pageSize + " order by 行号 ");
                sb.Append(@"
                    SELECT count(*) FROM #temp t
                    drop table #tempa,#temp ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    EventInfo info = new EventInfo();
                    info.callPhone = dr["首次呼救电话"].ToString();
                    info.acceptTimes = Convert.ToInt32(dr["受理次数"]);
                    info.eventName = dr["事件名称"].ToString();
                    info.firstAcceptTime = dr["首次受理时刻"].ToString();
                    info.firstDispatcher = dr["首次调度员编码"].ToString();
                    info.sendCarTimes = Convert.ToInt32(dr["派车次数"]);
                    info.finishedTimes = Convert.ToInt32(dr["正常完成"]);
                    list.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;
      
            }
            else if (em == E_StatisticsPermisson.STATION)
            {
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", driver, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", doctor, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", nurse, sb);
                WhereClauseUtility.AddStringEqual("ttpl.人员编码", stretcher, sb);
                WhereClauseUtility.AddStringEqual("tt.分站编码", selfStationID, sb);               
                
               sb.Append(@" 
                    order by 首次受理时刻 desc 

                    select * into #temp from #tempa where 1=1 ");

               sb.Append(@"
                    select top " + pageSize + " A.*  from #temp A where 行号>" + (pageIndex - 1) * pageSize + " order by 行号 ");
               sb.Append(@"
                    SELECT count(*) FROM #temp t
                    drop table #tempa,#temp ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    EventInfo info = new EventInfo();
                    info.callPhone = dr["首次呼救电话"].ToString();
                    info.acceptTimes = Convert.ToInt32(dr["受理次数"]);
                    info.eventName = dr["事件名称"].ToString();
                    info.firstAcceptTime = dr["首次受理时刻"].ToString();
                    info.firstDispatcher = dr["首次调度员编码"].ToString();
                    info.sendCarTimes = Convert.ToInt32(dr["派车次数"]);
                    info.finishedTimes = Convert.ToInt32(dr["正常完成"]);
                    list.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;

            }
            else if (em == E_StatisticsPermisson.SELF)
            {
                sb.Append("and ttpl.人员编码 = '" + selfWorkCode + @"' ");
                   
                sb.Append(@" 
                    order by 首次受理时刻 desc 

                    select * into #temp from #tempa where 1=1 ");

                sb.Append(@"
                    select top " + pageSize + " A.*  from #temp A where 行号>" + (pageIndex - 1) * pageSize + " order by 行号 ");
                sb.Append(@"
                    SELECT count(*) FROM #temp t
                    drop table #tempa,#temp ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    EventInfo info = new EventInfo();
                    info.callPhone = dr["首次呼救电话"].ToString();
                    info.acceptTimes = Convert.ToInt32(dr["受理次数"]);
                    info.eventName = dr["事件名称"].ToString();
                    info.firstAcceptTime = dr["首次受理时刻"].ToString();
                    info.firstDispatcher = dr["首次调度员编码"].ToString();
                    info.sendCarTimes = Convert.ToInt32(dr["派车次数"]);
                    info.finishedTimes = Convert.ToInt32(dr["正常完成"]);
                    list.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;

            }
            else
            {
                return null;
            }        

        }
        #endregion

        #region 获取combobox的值
        //获取事件类型
        public object GetEventTypeList()
        {
            List<P_Role> list = new List<P_Role>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,名称 from TZAlarmEventType where 是否有效 = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Role info = new P_Role();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取事件来源
        public object GetEventSourceList()
        {
            List<P_Role> list = new List<P_Role>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,名称 from TZAlarmEventOrigin where 是否有效 = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Role info = new P_Role();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }

        //获取病情判断
        public object GetIllnessStateList()
        {
            List<P_Role> list = new List<P_Role>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,名称 from TZIllState where 是否有效 = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Role info = new P_Role();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }

        //获取车辆分站
        public object GetStationList()
        {
            List<P_Role> list = new List<P_Role>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,名称 from TStation where 是否有效 = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Role info = new P_Role();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }

        //获取车辆编码
        public object GetAmbulanceCodeList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 车辆编码,实际标识 from TAmbulance where 是否有效 = 1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["车辆编码"].ToString();
                    info.Name = dr["实际标识"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取调度员
        public object GetDispatcherList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,姓名 from TPerson where 是否有效 = 1 and 类型编码 = 2  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["编码"].ToString();
                    info.Name = dr["姓名"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取司机
        public object GetDriverList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,姓名 from TPerson where 是否有效 = 1 and 类型编码 = 3  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["编码"].ToString();
                    info.Name = dr["姓名"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取医生
        public object GetDoctorList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,姓名 from TPerson where 是否有效 = 1 and 类型编码 = 4  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["编码"].ToString();
                    info.Name = dr["姓名"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取护士
        public object GetNurseList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,姓名 from TPerson where 是否有效 = 1 and 类型编码 = 5  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["编码"].ToString();
                    info.Name = dr["姓名"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取担架员
        public object GetStretcherList()
        {
            List<CheckModelExt> list = new List<CheckModelExt>();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 编码,姓名 from TPerson where 是否有效 = 1 and 类型编码 = 6  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CheckModelExt info = new CheckModelExt();
                    info.ID = dr["编码"].ToString();
                    info.Name = dr["姓名"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion
    }
}
