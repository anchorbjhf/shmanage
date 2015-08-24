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

    /// <summary>
    /// 流水数据访问
    /// </summary>
    public class LSDAL
    {

        public static readonly SqlConnectionStringBuilder DispatchBuilder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);//取调度库的链接字符串
        public static readonly string DispatchDatabase = DispatchBuilder.InitialCatalog;

        #region 重大突发性灾害事故流水表
        /// <summary>
        /// 重大突发性灾害事故流水表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_ZDTFXZHSG(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"            
                select distinct  AlarmEventCode = tae.事件编码,FirstAcceptMoment = 首次受理时刻,AccidentName = tae.现场地址+'('+isnull(tzaccit.名称,'')+')'
                ,CriticalNumber = sum(重伤人数),CommonNumber = sum(轻伤人数),DeathNumber = sum(死亡人数),HospitalNumber = sum(送院人数)
                ,RescueCarNumber =count(distinct(tt.任务编码))
                ,RescuePersonNumber=(select count(人员编码) from TTaskPersonLink where substring(任务编码,1,18)=tae.事件编码)              
                from  dbo.TAlarmEvent tae 
                left join TTask tt on tt.事件编码=tae.事件编码
                left join TAccident tacc on tacc.任务编码=tt.任务编码
                left join TZAccidentType tzaccit on tzaccit.编码=tae.事故类型编码               
                where 事件类型编码=3 and 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                group by tae.事件编码,首次受理时刻,tae.现场地址,tzaccit.名称
                order by 首次受理时刻 desc");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度日志流水表
        /// <summary>
        /// 调度日志流水表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_DDRZ(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                     if datediff(day,@BeginTime,@EndTime)=0
                     select 
                     DeskCode=td.显示名称,Date=日期,Content=早班内容,Person=早班人员
                     from TReliefLog trl
                     left join TDesk td on td.台号=trl.台号
                     where 日期=convert(char(10),@BeginTime,112)
                     else
                     select 
                     DeskCode=td.显示名称,Date=日期,Content=早班内容,Person=早班人员
                     from TReliefLog trl
                     left join TDesk td on td.台号=trl.台号
                     where 日期>=convert(char(10),@BeginTime,112) and 日期<=convert(char(10),@EndTime,112)
                        ");

            SqlParameter[] prams = new SqlParameter[] { 
                new SqlParameter("@BeginTime",SqlDbType.DateTime)
               ,new SqlParameter("@EndTime",SqlDbType.DateTime)
            };
            prams[0].Value = beginTime;
            prams[1].Value = endTime;

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), prams);
            return ds.Tables[0];
        }
        #endregion

        #region  发送通知流水表
        /// <summary>
        /// 发送通知流水表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_FSTZ(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select Operator=tp.姓名,SendTime=发送时刻,Content=内容,SendType=tznt.名称  
                      from TNotice tn
                      left join TPerson tp on tp.编码=tn.操作员编码
                      left join TZNoticeType tznt on tznt.编码=tn.类型编码
                      where 发送时刻>='" + beginTime + "' and 发送时刻<='" + endTime + @"' and tp.是否有效=1 ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 日常急救到达率分段（时间段）表
        public DataTable Get_LS_RCJJDDLFDSJD(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                set ansi_warnings off
                set nocount on 
                select 
                Center=tc.名称,
                AlarmEventType=alet.名称,
                TotalTimes=count(*),
                TimesIn10Minute=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=600 then 1 else 0 end)
                ,TimesBetween10And12Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>600 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=720 then 1 else 0 end)
                ,TimesBetween12And15Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>720 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=900 then 1 else 0 end)
                ,TimesBetween15And20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>900 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=1200 then 1 else 0 end)
                ,TimesOver20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>1200 then 1 else 0 end)               
                from dbo.TTask tt
                inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                left join dbo.TZAlarmEventType alet on alet.编码=ale.事件类型编码
                left join TStation ts on tt.分站编码=ts.编码
                left join TCenter tc on ts.中心编码=tc.编码
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and 事件类型编码 in (0,1,2)
                and ts.中心编码<>1
                group by tc.名称,alet.名称
                order by tc.名称,alet.名称
                set nocount off
                set ansi_warnings on  ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_LS_RCJJDDLFDSJDSum(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                select 
                SumAlarmEventType=alet.名称,
                SumTotalTimes=count(*),
                SumTimesIn10Minute=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=600 then 1 else 0 end)
                ,SumTimesBetween10And12Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>600 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=720 then 1 else 0 end)
                ,SumTimesBetween12And15Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>720 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=900 then 1 else 0 end)
                ,SumTimesBetween15And20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>900 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=1200 then 1 else 0 end)
                ,SumTimesOver20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>1200 then 1 else 0 end)
                from dbo.TTask tt
                inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                left join dbo.TZAlarmEventType alet on alet.编码=ale.事件类型编码
                left join TStation ts on tt.分站编码=ts.编码
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                and 事件类型编码 in (0,1,2)
                and ts.中心编码<>1
                group by alet.名称
                order by alet.名称");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 手机定位成功率统计表
        /// <summary>
        /// 手机定位成功率统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_SJDWCGL(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                select ServiceProvider=运营商类型,TotalAlarm=count(电话号码)
                into #temp1
                from dbo.TTelLocation
                where 请求时刻>='" + beginTime + "' and 请求时刻<='" + endTime + @"' 
                and  运营商类型!='未知'
                group by 运营商类型

                select ServiceProvider=运营商类型,SucceedLocation=count(电话号码) 
                into #temp2
                from dbo.TTelLocation
                where 请求时刻>='" + beginTime + "' and 请求时刻<='" + endTime + @"' 
                and  运营商类型!='未知' and 是否定位=1
                group by 运营商类型
                select t1.ServiceProvider,TotalAlarm,SucceedLocation
                from #temp1 t1 left join #temp2 t2 on t1.ServiceProvider=t2.ServiceProvider
                drop table #temp1
                drop table #temp2");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 危重病人病情预报登记表
        /// <summary>
        /// 危重病人病情预报登记表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_WZBRBQYBDJ(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                    Date=convert(varchar(10),时间,120),
                    Time=convert(varchar(5),时间,114),
                    Name=患者姓名
                    ,Assessment=拟诊
                    ,ToHospital=送往医院
                    ,ReportName=预报人
                    ,RegisterName=登录人
                    ,Remark=tgd.备注
                    ,CarNumber=tam.实际标识

                    from TGreenDate tgd
                    left join TTask tt on tt.任务编码=tgd.任务编码
                    left join TAmbulance tam on tam.车辆编码=tt.车辆编码
                    where 时间>='" + beginTime + "' and 时间<='" + endTime + @"'
                    order by 时间");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 医院搁置担架统计表
        /// <summary>
        /// 医院搁置担架统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_YYGZDJ(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select Hosptial=送往医院,Times=count(*)
                    from TLitterLog tll
                    left join dbo.TZLitterReasonType tzlrt on tzlrt.编码=tll.原因编码
                    where 时间>='" + beginTime + "' and 时间<='" + endTime + @"'
                    group by 送往医院
                    order by 送往医院");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 医院搁置救护车担架影响出车统计表
        /// <summary>
        /// 医院搁置救护车担架影响出车统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataTable Get_LS_YYGZJHCDJYXCC(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                    Date=convert(varchar(10),时间,120)
                    ,Time=convert(varchar(5),时间,114)
                    ,ToHosptial=送往医院
                    ,Reason=tzlrt.名称
                    ,StayTime=搁置时间
                    ,Name=姓名
                    from TLitterLog tll
                    left join dbo.TZLitterReasonType tzlrt on tzlrt.编码=tll.原因编码
                    where 时间>='" + beginTime + "' and 时间<='" + endTime + @"'");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度员工作状态流水表
        /// <summary>
        /// 调度员工作状态流水表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="workState"></param>
        /// <param name="time"></param>
        /// <param name="name"></param>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public DataTable Get_LS_DDYGZZT(DateTime beginTime, DateTime endTime, string workState, string time, string name, string personCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                 set ansi_warnings off
                  select 
                   人员姓名
                  ,开始时刻
                  ,开始类型
                  ,结束时刻
                  ,结束类型
                  ,Time=datediff(second,开始时刻,结束时刻)  
                  ,台号
				  ,开始类型编码=case when topnr.开始类型='登录' then 0 when topnr.开始类型='电话受理' then 1 when topnr.开始类型='手工受理' then 2 
				   when topnr.开始类型='就绪' then 3  when topnr.开始类型='暂停' then 4 else 5 end 
				   into #temp
				   from dbo.TOperatorNotReadyRecord topnr
				   left join  dbo.TPerson TP ON (topnr.人员编码=TP.编码)
				   where 开始时刻>='" + beginTime + "' and 开始时刻<='" + endTime + @"'
                   and (datediff(second,开始时刻,结束时刻)>'" + time + "'*60 or '" + time + @"'='')  and TP.是否有效=1 

                 select 
                   Name=人员姓名
                  ,StartTime=开始时刻
                  ,StartType=开始类型
                  ,EndTime=结束时刻
                  ,EndType=结束类型
                  ,Time=datediff(second,开始时刻,结束时刻)  
                  ,DeskCode=台号
                  ,StartTypeCode=开始类型编码
                 from  #temp  where 1=1 ");
            WhereClauseUtility.AddStringEqual("人员姓名", name, sb);
            WhereClauseUtility.AddStringLike("tp.工号", personCode, sb);
            WhereClauseUtility.AddInSelectQuery("开始类型编码", workState, sb);

            sb.Append(" order by 人员姓名,开始时刻");
            sb.Append(" drop table #temp ");
            sb.Append(" set ansi_warnings on ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public object Get_LS_DDYGZZT_RYXM()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select tp.编码,tp.姓名
                    from TPerson tp 
                    left join TRole tr on tr.编码 = tp.类型编码 
                    where tp.类型编码 = 2 and tp.是否有效 = 1");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["姓名"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 分站人员上下班流水表
        public DataTable Get_LS_FZRYSXB(DateTime beginTime, DateTime endTime, string center, string station, string role, string carNumber, string workCode, string name,
            E_StatisticsPermisson em, string selfName, string selfWorkCode, string selfCenterID, string selfStationID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                     SET NOCOUNT ON
                     select  Center=tc.名称 
                            ,Sation=ts.名称 
                            ,Name=tp.姓名
                            ,PersonCode=t.人员编码
                            ,WorkNumber=tp.工号
                            ,CarCode=t.车辆编码
                            ,Role=tr.名称
                            ,CarNumber=ta.实际标识
                            ,OnDutyOperateMoment=t.操作时刻
                            ,OnDutyType=上班方式
                            ,OffDutyOperateMoment=t1.操作时刻
                            ,OffDutyType=下班方式
                     from (select 人员编码,
                                  操作时刻,人员类型编码,车辆编码,操作人编码,是否上班操作,操作来源编码,上次操作时刻
                                  ,tzoo.名称 as 上班方式
		                    from TAmbulancePersonSign taps
                            left join TZOperationOrigin tzoo on taps.操作来源编码=tzoo.编码
		                    where 操作时刻 >= '" + beginTime + "' and 操作时刻 <= '" + endTime + @"' and 是否上班操作 = 1
		                    )t
                    left join (select 人员编码,
		                              操作时刻,人员类型编码,车辆编码,操作人编码,是否上班操作,操作来源编码,上次操作时刻
                                  ,tzoo.名称 as 下班方式
		                      from TAmbulancePersonSign taps
                              left join TZOperationOrigin tzoo on taps.操作来源编码=tzoo.编码
		                       where 操作时刻 >=  '" + beginTime + "' and 操作时刻 <= '" + endTime + @"' and 是否上班操作 = 0
		                       )t1 on t.操作时刻 = t1.上次操作时刻 and t.人员编码=t1.人员编码 and t.人员类型编码=t1.人员类型编码
                    left join TPerson tp on t.人员编码=tp.编码
                    left join TAmbulance tam on t.车辆编码=tam.车辆编码
                    left join dbo.TCenter tc on tam.中心编码=tc.编码
                    left join dbo.TStation ts on tam.分站编码 = ts.编码
                    left join TRole tr on t.人员类型编码=tr.编码
                    left join TAmbulance ta on t.车辆编码=ta.车辆编码
                    where 1=1
                    ");
            if (em == E_StatisticsPermisson.None)
                return null;
            else if (em == E_StatisticsPermisson.ALL)
            {

                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                WhereClauseUtility.AddInSelectQuery("tc.编码", center, sb);
                WhereClauseUtility.AddInSelectQuery("ts.编码", station, sb);
                WhereClauseUtility.AddInSelectQuery("tr.编码", role, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("ta.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,t.车辆编码,t.操作时刻");

                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.CENTER)
            {

                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                WhereClauseUtility.AddStringEqual("tc.编码", selfCenterID, sb);
                WhereClauseUtility.AddInSelectQuery("ts.编码", station, sb);
                WhereClauseUtility.AddInSelectQuery("tr.编码", role, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("ta.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,t.车辆编码,t.操作时刻");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.STATION)
            {
                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                WhereClauseUtility.AddStringEqual("ts.编码", selfStationID, sb);
                WhereClauseUtility.AddInSelectQuery("tr.编码", role, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("ta.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,t.车辆编码,t.操作时刻");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.SELF)
            {
                WhereClauseUtility.AddStringLike("tp.姓名", selfName, sb);
                WhereClauseUtility.AddInSelectQuery("tr.编码", role, sb);
                WhereClauseUtility.AddStringEqual("tp.工号", selfWorkCode, sb);
                WhereClauseUtility.AddStringLike("ta.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,t.车辆编码,t.操作时刻");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 下拉框取分中心可多选，连动分站
        //取分中心
        public object GetCenter()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select 编码,名称 
                      from TCenter where 是否有效 =1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        //根据中心编码取分站 （中心，分站联动，统计流水用）
        //public string ListToString(List<int> list)
        //{
        //    string result = "";
        //    foreach (int i in list)
        //    {
        //        result = result + i.ToString() + ",";
        //    }
        //    return result.TrimEnd(',');
        //}
        //根据中心编码取分站 （中心，分站联动，统计流水用）
        public object GetStation(string centerID)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                     select 编码,名称
                     from TStation  ");
            sb.Append("where 是否有效 = 1");
            WhereClauseUtility.AddInSelectQuery("中心编码", centerID, sb);

            //where 中心编码 in (" + centerID + ") and  是否有效 =1 ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;
        }
        #endregion
        
        public DataTable GetCenterOnaspx()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select 编码,名称 
                      from TCenter where 是否有效 =1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        public DataTable GetStationOnaspx(string centerID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                     select 编码,名称
                     from TStation  ");
            sb.Append("where 是否有效 = 1");
            WhereClauseUtility.AddInSelectQuery("中心编码", centerID, sb);
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }


        #region 分中心暂停调用流水表
        public DataTable Get_LS_FZXZTDY(DateTime beginTime, DateTime endTime, string center, string station, string carNumber, string workCode, string name,
            E_StatisticsPermisson em, string selfWorkCode, string selfCenterID, string selfStationID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                
                select 
	                 Center=tc.名称,
	                 Sation=ts.名称,
                     CarNumber=tam.实际标识,
                     RecordTime=tpr.操作时刻1,Operator1=tp1.姓名,
                     EndTime=tpr.操作时刻2,Operator2=tp2.姓名,
                     PauseTime=datediff(ss,tpr.操作时刻1,tpr.操作时刻2),
                     PauseReason=tzpr.名称,
                     DriverWorkNumber=tdriver.工号,
	                 Driver=tdriver.姓名
	                ,Doctor=dbo.CodeToName(tpr.医生编码),Stretcher=dbo.CodeToName(tpr.担架工编码)
                from (select 车辆编码=tp.车辆编码,操作时刻1=tp.操作时刻,操作时刻2=min(tpr.操作时刻),
                             暂停原因编码=tp.暂停原因编码,tp.司机编码,tp.医生编码,tp.担架工编码              
                      from (select 车辆编码,操作时刻,暂停原因编码,司机编码,医生编码,担架工编码   from TPauseRecord  where 是否暂停操作 = 1 ) tp                             
                      left join ( select 车辆编码,操作时刻,暂停原因编码,司机编码,医生编码,担架工编码        
                                  from TPauseRecord  where 是否暂停操作 = 0 ) tpr on tpr.车辆编码=tp.车辆编码 and tpr.操作时刻 > tp.操作时刻
                      group by tp.车辆编码,tp.操作时刻,tp.暂停原因编码,tp.司机编码,tp.医生编码,tp.担架工编码
                   ) tpr 
                left join TAmbulance tam on tpr.车辆编码 = tam.车辆编码
                left join TCenter tc on tc.编码=tam.中心编码
                left join TStation ts on ts.编码=tam.分站编码
                left join TZPauseReason tzpr on tpr.暂停原因编码 = tzpr.编码
                left join TPerson tdriver on tdriver.编码 = tpr.司机编码
                left join TPauseRecord tpau1 on tpau1.车辆编码=tpr.车辆编码 and tpau1.操作时刻=tpr.操作时刻1
                left join TPerson tp1 on tp1.编码=tpau1.操作人员编码
                left join TPauseRecord tpau2 on tpau2.车辆编码=tpr.车辆编码 and tpau2.操作时刻=tpr.操作时刻2
                left join TPerson tp2 on tp2.编码=tpau2.操作人员编码
                where tpr.操作时刻1>='" + beginTime + "' and tpr.操作时刻1<='" + endTime + @"' 
            ");
            if (em == E_StatisticsPermisson.None)
                return null;

            else if (em == E_StatisticsPermisson.ALL)
            {
                WhereClauseUtility.AddStringLike("tdriver.姓名", name, sb);
                WhereClauseUtility.AddInSelectQuery("tc.编码", center, sb);
                WhereClauseUtility.AddInSelectQuery("ts.编码", station, sb);
                WhereClauseUtility.AddStringLike("tdriver.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tam.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,ts.顺序号,tpr.操作时刻1");

                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.CENTER)
            {
                WhereClauseUtility.AddStringLike("tdriver.姓名", name, sb);
                WhereClauseUtility.AddStringEqual("tc.编码", selfCenterID, sb);
                WhereClauseUtility.AddInSelectQuery("ts.编码", station, sb);
                WhereClauseUtility.AddStringLike("tdriver.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tam.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,ts.顺序号,tpr.操作时刻1");

                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.STATION)
            {
                WhereClauseUtility.AddStringLike("tdriver.姓名", name, sb);
                WhereClauseUtility.AddStringEqual("ts.编码", selfStationID, sb);
                WhereClauseUtility.AddStringLike("tdriver.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tam.实际标识", carNumber, sb);
                sb.Append("order by tc.顺序号,ts.顺序号,tpr.操作时刻1");

                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region 病历填写时间流水表
        public DataTable Get_LS_BLTXSJ(DateTime beginTime, DateTime endTime, string workCode, string centerID,
             string stationID, E_StatisticsPermisson em, string selfWorkCode, string selfCenterID, string selfStationID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                   select CallOrder=pr.CallOrder,
                   Name=pr.Name,
                   LocalAddress=pr.LocalAddress,
                   Driver=pr.Driver,
                   StretcherBearersI=pr.StretcherBearersI,                 
                   DoctorAndNurse=pr.DoctorAndNurse,
                   AgentName=pr.AgentName,
                   AgentWorkID=pr.AgentWorkID,                 
                   BeginFillPatientTime=pr.BeginFillPatientTime,
                   MedicalRecordGenerationTime=pr.MedicalRecordGenerationTime,
                   SubmitTime=pr.SubmitTime,
                   MedicalState=case when pr.MedicalStateCode = 0  then '暂存' else '提交' end
	    
                   from M_PatientRecord pr
                   left join P_Department d on d.DispatchSationID = pr.OutStationCode
                   where MedicalRecordGenerationTime>='" + beginTime + "' and  MedicalRecordGenerationTime<'" + endTime + "'");

            if (em == E_StatisticsPermisson.None)
                return null;

            else if (em == E_StatisticsPermisson.ALL)
            {
                WhereClauseUtility.AddStringEqual("pr.AgentWorkID", workCode, sb);
                WhereClauseUtility.AddInSelectQuery("d.DispatchSubCenterID", centerID, sb);
                WhereClauseUtility.AddInSelectQuery("d.DispatchSationID", stationID, sb);
                sb.Append("order by TaskCode");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.CENTER)
            {
                WhereClauseUtility.AddStringEqual("d.DispatchSubCenterID", selfCenterID, sb);
                WhereClauseUtility.AddInSelectQuery("d.DispatchSationID", stationID, sb);
                WhereClauseUtility.AddStringEqual("pr.AgentWorkID", workCode, sb);
                sb.Append("order by TaskCode");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.STATION)
            {
                WhereClauseUtility.AddStringEqual("d.DispatchSationID", selfStationID, sb);
                WhereClauseUtility.AddStringEqual("pr.AgentWorkID", workCode, sb);
                sb.Append("order by TaskCode");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else if (em == E_StatisticsPermisson.SELF)
            {
                sb.Append("and AgentWorkID = '" + selfWorkCode + @"'
                   order by TaskCode");
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region  获取要车性质（事件类型）  （呼救事件流水表用）
        //获取要车性质（事件类型）
        public object GetAlarmEventType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select 编码,名称 from TZAlarmEventType where 是否有效=1
                        ");
            DataSet ds = (SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null));
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion

        #region  获取车辆任务状态 （呼救事件流水表用）
        public object GetCarState()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 名称 into #temp from TZAmbulanceState where 是否有效=1
                    insert into #temp values('完成')
                    insert into #temp values('未出车')
                    select 名称 from #temp
                    drop table #temp
                        ");
            DataSet ds = (SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null));
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    // info.ID = Convert.ToString(dr["编码"]);
                    info.Name = dr["名称"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }



        #endregion

        #region 呼救事件流水表

        public DataTable Get_LS_HJSJ(DateTime beginTime, DateTime endTime, string alarmTypeCoding, string ambulanceState, string callNumber,
            string name, string ambulance, string driver, string doctor, string sceneAddress, string sendAddress, string illReason)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"            
                        select tae.事件编码,呼救电话=tae.呼救电话,往救地点=tae.现场地址,受理时间=tae.首次受理时刻
                        ,要车性质=tzaet.名称,呼救主诉=tae.主诉,车号=tam.实际标识
                        ,司机=isnull(dbo.GetStr(tt.任务编码,3),''),医生=isnull(dbo.GetStr(tt.任务编码,4),'')
                        ,tt.任务编码,任务状态=case when tt.是否执行中=0 then '完成' 
                        when tt.是否执行中=1 then tzas.名称 else '未出车' end,
                        任务状态编码=case when tt.是否执行中=0 then 10 when tt.是否执行中=1 then tzas.编码 else 11 end
                        into #temp1
                        from dbo.TAlarmEvent tae 
                        left join dbo.TTask tt on tt.事件编码=tae.事件编码
                        left join dbo.TZAlarmEventType tzaet on tzaet.编码=tae.事件类型编码
                        left join dbo.TAmbulance tam on tam.车辆编码=tt.车辆编码
                        left join dbo.TZAmbulanceState tzas on tzas.编码=tam.工作状态编码
                        where tae.首次受理时刻>='" + beginTime + "' and tae.首次受理时刻<='" + endTime + @"' and tae.是否测试=0

                        and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                        and tae.事件编码>'" + eventCodeB + "' and tae.事件编码<='" + eventCodeE + @"' ");
            WhereClauseUtility.AddInSelectQuery("tae.事件类型编码 ", alarmTypeCoding, sb);
            WhereClauseUtility.AddStringLike("tae.现场地址", sceneAddress, sb);
            WhereClauseUtility.AddStringLike("tae.送往地点", sendAddress, sb);
            WhereClauseUtility.AddStringLike("tae.呼救电话", callNumber, sb);
            WhereClauseUtility.AddStringEqual("tae.患者姓名", name, sb);
            WhereClauseUtility.AddStringLike("tae.主诉", illReason, sb);
            WhereClauseUtility.AddStringLike("tam.实际标识", ambulance, sb);

            sb.Append(@"select AlarmEventCode=事件编码,PhoneNumber=呼救电话,Address=往救地点,AcceptTime=受理时间,AlarmType=要车性质,
                               MainSuit=呼救主诉,CarNumber=车号,Driver=司机,Doctor=医生,TaskCode=任务编码,TaskState=任务状态,TaskStateCode=任务状态编码
                        from  #temp1 t1 
                        where 1=1");
            WhereClauseUtility.AddStringLike("司机", driver, sb);
            WhereClauseUtility.AddStringLike("医生", doctor, sb);
            WhereClauseUtility.AddInSelectQuery("任务状态编码", ambulanceState, sb);
            //WhereClauseUtility.AddStringEqual("任务状态", ambulanceState, sb);
            sb.Append(" order by 受理时间 desc ");
            sb.Append("  drop table #temp1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        #endregion

        #region 来电记录流水表
        public DataTable Get_LS_LDJL(DateTime beginTime, DateTime endTime, string callNumber, string actionResult)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"set ansi_warnings off
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                                            
                        select 来电时刻,主叫号码,调度员工号,姓名=tp.姓名,结束时刻,录音号
                        ,处理结果=case when 震铃时刻 is not null and 结果类型=1 then '未接听' when 结果类型=0 then '已接听' else tztrt.名称 end
                        ,处理结果编码=case when 震铃时刻 is not null and 结果类型=1 then 0 when 结果类型=0 then 1 else 2 end
                        into #temp
                        from dbo.TTelRecord ttr 
                        left join dbo.TZTelRecordType tztrt on tztrt.编码=ttr.结果类型 
                        left join dbo.TPerson tp on tp.工号=ttr.调度员工号 
                        left join dbo.TDesk td on td.台号=ttr.台号 
                        where 来电时刻 >= @BeginTime and 来电时刻<=@EndTime and 是否呼出=0 and tp.是否有效=1 ");
            WhereClauseUtility.AddStringLike("主叫号码", callNumber, sb);

            sb.Append(@" select AlarmTime=来电时刻,PhoneNumber=主叫号码,WorkNumber=调度员工号,
                               Name=姓名,EndTime=结束时刻,Result=处理结果,RecordNumber=录音号,ResultCode=处理结果编码
                         from #temp 
                         where 1=1 ");
            WhereClauseUtility.AddInSelectQuery("处理结果编码", actionResult, sb);
            sb.Append(" order by 来电时刻 desc ");
            sb.Append(" drop table #temp ");
            sb.Append(" set ansi_warnings on ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 取图表数据  日常急救到达率分段(月)
        public DataTable Get_LS_RCJJDDLFD_TuBiao(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"                      
                        set ansi_warnings off
                        select tt.任务编码, ale.首次受理时刻,tt.到达现场时刻
                        into #temp1
                        from dbo.TTask tt
                        inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                        join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                        left join TStation ts on tt.分站编码=ts.编码
                        where datepart(yy,首次受理时刻)=datepart(yy,dateadd(m,-1,convert(datetime,convert(varchar(4),'" + year + "')+'-'+convert(varchar(2),'" + month + @"')+'-01')))
                        and datepart(mm,首次受理时刻)=datepart(mm,dateadd(m,-1,convert(datetime,convert(varchar(4),'" + year + "')+'-'+convert(varchar(2),'" + month + @"')+'-01')))
                        and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                        and 事件类型编码 in (0,1,2) and ts.中心编码<>1
                        select  tt.任务编码, ale.首次受理时刻,tt.到达现场时刻
                        into #temp2
                        from dbo.TTask tt
                        inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                        join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                        left join TStation ts on tt.分站编码=ts.编码
                        where datepart(yy,首次受理时刻)='" + year + @"'
                        and datepart(mm,首次受理时刻)='" + month + @"'
                        and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                        and 事件类型编码 in (0,1,2)  and ts.中心编码<>1

                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='总次数',
                        数量=count(*)
                        ,顺序号=-1
                        into #temp
                        from #temp1
                        group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='总次数',
                        数量=count(*)
                        ,顺序号=0
                        from #temp2
                        group by datepart(mm,首次受理时刻)
                        union
                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='10分钟内',
                        数量=count(*)
                        ,顺序号=1
                        from #temp1
                        where datediff(second,首次受理时刻,到达现场时刻)<=600 group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='10分钟内',
                        数量=count(*)
                        ,顺序号=2
                        from #temp2
                        where datediff(second,首次受理时刻,到达现场时刻)<=600 group by datepart(mm,首次受理时刻)
                        union
                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='10-12分钟',
                        数量=count(*)
                        ,顺序号=3
                        from #temp1
                        where datediff(second,首次受理时刻,到达现场时刻)>600 and datediff(second,首次受理时刻,到达现场时刻)<=720 group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='10-12分钟',
                        数量=count(*)
                        ,顺序号=4
                        from #temp2
                        where datediff(second,首次受理时刻,到达现场时刻)>600 and datediff(second,首次受理时刻,到达现场时刻)<=720 group by datepart(mm,首次受理时刻)
                        union
                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='12-15分钟',
                        数量=count(*)
                        ,顺序号=5
                        from #temp1
                        where datediff(second,首次受理时刻,到达现场时刻)>720 and datediff(second,首次受理时刻,到达现场时刻)<=900 group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='12-15分钟',
                        数量=count(*)
                        ,顺序号=6
                        from #temp2
                        where datediff(second,首次受理时刻,到达现场时刻)>720 and datediff(second,首次受理时刻,到达现场时刻)<=900 group by datepart(mm,首次受理时刻)
                        union
                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='15-20分钟',
                        数量=count(*)
                        ,顺序号=7
                        from #temp1
                        where datediff(second,首次受理时刻,到达现场时刻)>900 and datediff(second,首次受理时刻,到达现场时刻)<=1200 group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='15-20分钟',
                        数量=count(*)
                        ,顺序号=8
                        from #temp2
                        where datediff(second,首次受理时刻,到达现场时刻)>900 and datediff(second,首次受理时刻,到达现场时刻)<=1200 group by datepart(mm,首次受理时刻)
                        union
                        select 
                        序列=datepart(mm,首次受理时刻),
                        类别='20分钟以上',
                        数量=count(*)
                        ,顺序号=9
                        from #temp1
                        where datediff(second,首次受理时刻,到达现场时刻)>1200 group by datepart(mm,首次受理时刻)
                        union 
                        select
                        序列=datepart(mm,首次受理时刻),
                        类别='20分钟以上',
                        数量=count(*)
                        ,顺序号=10
                        from #temp2
                        where datediff(second,首次受理时刻,到达现场时刻)>1200 group by datepart(mm,首次受理时刻)

                        select XuLie=序列,NeiBie=类别,Number=数量,SN=顺序号 from #temp order by 顺序号

                        drop table #temp1,#temp2
                        drop table #temp
            ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];

        }
        #endregion

        #region 日常急救到达率分段（月）流水表
        public DataTable Get_LS_RCJJDDLFD(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        set ansi_warnings off  
                        set nocount on                   
                        select 
                        Center=tc.名称,
                        AlarmEventType=alet.名称,
                        TotalTimes=count(*),
                        TimesIn10Minute=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=600 then 1 else 0 end)
                        ,TimesBetween10And12Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>600 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=720 then 1 else 0 end)
                        ,TimesBetween12And15Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>720 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=900 then 1 else 0 end)
                        ,TimesBetween15And20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>900 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=1200 then 1 else 0 end)
                        ,TimesOver20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>1200 then 1 else 0 end)
                        from dbo.TTask tt
                        inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                        join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                        left join dbo.TZAlarmEventType alet on alet.编码=ale.事件类型编码
                        left join TStation ts on tt.分站编码=ts.编码
                        left join TCenter tc on ts.中心编码=tc.编码
                        where datepart(yy,首次受理时刻)='" + year + "' and datepart(mm,首次受理时刻)='" + month + @"'
                        and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                        and 事件类型编码 in (0,1,2)
                        and ts.中心编码<>1
                        group by tc.名称,alet.名称
                        order by tc.名称,alet.名称
                        set nocount off
                        set ansi_warnings on
              ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        public DataTable Get_LS_RCJJDDLFDSum(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        set nocount on                 
                        select 
                        SumAlarmEventType=alet.名称,
                        SumTotalTimes=count(*),
                        SumTimesIn10Minute=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=600 then 1 else 0 end)
                        ,SumTimesBetween10And12Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>600 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=720 then 1 else 0 end)
                        ,SumTimesBetween12And15Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>720 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=900 then 1 else 0 end)
                        ,SumTimesBetween15And20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>900 and datediff(second,ale.首次受理时刻,tt.到达现场时刻)<=1200 then 1 else 0 end)
                        ,SumTimesOver20Minutes=sum(case when datediff(second,ale.首次受理时刻,tt.到达现场时刻)>1200 then 1 else 0 end)
                        from dbo.TTask tt
                        inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                        join dbo.TAlarmEvent ale on tt.事件编码=ale.事件编码
                        left join dbo.TZAlarmEventType alet on alet.编码=ale.事件类型编码
                        left join TStation ts on tt.分站编码=ts.编码
                        where datepart(yy,首次受理时刻)='" + year + "' and datepart(mm,首次受理时刻)='" + month + @"'
                        and tt.到达现场时刻 is not null and ttt.车辆状态编码=3 and 操作来源编码=2 and ale.是否测试=0
                        and 事件类型编码 in (0,1,2)
                        and ts.中心编码<>1
                        group by alet.名称
                        order by alet.名称
                        set nocount off
                        
                    ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region  受理记录流水表
        public DataTable Get_LS_SLJL(DateTime beginTime, DateTime endTime, string acceptType, string callType)
        {
            StringBuilder sb = new StringBuilder();
            if (acceptType == "" && callType == "")
            {
                sb.Append(@"
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        DECLARE @acceptType varchar
                        DECLARE @callType varchar
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                        set @acceptType = '" + acceptType + @"'
                        set @callType = '" + callType + @"'
       
                       select * from(
                        select top 10000000 CallTime=tac.通话时刻,CallNumber=tac.主叫号码,AcceptType=tzaet.名称
                        ,CallCode=tac.事件编码,WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCall tac
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=tac.通话类型编码
                        left join dbo.TPerson tp on tp.编码=tac.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
                //sb.Append(@" and tzaet.编码 in( @acceptType)");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco.通话时刻,CallNumber=taco.主叫号码,AcceptType=tzct.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco
                        left join dbo.TZCallType tzct on tzct.编码=taco.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco.通话类型编码<=10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzct.编码", callType, sb);
                //sb.Append(@"  and tzct.编码 in(@callType) ");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco2.通话时刻,CallNumber=taco2.主叫号码,AcceptType=tzaet.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco2
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=taco2.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco2.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco2.通话类型编码>10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
                //sb.Append(@" and tzaet.编码 in( @acceptType)");

                sb.Append("  ) as A ");
                sb.Append("order by CallTime ");
            }
            else if (acceptType == "" && callType != "")
            {
                sb.Append(@"
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        DECLARE @acceptType varchar
                        DECLARE @callType varchar
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                        set @acceptType = '" + acceptType + @"'
                        set @callType = '" + callType + @"'
       
                       select * from(
                        select top 10000000 CallTime=tac.通话时刻,CallNumber=tac.主叫号码,AcceptType=tzaet.名称
                        ,CallCode=tac.事件编码,WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCall tac
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=tac.通话类型编码
                        left join dbo.TPerson tp on tp.编码=tac.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime and tp.是否有效=1 ");
                sb.Append(@" and tzaet.编码 in( 1000)");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco.通话时刻,CallNumber=taco.主叫号码,AcceptType=tzct.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco
                        left join dbo.TZCallType tzct on tzct.编码=taco.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco.通话类型编码<=10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzct.编码", callType, sb);
                //sb.Append(@"  and tzct.编码 in(@callType) ");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco2.通话时刻,CallNumber=taco2.主叫号码,AcceptType=tzaet.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco2
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=taco2.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco2.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco2.通话类型编码>10 and tp.是否有效=1 ");
                sb.Append(@" and tzaet.编码 in( 1000)");

                sb.Append("  ) as A ");
                sb.Append("order by CallTime ");
            }
            else if (acceptType != "" && callType == "")
            {
                sb.Append(@"
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        DECLARE @acceptType varchar
                        DECLARE @callType varchar
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                        set @acceptType = '" + acceptType + @"'
                        set @callType = '" + callType + @"'
       
                       select * from(
                        select top 10000000 CallTime=tac.通话时刻,CallNumber=tac.主叫号码,AcceptType=tzaet.名称
                        ,CallCode=tac.事件编码,WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCall tac
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=tac.通话类型编码
                        left join dbo.TPerson tp on tp.编码=tac.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
                //sb.Append(@" and tzaet.编码 in( @acceptType)");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco.通话时刻,CallNumber=taco.主叫号码,AcceptType=tzct.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco
                        left join dbo.TZCallType tzct on tzct.编码=taco.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco.通话类型编码<=10 and tp.是否有效=1 ");
                sb.Append(@"  and tzct.编码 in(1000) ");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco2.通话时刻,CallNumber=taco2.主叫号码,AcceptType=tzaet.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco2
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=taco2.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco2.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco2.通话类型编码>10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
                //sb.Append(@" and tzaet.编码 in( @acceptType)");

                sb.Append("  ) as A ");
                sb.Append("order by CallTime ");
            }
            else if (acceptType != "" && callType != "")
            {
                sb.Append(@"
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        DECLARE @acceptType varchar
                        DECLARE @callType varchar
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                        set @acceptType = '" + acceptType + @"'
                        set @callType = '" + callType + @"'
       
                       select * from(
                        select top 10000000 CallTime=tac.通话时刻,CallNumber=tac.主叫号码,AcceptType=tzaet.名称
                        ,CallCode=tac.事件编码,WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCall tac
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=tac.通话类型编码
                        left join dbo.TPerson tp on tp.编码=tac.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
                //sb.Append(@" and tzaet.编码 in( @acceptType)");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco.通话时刻,CallNumber=taco.主叫号码,AcceptType=tzct.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco
                        left join dbo.TZCallType tzct on tzct.编码=taco.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco.通话类型编码<=10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzct.编码", callType, sb);
               // sb.Append(@"  and tzct.编码 in(@callType) ");
                sb.Append(" union ");
                sb.Append(@" select top 10000000 CallTime=taco2.通话时刻,CallNumber=taco2.主叫号码,AcceptType=tzaet.名称
                        ,CallCode='',WorkCode=tp.工号,RecordCode=录音号
                        from dbo.TAlarmCallOther taco2
                        left join dbo.TZAcceptEventType tzaet on tzaet.编码=taco2.通话类型编码
                        left join dbo.TPerson tp on tp.编码=taco2.调度员编码
                        where 通话时刻>=@BeginTime and 通话时刻<=@EndTime
                        and taco2.通话类型编码>10 and tp.是否有效=1 ");
                WhereClauseUtility.AddInSelectQuery("tzaet.编码", acceptType, sb);
               // sb.Append(@" and tzaet.编码 in( @acceptType)");

                sb.Append("  ) as A ");
                sb.Append("order by CallTime ");
            }
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        #endregion

        #region 司机出车大于5分钟流水表
        public DataTable Get_LS_SJCCQK(DateTime beginTime, DateTime endTime, string center, string station, string name, string workCode, string carNumber, int sendCarTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" set ansi_warnings off
                        DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'
                        --declare @s int
                        --set @s=5

                        declare @EventCodeB char(18)
                        declare @EventCodeE char(18)
                        declare @TaskCodeB char(22)
                        declare @TaskCodeE char(22)

                        set @EventCodeB = convert(char(8),@BeginTime,112)+'0000000000'
                        set @EventCodeE = convert(char(8),dateadd(day,1,@EndTime),112)+'0000000000'
                        set @TaskCodeB = @EventCodeB+'0000'
                        set @TaskCodeE = @EventCodeE+'0000'



                        select 任务编码,操作来源编码, 车辆状态编码
                        into #temptime
                        from TTaskTime
                        where 任务编码>@TaskCodeB and 任务编码<=@TaskCodeE 

                        select 任务编码,收到指令方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp1
                        from #temptime
                        where 车辆状态编码=1
                        select 任务编码,驶向现场方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp2
                        from #temptime
                        where 车辆状态编码=2
                        select 任务编码,抢救转运方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp3
                        from TTaskTime 
                        where 车辆状态编码=3
                        select 任务编码,病人上车方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp4
                        from #temptime
                        where 车辆状态编码=4
                        select 任务编码,到达医院方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp5
                        from #temptime
                        where 车辆状态编码=5
                        select 任务编码,途中待命方式=case when 操作来源编码=1  then '台' when 操作来源编码=2 then '车载' else '其他' end
                        into #temp6
                        from #temptime
                        where 车辆状态编码=6

                        select tt.任务编码,收到指令方式,驶向现场方式,抢救转运方式,病人上车方式,到达医院方式,途中待命方式
                        into #temp7
                        from TTask tt left join #temp1 t1 on t1.任务编码=tt.任务编码
                        left join #temp2 t2 on t2.任务编码=tt.任务编码
                        left join #temp3 t3 on t3.任务编码=tt.任务编码
                        left join #temp4 t4 on t4.任务编码=tt.任务编码
                        left join #temp5 t5 on t5.任务编码=tt.任务编码
                        left join #temp6 t6 on t6.任务编码=tt.任务编码
                        where tt.任务编码>@TaskCodeB and tt.任务编码<=@TaskCodeE


                        select
                         TaskCode=tt.任务编码,
                        Center=tc.名称,
                        Station=ts.名称,
                        CarNumber=tam.实际标识,
                        Driver=tp.姓名,
                        AskTime=convert(varchar(16),tt.生成任务时刻,21),
                        SendTime=convert(varchar(16),tt.出车时刻,21),
                        ArriveScenesTime=convert(varchar(16),tt.到达现场时刻,21),
                        PaientInCarTime=convert(varchar(16),tt.离开现场时刻,21),
                        ArriveHospitalTime=convert(varchar(16),tt.到达医院时刻,21),
                        FinishMissionTime=convert(varchar(16),tt.完成时刻,21),
                        keyType=case when 收到指令方式='台' and 驶向现场方式='台' and 抢救转运方式='台' 
                        and 病人上车方式='台' and 到达医院方式='台' and 途中待命方式='台' then '调度'
                        when 收到指令方式='车载' and 驶向现场方式='车载' and 抢救转运方式='车载' 
                        and 病人上车方式='车载' and 到达医院方式='车载' and 途中待命方式='车载' then 'GPS'
                        else '其他' end ,
                        Time='" + sendCarTime + @"'

                        from TTask tt
                        left join TAlarmEvent tae on tae.事件编码=tt.事件编码
                        left join TAmbulance tam on tam.车辆编码=tt.车辆编码
                        left join TStation ts on ts.编码=tt.分站编码
                        left join TCenter tc on tc.编码=ts.中心编码
                        left join TTaskPersonLink ttpl on ttpl.任务编码=tt.任务编码 and ttpl.人员类型编码=3
                        left join TPerson tp on tp.编码=ttpl.人员编码
                        left join #temp7 ttt on ttt.任务编码=tt.任务编码

                        where 生成任务时刻>=@BeginTime and 生成任务时刻<=@EndTime and 是否测试=0
                        and tt.任务编码>@TaskCodeB and tt.任务编码<=@TaskCodeE
                        and tt.是否正常结束=1 
                        and datediff(second,tt.生成任务时刻,出车时刻)>'" + sendCarTime + @"'*60 ");
            WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
            WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
            WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
            WhereClauseUtility.AddStringLike("tam.实际标识", carNumber, sb);

            sb.Append(@"
                        order by tc.顺序号,ts.名称,tam.实际标识,tp.姓名,tt.生成任务时刻
                        drop table #temptime
                        drop table #temp1
                        drop table #temp2
                        drop table #temp3
                        drop table #temp4
                        drop table #temp5
                        drop table #temp6
                        drop table #temp7
                        set ansi_warnings on
                  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 病历流水表
        public DataTable Get_LS_BL(DateTime beginTime, DateTime endTime, string eventType, string name, string centerID, string stationID, string diseasesClassification, string doctorAndNurse, string driver, string stretcher, string alarmReason,
                                   string illnessClassification, string illnessForecast, string firstAidEffect, string diseaseCooperation, string firstImpression, string deathCase, string deathCertificate, string treatmentMeasure)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                  select PatientVersion= mpr.PatientVersion,Name=mpr.Name,Sex=mpr.Sex,AgeType=mpr.AgeType,
                         Age= case when AgeType = '不详' then '不详' else Age+AgeType  end 
                        ,Station=mpr.Station,LocalAddress=mpr.LocalAddress,DrivingTime=mpr.DrivingTime,ArriveSceneTime=mpr.ArriveSceneTime,SendAddress=mpr.SendAddress,LeaveSceneTime=mpr.LeaveSceneTime
                        ,ArriveDestinationTime=mpr.ArriveDestinationTime,DoctorAndNurse=mpr.DoctorAndNurse,TeachingPractice=mpr.TeachingPractice,Driver=mpr.Driver,StretcherBearersI=mpr.StretcherBearersI
                        ,ProvideMedicalHistoryPeople=mpr.ProvideMedicalHistoryPeople,DiseasesClassification=mpr.DiseasesClassification,AlarmReason=mpr.AlarmReason,ContactTelephone=mpr.ContactTelephone
                        ,HistoryOfPresentIllness=mpr.HistoryOfPresentIllness,PastMedicalHistory=mpr.PastMedicalHistory,PastMedicalHistorySupplement=mpr.PastMedicalHistorySupplement,TChoose=mpra.TChoose
                        ,T=mpra.T,PChoose=mpra.PChoose,P=mpra.P,RChoose=mpra.RChoose,R=mpra.R,BPChoose=mpra.BPChoose,BP1=mpra.BP1,BP2=mpra.BP2,BP3=mpra.BP3,BP4=mpra.BP4,Consciousness=mpra.Consciousness
                        ,Pupilla=mpra.Pupilla,PupillaSupplement=mpra.PupillaSupplement,LightReflex=mpra.LightReflex,LightReflexSupplement=mpra.LightReflexSupplement,Skin=mpra.Skin,SkinSupplement=mpra.SkinSupplement
                        ,Head=mpra.Head,HeadSupplement=mpra.HeadSupplement,Neck=mpra.Neck,NeckSupplement=mpra.NeckSupplement,Chest=mpra.Chest,ChestExtrusionTest=mpra.ChestExtrusionTest,ChestSupplement=mpra.ChestSupplement
                        ,Lungs=mpra.Lungs,TwoLungBreathingSound=mpra.TwoLungBreathingSound,LeftLung=mpra.LeftLung,RightLung=mpra.RightLung,LungsSupplement=mpra.LungsSupplement,Heart=mpra.Heart,HeartRate=mpra.HeartRate
                        ,HeartRhythm=mpra.HeartRhythm,HeartMurmur=mpra.HeartMurmur,HeartSupplement=mpra.HeartSupplement,Abdomen=mpra.Abdomen,AbdomenSupplement=mpra.AbdomenSupplement,Spine=mpra.Spine
                        ,SpineSupplement=mpra.SpineSupplement,Limbs=mpra.Limbs,LimbsSupplement=mpra.LimbsSupplement,PelvicExtrusionTest=mpra.PelvicExtrusionTest,NervousSystem=mpra.NervousSystem
                        ,NervousSystemFace=mpra.NervousSystemFace,NervousSystemSpeech=mpra.NervousSystemSpeech,NervousSystemSupplement=mpra.NervousSystemSupplement,LeftUpperExtremity=mpra.LeftUpperExtremity
                        ,LeftLowerExtremity=mpra.LeftLowerExtremity,RightUpperLimb=mpra.RightUpperLimb,RightLowerLimb=mpra.RightLowerLimb,BabinskiSign=mpra.BabinskiSign,ObstetricExamination=mpra.ObstetricExamination
                        ,GongGaoCartilagoEnsiformis=mpra.GongGaoCartilagoEnsiformis,GongGaoNavel=mpra.GongGaoNavel,GongGaoSuprapubic=mpra.GongGaoSuprapubic,GongGaoBelowUmbilicus=mpra.GongGaoBelowUmbilicus
                        ,ObstetricExaminationSupplement=mpra.ObstetricExaminationSupplement,OpenReaction=mpra.OpenReaction,LanguageReaction=mpra.LanguageReaction,MotionResponse=mpra.MotionResponse,Position=mpra.Position
                        ,DamageWay=mpra.DamageWay,CirculationChange=mpra.CirculationChange,BreathingChange=mpra.BreathingChange,ConsciousnessChange=mpra.ConsciousnessChange,HeartRatePerMinute=mpra.HeartRatePerMinute
                        ,Breathing=mpra.Breathing,MuscleTension=mpra.MuscleTension,LaryngealReflex=mpra.LaryngealReflex,SkinColor=mpra.SkinColor,SymptomPendingInvestigation=mpra.SymptomPendingInvestigation
                        ,SymptomPendingInvestigationSupplement=mpra.SymptomPendingInvestigationSupplement,FirstImpression=mpra.FirstImpression,FirstImpressionSupplement=mpra.FirstImpressionSupplement
                        ,ECGImpression=mpra.ECGImpression,ECGImpressionSupplement=mpra.ECGImpressionSupplement,ECGImpressionRetestI=mpra.ECGImpressionRetestI,ECGImpressionRetestISupplement=mpra.ECGImpressionRetestISupplement
                        ,ECGImpressionRetestII=mpra.ECGImpressionRetestII,ECGImpressionRetestIISupplement=mpra.ECGImpressionRetestIISupplement,BloodSugar=mpra.BloodSugar,BloodSugarRetestI=mpra.BloodSugarRetestI
                        ,BloodSugarRetestII=mpra.BloodSugarRetestII,BloodOxygenSaturation=mpra.BloodOxygenSaturation,BloodOxygenSaturationRetestI=mpra.BloodOxygenSaturationRetestI,BloodOxygenSaturationRetestII=mpra.BloodOxygenSaturationRetestII
                        ,CPRIFSuccess=mpr.CPRIFSuccess,IllnessClassification=mpr.IllnessClassification,IllnessForecast=mpr.IllnessForecast,DeathCase=mpr.DeathCase,DeathCertificate=mpr.DeathCertificate,FirstAidEffect=mpr.FirstAidEffect
                        ,MajorEmergencies=mpr.MajorEmergencies,DiseaseCooperation=mpr.DiseaseCooperation,DiseaseNotCooperationSupplement=mpr.DiseaseNotCooperationSupplement,Witness=mprc.Witness,CarToBeforeCPR=mprc.CarToBeforeCPR
                        ,CarToBeforeDefibrillation=mprc.CarToBeforeDefibrillation,CardiacArrestReason=mprc.CardiacArrestReason,CardiacArrestReasonSupplement=mprc.CardiacArrestReasonSupplement
                        ,AfterResuscitationECGDiagnosis=mprc.AfterResuscitationECGDiagnosis,AfterResuscitationBP=mprc.AfterResuscitationBP,AfterResuscitationSaO2=mprc.AfterResuscitationSaO2,PulsationAppearTime= mprc.PulsationAppearTime
                        ,BreatheAppearTime=mprc.BreatheAppearTime,IFAdmittedToHospital=mprc.IFAdmittedToHospital,DoctorFollowUp=mprc.DoctorFollowUp
                                    
                   from  M_PatientRecord mpr 
                   left join  M_PatientRecordAppend mpra on mpr.TaskCode = mpra.TaskCode
                   left join  M_PatientRecordCPR  mprc on mpr.TaskCode = mprc.TaskCode ");
            sb.Append("left join ").Append(DispatchBuilder.InitialCatalog).Append(".dbo.TTask tt on mpr.TaskCode=tt.任务编码 ");
            sb.Append("left join ").Append(DispatchBuilder.InitialCatalog).Append(".dbo.TStation ts on tt.分站编码=ts.编码 ");
            sb.Append(" where mpr.MedicalRecordGenerationTime>='" + beginTime + "' and mpr.MedicalRecordGenerationTime<'" + endTime + @"' and mpr.SubmitLogo=1 ");
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", centerID, sb);
            WhereClauseUtility.AddInSelectQuery("tt.分站编码", stationID, sb);
            WhereClauseUtility.AddStringEqual("mpr.PatientVersion", eventType, sb);
            WhereClauseUtility.AddStringLike("mpr.DiseasesClassification", diseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.Driver", driver, sb);
            WhereClauseUtility.AddStringEqual("mpr.Name", name, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndNurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", stretcher, sb);
            WhereClauseUtility.AddStringLike("mpr.AlarmReason", alarmReason, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", illnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessForecast", illnessForecast, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", firstAidEffect, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseaseCooperation", diseaseCooperation, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", firstImpression, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", deathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCertificate", deathCertificate, sb);
            WhereClauseUtility.AddStringLike("mpr.TreatmentMeasure", treatmentMeasure, sb);
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 取分站（病历流水表）
        //取分站
        public object GetStations()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select 编码,名称 
                      from TStation  where 是否有效 =1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取病种类型（病历流水表）
        //取病种类型
        public object GetDiseasesClassifications()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_ZCaseTemplate  where IsActive =1 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取病情类型（病历流水表）
        public object GetIllnessClassification()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'IllState' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取病情预报（病历流水表）
        public object GetIllnessForecast()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'IllStateReport' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取急救效果（病历流水表）
        public object GetFirstAidEffect()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'FirstAidEffect' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取病家合作度（病历流水表）
        public object GetDiseaseCooperation()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'DiseaseCooperation' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取事件类型（病历流水表）
        public object GetEventType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'PatientVersion' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取死亡类型(病历流水表)
        public object GetDeathCase()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'DeathCase' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取死亡证明类型(病历流水表)
        public object GetDeathCertificate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                      select ID,Name 
                      from M_Dictionary  where IsActive = 1 and TypeID = 'DeathCertificate' ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<CheckModelExt> list = new List<CheckModelExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                CheckModelExt info = new CheckModelExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new CheckModelExt();
                    info.ID = Convert.ToString(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取受理类型

        public object GetAcceptType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select  编码,名称 from TZAcceptEventType
                        where 是否有效=1                 
                        ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        //取电话类型
        public object GetCallType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select 编码,名称 from TZCallType
                        where 是否有效=1 and 编码 in (0,4,10)");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取车辆状态类型

        public object GetAmbulanceState()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select  编码,名称 from TZAmbulanceState
                        where 是否有效=1                 
                        ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        #endregion

        #region 取车辆状态类型

        public object GetRole()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select  编码,名称 from TRole
                        where 是否有效=1  and 编码 in (3,4,5,6)               
                        ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            List<P_User> list = new List<P_User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                P_User info = new P_User();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new P_User();
                    info.ID = Convert.ToInt32(dr["编码"]);
                    info.Name = dr["名称"].ToString();

                    list.Add(info);
                }
            }
            return list;

        }
        #endregion
    }
}
