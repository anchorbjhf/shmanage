using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL.TJDAL
{

    /// <summary>
    /// 统计类
    /// </summary>
    public class TJDAL
    {
        public static readonly SqlConnectionStringBuilder DispatchBuilder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);//取调度库的链接字符串
        public static readonly string DispatchDatabase = DispatchBuilder.InitialCatalog;

        #region 出车次数分段统计表
        /// <summary>
        /// 出车次数分段统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_CCCSFD(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"            
                select  任务编码,tae.事件类型编码,日期 =case when datepart(hh,生成任务时刻)<8 then CONVERT(varchar(12),dateadd(dd,-1,生成任务时刻),102) else CONVERT(varchar(12),生成任务时刻,102) end, 生成任务时刻
                into #temp0
                from dbo.TTask tt left join dbo.TAlarmEvent tae on tt.事件编码=tae.事件编码
                where tt.生成任务时刻>='" + beginTime.ToString() + "' AND tt.生成任务时刻<='" + endTime.ToString() + @"'
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and tae.是否测试=0
                and tt.出车时刻 is not null

                SELECT 
                Date=日期,
                Week=datename(weekday,日期),
                MAidNumber = case when 事件类型编码 =0 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)<16 then 1 else 0 end,
                MHomeNumber = case when 事件类型编码 =1 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)<16 then 1 else 0 end,
                MTransferNumber = case when 事件类型编码 =2 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)<16 then 1 else 0 end,
                AAidNumber = case when 事件类型编码 in(0,2) and datepart(hh,生成任务时刻)>=16 and datepart(hh,生成任务时刻)<20 then 1 else 0 end,
                AHomeNumber = case when 事件类型编码 =1 and datepart(hh,生成任务时刻)>=16 and datepart(hh,生成任务时刻)<20 then 1 else 0 end,
                NAidNumber = case when datepart(hh,生成任务时刻)>=20 or datepart(hh,生成任务时刻)<8  then 1 else 0 end
                INTO #temp1
                FROM #temp0

                SELECT  Date,Week, 
                MAidNumber=isnull(SUM(MAidNumber),0),
                MTransferNumber=isnull(SUM(MTransferNumber),0),
                MHomeNumber=isnull(SUM(MHomeNumber),0),
                AAidNumber=isnull(SUM(AAidNumber),0),
                AHomeNumber=isnull(SUM(AHomeNumber),0),
                NAidNumber=isnull(SUM(NAidNumber),0)
                FROM  #temp1  
                GROUP BY Date,Week
                ORDER BY Date,Week

                DROP TABLE #temp0
                DROP TABLE #temp1");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 出车情况统计表
        /// <summary>
        /// 出车情况统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_CCQK(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                 --set ansi_warnings off      
                  select 
                  Center=tc.名称 
                  ,Times=t.次数
                  ,TimesIn2Minutes=t.两分钟内次数
                  ,TimesBetween2And5Minutes=t.[2-5分钟次数]
                  ,TimesOver5Minutes=t.五分钟以上次数
                  ,InvalidTimes=t.次数-t.两分钟内次数-t.[2-5分钟次数]-t.五分钟以上次数 
                  ,GPSPushTimes=t.GPS按压次数
      
                  from TCenter tc
                  left join (select
			             ts.中心编码
			             ,count(*) as 次数
			             ,两分钟内次数=sum(case when datediff(second,tt.生成任务时刻,tt.出车时刻)<120 then 1 else 0 end)
			             ,[2-5分钟次数]=sum(case when datediff(second,tt.生成任务时刻,tt.出车时刻)>=120 and datediff(second,tac.发送指令时刻,tt.出车时刻)<=300 then 1 else 0 end)
			             ,五分钟以上次数=sum(case when datediff(second,tt.生成任务时刻,tt.出车时刻)>300 then 1 else 0 end)
			             ,GPS按压次数=count(t.任务编码)

			             from TTask tt
				            inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
				            inner join TAlarmEvent tae on tae.事件编码=tac.事件编码
                            left join TStation ts on ts.编码 = tt.分站编码
				            left join dbo.TCenter tc on ts.中心编码=tc.编码
				            left join (select distinct ttt.任务编码
							        from dbo.TTaskTime ttt
							        where 车辆状态编码=6 and 操作来源编码=2 and ttt.任务编码>'" + taskCodeB + "' and ttt.任务编码<='" + taskCodeE
                                    + @"')t on tt.任务编码 = t.任务编码
			            where 生成任务时刻 >= '" + beginTime.ToString() + "' and 生成任务时刻<='" + endTime.ToString() + @"' and tae.是否测试 = 0
				        and tt.是否正常结束 = 1  and 是否执行中=0 and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
			            group by ts.中心编码
                        )t on tc.编码 = t.中心编码
                   where tc.编码 <> 1
                   order by tc.顺序号
                   --set ansi_warnings on ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度员工作量统计表
        /// <summary>
        /// 调度员工作量统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_DDYGZL(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  set ansi_warnings off          
                    ------受理数（调度员填了单子的）--------------
                    select 
                           tac.责任受理人编码 as 调度员编码
                           --,count(tac.事件编码) as 受理数
                           ,sum(case when tac.受理类型编码 in (0,1,2,3,4,5,6,14,18,19,21,22,23) then 1 else 0 end) as 受理数
                           ,sum(case when tac.受理类型编码 in (18,19) then 1 else 0 end) as 催车数
                           ,sum(case when tac.受理类型编码=4 then 1 else 0 end) as 回车数
                    into #temp
                    from dbo.TAcceptEvent tac
                    left join dbo.TAlarmEvent tae on tac.事件编码 = tae.事件编码

                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                          --and tac.受理类型编码 in (0,1,2,3,4,5,6,14,18,19)
                    group by tac.责任受理人编码

                    -------------------------------------------
                    ------平均受理时间（考核调度员的制单效率，所以只取受理序号是1的）-----------------------------------
                    --select ace.责任受理人编码 as 调度员编码,avg(datediff(second,ace.开始受理时刻,ace.结束受理时刻)) as 平均受理时间
                    --,sum(datediff(second,ace.开始受理时刻,ace.结束受理时刻)) as 总制单时间,count(ale.事件编码)as 制单次数
                    --into #temp0
                    --from dbo.TAlarmEvent ale
                    --left join dbo.TAcceptEvent  ace on ale.事件编码=ace.事件编码 and ace.受理序号=1
                    --where ale.首次受理时刻 >= '" + beginTime + "' and ale.首次受理时刻<'" + endTime + @"' and ale.是否测试 = 0
                    --group by ace.责任受理人编码

                    select 调度员编码,avg(datediff(second,通话时刻,结束时刻)) as 平均受理时间,sum(datediff(second,通话时刻,结束时刻)) as 总制单时间,count(ale.事件编码)as 制单次数
                    into #temp0
                    from dbo.TAlarmEvent ale left join  dbo.TAlarmCall alc on ale.事件编码=alc.事件编码 
                    and alc.受理序号=1
                    where ale.首次受理时刻 >= '" + beginTime + "' and ale.首次受理时刻<'" + endTime + @"' and ale.是否测试 = 0
                    and alc.是否呼出=0
                    group by 调度员编码
                    --order by 调度员编码

                    -------------------------------------------
                    ------派车的-----------------------------------
                    select 
                           tt.责任调度人编码 as 调度员编码
                           ,count(tt.任务编码) as 派车数
                           ,sum(case when (tt.是否正常结束=1 or (tt.异常结束原因编码=5 and tt.到达现场时刻 is not null)) then 1 else 0 end) as 有效受理数
                    --       ,avg(case when datediff(second,tt.开始受理时刻,tt.结束受理时刻)>0 then datediff(second,tt.开始受理时刻,tt.结束受理时刻) end) as 平均受理时间
                    into #temp1
                    from TTask tt
                    inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
                    inner join TAlarmEvent tae on tt.事件编码=tae.事件编码

                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                    group by tt.责任调度人编码

                    -------------------------------------------
                    --------转分中心和转长途-------------------------------
                    select 
	                       首次调度员编码 as 调度员编码,
		                    count(*) as 转车数
                    into #temp2
                    from dbo.TAlarmEvent tae
                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.事件类型编码 in (5,6) and tae.是否测试 = 0
                    group by tae.首次调度员编码

                    -------------------------------------------
                    -------接听电话和迟缓接听电话------------------------
                    select 
                           调度员编码
                           ,count(通话时刻) as 接听电话
                           ,sum(case when (datediff(second,振铃时刻,通话时刻)>10) then 1 else 0 end) as 迟缓接听电话数
                    into #temp3
                    from dbo.TAlarmCall 
                    where 通话时刻>= '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                    group by 调度员编码

                    select 
                           调度员编码
                           ,count(通话时刻) as 接听电话
                           ,sum(case when (datediff(second,振铃时刻,通话时刻)>10) then 1 else 0 end) as 迟缓接听电话数
                    into #temp4
                    from  dbo.TAlarmCallOther
                    where 通话时刻 >= '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                    group by 调度员编码

                    -------------------------------------------
                    -------未接听电话------------------------------------
                    select 
                           调度员工号
                           --,sum(case when 结果类型=0 then 1 else 0 end) as 接听电话数
                           ,sum(case when (结果类型=1 and 震铃时刻 is not null  and datediff(second,震铃时刻,结束时刻)>30)  then 1 else 0 end) as 未接听电话数
                           --,sum(case when 结果类型=1 and datediff(second,来电时刻,结束时刻)>30 then 1 else 0 end) as 正常呼救未接听数    
                    into #temp5      
                    from dbo.TTelRecord
                    where 来电时刻 >= '" + beginTime + "' and 来电时刻<'" + endTime + @"' and 是否呼出 = 0
                    group by 调度员工号

                    -------------------------------------------
                    -------上岗时间--------------------------------
                    SELECT distinct 人员编码,
                           --上岗时间 = SUM(CASE 记录状态 WHEN '退出' THEN DATEDIFF(minute, 登录时刻, 退出时刻) ELSE 0 END)
                           上岗时间 = SUM(CASE
                                 WHEN  登录时刻>= '" + beginTime + "'  AND  退出时刻 < '" + endTime + @"'  THEN DATEDIFF(second, 登录时刻,退出时刻)              --正常
                                 WHEN  登录时刻>= '" + beginTime + "' and 登录时刻<  '" + endTime + @"' AND  退出时刻 >  '" + endTime + @"'  THEN DATEDIFF(second, 登录时刻, '" + endTime + @"')
                                 WHEN  登录时刻< '" + beginTime + "'  and 退出时刻>'" + beginTime + "' AND 退出时刻 <  '" + endTime + "' THEN  DATEDIFF(second,'" + beginTime + @"',退出时刻)    --查询开始时刻大于登陆时刻和结束时刻大于退出时刻
	                             WHEN  登录时刻< '" + beginTime + "' and 退出时刻> '" + endTime + "' THEN DATEDIFF(second,'" + beginTime + "', '" + endTime + @"')
                                         ELSE 0
                                         END)
                    INTO #temp6
                    FROM dbo.TOperatorSign
                    WHERE 登录时刻 IS NOT NULL AND 登录时刻<退出时刻
                        AND  (退出时刻 < GetDate()) --剔除非法数据 
                    group by 人员编码
                    -------------------------------------------
                    --------平均接听时间-----------------------
                    select 
                          调度员编码
                          ,avg(datediff(second,振铃时刻,通话时刻)) as 平均接听时间
                    into #temp7
                    from (select 
                               通话时刻
                               ,振铃时刻
                               ,调度员编码
                         from dbo.TAlarmCall
                         where 通话时刻 >=  '" + beginTime + "' and 通话时刻< '" + endTime + @"' and 是否呼出=0 and datediff(second,振铃时刻,通话时刻)>0
                         union all
                         select 
                               通话时刻
                               ,振铃时刻
                               ,调度员编码
                         from dbo.TAlarmCallOther
                         where 通话时刻 >=  '" + beginTime + "' and 通话时刻< '" + endTime + @"' and 是否呼出=0 and datediff(second,振铃时刻,通话时刻)>0
                         )t
                    group by 调度员编码
                    -------------------------------------------
                    select 
                          Name=tp.姓名
                          ,WorkNumber=tp.工号
                          ,AcceptedNumber=t.受理数
                          ,ValidAcceptedNumber=t1.有效受理数
                          ,SendNumber=t1.派车数
                          ,HuryNumber=t.催车数
                          ,BackNumber=t.回车数
                          ,TransferNumber=t2.转车数
                          ,Others=t.受理数-t1.有效受理数-t.催车数-t.回车数-isnull(t2.转车数,0) 
                          ,AVGAcceptedTime=t0.平均受理时间
                          ,TotalMakeFileTime=t0.总制单时间
                          ,MakeFileTimes=t0.制单次数
                          ,WorkingTime=t6.上岗时间
                          ,AnswerCallNumber=t3.接听电话+t4.接听电话 
                          ,MissedCallNumber=t5.未接听电话数
                          --,t5.正常呼救未接听数
                          ,DelayedCallNumber=t3.迟缓接听电话数+t4.迟缓接听电话数 
                          ,AVGAnswerTime=t7.平均接听时间
                    from dbo.TPerson tp
                    left join #temp t on tp.编码=t.调度员编码
                    left join #temp0 t0 on tp.编码=t0.调度员编码
                    left join #temp1 t1 on tp.编码=t1.调度员编码
                    left join #temp2 t2 on tp.编码=t2.调度员编码
                    left join #temp3 t3 on tp.编码=t3.调度员编码
                    left join #temp4 t4 on tp.编码=t4.调度员编码
                    left join #temp5 t5 on tp.工号=t5.调度员工号
                    left join #temp6 t6 on tp.编码=t6.人员编码
                    left join #temp7 t7 on tp.编码=t7.调度员编码
                    where tp.类型编码 = 2 and tp.是否有效=1

                    drop table #temp
                    drop table #temp0
                    drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp4
                    drop table #temp5
                    drop table #temp6
                    drop table #temp7
                    set ansi_warnings on ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度员工作量统计表(二)
        /// <summary>
        /// 调度员工作量统计表（二）
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_DDYGZL2(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"            
                ------受理数（调度员填了单子的）--------------
                select 
                        tac.责任受理人编码 as 调度员编码
                        --,count(tac.事件编码) as 受理数
                        ,sum(case when tac.受理类型编码 in (0,1,2,3,4,5,6,14,18,19,21,22,23) then 1 else 0 end) as 受理数
                        ,sum(case when tac.受理类型编码 in (18,19) then 1 else 0 end) as 催车数
                        ,sum(case when tac.受理类型编码=4 then 1 else 0 end) as 回车数
                into #temp
                from dbo.TAcceptEvent tac
                left join dbo.TAlarmEvent tae on tac.事件编码 = tae.事件编码

                where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                        --and tac.受理类型编码 in (0,1,2,3,4,5,6,14,18,19)
                group by tac.责任受理人编码

                -------------------------------------------
                ------派车的-----------------------------------
                select 
                        tt.责任调度人编码 as 调度员编码
                        ,count(tt.任务编码) as 派车数
                        ,sum(case when (tt.是否正常结束=1 or (tt.异常结束原因编码=5 and tt.到达现场时刻 is not null)) then 1 else 0 end) as 有效受理数
                        ,avg(case when datediff(second,tt.开始受理时刻,tt.结束受理时刻)>0 then datediff(second,tt.开始受理时刻,tt.结束受理时刻) end) as 平均受理时间
                into #temp1
                from TTask tt
                inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
                inner join TAlarmEvent tae on tt.事件编码=tae.事件编码

                where tae.首次受理时刻 >='" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                group by tt.责任调度人编码

                -------------------------------------------
                --------转分中心和转长途-------------------------------
                select 
	                    首次调度员编码 as 调度员编码,
		                count(*) as 转车数
                into #temp2
                from dbo.TAlarmEvent tae
                where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.事件类型编码 in (5,6) and tae.是否测试 = 0
                group by tae.首次调度员编码

                -------------------------------------------
                -------接听电话和迟缓接听电话------------------------
                select 
                        调度员编码
                        ,count(通话时刻) as 接听电话
                        ,sum(case when (datediff(second,振铃时刻,通话时刻)>10) then 1 else 0 end) as 迟缓接听电话数
                into #temp3
                from dbo.TAlarmCall 
                where 通话时刻 >= '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                group by 调度员编码

                select 
                        调度员编码
                        ,count(通话时刻) as 接听电话
                        ,sum(case when (datediff(second,振铃时刻,通话时刻)>10) then 1 else 0 end) as 迟缓接听电话数
                into #temp4
                from  dbo.TAlarmCallOther
                where 通话时刻 >= '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                group by 调度员编码

                -------------------------------------------
                -------未接听电话------------------------------------
                select 
                        调度员工号
                        --,sum(case when 结果类型=0 then 1 else 0 end) as 接听电话数
                        ,sum(case when (结果类型=1 and 震铃时刻 is not null  and datediff(second,震铃时刻,结束时刻)>60)  then 1 else 0 end) as 未接听电话数
                        --,sum(case when (结果类型=0 and datediff(second,震铃时刻,通话时刻)>10) then 1 else 0 end) as 迟缓接听电话数
                into #temp5      
                from dbo.TTelRecord
                where 来电时刻 >= '" + beginTime + "' and 来电时刻<'" + endTime + @"' and 是否呼出 = 0
                group by 调度员工号

                -------------------------------------------
                -------上岗时间--------------------------------
                SELECT distinct 人员编码,
                        --上岗时间 = SUM(CASE 记录状态 WHEN '退出' THEN DATEDIFF(minute, 登录时刻, 退出时刻) ELSE 0 END)
                        上岗时间 = SUM(CASE
                                        WHEN  登录时刻>= '" + beginTime + "'  AND  退出时刻 < '" + endTime + @"'  THEN DATEDIFF(second, 登录时刻,退出时刻)                --正常
                                        WHEN  登录时刻<'" + beginTime + "' AND 退出时刻 < '" + endTime + "' and 退出时刻>'" + beginTime + "' THEN  DATEDIFF(second,'" + beginTime + @"',退出时刻)    --查询开始时刻大于登陆时刻和结束时刻大于退出时刻
                                        WHEN  退出时刻> '" + endTime + @"' AND  登录时刻 >'" + beginTime + "' and 登录时刻<'" + endTime + @"' THEN   DATEDIFF(second,登录时刻,'" + endTime + @"')    --查询开始时刻小于登陆时刻和结束时刻小于退出时刻
                                        WHEN  登录时刻< '" + beginTime + "' AND 退出时刻>'" + endTime + @"' THEN  DATEDIFF(second,'" + beginTime + "','" + endTime + @"')
                                        --WHEN  登录时刻< '" + beginTime + "'  AND 退出时刻 is null THEN  DATEDIFF(second,'" + beginTime + @"',GetDate())
                                        --WHEN  登录时刻>= '" + beginTime + @"'  AND 退出时刻 is null THEN  DATEDIFF(second,登录时刻,GetDate())  
                                        ELSE 0
                                        END)
                INTO #temp6
                FROM dbo.TOperatorSign
                WHERE 登录时刻 IS NOT NULL 
                --AND 登录时刻<退出时刻 AND  (退出时刻 < GetDate()) --剔除非法数据 
                group by 人员编码
                -------------------------------------------

                --------平均接听时间-----------------------
                select 
                        调度员编码
                        ,avg(case when (datediff(second,振铃时刻,通话时刻)>0) then datediff(second,振铃时刻,通话时刻) else 0 end) as 平均接听时间
                into #temp7
                from (select 
                            通话时刻
                            ,振铃时刻
                            ,调度员编码
                        from dbo.TAlarmCallOther
                        where 通话时刻 >= '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                        union all
                        select 
                            通话时刻
                            ,振铃时刻
                            ,调度员编码
                        from dbo.TAlarmCallOther
                        where 通话时刻 >=  '" + beginTime + "' and 通话时刻<'" + endTime + @"' and 是否呼出=0
                        )t
                group by 调度员编码
                -------------------------------------------

                select 
                        Name=tp.姓名
                        ,WorkNumber=tp.工号
                        --,t.受理数+t2.转车数 as 受理数
                        ,AcceptedNumber=t.受理数
                        ,ValidAcceptedNumber=t1.有效受理数
                        ,SendNumber=t1.派车数
                        ,HuryNumber=t.催车数
                        ,BackNumber=t.回车数
                        ,TransferNumber=t2.转车数
                        ,Others=t.受理数-t1.有效受理数-t.催车数-t.回车数-isnull(t2.转车数,0) 
                        ,AVGAcceptedTime=t1.平均受理时间
                        ,WorkingTime=t6.上岗时间
                        ,AnswerCallNumber=t3.接听电话+t4.接听电话 
                        ,MissedCallNumber=t5.未接听电话数
                        ,DelayedCallNumber=t3.迟缓接听电话数+t4.迟缓接听电话数 
                        ,AVGAnswerTime=t7.平均接听时间
                from dbo.TPerson tp
                left join #temp t on tp.编码=t.调度员编码
                left join #temp1 t1 on tp.编码=t1.调度员编码
                left join #temp2 t2 on tp.编码=t2.调度员编码
                left join #temp3 t3 on tp.编码=t3.调度员编码
                left join #temp4 t4 on tp.编码=t4.调度员编码
                left join #temp5 t5 on tp.工号=t5.调度员工号
                left join #temp6 t6 on tp.编码=t6.人员编码
                left join #temp7 t7 on tp.编码=t7.调度员编码
                where tp.类型编码 = 2 and tp.是否有效=1

                drop table #temp
                drop table #temp1
                drop table #temp2
                drop table #temp3
                drop table #temp4
                drop table #temp5
                drop table #temp6
                drop table #temp7");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度员工作效率统计表
        /// <summary>
        /// 调度员工作效率统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_DDYGZXL(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    ------催车数--------------
                    select tac.责任受理人编码 as 调度员编码,sum(case when tac.受理类型编码 in(18,19) then 1 else 0 end) as 催车数
                    into #temp0
                    from dbo.TAcceptEvent tac
                    left join dbo.TAlarmEvent tae on tac.事件编码 = tae.事件编码
                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"'  
                    group by tac.责任受理人编码

                    --------转车数(转分中心和转长途)-------------------------------
                    select 首次调度员编码 as 调度员编码,count(*) as 转车数
                    into #temp1
                    from dbo.TAlarmEvent tae
                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.事件类型编码 in (5,6) and tae.是否测试 = 0
                    group by tae.首次调度员编码
                    -------------------------------------------
                    select 任务编码,tt.事件编码,tac.责任受理人编码,tae.首次调度员编码
                    into #temp2
                    from TTask tt
                    inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
                    inner join TAlarmEvent tae on tt.事件编码=tae.事件编码
                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<'" + endTime + @"' and tae.是否测试 = 0
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                    and (tt.是否正常结束=1 or (tt.异常结束原因编码=5 and tt.到达现场时刻 is not null))

                    select  责任受理人编码 as 调度员编码,有效派车数=count(*)
                    into #temp3
                    from #temp2 
                    group by 责任受理人编码
                    order by 责任受理人编码

                    select 首次调度员编码 as 调度员编码,有效受理量=count(distinct(事件编码))
                    into #temp4
                    from #temp2 
                    group by 首次调度员编码
                    order by 首次调度员编码
                    ---------------------------------------------
                    select 
                          Name=tp.姓名
                          ,WorkNumber=tp.工号
                          ,ValidSendNumber=t3.有效派车数
                          ,ValidAcceptedNumber=t4.有效受理量
                          ,HuryNumber=t.催车数
                          ,TransferNumber=t1.转车数
                          ,ValidWorkNumber=t3.有效派车数+t4.有效受理量+(t.催车数+t1.转车数)*2/3
 
                    from dbo.TPerson tp
                    left join #temp0 t on tp.编码=t.调度员编码
                    left join #temp1 t1 on tp.编码=t1.调度员编码
                    left join #temp3 t3 on tp.编码=t3.调度员编码
                    left join #temp4 t4 on tp.编码=t4.调度员编码
                    where tp.类型编码 = 2 and tp.是否有效=1

                    drop table #temp0,#temp1,#temp2,#temp3,#temp4");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 对讲机统计表
        /// <summary>
        /// 对讲机统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_DJJ(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    Dispatcher=tp.姓名 
                    ,Times=count(*) 
                    from dbo.TTelRecord ttr
                    left join TPerson tp on ttr.调度员工号=tp.工号
                    where 通话时刻 >= '" + beginTime + "' and 通话时刻 <= '" + endTime + @"'
                    and ttr.是否无线=1
                    group by tp.姓名");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 放车情况明细统计表（二）
        /// <summary>
        /// 放车情况明细统计表（二）
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FCQKMX2(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select tae.事件编码,tae.事件类型编码,tae.首次受理时刻,tae.是否测试
                ,现场地址=case when 事件类型编码=2 and charindex(' ',现场地址)>0 then substring(现场地址,0,charindex(' ',现场地址)) else 现场地址 end
                ,送往地点=case when 事件类型编码=2 and charindex(' ',送往地点)>0 then substring(送往地点,0,charindex(' ',送往地点)) else 送往地点 end
                into #temp1
                FROM TTask tt
                inner join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                inner join TAlarmEvent tae on tae.事件编码=tac.事件编码
                WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"'
                and tae.是否测试=0 and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 

                select tae.事件编码,tae.事件类型编码,tae.首次受理时刻,
	                dcyy.等级编码 as fromyy, --等车医院等级
	                swyy.等级编码 as toyy --送往医院等级
                into #temp0
                FROM #temp1 tae
                left join THospitalInfo dcyy on tae.现场地址=dcyy.名称
                left join THospitalInfo swyy on tae.送往地点=swyy.名称
                WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' 
                and tae.是否测试=0

                SELECT 
                时间区段 = CASE DATEPART(hour, 首次受理时刻)
                                    WHEN 0 THEN '00:00-01:00'
                                    WHEN 1 THEN '01:00-02:00'
                                    WHEN 2 THEN '02:00-03:00'
                                    WHEN 3 THEN '03:00-04:00'
                                    WHEN 4 THEN '04:00-05:00'
                                    WHEN 5 THEN '05:00-06:00'
                                    WHEN 6 THEN '06:00-07:00'
                                    WHEN 7 THEN '07:00-08:00'
                                    WHEN 8 THEN '08:00-09:00'
                                    WHEN 9 THEN '09:00-10:00'
                                    WHEN 10 THEN '10:00-11:00'
                                    WHEN 11 THEN '11:00-12:00'
                                    WHEN 12 THEN '12:00-13:00'
                                    WHEN 13 THEN '13:00-14:00'
                                    WHEN 14 THEN '14:00-15:00'
                                    WHEN 15 THEN '15:00-16:00'
                                    WHEN 16 THEN '16:00-17:00'
                                    WHEN 17 THEN '17:00-18:00'
                                    WHEN 18 THEN '18:00-19:00'
                                    WHEN 19 THEN '19:00-20:00'
                                    WHEN 20 THEN '20:00-21:00'
                                    WHEN 21 THEN '21:00-22:00'
                                    WHEN 22 THEN '22:00-23:00'
                                    WHEN 23 THEN '23:00-24:00'
                                    END,
                        次数 = 1,
                        急救数 = CASE 
                                        WHEN 事件类型编码 in (0,3) THEN 1 ELSE 0
                                    END,
                        非急救数 = CASE 
                                        WHEN 事件类型编码=1 THEN 1 ELSE 0
                                    END,
                        转院数 = CASE
                                        WHEN 事件类型编码=2 THEN 1 ELSE 0
                                    END,
	                等级转院 = 	CASE
                                        WHEN 事件类型编码=2 and (fromyy=toyy or fromyy is null or toyy is null) THEN 1 ELSE 0
                                    END,
	                下转上 = 	CASE
                                        WHEN 事件类型编码=2 and fromyy<toyy THEN 1 ELSE 0
                                    END,	
	                上转下 = 	CASE
                                        WHEN 事件类型编码=2 and fromyy>toyy THEN 1 ELSE 0
                                    END,
	                死亡不送=0,
	                病人不去=0,
	                空车数=0,
	                车到人走=0,
	                病家退车=0,
	                另接任务=0,
	                暂停调用=0
                INTO #temp
                FROM #temp0

                UNION ALL
                SELECT 
                时间区段 = CASE DATEPART(hour, tae.首次受理时刻)
                                    WHEN 0 THEN '00:00-01:00'
                                    WHEN 1 THEN '01:00-02:00'
                                    WHEN 2 THEN '02:00-03:00'
                                    WHEN 3 THEN '03:00-04:00'
                                    WHEN 4 THEN '04:00-05:00'
                                    WHEN 5 THEN '05:00-06:00'
                                    WHEN 6 THEN '06:00-07:00'
                                    WHEN 7 THEN '07:00-08:00'
                                    WHEN 8 THEN '08:00-09:00'
                                    WHEN 9 THEN '09:00-10:00'
                                    WHEN 10 THEN '10:00-11:00'
                                    WHEN 11 THEN '11:00-12:00'
                                    WHEN 12 THEN '12:00-13:00'
                                    WHEN 13 THEN '13:00-14:00'
                                    WHEN 14 THEN '14:00-15:00'
                                    WHEN 15 THEN '15:00-16:00'
                                    WHEN 16 THEN '16:00-17:00'
                                    WHEN 17 THEN '17:00-18:00'
                                    WHEN 18 THEN '18:00-19:00'
                                    WHEN 19 THEN '19:00-20:00'
                                    WHEN 20 THEN '20:00-21:00'
                                    WHEN 21 THEN '21:00-22:00'
                                    WHEN 22 THEN '22:00-23:00'
                                    WHEN 23 THEN '23:00-24:00'
                                    END,
                        次数 = 0,
                        急救数 = 0,
                        非急救数 =0,
                        转院数 = 0,
	                等级转院=0,
	                下转上=0,
	                上转下=0,
	                死亡不送= case when tt.异常结束原因编码=2 then 1 else 0 end,
	                病人不去=case when tt.异常结束原因编码=3 then 1 else 0 end,
	                空车数=case when tt.异常结束原因编码 in(1,2,3,4) then 1 else 0 end,--已救治+死亡不送+病人不去+车到人走
	                车到人走=case when tt.异常结束原因编码=4 then 1 else 0 end,
	                病家退车=case when tt.异常结束原因编码=5 then 1 else 0 end,
	                另接任务=case when tt.异常结束原因编码=7 then 1 else 0 end,
	                暂停调用=0
                FROM TTask tt
                left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
                left join TAlarmEvent tae on tae.事件编码=tac.事件编码
                WHERE  tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"'
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                and tae.是否测试=0

                UNION ALL
                SELECT 
                时间区段 = CASE DATEPART(hour, 操作时刻)
                                    WHEN 0 THEN '00:00-01:00'
                                    WHEN 1 THEN '01:00-02:00'
                                    WHEN 2 THEN '02:00-03:00'
                                    WHEN 3 THEN '03:00-04:00'
                                    WHEN 4 THEN '04:00-05:00'
                                    WHEN 5 THEN '05:00-06:00'
                                    WHEN 6 THEN '06:00-07:00'
                                    WHEN 7 THEN '07:00-08:00'
                                    WHEN 8 THEN '08:00-09:00'
                                    WHEN 9 THEN '09:00-10:00'
                                    WHEN 10 THEN '10:00-11:00'
                                    WHEN 11 THEN '11:00-12:00'
                                    WHEN 12 THEN '12:00-13:00'
                                    WHEN 13 THEN '13:00-14:00'
                                    WHEN 14 THEN '14:00-15:00'
                                    WHEN 15 THEN '15:00-16:00'
                                    WHEN 16 THEN '16:00-17:00'
                                    WHEN 17 THEN '17:00-18:00'
                                    WHEN 18 THEN '18:00-19:00'
                                    WHEN 19 THEN '19:00-20:00'
                                    WHEN 20 THEN '20:00-21:00'
                                    WHEN 21 THEN '21:00-22:00'
                                    WHEN 22 THEN '22:00-23:00'
                                    WHEN 23 THEN '23:00-24:00'
                                    END,
                        次数 = 0,
                        急救数 = 0,
                        非急救数 =0,
                        转院数 = 0,
	                等级转院=0,
	                下转上=0,
	                上转下=0,
	                死亡不送= 0,
	                病人不去=0,
	                空车数=0,
	                车到人走=0,
	                病家退车=0,
	                另接任务=0,
	                暂停调用=case when 是否暂停操作=1 then 1 else 0 end
                FROM TPauseRecord tpa
                WHERE  操作时刻>= '" + beginTime + "' AND 操作时刻 <= '" + endTime + @"'

                SELECT 
                    TimeSegment=时间区段,
                    Times = sum(次数),
                    AidNumber = sum(急救数),
                    NotAidNumber =sum(非急救数),
                    TransferNumber = sum(转院数),
	                LevelTransfer=sum(等级转院),
	                LowToHigh=sum(下转上),
	                HighToLow=sum(上转下),
	                DeathNotSend= sum(死亡不送),
	                PatientNotGo=sum(病人不去),
	                EmptyNumber=sum(空车数),
	                ArrivePatientGone=sum(车到人走),
	                PatientReturnCar=sum(病家退车),
	                ReceiveOtherMission=sum(另接任务),
	                PauseUsing=sum(暂停调用)
                FROM #temp
                GROUP BY 时间区段
                ORDER BY 时间区段

                DROP TABLE #temp0
                DROP TABLE #temp
                DROP TABLE #temp1");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 放车情况明细统计表（一）
        /// <summary>
        /// 放车情况明细统计表（一）
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FCQKMX1(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                into #temp0
                FROM TTask tt
                join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                join TAlarmEvent tae on tae.事件编码=tac.事件编码

                WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0
                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                SELECT 
                时间区段 = CASE DATEPART(hour, 首次受理时刻)
                                    WHEN 0 THEN '00:00-01:00'
                                    WHEN 1 THEN '01:00-02:00'
                                    WHEN 2 THEN '02:00-03:00'
                                    WHEN 3 THEN '03:00-04:00'
                                    WHEN 4 THEN '04:00-05:00'
                                    WHEN 5 THEN '05:00-06:00'
                                    WHEN 6 THEN '06:00-07:00'
                                    WHEN 7 THEN '07:00-08:00'
                                    WHEN 8 THEN '08:00-09:00'
                                    WHEN 9 THEN '09:00-10:00'
                                    WHEN 10 THEN '10:00-11:00'
                                    WHEN 11 THEN '11:00-12:00'
                                    WHEN 12 THEN '12:00-13:00'
                                    WHEN 13 THEN '13:00-14:00'
                                    WHEN 14 THEN '14:00-15:00'
                                    WHEN 15 THEN '15:00-16:00'
                                    WHEN 16 THEN '16:00-17:00'
                                    WHEN 17 THEN '17:00-18:00'
                                    WHEN 18 THEN '18:00-19:00'
                                    WHEN 19 THEN '19:00-20:00'
                                    WHEN 20 THEN '20:00-21:00'
                                    WHEN 21 THEN '21:00-22:00'
                                    WHEN 22 THEN '22:00-23:00'
                                    WHEN 23 THEN '23:00-24:00'
                                    END,
                        受理数 = 1,
                        回车数 = 0,
                        实际放车数 = 1,
                        正常放车数 = CASE 
                                        WHEN 事件类型编码 in(0,2,3) THEN
                                        CASE                       
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 5 THEN 1 
                                            ELSE 0
                                        END
                                        WHEN 事件类型编码 = 1 THEN
                                        CASE                       
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 30 THEN 1 
                                            ELSE 0
                                        END
                                    END,
                        迟缓放车数 = CASE
                                        WHEN 事件类型编码 in(0,2,3) THEN
                                        CASE                       
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 THEN 1 
                                            ELSE 0
                                        END
                                        WHEN 事件类型编码 = 1 THEN
                                        CASE                       
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >30 THEN 1 
                                            ELSE 0
                                        END
                                    END,
                        急救放车数 = CASE 
                                        WHEN 事件类型编码 in(0,3) THEN 1
                                        ELSE 0
                                    END,
                        正常急救放车数 = CASE 
                                        WHEN 事件类型编码 in(0,3) THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 5 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END,
                        迟缓急救放车数 = CASE
                                        WHEN 事件类型编码 in(0,3) THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END,
                        非急救放车数 = CASE 
                                        WHEN 事件类型编码 = 1 THEN 1
                                        ELSE 0
                                        END,
                        正常非急救放车数 = CASE 
                                        WHEN 事件类型编码 =1 THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<= 30 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END,
                        迟缓非急救放车数 = CASE
                                        WHEN 事件类型编码 =1 THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >30 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END,
                        转院放车数 = CASE 
                                        WHEN 事件类型编码 =2 THEN 1
                                        ELSE 0
                                    END,
                        正常转院放车数 = CASE 
                                        WHEN 事件类型编码 =2 THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <=5 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END,
                        迟缓转院放车数 = CASE
                                        WHEN 事件类型编码 =2 THEN 
                                            CASE
                                            WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 THEN 1 
                                            ELSE 0
                                            END
                                        ELSE 0
                                        END

                INTO #temp
                FROM #temp0

                UNION ALL
                SELECT 
                时间区段 = CASE DATEPART(hour, tae.首次受理时刻)
                                    WHEN 0 THEN '00:00-01:00'
                                    WHEN 1 THEN '01:00-02:00'
                                    WHEN 2 THEN '02:00-03:00'
                                    WHEN 3 THEN '03:00-04:00'
                                    WHEN 4 THEN '04:00-05:00'
                                    WHEN 5 THEN '05:00-06:00'
                                    WHEN 6 THEN '06:00-07:00'
                                    WHEN 7 THEN '07:00-08:00'
                                    WHEN 8 THEN '08:00-09:00'
                                    WHEN 9 THEN '09:00-10:00'
                                    WHEN 10 THEN '10:00-11:00'
                                    WHEN 11 THEN '11:00-12:00'
                                    WHEN 12 THEN '12:00-13:00'
                                    WHEN 13 THEN '13:00-14:00'
                                    WHEN 14 THEN '14:00-15:00'
                                    WHEN 15 THEN '15:00-16:00'
                                    WHEN 16 THEN '16:00-17:00'
                                    WHEN 17 THEN '17:00-18:00'
                                    WHEN 18 THEN '18:00-19:00'
                                    WHEN 19 THEN '19:00-20:00'
                                    WHEN 20 THEN '20:00-21:00'
                                    WHEN 21 THEN '21:00-22:00'
                                    WHEN 22 THEN '22:00-23:00'
                                    WHEN 23 THEN '23:00-24:00'
                                    END,
                        受理数 = 1,
                        回车数 = 1,
                        实际放车数 = 0,
                        正常放车数 = 0,
                        迟缓放车数 = 0,

                        急救放车数 = 0, 
	                    正常急救放车数 = 0,
                        迟缓急救放车数 = 0,
       
                        非急救放车数 = 0,
                        正常非急救放车数 = 0,

                        转院放车数 = 0,
                        迟缓非急救放车数 = 0,
       
                        正常转院放车数 = 0,
                        迟缓转院放车数 = 0
       
                FROM TAcceptEvent tac 
                join TAlarmEvent tae on tae.事件编码=tac.事件编码
                WHERE  tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <=  '" + endTime + @"'
                and tac.事件编码> '" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"' 
                and 受理类型编码 =4 and tae.是否测试=0

                SELECT  
                        TimeSegment=时间区段, 
                        AcceptedNumber = SUM(受理数),
                        BackNumber = SUM(回车数),
	                    BackPercentage=convert(varchar(6),convert(decimal(10,1),SUM(回车数)*100.00/case when SUM(受理数)>0 then SUM(受理数) else 1 end))+'%',
                        RealSendNumber = SUM(实际放车数),
	                    RealSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(实际放车数)*100.00/case when SUM(受理数)>0 then SUM(受理数) else 1 end))+'%',
                        NormalSendNumber = SUM(正常放车数),
	                    NormalSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        DelayedSendNumber = SUM(迟缓放车数),
	                    DelayedSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',

                        AidSendNumber = SUM(急救放车数),
	                    AidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        NormalAidSendNumber = SUM(正常急救放车数),
	                    NormalAidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常急救放车数)*100.00/case when SUM(急救放车数)>0 then SUM(急救放车数) else 1 end))+'%',
	                    NormalAidSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        DelayedAidSendNumber = SUM(迟缓急救放车数),
	                    DelayedAidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓急救放车数)*100.00/case when SUM(急救放车数)>0 then SUM(急救放车数) else 1 end))+'%',
	                    DelayedAidSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',

                        NotAidSendNumber = SUM(非急救放车数),
	                    NotAidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(非急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        NormalNotAidSendNumber = SUM(正常非急救放车数),
	                    NormalNotAidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常非急救放车数)*100.00/case when SUM(非急救放车数)>0 then SUM(非急救放车数) else 1 end))+'%',
	                    NormalNotAidSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常非急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        DelayedNotAidSendNumber = SUM(迟缓非急救放车数),
	                    DelayedNotAidSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓非急救放车数)*100.00/case when SUM(非急救放车数)>0 then SUM(非急救放车数) else 1 end))+'%',
	                    DelayedNotAidSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓非急救放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',

                        TransferSendNumber = SUM(转院放车数),
	                    TransferSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(转院放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        NormalTransferSendNumber = SUM(正常转院放车数),
	                    NormalTransferSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常转院放车数)*100.00/case when SUM(转院放车数)>0 then SUM(转院放车数) else 1 end))+'%',
	                    NormalTransferSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(正常转院放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%',
                        DelayedTransferSendNumber = SUM(迟缓转院放车数),
	                    DelayedTransferSendPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓转院放车数)*100.00/case when SUM(转院放车数)>0 then SUM(转院放车数) else 1 end))+'%',
	                    DelayedTransferSendTotalPercentage =convert(varchar(6),convert(decimal(10,1),SUM(迟缓转院放车数)*100.00/case when SUM(实际放车数)>0 then SUM(实际放车数) else 1 end))+'%'

                FROM #temp

                GROUP BY 时间区段
                ORDER BY 时间区段

                DROP TABLE #temp0
                DROP TABLE #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_FCQKMX1BT1(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                into #temp0
                FROM TTask tt
                join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                join TAlarmEvent tae on tae.事件编码=tac.事件编码
                WHERE tae.首次受理时刻>=  '" + beginTime + "' AND tae.首次受理时刻 <=  '" + endTime + @"' and tae.是否测试=0
                and tt.任务编码> '" + taskCodeB + "' and tt.任务编码<= '" + taskCodeE + @"'
                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"' 
                group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                SELECT Name='实际放车数',
                       Number = isnull(sum(1),0)     
                FROM #temp0
                union all
                select Name='回车数',
                       Number = isnull(sum(1),0) 
                FROM TAcceptEvent tac 
                join TAlarmEvent tae on tae.事件编码=tac.事件编码
                WHERE  tae.首次受理时刻 >=  '" + beginTime + "' AND tae.首次受理时刻 <=  '" + endTime + @"'
                and tac.事件编码> '" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"' 
                and 受理类型编码=4 and tae.是否测试=0

                drop table #temp0 ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_FCQKMX1BT2(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                    select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                    into #temp0
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE tae.首次受理时刻 >=  '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0
                    and tt.任务编码> '" + taskCodeB + "' and tt.任务编码<= '" + taskCodeE + @"'
                    and tac.事件编码> '" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"'
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    select Name='正常急救放车',
                           Number = isnull(sum(CASE 
                                            WHEN 事件类型编码 in(0,3) THEN 
                                              CASE
                                                WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 5 THEN 1 
                                                ELSE 0
                                              END
                                            ELSE 0
                                          END),0)
                    FROM #temp0
                    union all
                    SELECT Name='迟缓急救放车',

                           Number = isnull(sum(CASE
                                            WHEN 事件类型编码 in(0,3) THEN 
                                              CASE
                                                WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 THEN 1 
                                                ELSE 0
                                              END
                                            ELSE 0
                                          END),0)
                    FROM #temp0
                    union all
                    select Name='正常非急救放车',
                           Number = isnull(sum(CASE 
                                              WHEN 事件类型编码 = 1 THEN 
                                                CASE
                                                  WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 30 THEN 1 
                                                  ELSE 0
                                                END
                                              ELSE 0
                                            END),0)
                    FROM #temp0
                    union all
                    select Name='迟缓非急救放车',
                           Number = isnull(sum(CASE 
                                              WHEN 事件类型编码 = 1 THEN 
                                                CASE
                                                  WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >30 THEN 1 
                                                  ELSE 0
                                                END
                                              ELSE 0
                                            END),0)
                    FROM #temp0
                    union all
                    select Name='正常转院放车',
                           Number = isnull(sum(CASE 
                                              WHEN 事件类型编码 = 2 THEN 
                                                CASE
                                                  WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) <= 5 THEN 1 
                                                  ELSE 0
                                                END
                                              ELSE 0
                                            END),0)
                    FROM #temp0
                    union all
                    select Name='迟缓转院放车',
                           Number = isnull(sum(CASE 
                                              WHEN 事件类型编码 = 2 THEN 
                                                CASE
                                                  WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 THEN 1 
                                                  ELSE 0
                                                END
                                              ELSE 0
                                            END),0)
                    FROM #temp0

                    drop table #temp0");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_FCQKMX1ZX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                    select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                    into #temp0
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE tae.首次受理时刻 >=  '" + beginTime + "' AND tae.首次受理时刻 <=  '" + endTime + @"' and tae.是否测试=0
                    and tt.任务编码> '" + taskCodeB + "' and tt.任务编码<= '" + taskCodeE + @"'
                    and tac.事件编码> '" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"' 
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    select * from
                    (
                    SELECT Name='实际放车数',
                    TimeSegment = CASE DATEPART(hour, 首次受理时刻)
                                        WHEN 0 THEN '00:00-01:00'
                                        WHEN 1 THEN '01:00-02:00'
                                        WHEN 2 THEN '02:00-03:00'
                                        WHEN 3 THEN '03:00-04:00'
                                        WHEN 4 THEN '04:00-05:00'
                                        WHEN 5 THEN '05:00-06:00'
                                        WHEN 6 THEN '06:00-07:00'
                                        WHEN 7 THEN '07:00-08:00'
                                        WHEN 8 THEN '08:00-09:00'
                                        WHEN 9 THEN '09:00-10:00'
                                        WHEN 10 THEN '10:00-11:00'
                                        WHEN 11 THEN '11:00-12:00'
                                        WHEN 12 THEN '12:00-13:00'
                                        WHEN 13 THEN '13:00-14:00'
                                        WHEN 14 THEN '14:00-15:00'
                                        WHEN 15 THEN '15:00-16:00'
                                        WHEN 16 THEN '16:00-17:00'
                                        WHEN 17 THEN '17:00-18:00'
                                        WHEN 18 THEN '18:00-19:00'
                                        WHEN 19 THEN '19:00-20:00'
                                        WHEN 20 THEN '20:00-21:00'
                                        WHEN 21 THEN '21:00-22:00'
                                        WHEN 22 THEN '22:00-23:00'
                                        WHEN 23 THEN '23:00-24:00'
                                        END,
                            Number = isnull(sum(1),0)     
                    FROM #temp0
                    GROUP BY DATEPART(hour, 首次受理时刻)

                    union all

                    select Name='回车数',
                    TimeSegment = CASE DATEPART(hour, tae.首次受理时刻)
                                        WHEN 0 THEN '00:00-01:00'
                                        WHEN 1 THEN '01:00-02:00'
                                        WHEN 2 THEN '02:00-03:00'
                                        WHEN 3 THEN '03:00-04:00'
                                        WHEN 4 THEN '04:00-05:00'
                                        WHEN 5 THEN '05:00-06:00'
                                        WHEN 6 THEN '06:00-07:00'
                                        WHEN 7 THEN '07:00-08:00'
                                        WHEN 8 THEN '08:00-09:00'
                                        WHEN 9 THEN '09:00-10:00'
                                        WHEN 10 THEN '10:00-11:00'
                                        WHEN 11 THEN '11:00-12:00'
                                        WHEN 12 THEN '12:00-13:00'
                                        WHEN 13 THEN '13:00-14:00'
                                        WHEN 14 THEN '14:00-15:00'
                                        WHEN 15 THEN '15:00-16:00'
                                        WHEN 16 THEN '16:00-17:00'
                                        WHEN 17 THEN '17:00-18:00'
                                        WHEN 18 THEN '18:00-19:00'
                                        WHEN 19 THEN '19:00-20:00'
                                        WHEN 20 THEN '20:00-21:00'
                                        WHEN 21 THEN '21:00-22:00'
                                        WHEN 22 THEN '22:00-23:00'
                                        WHEN 23 THEN '23:00-24:00'
                                        END,
                            Number = isnull(sum(1),0) 
                    FROM TAcceptEvent tac 
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE  tae.首次受理时刻 >=  '" + beginTime + "' AND tae.首次受理时刻<= '" + endTime + @"' and 受理类型编码 in (4,6) and tae.是否测试=0
                    and tac.事件编码> '" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"' 
                    group by DATEPART(hour, 首次受理时刻)
                    ) t
                    order by Name,TimeSegment

                    drop table #temp0");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 放车与区域时间关系统计表
        /// <summary>
        /// 放车与区域时间关系统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FCYQYSJGX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    set nocount on
                    select 名称  into #temp from dbo.TZArea where 是否有效=1 order by 顺序号

                    select 
                    区域名称=case when tae.区域 like '崇明%' then '崇明区' else tae.区域 end,
                    a = sum(case when datename(hour,首次受理时刻) >=0 and datename(hour,首次受理时刻) <2 then 1 else 0 end),
                    b = sum(case when datename(hour,首次受理时刻) >=2 and datename(hour,首次受理时刻) < 4 then 1 else 0 end),
                    c = sum(case when datename(hour,首次受理时刻) >=4 and datename(hour,首次受理时刻) < 6 then 1 else 0 end),
                    d = sum(case when datename(hour,首次受理时刻) >=6 and datename(hour,首次受理时刻) < 8 then 1 else 0 end),
                    e = sum(case when datename(hour,首次受理时刻) >=8 and datename(hour,首次受理时刻) < 10 then 1 else 0 end),
                    f = sum(case when datename(hour,首次受理时刻) >=10 and datename(hour,首次受理时刻) < 12 then 1 else 0 end),
                    g = sum(case when datename(hour,首次受理时刻) >=12 and datename(hour,首次受理时刻) < 14 then 1 else 0 end),
                    h = sum(case when datename(hour,首次受理时刻) >=14 and datename(hour,首次受理时刻) < 16 then 1 else 0 end),
                    i = sum(case when datename(hour,首次受理时刻) >=16 and datename(hour,首次受理时刻) < 18 then 1 else 0 end),
                    j = sum(case when datename(hour,首次受理时刻) >=18 and datename(hour,首次受理时刻) < 20 then 1 else 0 end),
                    k = sum(case when datename(hour,首次受理时刻) >=20 and datename(hour,首次受理时刻) < 22 then 1 else 0 end),
                    l = sum(case when datename(hour,首次受理时刻) >=22 and datename(hour,首次受理时刻) < 24 then 1 else 0 end),
                    总数=count(*)
                    into #temp1
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE tae.首次受理时刻 >= '" + beginTime.ToString() + "' AND tae.首次受理时刻 <= '" + endTime.ToString() + @"' and tae.是否测试=0
                    --and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    --and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"'

                    group by tae.区域

                    select AreaName=t.名称,
                    ID00_00_01_59=a,
                    ID02_00_03_59=b,
                    ID04_00_05_59=c ,
                    ID06_00_07_59=d ,
                    ID08_00_09_59=e ,
                    ID10_00_11_59=f,
                    ID12_00_13_59=g ,
                    ID14_00_15_59=h ,
                    ID16_00_17_59=i,
                    ID18_00_19_59=j ,
                    ID20_00_21_59=k ,
                    ID22_00_23_59=l,
                    Total=总数
                    from #temp t left join #temp1 t1 on t.名称=t1.区域名称

                    drop table #temp
                    drop table #temp1 
                    set nocount off 
                    ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 分中心上下班问题统计表
        /// <summary>
        /// 分中心上下班问题统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FZXSXBWT(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    Center=tc.名称,
                    IrregularWork=sum(case when 操作来源编码 not in (0,2,4) then 1 else 0 end)
                    from TAmbulancePersonSign taps
                    left join TPerson tp on tp.编码=taps.人员编码
                    left join TCenter tc on tc.编码=tp.中心编码
                    where 操作时刻>='" + beginTime.ToString() + "' and 操作时刻<='" + endTime.ToString() + @"'
                    group by tc.名称,tc.顺序号
                    order by tc.顺序号");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 分中心暂停调用统计
        /// <summary>
        /// 分中心暂停调用统计
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FZXZTDY(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    Center=tc.名称,
                    PauseReason=tzpa.名称,
                    PauseTimes=sum(1)
                    from TPauseRecord tpa
                    left join TZPauseReason tzpa on tzpa.编码=tpa.暂停原因编码
                    left join TAmbulance tam on tam.车辆编码=tpa.车辆编码
                    left join TCenter tc on tc.编码=tam.中心编码
                    where 操作时刻>='" + beginTime + "' and 操作时刻<='" + endTime + @"'
                    and 是否暂停操作=1
                    group by tc.名称,tzpa.名称,tc.顺序号,tzpa.顺序号
                    order by tc.顺序号,tzpa.顺序号");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 呼救病种情况统计表
        /// <summary>
        /// 呼救病种情况统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_HJBZQK(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select 
                疾病种类= case when tae.主诉 not in (select 名称 from TZMedicalJudge where 是否有效=1) then '其他' else tae.主诉 end,
                总人数=伤亡人数,
                男=case when tae.性别='男' then 1 else 0 end,
                女=case when tae.性别='女' then 1 else 0 end,
                性别不详=case when tae.性别='不详' then 1 else 0 end,
                老年=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>=66 ) or tae.年龄='老年' then 1 else 0 end,
                中年=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='41' and cast(tae.年龄 as float)<=65) or tae.年龄='中年' then 1 else 0 end,
                青年=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='18' and cast(tae.年龄 as float)<=40) or tae.年龄='青年' then 1 else 0 end,
                少儿=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='7' and cast(tae.年龄 as float)<=17) or tae.年龄='少儿' then 1 else 0 end,
                婴儿=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>=0 and cast(tae.年龄 as float)<=6) or tae.年龄='婴儿' then 1 else 0 end,
                年龄不详=case when (isnumeric(tae.年龄)=0 and tae.年龄!='老年' and tae.年龄!='中年' and tae.年龄!='青年' and tae.年龄!='少儿' and tae.年龄!='婴儿') or tae.年龄='不详' then 1 else 0 end
                into #temp
                from TAlarmEvent tae
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and 是否测试=0
                and tae.事件编码>'" + eventCodeB + "' and tae.事件编码<='" + eventCodeE + @"'
                and tae.事件类型编码 in(0,1,2)
                and tae.年龄 not like '%+%'

                select
                DiseasesType=疾病种类, 
                TotalPersonNumber=sum(总人数),
                Male=sum(男),
                Female=sum(女),
                UnknownSex=sum(性别不详),
                OldAge=sum(老年),
                MiddleAge=sum(中年),
                YoungAge=sum(青年),
                JuniorAge=sum(少儿),
                BabyAge=sum(婴儿),
                UnknownAge=sum(年龄不详)
                from #temp group by 疾病种类

                drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_HJBZQK_XBYJB(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select 
                疾病种类= case when tae.主诉 not in (select 名称 from TZMedicalJudge where 是否有效=1) then '其他' else tae.主诉 end,
                性别
                into #temp
                from TAlarmEvent tae
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and 是否测试=0
                and tae.事件类型编码 in(0,1,2)

                select
                DiseasesType=疾病种类, 
                Sex=性别,
                Number=count(*)
                from #temp group by 疾病种类,性别
                drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_HJBZQK_NLYJB(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select 
                疾病种类= case when tae.主诉 not in (select 名称 from TZMedicalJudge where 是否有效=1) then '其他' else tae.主诉 end,
                年龄=case when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>=66 ) or tae.年龄='老年' then '老年'
                          when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='41' and cast(tae.年龄 as float)<=65) or tae.年龄='中年' then '中年'
                          when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='18' and cast(tae.年龄 as float)<=40) or tae.年龄='青年' then '青年'
                          when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>='7' and cast(tae.年龄 as float)<=17) or tae.年龄='少儿' then '少儿'
                          when (isnumeric(tae.年龄)=1 and cast(tae.年龄 as float)>0 and cast(tae.年龄 as float)<=6) or tae.年龄='婴儿' then '婴儿' 
                          when (isnumeric(tae.年龄)=0 and tae.年龄!='老年' and tae.年龄!='中年' and tae.年龄!='青年' and tae.年龄!='少儿' and tae.年龄!='婴儿') or tae.年龄='不详' then '年龄不详' end

                into #temp
                from TAlarmEvent tae
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and 是否测试=0
                and tae.事件类型编码 in(0,1,2)
                and tae.年龄 not like '%+%'

                select
                DiseasesType=疾病种类, 
                Age=年龄,
                Number=count(*)
                from #temp group by 疾病种类,年龄
                drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 呼救电话排队峰值统计表
        /// <summary>
        /// 呼救电话排队峰值统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_HJDHPDFZ(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    SELECT 
                    AlarmTime = CASE DATEPART(hour, 来电时刻)
                                       WHEN 0 THEN '00:00-01:00'
                                       WHEN 1 THEN '01:00-02:00'
                                       WHEN 2 THEN '02:00-03:00'
                                       WHEN 3 THEN '03:00-04:00'
                                       WHEN 4 THEN '04:00-05:00'
                                       WHEN 5 THEN '05:00-06:00'
                                       WHEN 6 THEN '06:00-07:00'
                                       WHEN 7 THEN '07:00-08:00'
                                       WHEN 8 THEN '08:00-09:00'
                                       WHEN 9 THEN '09:00-10:00'
                                       WHEN 10 THEN '10:00-11:00'
                                       WHEN 11 THEN '11:00-12:00'
                                       WHEN 12 THEN '12:00-13:00'
                                       WHEN 13 THEN '13:00-14:00'
                                       WHEN 14 THEN '14:00-15:00'
                                       WHEN 15 THEN '15:00-16:00'
                                       WHEN 16 THEN '16:00-17:00'
                                       WHEN 17 THEN '17:00-18:00'
                                       WHEN 18 THEN '18:00-19:00'
                                       WHEN 19 THEN '19:00-20:00'
                                       WHEN 20 THEN '20:00-21:00'
                                       WHEN 21 THEN '21:00-22:00'
                                       WHEN 22 THEN '22:00-23:00'
                                       WHEN 23 THEN '23:00-24:00'
                                     END,
                           ListNumber = isnull(max(排队数量),0)     
                    from dbo.TTelRecord
                    where 来电时刻 >= '" + beginTime + "' and 来电时刻 <='" + endTime + @"' 
                    and 排队数量 >1 
                    GROUP BY DATEPART(hour, 来电时刻)");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 回车与区域时间关系统计表
        /// <summary>
        /// 回车与区域时间关系统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_HCYQYSJGX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    set nocount on
                    select 名称  into #temp from dbo.TZArea where 是否有效=1 order by 顺序号
                    insert into #temp values('未选择')

                    select 
                    区域名称=case when tae.区域 ='' then '未选择' when tae.区域 like '崇明%' then '崇明区' else tae.区域 end,
                    a = sum(case when datename(hour,首次受理时刻) >=0 and datename(hour,首次受理时刻) <2 and tae.事件类型编码=0 then 1 else 0 end),
                    b = sum(case when datename(hour,首次受理时刻) >=0 and datename(hour,首次受理时刻) <2 and tae.事件类型编码=1 then 1 else 0 end),
                    c = sum(case when datename(hour,首次受理时刻) >=0 and datename(hour,首次受理时刻) <2and tae.事件类型编码=2 then 1 else 0 end),

                    d = sum(case when datename(hour,首次受理时刻) >=2 and datename(hour,首次受理时刻) < 4 and tae.事件类型编码=0 then 1 else 0 end),
                    e = sum(case when datename(hour,首次受理时刻) >=2 and datename(hour,首次受理时刻) < 4 and tae.事件类型编码=1 then 1 else 0 end),
                    f = sum(case when datename(hour,首次受理时刻) >=2 and datename(hour,首次受理时刻) < 4 and tae.事件类型编码=2 then 1 else 0 end),

                    g = sum(case when datename(hour,首次受理时刻) >=4 and datename(hour,首次受理时刻) < 6 and tae.事件类型编码=0 then 1 else 0 end),
                    h = sum(case when datename(hour,首次受理时刻) >=4 and datename(hour,首次受理时刻) < 6 and tae.事件类型编码=1 then 1 else 0 end),
                    i = sum(case when datename(hour,首次受理时刻) >=4 and datename(hour,首次受理时刻) < 6 and tae.事件类型编码=2 then 1 else 0 end),

                    j = sum(case when datename(hour,首次受理时刻) >=6 and datename(hour,首次受理时刻)< 8 and tae.事件类型编码=0 then 1 else 0 end),
                    k = sum(case when datename(hour,首次受理时刻) >=6 and datename(hour,首次受理时刻) < 8 and tae.事件类型编码=1 then 1 else 0 end),
                    l = sum(case when datename(hour,首次受理时刻) >=6 and datename(hour,首次受理时刻) < 8 and tae.事件类型编码=2 then 1 else 0 end),

                    m = sum(case when datename(hour,首次受理时刻) >=8 and datename(hour,首次受理时刻) < 10 and tae.事件类型编码=0 then 1 else 0 end),
                    n = sum(case when datename(hour,首次受理时刻) >=8 and datename(hour,首次受理时刻) < 10 and tae.事件类型编码=1 then 1 else 0 end),
                    o = sum(case when datename(hour,首次受理时刻) >=8 and datename(hour,首次受理时刻) < 10 and tae.事件类型编码=2 then 1 else 0 end),

                    p = sum(case when datename(hour,首次受理时刻) >=10 and datename(hour,首次受理时刻) < 12 and tae.事件类型编码=0 then 1 else 0 end),
                    q = sum(case when datename(hour,首次受理时刻) >=10 and datename(hour,首次受理时刻) < 12 and tae.事件类型编码=1 then 1 else 0 end),
                    r = sum(case when datename(hour,首次受理时刻) >=10 and datename(hour,首次受理时刻) < 12 and tae.事件类型编码=2 then 1 else 0 end),

                    s = sum(case when datename(hour,首次受理时刻) >=12 and datename(hour,首次受理时刻) < 14 and tae.事件类型编码=0 then 1 else 0 end),
                    t = sum(case when datename(hour,首次受理时刻) >=12 and datename(hour,首次受理时刻) < 14 and tae.事件类型编码=1 then 1 else 0 end),
                    u = sum(case when datename(hour,首次受理时刻) >=12 and datename(hour,首次受理时刻) < 14 and tae.事件类型编码=2 then 1 else 0 end),

                    v = sum(case when datename(hour,首次受理时刻) >=14 and datename(hour,首次受理时刻) < 16 and tae.事件类型编码=0 then 1 else 0 end),
                    w = sum(case when datename(hour,首次受理时刻) >=14 and datename(hour,首次受理时刻) < 16 and tae.事件类型编码=1 then 1 else 0 end),
                    x = sum(case when datename(hour,首次受理时刻) >=14 and datename(hour,首次受理时刻) < 16 and tae.事件类型编码=2 then 1 else 0 end),

                    y = sum(case when datename(hour,首次受理时刻) >=16 and datename(hour,首次受理时刻) < 18 and tae.事件类型编码=0 then 1 else 0 end),
                    z = sum(case when datename(hour,首次受理时刻) >=16 and datename(hour,首次受理时刻) < 18 and tae.事件类型编码=1 then 1 else 0 end),
                    a1 = sum(case when datename(hour,首次受理时刻) >=16 and datename(hour,首次受理时刻) < 18 and tae.事件类型编码=2 then 1 else 0 end),

                    b1 = sum(case when datename(hour,首次受理时刻) >=18 and datename(hour,首次受理时刻) < 20 and tae.事件类型编码=0 then 1 else 0 end),
                    c1 = sum(case when datename(hour,首次受理时刻) >=18 and datename(hour,首次受理时刻) < 20 and tae.事件类型编码=1 then 1 else 0 end),
                    d1 = sum(case when datename(hour,首次受理时刻) >=18 and datename(hour,首次受理时刻) < 20 and tae.事件类型编码=2 then 1 else 0 end),

                    e1 = sum(case when datename(hour,首次受理时刻) >=20 and datename(hour,首次受理时刻) < 22 and tae.事件类型编码=0 then 1 else 0 end),
                    f1 = sum(case when datename(hour,首次受理时刻) >=20 and datename(hour,首次受理时刻) < 22 and tae.事件类型编码=1 then 1 else 0 end),
                    g1 = sum(case when datename(hour,首次受理时刻) >=20 and datename(hour,首次受理时刻) < 22 and tae.事件类型编码=2 then 1 else 0 end),

                    h1 = sum(case when datename(hour,首次受理时刻) >=22 and datename(hour,首次受理时刻) < 24 and tae.事件类型编码=0 then 1 else 0 end),
                    i1 = sum(case when datename(hour,首次受理时刻) >=22 and datename(hour,首次受理时刻) < 24 and tae.事件类型编码=1 then 1 else 0 end),
                    j1 = sum(case when datename(hour,首次受理时刻) >=22 and datename(hour,首次受理时刻) < 24 and tae.事件类型编码=2 then 1 else 0 end),
                    总数=count(*)
                    into #temp1
                    from  TAcceptEvent tac
                         inner join TAlarmEvent tae on tac.事件编码=tae.事件编码
                    where tae.首次受理时刻 >= '" + beginTime + "' and tae.首次受理时刻<='" + endTime + @"' and tae.是否测试 = 0
                          and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<= '" + eventCodeE + @"'
                          and tac.受理类型编码 = 4
                    group by tae.区域

                    select AreaName=t.名称,
                    Aid00_00_01_59=a,
                    NotAid00_00_01_59=b,
                    Transfer00_00_01_59=c,

                    Aid02_00_03_59=d,
                    NotAid02_00_03_59=e,
                    Transfer02_00_03_59=f,

                    Aid04_00_05_59=g ,
                    NotAid04_00_05_59=h,
                    Transfer04_00_05_59=i ,

                    Aid06_00_07_59=j,
                    NotAid06_00_07_59=k,
                    Transfer06_00_07_59=l ,

                    Aid08_00_09_59=m,
                    NotAid08_00_09_59=n,
                    Transfer08_00_09_59=o ,

                    Aid10_00_11_59=p,
                    NotAid10_00_11_59=q ,
                    Transfer10_00_11_59=r ,

                    Aid12_00_13_59=s,
                    NotAid12_00_13_59=t ,
                    Transfer12_00_13_59=u,

                    Aid14_00_15_59=v,
                    NotAid14_00_15_59=w ,
                    Transfer14_00_15_59=x,

                    Aid16_00_17_59=y,
                    NotAid16_00_17_59=z,
                    Transfer16_00_17_59=a1 ,

                    Aid18_00_19_59=b1 ,
                    NotAid18_00_19_59=c1,
                    Transfer18_00_19_59=d1 ,

                    Aid20_00_21_59=e1 ,
                    NotAid20_00_21_59=f1 ,
                    Transfer20_00_21_59=g1 ,

                    Aid22_00_23_59=h1 ,
                    NotAid22_00_23_59=i1 ,
                    Transfer22_00_23_59=j1,
                    Total=总数
                    from #temp t left join #temp1 t1 on t.名称=t1.区域名称

                    drop table #temp
                    drop table #temp1");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 急救反应时间(日报)统计表
        /// <summary>
        /// 急救反应时间（日报）统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_JJFYSJRB(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    SET NOCOUNT ON

                    SELECT  tt.任务编码,tae.事件编码, tae.事件类型编码, tae.首次受理时刻, 
                    MIN(tac.发送指令时刻) AS 发送指令时刻, MIN(tt.到达现场时刻) AS 到达现场时刻
                    into #temp0
                    FROM TTask tt
                    inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    left join TStation ts on tt.分站编码=ts.编码
                    WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"'
                        and ttt.车辆状态编码=3 and 操作来源编码=2 
                        and ts.中心编码<>1--平均值不计算中心的
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    SELECT 
                    TotalResponseTime = AVG(CASE WHEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
                                    END),
                    AidResponseTime = AVG(CASE WHEN (事件类型编码 IN (0, 3)) AND (DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    NotAidResponseTime = AVG(CASE WHEN (事件类型编码 = 1) AND (DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
                                        THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    TransferResponseTime=AVG(CASE WHEN (事件类型编码 = 2) AND (DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
				                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),

                    NormalSendResponseTime = AVG(CASE WHEN (((事件类型编码 IN (0, 2, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 300) OR
                                        (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 1800)) AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
                                        THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    NormalSendAidResponseTime = AVG(CASE WHEN (事件类型编码 IN (0, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻)<= 300 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0)
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
                                        END),
                    NormalSendNotAidResponseTime = AVG(CASE WHEN (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 1800 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    NormalSendTransferResponseTime=AVG(CASE WHEN (事件类型编码 = 2 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 300 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
                                        THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),


                    DelayedSendResponseTime = AVG(CASE WHEN (((事件类型编码 IN (0, 2, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 300) OR
                                        (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 1800)) AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
                                        THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    DelayedSendAidResponseTime = AVG(CASE WHEN (事件类型编码 IN (0, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 300 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻)
                                    END ),
                    DelayedSendNotAidResponseTime = AVG(CASE WHEN (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻)> 1800 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
				                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END),
                    DelayedSendTransferResponseTime=AVG(CASE WHEN (事件类型编码 = 2 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 300 AND DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0) 
				                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) 
				                    END)

                    FROM #temp0
                    where 到达现场时刻 IS NOT NULL

                    drop table #temp0


                    SET NOCOUNT OFF");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_JJFYSJRB1(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    SELECT  TaskCode=tt.任务编码,AlarmEventCode=tae.事件编码, AlarmEventType=tae.事件类型编码, FirstAcceptTime=tae.首次受理时刻, 
                    SendMessageTime=MIN(tac.发送指令时刻),ArriveSceneTime= MIN(tt.到达现场时刻) 
                    into #temp
                    FROM TTask tt
                    inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    left join TStation ts on tt.分站编码=ts.编码
                    WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0 
                    and ttt.车辆状态编码=3 and 操作来源编码=2 
                    and ts.中心编码<>1
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    select * from #temp
                    where ArriveSceneTime is not null
                    drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_JJFYSJRB2(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    SELECT  TaskCode=tt.任务编码,AlarmEventCode=tae.事件编码, AlarmEventType=tae.事件类型编码,FirstAcceptTime= tae.首次受理时刻, 
                    SendMessageTime=MIN(tac.发送指令时刻),ArriveSceneTime= MIN(tt.到达现场时刻)
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    left join TStation ts on tt.分站编码=ts.编码
                    WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0
                    and ts.中心编码<>1
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_JJFYSJRB_CenterTime(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                   SELECT  tt.任务编码,tae.事件编码,tt.受理序号, tae.事件类型编码, tae.首次受理时刻, 
                    MIN(tac.发送指令时刻) AS 发送指令时刻, MIN(tt.到达现场时刻) AS 到达现场时刻,ts.名称 as 分站,tc.名称 as 分中心
                    into #temp0
                    FROM TTask tt
                    inner join dbo.TTaskTime ttt on tt.任务编码=ttt.任务编码
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    left join TStation ts on tt.分站编码=ts.编码
                    left join TCenter tc on ts.中心编码=tc.编码
                    WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0 
                    and ttt.车辆状态编码=3 and 操作来源编码=2 and ts.中心编码<>1
                    group by tt.任务编码,tae.事件编码,tt.受理序号,tae.事件类型编码,tae.首次受理时刻,ts.名称,tc.名称
                    order by tt.任务编码

                    SELECT Center=分中心,
                    TotalResponseTime = avg(DATEDIFF(second, 首次受理时刻, 到达现场时刻)),

                    AidResponseTime = avg(CASE WHEN 事件类型编码 IN (0, 3) THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    NotAidResponseTime = avg(CASE WHEN 事件类型编码 = 1 THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    TransferResponseTime =avg(CASE WHEN 事件类型编码 = 2 THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    NormalSendResponseTime = avg(CASE WHEN ((事件类型编码 IN (0, 2, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 300) OR
                                      (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 1800))  
                                      THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    NormalSendAidResponseTime = avg(CASE WHEN (事件类型编码 IN (0, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 300 )
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    NormalSendNotAidResponseTime = avg(CASE WHEN (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 1800) 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    NormalSendTransferResponseTime=avg(CASE WHEN (事件类型编码 = 2 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) <= 300 ) 
                                      THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    DelayedSendResponseTime = avg(CASE WHEN ((事件类型编码 IN (0, 2, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) >300) OR
                                      (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 1800))  
                                      THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    DelayedSendAidResponseTime = avg(CASE WHEN (事件类型编码 IN (0, 3) AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 300 ) 
					                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻)END ),

                    DelayedSendNotAidResponseTime = avg(CASE WHEN (事件类型编码 = 1 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 1800 ) 
				                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END),

                    DelayedSendTransferResponseTime=avg(CASE WHEN (事件类型编码 = 2 AND DATEDIFF(second, 首次受理时刻, 发送指令时刻) > 300 ) 
				                    THEN DATEDIFF(second, 首次受理时刻, 到达现场时刻) END)
                    FROM #temp0
                    where 到达现场时刻 IS NOT NULL
                    and DATEDIFF(second, 首次受理时刻, 到达现场时刻) > 0
                    group by 分中心

                    drop table #temp0");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 疾病与时间关系统计表
        /// <summary>
        /// 疾病与时间关系统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_JBYSJGX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    疾病种类= case when tae.主诉 not in (select 名称 from TZMedicalJudge where 是否有效=1) then '其他' else tae.主诉 end,
                    总人数=伤亡人数,
                    人数_00=case WHEN DATEPART(hour, 首次受理时刻) =0 or DATEPART(hour, 首次受理时刻) =1 THEN 1 else 0 end,
                    人数_02=case WHEN DATEPART(hour, 首次受理时刻) =2 or DATEPART(hour, 首次受理时刻) =3 THEN 1 else 0 end,
                    人数_04=case WHEN DATEPART(hour, 首次受理时刻) =4 or DATEPART(hour, 首次受理时刻) =5 THEN 1 else 0 end,
                    人数_06=case WHEN DATEPART(hour, 首次受理时刻) =6 or DATEPART(hour, 首次受理时刻) =7 THEN 1 else 0 end,
                    人数_08=case WHEN DATEPART(hour, 首次受理时刻) =8 or DATEPART(hour, 首次受理时刻) =9 THEN 1 else 0 end,
                    人数_10=case WHEN DATEPART(hour, 首次受理时刻) =10 or DATEPART(hour, 首次受理时刻) =11 THEN 1 else 0 end,
                    人数_12=case WHEN DATEPART(hour, 首次受理时刻) =12 or DATEPART(hour, 首次受理时刻) =13 THEN 1 else 0 end,
                    人数_14=case WHEN DATEPART(hour, 首次受理时刻) =14 or DATEPART(hour, 首次受理时刻) =15 THEN 1 else 0 end,
                    人数_16=case WHEN DATEPART(hour, 首次受理时刻) =16 or DATEPART(hour, 首次受理时刻) =17 THEN 1 else 0 end,
                    人数_18=case WHEN DATEPART(hour, 首次受理时刻) =18 or DATEPART(hour, 首次受理时刻) =19 THEN 1 else 0 end,
                    人数_20=case WHEN DATEPART(hour, 首次受理时刻) =20 or DATEPART(hour, 首次受理时刻) =21 THEN 1 else 0 end,
                    人数_22=case WHEN DATEPART(hour, 首次受理时刻) =22 or DATEPART(hour, 首次受理时刻) =23 THEN 1 else 0 end
                    into #temp
                    from TAlarmEvent tae
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0
                    and tae.事件编码>'" + eventCodeB + "' and tae.事件编码<='" + eventCodeE + @"'

                    select
                    DiseasesType=疾病种类, 
                    Total=sum(总人数),
                    N_00=sum(人数_00),
                    N_02=sum(人数_02),
                    N_04=sum(人数_04),
                    N_06=sum(人数_06),
                    N_08=sum(人数_08),
                    N_10=sum(人数_10),
                    N_12=sum(人数_12),
                    N_14=sum(人数_14),
                    N_16=sum(人数_16),
                    N_18=sum(人数_18),
                    N_20=sum(人数_20),
                    N_22=sum(人数_22)
                    from #temp group by 疾病种类

                    drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 人均日出车次数分布统计表
        /// <summary>
        /// 人均日出车次数分布统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_RJRCCCSFB(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                   --已下班
                    select 人员编码,上班时间=上次操作时刻,下班时间=操作时刻 
                    into #temp1
                    from dbo.TAmbulancePersonSign
                    where 人员类型编码=3 and 
                    上次操作时刻>='" + beginTime + "' and 上次操作时刻<='" + endTime + @"'
                    and 是否上班操作=0
                    --未下班
                    select A.人员编码,上班时间=最后操作时刻
                    ,下班时间=(select 生成任务时刻 from dbo.TTask 
                    where 任务编码=(select max(任务编码) from dbo.TTaskPersonLink where 人员编码=A.人员编码)
                    and 生成任务时刻>最后操作时刻)
                    into #temp2
                    from (select 人员编码,最后操作时刻=max(操作时刻) from dbo.TAmbulancePersonSign where 人员类型编码=3 group by 人员编码 ) A
                    left join TAmbulancePersonSign B on A.人员编码=B.人员编码 and A.最后操作时刻=B.操作时刻
                    where 是否上班操作=1 and B.操作时刻>='" + beginTime + "' and B.操作时刻<='" + endTime + @"'

                    select 人员编码,上班时间,下班时间 into #temp3 from #temp1 union (select 人员编码,上班时间,下班时间 from #temp2)

                    select 人员编码,count(*) as 当班数 
                    into #temp4
                    from (select distinct 人员编码, convert(varchar(10),上班时间,112) as 上班日期 from #temp3
                    where datediff(hh,上班时间,下班时间)>2 ) A
                    group by A.人员编码

                    select 人员编码,出车次数=count(distinct ttpl.任务编码),
                    起始时间=min(生成任务时刻),结束时间=max(生成任务时刻)
                    into #temp5
                    from dbo.TTaskPersonLink ttpl
                    left join TTask tt on tt.任务编码=ttpl.任务编码
                    where 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻 <= '" + endTime + @"'
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tt.出车时刻 is not null --2010-6-3
                    and 人员类型编码=3
                    group by 人员编码

                    --select 司机,出车次数,天数=case when datediff(day,起始时间,结束时间)=0 then 1 else datediff(day,起始时间,结束时间) end
                    --into #temp2
                    --from #temp

                    select 司机=tp.工号,日出车次数=cast(convert(float,(100.00*出车次数/t4.当班数/100)) as decimal(10,2))
                    into #temp6
                    from #temp5  t5
                    left join #temp4 t4 on t4.人员编码=t5.人员编码
                    left join TPerson tp on tp.编码=t5.人员编码

                    select 司机,[0]=case when 日出车次数=0 then 1 else 0 end
                    ,[1-2]=case when 日出车次数>=1 and 日出车次数<=2 then 1 else 0 end
                    ,[3-4]=case when 日出车次数>=3 and 日出车次数<=4  then 1 else 0 end
                    ,[5-6]=case when 日出车次数>=5 and 日出车次数<=6  then 1 else 0 end
                    ,[7-8]=case when 日出车次数>=7 and 日出车次数<=8  then 1 else 0 end
                    ,[9-10]=case when 日出车次数>=9 and 日出车次数<=10  then 1 else 0 end
                    ,[11-12]=case when 日出车次数>=11 and 日出车次数<=12  then 1 else 0 end
                    ,[13-以上]=case when 日出车次数>=13   then 1 else 0 end
                    into #temp7
                    from #temp6

                    declare @chuchecishu table(AVGSendTime varchar(20),PersonNumber int,DriverWorkNumberList varchar(2000))
                    declare @renshu1 int set @renshu1=0
                    declare @gonghao1 varchar(2000) set @gonghao1=''
                    declare @renshu2 int set @renshu2=0
                    declare @gonghao2 varchar(2000) set @gonghao2=''
                    declare @renshu3 int set @renshu3=0
                    declare @gonghao3 varchar(2000) set @gonghao3=''
                    declare @renshu4 int set @renshu4=0
                    declare @gonghao4 varchar(2000) set @gonghao4=''
                    declare @renshu5 int set @renshu5=0
                    declare @gonghao5 varchar(2000) set @gonghao5=''
                    declare @renshu6 int set @renshu6=0
                    declare @gonghao6 varchar(2000) set @gonghao6=''
                    declare @renshu7 int set @renshu7=0
                    declare @gonghao7 varchar(2000) set @gonghao7=''
                    declare @renshu8 int set @renshu8=0
                    declare @gonghao8 varchar(2000) set @gonghao8=''

                    select @renshu1=sum([0]) from #temp7 
                    select @gonghao1=@gonghao1+','+司机 from #temp7  where [0]>0 group by 司机
                    insert into @chuchecishu values('0',@renshu1,Stuff(@gonghao1,1,1,''))

                    select @renshu2=sum([1-2]) from #temp7 
                    select @gonghao2=@gonghao2+','+司机 from #temp7  where [1-2]>0 group by 司机
                    insert into @chuchecishu values('1-2',@renshu2,Stuff(@gonghao2,1,1,''))

                    select @renshu3=sum([3-4]) from #temp7 
                    select @gonghao3=@gonghao3+','+司机 from #temp7  where [3-4]>0 group by 司机
                    insert into @chuchecishu values('3-4',@renshu3,Stuff(@gonghao3,1,1,''))

                    select @renshu4=sum([5-6]) from #temp7 
                    select @gonghao4=@gonghao4+','+司机 from #temp7  where [5-6]>0 group by 司机
                    insert into @chuchecishu values('5-6',@renshu4,Stuff(@gonghao4,1,1,''))

                    select @renshu5=sum([7-8]) from #temp7 
                    select @gonghao5=@gonghao5+','+司机 from #temp7  where [7-8]>0 group by 司机
                    insert into @chuchecishu values('7-8',@renshu5,Stuff(@gonghao5,1,1,''))

                    select @renshu6=sum([9-10]) from #temp7 
                    select @gonghao6=@gonghao6+','+司机 from #temp7  where [9-10]>0 group by 司机
                    insert into @chuchecishu values('9-10',@renshu6,Stuff(@gonghao6,1,1,''))

                    select @renshu7=sum([11-12]) from #temp7 
                    select @gonghao7=@gonghao7+','+司机 from #temp7  where [11-12]>0 group by 司机
                    insert into @chuchecishu values('11-12',@renshu7,Stuff(@gonghao7,1,1,''))

                    select @renshu8=sum([13-以上]) from #temp7 
                    select @gonghao8=@gonghao8+','+司机 from #temp7  where [13-以上]>0 group by 司机
                    insert into @chuchecishu values('13-以上',@renshu8,Stuff(@gonghao8,1,1,''))

                    select * from @chuchecishu

                    drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp4
                    drop table #temp5
                    drop table #temp6
                    drop table #temp7");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 送往地点类型统计表
        /// <summary>
        /// 送往地点类型统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_SWDDLX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    送往地点类型=case 事件类型编码 when 1 then '家庭' else '医院' end,
                    tae.伤亡人数
                    into #temp
                    from TAlarmEvent tae
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0 and 事件类型编码 not in (5,6)

                    and 事件编码>'" + eventCodeB + "' and 事件编码<='" + eventCodeE + @"' 

                    select SentPlaceType=送往地点类型,PersonNumber=sum(伤亡人数)
                    from #temp
                    group by 送往地点类型

                    drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 送往地点统计表
        /// <summary>
        /// 送往地点统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_SWDD(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select 
                    送往地点=case when tae.送往地点 not in(select 名称 from THospitalInfo where 是否有效=1) then '其他' else tae.送往地点 end,
                    人数=tae.伤亡人数
                    into #temp
                    from TAlarmEvent tae
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0 
                    and tae.事件编码>'" + eventCodeB + "' and tae.事件编码<='" + eventCodeE + @"'
                    --and 送往地点类型编码=0

                    select SentPlace=送往地点,PersonNumber=sum(人数)
                    from #temp
                    group by 送往地点
                    order by sum(人数)

                    drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 危重病人病情预报登记统计表
        /// <summary>
        /// 危重病人病情预报登记统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_WZBRBQYBDJ(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select distinct 任务编码,送往医院
                    into #temp
                    from TGreenDate tgd
                    where 时间>='" + beginTime + "' and 时间<='" + endTime + @"'
                    --order by 任务编码
                    select ToHospital=送往医院,Times=count(*)
                    from #temp
                    group by 送往医院

                    drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 未及时放车时间分析表
        /// <summary>
        /// 未及时放车时间分析表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_WJSFCSJFX(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                    select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                    into #temp0
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码

                    WHERE tae.首次受理时刻 >= '" + beginTime + "' AND tae.首次受理时刻 <= '" + endTime + @"' and tae.是否测试=0
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    SELECT 时间区段 = CASE DATEPART(hour, 首次受理时刻)
                                       WHEN 0 THEN '00:00-01:00'
                                       WHEN 1 THEN '01:00-02:00'
                                       WHEN 2 THEN '02:00-03:00'
                                       WHEN 3 THEN '03:00-04:00'
                                       WHEN 4 THEN '04:00-05:00'
                                       WHEN 5 THEN '05:00-06:00'
                                       WHEN 6 THEN '06:00-07:00'
                                       WHEN 7 THEN '07:00-08:00'
                                       WHEN 8 THEN '08:00-09:00'
                                       WHEN 9 THEN '09:00-10:00'
                                       WHEN 10 THEN '10:00-11:00'
                                       WHEN 11 THEN '11:00-12:00'
                                       WHEN 12 THEN '12:00-13:00'
                                       WHEN 13 THEN '13:00-14:00'
                                       WHEN 14 THEN '14:00-15:00'
                                       WHEN 15 THEN '15:00-16:00'
                                       WHEN 16 THEN '16:00-17:00'
                                       WHEN 17 THEN '17:00-18:00'
                                       WHEN 18 THEN '18:00-19:00'
                                       WHEN 19 THEN '19:00-20:00'
                                       WHEN 20 THEN '20:00-21:00'
                                       WHEN 21 THEN '21:00-22:00'
                                       WHEN 22 THEN '22:00-23:00'
                                       WHEN 23 THEN '23:00-24:00'
                                       ELSE '未知'
                                     END,
                           迟缓放车数 = CASE
                                        WHEN ((DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 and 事件类型编码 in(0,2,3))
                                         or (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >30 and 事件类型编码=1))  THEN 1
                                        ELSE 0
                                      END,
	                       迟缓放车数_急 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 AND 事件类型编码 in(0,3)) THEN 1
                                           ELSE 0
                                         END,
                          迟缓放车数_转院 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >5 AND 事件类型编码=2) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车数_非 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) >30 AND 事件类型编码 = 1) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车5_10分钟 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 5 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=10
                                                 and 事件类型编码 in(0,2,3)) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车5_10分钟_急 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 5 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=10 
                                                AND 事件类型编码 in(0,3)) THEN 1
                                           ELSE 0
                                         END,

                           迟缓放车5_10分钟_转院 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 5 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=10 
                                                AND 事件类型编码 = 2) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车10_15分钟 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 10 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=15
                                                 and 事件类型编码 in(0,2,3)) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车10_15分钟_急 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 10 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=15 
                                                 AND 事件类型编码 in(0,3)) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车10_15分钟_转院 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 10 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=15 
                                                 AND 事件类型编码=2) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车15分钟以上 = CASE
                                                WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 15 and 事件类型编码 in(0,2,3) THEN 1
                                                ELSE 0
                                              END,
                           迟缓放车15分钟以上_急 = CASE
                                                   WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 15 AND 事件类型编码 in(0,3) THEN 1
                                                   ELSE 0
                                                 END,
                           迟缓放车15分钟以上_转院 = CASE
                                                   WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 15 AND 事件类型编码 =2 THEN 1
                                                   ELSE 0
                                                 END,
                           迟缓放车30_45分钟_非 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 30 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=45 
                                                 AND 事件类型编码 = 1) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车45_60分钟_非 = CASE
                                           WHEN (DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 45 and DATEDIFF(minute, 首次受理时刻, 发送指令时刻)<=60 
                                                 AND 事件类型编码=1) THEN 1
                                           ELSE 0
                                         END,
                           迟缓放车60分钟以上_非 = CASE
                                                   WHEN DATEDIFF(minute, 首次受理时刻, 发送指令时刻) > 60 AND 事件类型编码 =1 THEN 1
                                                   ELSE 0
                                                 END
                    INTO #temp
                    FROM #temp0 

                    SELECT TimeSegment=时间区段,
                           DelayedSend = SUM(迟缓放车数),
                           DelayedSendAid = SUM(迟缓放车数_急),
                           DelayedSendNotAid = SUM(迟缓放车数_非),
                           DelayedSendTransfer = SUM(迟缓放车数_转院),
                           DelayedSend5_10M = SUM(迟缓放车5_10分钟),
                           DelayedSend5_10MAid = SUM(迟缓放车5_10分钟_急),
                           DelayedSend5_10MTransfer = SUM(迟缓放车5_10分钟_转院),
                           DelayedSend10_15M = SUM(迟缓放车10_15分钟),
                           DelayedSend10_15MAid = SUM(迟缓放车10_15分钟_急),
                           DelayedSend10_15MTransfer = SUM(迟缓放车10_15分钟_转院),
                           DelayedSendOver15M = SUM(迟缓放车15分钟以上),
                           DelayedSendOver15MAid = SUM(迟缓放车15分钟以上_急),
                           DelayedSendOver15MTransfer = SUM(迟缓放车15分钟以上_转院),
                           DelayedSend30_45MNotAid = SUM(迟缓放车30_45分钟_非),
                           DelayedSend45_60MNotAid = SUM(迟缓放车45_60分钟_非),
                           DelayedSendOver60MNotAid = SUM(迟缓放车60分钟以上_非)
                    FROM #temp
                    GROUP BY 时间区段
                    ORDER BY 时间区段

                    DROP TABLE #temp0
                    DROP TABLE #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 现场地点统计表
        /// <summary>
        /// 现场地点统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_XCDD(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                   select 
                    --现场地点=tzlat.名称,
                    LocaleAddress=case 事件类型编码 when 0 then '家庭' else '医院' end,
                    AlarmEventType=tzaet.名称,
                    PersonNumber=sum(tae.伤亡人数)
                    from TAlarmEvent tae
                    --left join TZLocalAddrType tzlat on tzlat.编码=tae.往救地点类型编码
                    left join dbo.TZAlarmEventType tzaet on tzaet.编码=tae.事件类型编码
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0
                    and 事件编码>'" + eventCodeB + "' and 事件编码<='" + eventCodeE + @"' 
                    and 事件类型编码 not in (5,6)
                    --group by tzlat.名称,tzaet.名称
                    group by 事件类型编码,tzaet.名称");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 转分中心次数统计表
        /// <summary>
        /// 转分中心次数统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_ZFZXCS(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                set nocount on
                select 
                病种判断 as 分类编码
                ,a = sum(case when datename(hour,首次受理时刻) >=0 and datename(hour,首次受理时刻) < 2 then 1 else 0 end)
                ,b = sum(case when datename(hour,首次受理时刻) >=2 and datename(hour,首次受理时刻) < 4 then 1 else 0 end)
                ,c = sum(case when datename(hour,首次受理时刻) >=4 and datename(hour,首次受理时刻) < 6 then 1 else 0 end)
                ,d = sum(case when datename(hour,首次受理时刻) >=6 and datename(hour,首次受理时刻) < 8 then 1 else 0 end)
                ,e = sum(case when datename(hour,首次受理时刻) >=8 and datename(hour,首次受理时刻) < 10 then 1 else 0 end)
                ,f = sum(case when datename(hour,首次受理时刻) >=10 and datename(hour,首次受理时刻) < 12 then 1 else 0 end)
                ,g = sum(case when datename(hour,首次受理时刻) >=12 and datename(hour,首次受理时刻) < 14 then 1 else 0 end)
                ,h = sum(case when datename(hour,首次受理时刻) >=14 and datename(hour,首次受理时刻) < 16 then 1 else 0 end)
                ,i = sum(case when datename(hour,首次受理时刻) >=16 and datename(hour,首次受理时刻) < 18 then 1 else 0 end)
                ,j = sum(case when datename(hour,首次受理时刻) >=18 and datename(hour,首次受理时刻) < 20 then 1 else 0 end)
                ,k = sum(case when datename(hour,首次受理时刻) >=20 and datename(hour,首次受理时刻) < 22 then 1 else 0 end)
                ,l = sum(case when datename(hour,首次受理时刻) >=22 and datename(hour,首次受理时刻) < 24 then 1 else 0 end)
                ,总数=count(*)
                ,手机= sum(case when len(首次呼救电话)=11 and left(首次呼救电话,1)='1' then 1 else 0 end)
                into #temp
                from dbo.TAlarmEvent
                where 首次受理时刻 >= '" + beginTime + "' and 首次受理时刻<'" + endTime + @"' and 事件类型编码 in(5,6) and 是否测试 = 0
                and 事件编码>'" + eventCodeB + "' and 事件编码<='" + eventCodeE + @"' 
                group by 病种判断


                select
                        Name=tsp.名称
                        ,ID00_00_01_59=t.a
                        ,ID02_00_03_59=t.b
	                ,ID04_00_05_59=t.c
	                ,ID06_00_07_59=t.d
	                ,ID08_00_09_59=t.e
	                ,ID10_00_11_59=t.f
	                ,ID12_00_13_59=t.g
	                ,ID14_00_15_59=t.h
	                ,ID16_00_17_59=t.i
	                ,ID18_00_19_59=t.j
	                ,ID20_00_21_59=t.k
	                ,ID22_00_23_59=t.l
	                ,Total=t.总数
	                ,Telephone=(t.总数-t.手机) 
	                ,MobilePhone=t.手机
                from TSubPhoneType tsp
                left join #temp t on tsp.编码=t.分类编码
                union all
                select
                Name='中心接听'
                , ID00_00_01_59= sum(case when datename(hour,来电时刻) >=0 and datename(hour,来电时刻) <2 then 1 else 0 end)
                ,ID02_00_03_59 = sum(case when datename(hour,来电时刻)>=2 and datename(hour,来电时刻) < 4 then 1 else 0 end)
                ,ID04_00_05_59 = sum(case when datename(hour,来电时刻) >=4 and datename(hour,来电时刻) <6 then 1 else 0 end)
                ,ID06_00_07_59 = sum(case when datename(hour,来电时刻) >=6 and datename(hour,来电时刻) < 8 then 1 else 0 end)
                ,ID08_00_09_59 = sum(case when datename(hour,来电时刻) >=8 and datename(hour,来电时刻)< 10 then 1 else 0 end)
                ,ID10_00_11_59 = sum(case when datename(hour,来电时刻) >=10 and datename(hour,来电时刻) < 12 then 1 else 0 end)
                ,ID12_00_13_59 = sum(case when datename(hour,来电时刻) >=12 and datename(hour,来电时刻) < 14 then 1 else 0 end)
                ,ID14_00_15_59 = sum(case when datename(hour,来电时刻) >=14 and datename(hour,来电时刻) < 16 then 1 else 0 end)
                ,ID16_00_17_59 = sum(case when datename(hour,来电时刻) >=16 and datename(hour,来电时刻) < 18 then 1 else 0 end)
                ,ID18_00_19_59 = sum(case when datename(hour,来电时刻) >=18 and datename(hour,来电时刻) < 20 then 1 else 0 end)
                ,ID20_00_21_59 = sum(case when datename(hour,来电时刻) >=20 and datename(hour,来电时刻) < 22 then 1 else 0 end)
                ,ID22_00_23_59 = sum(case when datename(hour,来电时刻) >=22 and datename(hour,来电时刻) < 24 then 1 else 0 end)
                ,Total=count(*)
                ,Telephone= sum(case when len(主叫号码)<> 11 or left(主叫号码,1)<>'1' then 1 else 0 end)
                ,MobilePhone= sum(case when len(主叫号码)=11 and left(主叫号码,1)='1' then 1 else 0 end)
                from TTelRecord
                where 来电时刻>='" + beginTime + "' and 来电时刻<'" + endTime + @"' and 是否呼出=0 and 是否无线=0
                and 结果类型=0
                drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 转院数据统计表
        /// <summary>
        /// 转院数据统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_ZYSJ(DateTime beginTime, DateTime endTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  
                select 
                LevelTransfer=sum( case when thxc.等级编码=thsw.等级编码 then 1 else 0 end),
                LowToHigh=sum(case when thxc.等级编码>thsw.等级编码 then 1 else 0 end),
                HighToLow=sum(case when thxc.等级编码<thsw.等级编码 then 1 else 0 end)
                from TAlarmEvent tae
                left join THospitalInfo thxc on thxc.名称=tae.现场地址
                left join THospitalInfo thsw on thsw.名称=tae.送往地点
                where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                and tae.是否测试=0 and 事件类型编码=2");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 反应时间统计表
        /// <summary>
        /// 反应时间统计表
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataTable Get_TJ_FYSJ(DateTime beginTime, DateTime endTime, string center, string station, string name, string workCode, string type)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            if (type == "分中心")
            {
                sb.Append(@"  
   		                select 
		                Name=tc.名称,
		                WorkCode='',
                        SendCarTimes=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 then 1 else 0 end),
                        TotalReactionTime=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END),
		                ReactionTime=avg(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻)> 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END)
		                FROM TTask tt
		                left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
		                left join TAlarmEvent tae on tae.事件编码=tt.事件编码
		                left join TStation ts on ts.编码=tt.分站编码
		                left join TCenter tc on tc.编码=ts.中心编码
                        left join TAmbulance tam on tam.车辆编码=tt.车辆编码
                        left join TTaskPersonLink ttpl on ttpl.任务编码=tt.任务编码 and ttpl.人员类型编码=3
		                left join TPerson tp on tp.编码=ttpl.人员编码                        
		                WHERE 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻<= '" + endTime + @"' AND tae.是否测试=0
                                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' and ts.中心编码<>1");
                WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
                WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                sb.Append("group by tc.名称 ");
            }

            else if (type == "分站")
            {
                sb.Append(@"
		                select 
		                Name=ts.名称,
		                WorkCode='',
                        SendCarTimes=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 then 1 else 0 end),
                        TotalReactionTime=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END),
		                ReactionTime=avg(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END)
		                FROM TTask tt
		                left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
		                left join TAlarmEvent tae on tae.事件编码=tt.事件编码
		                left join TStation ts on ts.编码=tt.分站编码
		                left join TCenter tc on tc.编码=ts.中心编码
                                left join TAmbulance tam on tam.车辆编码=tt.车辆编码
                                left join TTaskPersonLink ttpl on ttpl.任务编码=tt.任务编码 and ttpl.人员类型编码=3
		                left join TPerson tp on tp.编码=ttpl.人员编码
		                WHERE 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻<= '" + endTime + @"' AND tae.是否测试=0
                                and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"' 
                                and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"'");
                WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
                WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                sb.Append("group by ts.名称");
            }
            else
            {
                sb.Append(@" 
   		                select
		                Name=tp.姓名,
		                WorkCode=tp.工号,
                        SendCarTimes=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 then 1 else 0 end),
                        TotalReactionTime=sum(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END),
		                ReactionTime=avg(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                         THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END)
		                FROM TTask tt
		                left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
		                left join TAlarmEvent tae on tae.事件编码=tt.事件编码
		                left join TStation ts on ts.编码=tt.分站编码
		                left join TCenter tc on tc.编码=ts.中心编码
                                left join TAmbulance tam on tam.车辆编码=tt.车辆编码
                                left join TTaskPersonLink ttpl on ttpl.任务编码=tt.任务编码 and ttpl.人员类型编码=3
		                left join TPerson tp on tp.编码=ttpl.人员编码
		                WHERE 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻 <= '" + endTime + @"' AND tae.是否测试=0");
                WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
                WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                sb.Append("group by tp.编码,tp.姓名,tp.工号");
            }

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        public DataTable Get_TJ_FYSJ_CenterTime(DateTime beginTime, DateTime endTime, string station, string name, string workCode, string type)
        {
            StringBuilder sb = new StringBuilder();
            if (type == "分中心")
            {
                sb.Append(@"
                    select 
		            Name=tc.名称,
		            ReactionTime=avg(CASE WHEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) > 0 
                                     THEN DATEDIFF(second, tt.开始受理时刻, 到达现场时刻) END)
		            FROM TTask tt
		            left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
		            left join TAlarmEvent tae on tae.事件编码=tt.事件编码
		            left join TStation ts on ts.编码=tt.分站编码
		            left join TCenter tc on tc.编码=ts.中心编码
                    left join TTaskPersonLink ttpl on ttpl.任务编码=tt.任务编码 and ttpl.人员类型编码=3
		            left join TPerson tp on tp.编码=ttpl.人员编码
		            WHERE 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻<= '" + endTime + @"' AND tae.是否测试=0
                    and ts.中心编码 =1 ");
                WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
                WhereClauseUtility.AddStringLike("tp.工号", workCode, sb);
                WhereClauseUtility.AddStringLike("tp.姓名", name, sb);
                sb.Append("group by tc.名称 ");
            }
            else
            {
                sb.Append(@"
                    select Name='', ReactionTime = 0
                    FROM TTask tt
		            left join TAcceptEvent tac on tac.事件编码=tt.事件编码 and tac.受理序号=tt.受理序号
		            left join TAlarmEvent tae on tae.事件编码=tt.事件编码
		            left join TStation ts on ts.编码=tt.分站编码
		            left join TCenter tc on tc.编码=ts.中心编码
                    WHERE 生成任务时刻 >= '" + beginTime + "' AND 生成任务时刻<= '" + endTime + @"' AND tae.是否测试=0");
            }
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 呼救事件来源统计表
        /// <summary>
        /// 呼救事件来源统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="name"></param>
        /// <param name="workCode"></param>
        /// <returns></returns>
        public DataTable Get_TJ_HJSJLY(DateTime beginTime, DateTime endTime, string name, string workCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                    Dispatcher=tp.姓名,
                    WorkCode=tp.工号,
                    EventSource=tzlae.名称,
                    Quantity=count(*)
                    from TAlarmEvent tae
                    left join TPerson tp on tp.编码=tae.首次调度员编码
                    left join TZAlarmEventOrigin tzlae on tzlae.编码=tae.事件来源编码
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0
                    and tp.姓名 like '%'+'" + name + "'+'%' and tp.工号 like '%'+'" + workCode + @"'+'%'
                    group by tp.姓名,tp.工号,tzlae.名称");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_TJ_HJSJLY_TL(DateTime beginTime, DateTime endTime, string name, string workCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                    EventSource=tzlae.名称,
                    Quantity=count(*)
                    from TAlarmEvent tae
                    left join TPerson tp on tp.编码=tae.首次调度员编码
                    left join TZAlarmEventOrigin tzlae on tzlae.编码=tae.事件来源编码
                    where 首次受理时刻>='" + beginTime + "' and 首次受理时刻<='" + endTime + @"'
                    and 是否测试=0
                    and tp.姓名 like '%'+'" + name + "'+'%' and tp.工号 like '%'+'" + workCode + @"'+'%'
                    group by tzlae.名称");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        #endregion

        #region 急救人员工作效率统计表
        /// <summary>
        /// 急救人员工作效率统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="center"></param>
        /// <param name="station"></param>
        /// <param name="name"></param>
        /// <param name="workCode"></param>
        /// <returns></returns>
        public DataTable Get_JJRYGZXL(DateTime beginTime, DateTime endTime, string center, string station, string name, string workCode)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    set ansi_warnings off
                    --已下班
                    select 人员编码,上班时间=上次操作时刻,下班时间=操作时刻 
                    into #temp1
                    from dbo.TAmbulancePersonSign
                    where 人员类型编码=3
                    and 上次操作时刻>='" + beginTime + "' and 上次操作时刻<='" + endTime + @"'
                    and 是否上班操作=0
                    --未下班
                    select A.人员编码,上班时间=最后操作时刻
                    ,下班时间=(select 生成任务时刻 from dbo.TTask 
                    where 任务编码=(select max(任务编码) from dbo.TTaskPersonLink where 人员编码=A.人员编码)
                    and 生成任务时刻>最后操作时刻)
                    into #temp2
                    from (select 人员编码,最后操作时刻=max(操作时刻) from dbo.TAmbulancePersonSign where 人员类型编码=3 group by 人员编码 ) A
                    left join TAmbulancePersonSign B on A.人员编码=B.人员编码 and A.最后操作时刻=B.操作时刻
                    where 是否上班操作=1 and B.操作时刻>='" + beginTime + "' and B.操作时刻<='" + endTime + @"'

                    select 人员编码,上班时间,下班时间 into #temp from #temp1 union (select 人员编码,上班时间,下班时间 from #temp2)

                    select 人员编码,count(*) as 当班数 
                    into #temp3
                    from (select distinct 人员编码, convert(varchar(10),上班时间,112) as 上班日期 from #temp
                    where datediff(hh,上班时间,下班时间)>2 ) A
                    group by A.人员编码

                    select ttpl.人员编码,
                          tp.姓名
                          ,tp.工号
                          ,count(*) as 出车次数
                          ,avg(case when datediff(second,tt.生成任务时刻,tt.出车时刻)>0 then datediff(second,tt.生成任务时刻,tt.出车时刻) end) as 平均出车时间
                          ,avg(case when datediff(second,tt.出车时刻,tt.到达现场时刻)>0 then datediff(second,tt.出车时刻,tt.到达现场时刻) end) as 平均到达现场时间
                          ,avg(case when datediff(second,tt.到达现场时刻,tt.离开现场时刻)>0 then datediff(second,tt.到达现场时刻,tt.离开现场时刻) end) as 平均现场抢救时间
                          ,avg(case when datediff(second,tt.离开现场时刻,tt.到达医院时刻)>0 then datediff(second,tt.离开现场时刻,tt.到达医院时刻) end) as 平均运送时间
                          ,avg(case when datediff(second,tt.到达医院时刻,tt.完成时刻)>0 then datediff(second,tt.到达医院时刻,tt.完成时刻) end) as 平均医院交接时间
                          ,avg(case when datediff(second,tt.开始受理时刻,tt.到达现场时刻)>0 then datediff(second,tt.开始受理时刻,tt.到达现场时刻) end) as 平均反应时间
                          ,avg(case when datediff(second,tt.出车时刻,tt.完成时刻)>0 then datediff(second,tt.出车时刻,tt.完成时刻) end) as 平均周转时间
                    into #temp4
                    from TTask tt
                    inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
                    inner join TAlarmEvent tae on tt.事件编码=tae.事件编码
                    left join dbo.TAmbulance ta on tt.车辆编码 = ta.车辆编码
                    left join dbo.TTaskPersonLink ttpl on tt.任务编码 = ttpl.任务编码 and ttpl.人员类型编码=3
                    left join dbo.TPerson tp on ttpl.人员编码 = tp.编码
                    left join dbo.Tstation ts on tt.分站编码 = ts.编码 
                    left join dbo.TCenter tc on tc.编码 = ts.中心编码
                    where tt.生成任务时刻 >= '" + beginTime + "' and tt.生成任务时刻<='" + endTime + @"' 
                              and tae.是否测试 = 0 
                              and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                              and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                              and tt.出车时刻 is not null --2010-6-3  --and 是否正常结束=1
                              and (tp.工号 like '%' +'" + workCode + @"'+ '%' or tp.工号 is null)
		                      and (tp.姓名 like '%' +'" + name + @"'+ '%')");
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
            WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
            sb.Append("group by ttpl.人员编码,tp.姓名,tp.工号");

            sb.Append(@" select   PersonCode=t1.人员编码,Name=姓名,WorkCode=工号,SendCarTimes=出车次数,Days=t2.当班数, 
	                    AvgDaySendCarTimes=cast(convert(float,(100.00*出车次数/t2.当班数/100)) as decimal(10,2)),
	                    AvgSendCarTime=平均出车时间,AvgArrivingSceneTime=平均到达现场时间,
	                    AvgSceneRescueTime=平均现场抢救时间,AvgTransportTime=平均运送时间,AvgHospitalDeliveryTime=平均医院交接时间,
                        AvgResponseTime=平均反应时间,AvgRevolveTime=平均周转时间
                    from #temp4 t1 left join #temp3 t2 on t1.人员编码=t2.人员编码
                    order by t1.人员编码

                    drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp4
                    drop table #temp 
                    set ansi_warnings on");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        #endregion

        #region 来电记录统计表
        /// <summary>
        /// 来电记录统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="telNumber"></param>
        /// <param name="disposeResult"></param>
        /// <returns></returns>
        public DataTable Get_LDJL(DateTime beginTime, DateTime endTime, string telNumber, string disposeResult)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    set ansi_warnings off
                    select 来电时刻,主叫号码
                    ,处理结果=case when 震铃时刻 is not null and 结果类型=1 then '未接听' when 结果类型=0 then '已接听' else tztrt.名称 end
                    ,处理结果编码=case when 震铃时刻 is not null and 结果类型=1 then 0 when 结果类型=0 then 1 else 2 end
                    into #temp
                    from dbo.TTelRecord ttr 
                    left join dbo.TZTelRecordType tztrt on tztrt.编码=ttr.结果类型 
                    where 来电时刻 >= '" + beginTime + "' and 来电时刻<='" + endTime + @"' and 是否呼出=0 
                    and 主叫号码 like '%'+'" + telNumber + @"'+'%'

                    select CallAmount=count(*),ResultCode=处理结果编码  from #temp
                    where 1=1");
            WhereClauseUtility.AddInSelectQuery("处理结果编码", disposeResult, sb);
            sb.Append(" group by 处理结果编码   drop table #temp    set ansi_warnings on ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 排队电话每日峰值统计表
        public DataTable Get_PDDHMRFZ(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    set ansi_warnings off
                    ----查询月的天数
                    DECLARE @days int
                    set @days=32 - Day(CONVERT(DateTime, '" + year + "'+'/'+'" + month + @"'+'/01')+31) 

                    DECLARE @SumLink TABLE(时间 varchar(4))

                    declare @i int 
                    set @i=1 
                    while @i<=@days 
                    begin 
                    INSERT INTO @SumLink values (right('0000'+CONVERT(varchar(2),@i),2)+'日')
                    set @i=@i+1 
                    end 
                    --select * from @SumLink

                    select 时间区段 = right('0000'+CONVERT(varchar(2),DATEPART(day, 来电时刻)),2)+'日',
                    最大数量 = isnull(max(排队数量),0) 
                    into #temp0
                    FROM dbo.TTelRecord
                    WHERE DATEPART(year,来电时刻 )='" + year + "' AND DATEPART(MONTH,来电时刻 )='" + month + @"' 
                    and 排队数量 >1 
                    GROUP BY DATEPART(d, 来电时刻)

                    SELECT  时间, 最大数量=sum(最大数量)
                    FROM @SumLink  t left join #temp0 t0 on t.时间=t0.时间区段
                    GROUP BY 时间
                    ORDER BY 时间

                    DROP TABLE #temp0 
                    set ansi_warnings on");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 受理情况峰值统计表
        public DataTable Get_SLQKFZ(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                     set ansi_warnings off
                    ----查询月的天数
                    DECLARE @days int
                    set @days=32 - Day(CONVERT(DateTime, '" + year + "'+'/'+'" + month + @"'+'/01')+31) 

                    DECLARE @SumLink TABLE(时间 varchar(4))

                    declare @i int 
                    set @i=1 
                    while @i<=@days 
                    begin 
                    INSERT INTO @SumLink values (right('0000'+CONVERT(varchar(2),@i),2)+'日')
                    set @i=@i+1 
                    end 
                    --select * from @SumLink

                    select tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻,min(tac.发送指令时刻) as 发送指令时刻
                    into #temp0
                    FROM TTask tt
                    join TAcceptEvent tac on tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE DATEPART(year,tae.首次受理时刻)='" + year + "' AND DATEPART(MONTH,tae.首次受理时刻)='" + month + @"' and tae.是否测试=0
                    group by tt.任务编码,tae.事件编码,tae.事件类型编码,tae.首次受理时刻

                    SELECT 
                    时间区段 = right('0000'+CONVERT(varchar(2),DATEPART(day, 首次受理时刻)),2)+'日',
                    受理数 = 1
                    INTO #temp1
                    FROM #temp0

                    UNION ALL

                    select 时间区段 = right('0000'+CONVERT(varchar(2),DATEPART(day, 首次受理时刻)),2)+'日',
                    受理数 = 1
                    FROM TAcceptEvent tac 
                    join TAlarmEvent tae on tae.事件编码=tac.事件编码
                    WHERE  DATEPART(year,tae.首次受理时刻)='" + year + "' AND DATEPART(MONTH,tae.首次受理时刻)='" + month + @"'
                    and 受理类型编码 =4 and tae.是否测试=0

                    SELECT  
	                       时间, 受理数 = isnull(SUM(受理数),0)
                    FROM @SumLink  t left join #temp1 t1 on t.时间=t1.时间区段
                    GROUP BY 时间
                    ORDER BY 时间

                    DROP TABLE #temp0
                    DROP TABLE #temp1 
                    set ansi_warnings on");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 司机出车大于5分钟统计表
        /// <summary>
        /// 司机出车大于5分钟统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="center"></param>
        /// <param name="station"></param>
        /// <param name="name"></param>
        /// <param name="workCode"></param>
        /// <param name="carNumber"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public DataTable Get_SJCCDY5FZ(DateTime beginTime, DateTime endTime, string center, string station, string name, string workCode, string carNumber, int time)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                           Center=tc.名称 
                           ,Station=ts.名称 
                           ,CarNumber=ta.实际标识 
                           ,Driver=dbo.GetStr(tt.任务编码,3) 
                           ,Times=count(*) 
                           ,Time= '" + time + @"'

                    from TTask tt
                    inner join TAcceptEvent tac on (tt.事件编码=tac.事件编码 and tt.受理序号=tac.受理序号) 
                    inner join TAlarmEvent tae on tt.事件编码=tae.事件编码
                    left join dbo.TAmbulance ta on tt.车辆编码 = ta.车辆编码
                    left join dbo.TStation ts on tt.分站编码 = ts.编码
                    left join dbo.TCenter tc on ts.中心编码 = tc.编码
                    left join dbo.TPerson tp on tp.姓名 = dbo.GetStr(tt.任务编码,3)

                    where 生成任务时刻 >= '" + beginTime + "' and 生成任务时刻<='" + endTime + @"' and tae.是否测试 = 0
                    and tt.任务编码>'" + taskCodeB + "' and tt.任务编码<='" + taskCodeE + @"'
                    and tac.事件编码>'" + eventCodeB + "' and tac.事件编码<='" + eventCodeE + @"' 
                    and (是否正常结束=1)
                    and (datediff(second,tt.生成任务时刻,tt.出车时刻)>'" + time + @"'*60)");
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
            WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
            sb.Append("and ta.实际标识 like '%' + '" + carNumber + @"' + '%' --or @chehao='')and (dbo.GetStr(tt.任务编码,3) = @siji or @siji='')
                    and tp.工号 like '%' + '" + workCode + "' + '%'  and tp.姓名 like '%' +'" + name + @"'+ '%'
                    group by tc.名称,ts.名称,ta.实际标识,dbo.GetStr(tt.任务编码,3)");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 司机暂停调用统计表
        /// <summary>
        /// 司机暂停调用统计表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="center"></param>
        /// <param name="station"></param>
        /// <param name="name"></param>
        /// <param name="workCode"></param>
        /// <param name="carNumber"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public DataTable Get_SJZTDY(DateTime beginTime, DateTime endTime, string center, string station, string name, string workCode, string carNumber)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
                    Center=tc.名称,Station=ts.名称,WorkCode=tp.工号,
                     Name=tp.姓名
                    ,CarNumber=tam.实际标识
                    ,StopCallReason=tzpa.名称
                    ,Times=sum(1)
                    from TPauseRecord tpa
                    left join TZPauseReason tzpa on tzpa.编码=tpa.暂停原因编码
                    left join TAmbulance tam on tam.车辆编码=tpa.车辆编码
                    left join TCenter tc on tc.编码=tam.中心编码
                    left join TStation ts on ts.编码=tam.分站编码
                    left join dbo.TPerson tp on tpa.司机编码=tp.编码
                    where 操作时刻>='" + beginTime + "' and 操作时刻<'" + endTime + @"' and 是否暂停操作=1");
            WhereClauseUtility.AddInSelectQuery("tc.编码", center, sb);
            WhereClauseUtility.AddInSelectQuery("ts.编码", station, sb);
            sb.Append("and tam.实际标识 like '%'+'" + carNumber + @"'+'%'
                    and (tp.工号 like '%'+'" + workCode + "'+'%' or tp.工号 is null)  and tp.姓名 like '%' +'" + name + @"'+ '%'
                    group by tc.名称,ts.名称,tp.工号,
                     tp.姓名,tam.实际标识,tzpa.名称,tc.顺序号,tzpa.顺序号
                    order by tp.姓名,tam.实际标识,tzpa.顺序号");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 业务数据分段统计表
        /// <summary>
        /// 业务数据分段统计表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable Get_YWSJFD(string year, string month)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    --set ansi_warnings off                  
                    ----查询月的天数
                    DECLARE @days int
                    set @days=32 - Day(CONVERT(DateTime, '" + year + "'+'/'+'" + month + @"'+'/01')+31) 
                    DECLARE @SumLink TABLE(日期 varchar(4))
                    declare @i int 
                    set @i=1 
                    while @i<=@days 
                    begin 
                    INSERT INTO @SumLink values (right('0000'+CONVERT(varchar(2),@i),2)+'日')
                    set @i=@i+1 
                    end 
                    ----当月救护车运行情况
                    select  年,月,星期,早班急,早班转,早班非,中班急,中班转,中班非,夜班急,夜班转,夜班非 
                    into #temp  from   srv_lnk.ESApp.dbo.救护车运行情况统计表_明细 
                    where 年='" + year + "' and  月='" + month + @"'

                    DECLARE @BeginTime datetime
                    DECLARE @EndTime datetime
                    SET @BeginTime = CONVERT(DateTime, '" + year + "'+'/'+'" + month + @"'+'/01 8:0:0')
                    SET @EndTime = dateadd(m,1, @BeginTime)

                    select  日期 =case when datepart(hh,生成任务时刻)<8 then CONVERT(varchar(12),dateadd(dd,-1,生成任务时刻),102) else CONVERT(varchar(12),生成任务时刻,102) end,
                    任务编码,tae.事件类型编码,生成任务时刻
                    into #temp0
                    from dbo.TTask tt left join dbo.TAlarmEvent tae on tt.事件编码=tae.事件编码
                    where tt.生成任务时刻>=@BeginTime and tt.生成任务时刻<=@EndTime
                    --DATEPART(year,tt.生成任务时刻)='" + year + "' AND DATEPART(MONTH,tt.生成任务时刻)='" + month + @"'
                    and tae.是否测试=0
                    and tt.出车时刻 is not null

                    SELECT 星期=datename(weekday,日期),
                    时间区段 = right('0000'+CONVERT(varchar(2),DATEPART(day, 日期)),2)+'日',
                    早班急救出车数 = case when 事件类型编码 =0 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)< 16 then 1 else 0 end,
                    早班回家出车数 = case when 事件类型编码 =1 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)< 16 then 1 else 0 end,
                    早班转院出车数 = case when 事件类型编码 =2 and datepart(hh,生成任务时刻)>=8 and datepart(hh,生成任务时刻)<16 then 1 else 0 end,
                    中班急救出车数 = case when 事件类型编码 =0 and datepart(hh,生成任务时刻)>=16 and datepart(hh,生成任务时刻)<20 then 1 else 0 end,
                    中班回家出车数 = case when 事件类型编码 =1 and datepart(hh,生成任务时刻)>=16 and datepart(hh,生成任务时刻)<20 then 1 else 0 end,
                    中班转院出车数 = case when 事件类型编码 =2 and datepart(hh,生成任务时刻)>=16 and datepart(hh,生成任务时刻)<20 then 1 else 0 end,
                    夜班急救出车数 = case when 事件类型编码 =0 and datepart(hh,生成任务时刻)>=20 or datepart(hh,生成任务时刻)<8 then 1 else 0 end,
                    夜班回家出车数 = case when 事件类型编码 =1 and datepart(hh,生成任务时刻)>=20 or datepart(hh,生成任务时刻)<8 then 1 else 0 end,
                    夜班转院出车数 = case when 事件类型编码 =2 and datepart(hh,生成任务时刻)>=20 or datepart(hh,生成任务时刻)<8 then 1 else 0 end
                    INTO #temp1
                    FROM #temp0

                    SELECT  日期,t1.星期, 
                    早班急救平均出车数=cast(isnull(SUM(早班急救出车数),0)/nullif(早班急,0) as decimal(10,2)),
                    早班回家平均出车数=cast(isnull(SUM(早班回家出车数),0)/nullif(早班非,0) as decimal(10,2)),
                    早班转院平均出车数=cast(isnull(SUM(早班转院出车数),0)/nullif(早班转,0) as decimal(10,2)),
                    中班急救平均出车数=cast(isnull(SUM(中班急救出车数),0)/nullif(中班急,0) as decimal(10,2)),
                    中班回家平均出车数=cast(isnull(SUM(中班回家出车数),0)/nullif(中班非,0) as decimal(10,2)),
                    中班转院平均出车数=cast(isnull(SUM(中班转院出车数),0)/nullif(中班转,0) as decimal(10,2)),
                    夜班急救平均出车数=cast(isnull(SUM(夜班急救出车数),0)/nullif(夜班急,0) as decimal(10,2)),
                    夜班回家平均出车数=cast(isnull(SUM(夜班回家出车数),0)/nullif(夜班非,0) as decimal(10,2)),
                    夜班转院平均出车数=cast(isnull(SUM(夜班转院出车数),0)/nullif(夜班转,0) as decimal(10,2))
                    FROM @SumLink sl left join #temp1 t1 on sl.日期=t1.时间区段
                    left join #temp t on t1.星期=t.星期
                    GROUP BY 日期,t1.星期,早班急,早班转,早班非,中班急,中班非,中班转,夜班急,夜班非,夜班转
                    ORDER BY 日期,t1.星期

                    DROP TABLE #temp,#temp0,#temp1
                    --set ansi_warnings on ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 未接来电统计表
        public DataTable Get_WJLD(DateTime beginTime, DateTime endTime, string time, string result, string name, string workCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" set ansi_warnings off 
                     select 来电时刻,主叫号码,调度员工号,姓名=tp.姓名,结束时刻,等待时间=datediff(second,来电时刻,结束时刻)
                    ,处理结果=case when 震铃时刻 is not null then '未接听' else tztrt.名称 end
					,处理结果编码=case when 震铃时刻 is not null then 0 else 1 end
                    into #temp
                    from dbo.TTelRecord ttr 
                    left join dbo.TZTelRecordType tztrt on tztrt.编码=ttr.结果类型 
                    left join dbo.TPerson tp on tp.工号=ttr.调度员工号 
                    where 来电时刻 >= '" + beginTime + "' and 来电时刻<='" + endTime + @"' and 是否呼出=0 and 结果类型=1 
                    and datediff(second,来电时刻,结束时刻)>='" + time + @"' ");
            WhereClauseUtility.AddStringEqual("姓名", name, sb);
            WhereClauseUtility.AddStringLike("调度员工号", workCode, sb);
            sb.Append(@" select  CallTime=来电时刻,CallNumber=主叫号码,WorkCode=调度员工号,
                       Name=姓名,EndTime=结束时刻,WaitTime=等待时间,ActionResult=处理结果,ActionResultCode=处理结果编码                                                                   
                       from #temp 
                       where 1=1 ");
            WhereClauseUtility.AddInSelectQuery("处理结果编码", result, sb);
            sb.Append(" order by 来电时刻 desc ");
            sb.Append("drop table #temp ");
            sb.Append(" set ansi_warnings on ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        public DataTable Get_WJLD_Reason(DateTime beginTime, DateTime endTime, string time, string result, string name, string workCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" set ansi_warnings off 
                    select NoAnswer=sum(case when 震铃时刻 is not null then 1 else 0 end),
                    ReleaseEarly=sum(case when 震铃时刻 is null then 1 else 0 end)
                    from dbo.TTelRecord ttr 
                    left join TPerson tp  on tp.工号=ttr.调度员工号
                    where 来电时刻 >= '" + beginTime + "' and 来电时刻<='" + endTime + @"' and 是否呼出=0 and 结果类型=1 
                    and datediff(second,来电时刻,结束时刻)>='" + time + @"' ");
            WhereClauseUtility.AddStringLike("ttr.调度员工号", workCode, sb);
            WhereClauseUtility.AddStringEqual("tp.姓名", name, sb);
            sb.Append(" set ansi_warnings on ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 车辆出动方式统计表
        public DataTable Get_TJ_CLCDFS(DateTime beginTime, DateTime endTime, string center, string station)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"DECLARE @BeginTime DATETIME
                        DECLARE @EndTime datetime
                        SET @BeginTime = '" + beginTime + @"'
                        SET @EndTime = '" + endTime + @"'

                        declare @EventCodeB char(18)
                        declare @EventCodeE char(18)
                        declare @TaskCodeB char(22)
                        declare @TaskCodeE char(22)

                        set @EventCodeB = convert(char(8),@BeginTime,112)+'0000000000'
                        set @EventCodeE = convert(char(8),dateadd(day,1,@EndTime),112)+'0000000000'
                        set @TaskCodeB = @EventCodeB+'0000'
                        set @TaskCodeE = @EventCodeE+'0000'

                        select 
                        Center=tc.名称
                        ,Station=ts.名称
                        ,SendTimesOnWay=SUM(case when tt.是否站内出动=0 then 1 else 0 end)
                        ,SendTimesOnHome=SUM(case when tt.是否站内出动=1 then 1 else 0 end)
                        from TTask tt
                        left join TStation ts on ts.编码=tt.分站编码
                        left join TCenter tc on ts.中心编码=tc.编码
                        where 开始受理时刻>=@BeginTime and 开始受理时刻<=@EndTime
                        and tt.任务编码>@TaskCodeB and tt.任务编码<=@TaskCodeE
                      ");
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", center, sb);
            WhereClauseUtility.AddInSelectQuery("tt.分站编码", station, sb);
            sb.Append(" group by tc.名称,ts.名称 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 调度员工作状态统计表
        public DataTable Get_TJ_DDYGZZT(DateTime beginTime, DateTime endTime, string type, string name, string PersonCode, string time)
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
                          ,Times=(datediff(second,开始时刻,结束时刻))
                          ,台号
				          ,开始类型编码=case when topnr.开始类型='登录' then 0 when topnr.开始类型='电话受理' then 1 when topnr.开始类型='手工受理' then 2 
				           when topnr.开始类型='就绪' then 3  when topnr.开始类型='暂停' then 4 else 5 end 
				           into #temp
				           from dbo.TOperatorNotReadyRecord topnr
				           left join  dbo.TPerson TP ON (topnr.人员编码=TP.编码)
				           where 开始时刻>='" + beginTime + "' and 开始时刻<='" + endTime + @"'
                           and (datediff(second,开始时刻,结束时刻)>'" + time + "'*60 or '" + time + @"'='')  and TP.是否有效=1 ");
            WhereClauseUtility.AddStringEqual("人员姓名", name, sb);
            WhereClauseUtility.AddStringLike("tp.工号", PersonCode, sb);
            sb.Append(@" group by 人员姓名,开始类型,开始时刻,结束时刻,台号,结束类型
					     order by 人员姓名,开始时刻 

                         select 
                               PersonName=人员姓名                              
                              ,StartType=开始类型                                                           
                              ,Time=sum(Times)                             
                              ,StartTypeCode=开始类型编码
                          from  #temp  where 1=1 
                          ");
            WhereClauseUtility.AddInSelectQuery("开始类型编码", type, sb);
            sb.Append(@"  group by 人员姓名,开始类型,开始类型编码
				          order by 人员姓名
                          drop table #temp
                         set ansi_warnings on ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 病情分类统计表
        public DataTable Get_TJ_BQFL(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
	                病情 = case when tpr.IllnessClassification =''then '未选' else tpr.IllnessClassification end, 
	                数量 = isnull(count(*),0) 
                    from M_PatientRecord tpr  
                    left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                    left join  M_PatientRecordRescue tprr on tprr.TaskCode=tpr.TaskCode and tprr.PatientOrder=tpr.PatientOrder
                    left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString); ;
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);

            sb.Append(@"
		            and  tpr.SubmitLogo='True' and tpr.IllnessClassification is not null
                    group by tpr.IllnessClassification ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 急救效果统计表
        public DataTable Get_TJ_JJXG(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            select 
            急救效果= case when tpr.FirstAidEffect='' then '未选' else tpr.FirstAidEffect end,
            数量=isnull(count(*),0)
            from M_PatientRecord tpr 
            left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
            left join  M_PatientRecordRescue tprr on tprr.TaskCode=tpr.TaskCode and tprr.PatientOrder=tpr.PatientOrder
            left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);
            sb.Append(@" and tpr.SubmitLogo='True'
            and  tpr.FirstAidEffect is not null
            group by tpr.FirstAidEffect ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 病情预报统计表
        public DataTable Get_TJ_BQYB(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select  
                    病情预报 =case when tpr.IllnessForecast = '' then '未选' else tpr.IllnessForecast end,
                    数量 = isnull(count(*),0)
                    from M_PatientRecord tpr 
                    left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                    left join  M_PatientRecordRescue tprr on tprr.TaskCode=tpr.TaskCode and tprr.PatientOrder=tpr.PatientOrder
                    left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);

            sb.Append(@" and tpr.SubmitLogo = 'True' and tpr.IllnessForecast is not null 
                    group by tpr.IllnessForecast ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 患者死亡统计表
        public DataTable Get_TJ_HZSW(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    set nocount on
                         select 
                        送医院数量=isnull(sum(case when tpr.SendAddress not like '%不送%'and tpr.DeathCase = '现场死亡' then 1 else 0 end),0),
                        未送医院数量=isnull(sum(case when tpr.SendAddress like '%不送%' and tpr.DeathCase ='现场死亡' then 1 else 0 end),0),
                        现场死亡数量=isnull(sum(case when tpr.DeathCase='现场死亡' then 1 else 0 end),0),
                        途中死亡数量=isnull(sum(case when tpr.DeathCase='途中死亡' then 1 else 0 end),0),
                        开死亡证明数量 = isnull(sum(case when tpr.DeathCertificate = '开' then 1 else 0 end),0),
                        未开死亡证明数量 = isnull(sum(case when tpr.DeathCertificate ='未开' then 1 else 0 end),0)
                        from M_PatientRecord tpr 
                        left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                        left join  M_PatientRecordRescue tprr on tprr.TaskCode=tpr.TaskCode and tprr.PatientOrder=tpr.PatientOrder
                        left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);
            sb.Append(@" and tpr.SubmitLogo='True'
                        and tpr.DeathCase<> ''  set nocount off");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 性别统计表
        public DataTable Get_TJ_XB(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select Sex = case when mpr.Sex='' or mpr.Sex is null then '未选' else  mpr.Sex  end,Number = count(*) 
                    from M_PatientRecord  mpr                                        
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder  ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >='" + beginTime + @"' and mpr.MedicalRecordGenerationTime <'" + endTime + @"'  
                       and mpr.SubmitLogo=1  ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append("  group by mpr.Sex ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 年龄段统计表
        public DataTable Get_TJ_NLFB(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"                   
                    declare @T table(年龄 varchar(8),区间 varchar(20),序号 int)
                    insert into @T values('婴儿','0~12个月',1)
                    insert into @T values('幼儿','1岁~6岁',2)
                    insert into @T values('少年','7岁~18岁',3)
                    insert into @T values('青年','19岁~40岁',4)
                    insert into @T values('中年','41岁~60岁',5)
                    insert into @T values('老年','61岁以上',6)                                   
                    insert into @T values('不详','不详',7)

                    select
                    Age = a.年龄,
                    Region = isnull(t.区间,''),
                    Number = sum(a.数量)
                    from 
                    (select 
                    年龄 = case 
                        when  Isnumeric(mpr.Age)=0 then '不详' 
	                    when mpr.Age is not null and mpr.AgeType = '岁' 
	                    and isnumeric(mpr.Age)=1 and (1<=cast(mpr.Age as float) and 6>=cast(mpr.Age as float)) then '幼儿'	                  
	                    when mpr.Age is not null and mpr.AgeType = '岁' 
	                    and isnumeric(mpr.Age)=1 and (7<=cast(mpr.Age as float) and 18>=cast(mpr.Age as float)) then '少年'
	                    when mpr.Age is not null and mpr.AgeType = '岁' 
	                    and isnumeric(mpr.Age)=1 and (19<cast(mpr.Age as float) and 40>=cast(mpr.Age as float)) then '青年'
	                    when mpr.Age is not null and mpr.AgeType = '岁' 
	                    and isnumeric(mpr.Age)=1 and (41<=cast(mpr.Age as float) and 60>=cast(mpr.Age as float)) then '中年'
	                    when mpr.Age is not null and mpr.AgeType = '岁' 
	                    and isnumeric(mpr.Age)=1 and (61<=cast(mpr.Age as float)) then '老年'                   
	                    when mpr.Age is not null and mpr.AgeType = '月' 
	                    and isnumeric(mpr.Age)=1 and (12>=cast(mpr.Age as float)) then '婴儿'  
                        when mpr.Age is not null and mpr.AgeType = '天' 
	                    and isnumeric(mpr.Age)=1 and (365>=cast(mpr.Age as float)) then '婴儿'               
	                    else '不详' end,
                    数量 = count(mpr.Age) 
                    from M_PatientRecord mpr                       
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder  ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >='" + beginTime + @"' and mpr.MedicalRecordGenerationTime <'" + endTime + @"'  
                        and mpr.SubmitLogo=1  ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by mpr.Age,mpr.AgeType ) a ");
            sb.Append(" left join @T t on t.年龄 = a.年龄 ");
            sb.Append("  group by a.年龄,t.序号,t.区间 ");
            sb.Append("  order by t.序号   ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 事件类型统计表
        public DataTable Get_TJ_SJLX(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select EventType = case when mpr.PatientVersion='' or mpr.PatientVersion is null then '未选' else mpr.PatientVersion end ,Number = count(*) 
                    from M_PatientRecord  mpr                                       
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder  ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append("  where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime<'" + endTime + @"'
                    and mpr.SubmitLogo=1 ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by mpr.PatientVersion  ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 任务类型统计表
        public DataTable Get_TJ_RWLX(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select TaskType = case when mpr.OriginalTaskType='' or mpr.OriginalTaskType is null then '未选' else mpr.OriginalTaskType end ,Number = count(*) 
                    from M_PatientRecord  mpr                                     
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder  ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append("  where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime<'" + endTime + @"'
                    and mpr.SubmitLogo=1 ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by mpr.OriginalTaskType  ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 病种分类统计表
        public DataTable Get_TJ_BZFL(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select DiseasesClassification = case when mpr.DiseasesClassification='' or mpr.DiseasesClassification is null then '未选' else mpr.DiseasesClassification end ,
                    Number = count(*)     from M_PatientRecord  mpr                                      
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder  ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime<'" + endTime + @"'
                    and mpr.SubmitLogo=1 ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by mpr.DiseasesClassification  ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 救治措施统计表
        public DataTable Get_TJ_JZCS(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                         set nocount on
                        select

                        出车分站= case when tpr.Station=''then '无' else tpr.Station end,
                        事件类型=case when tpr.PatientVersion=''then '无' else tpr.PatientVersion end,
                        急救医护人=case when tpr.DoctorAndNurse ='' then '无' else tpr.DoctorAndNurse end,
                        经办人工号=case when tpr.AgentWorkID ='' then '无' else tpr.AgentWorkID end,
                        --人工胸外按压,机械胸外按压
                        胸外按压数=sum(case when tpm.RescueMeasureCode in(449,450) then 1 else 0 end),
                        人工胸外按压=sum(case when tpm.RescueMeasureCode=449 then 1 else 0 end),
 						机械胸外按压=sum(case when tpm.RescueMeasureCode=450 then 1 else 0 end),
 						体外心脏除颤术=sum(case when tpm.RescueMeasureCode=341 and tprr.Measures not like('% 同步电复律%') then 1 else 0 end),
 						--人工胸外按压,机械胸外按压,体外心脏复律除颤术(备注中无同步电复律的)
 						心肺复苏数 = sum(case when tpm.RescueMeasureCode in(449,450,341) and tprr.Measures not like('% 同步电复律%')  then 1 else 0 end),
 						--电复律不算在心肺复苏里面 电复律率直接除以病历总数
 						体外心脏电复律术=sum(case when tpm.RescueMeasureCode=341 and tprr.Measures  like('% 同步电复律%') then 1 else 0 end),

                        呼吸机辅助呼吸=sum(case when tpm.RescueMeasureCode =338 then 1 else 0 end),
                        气管插管术=sum(case when tpm.RescueMeasureCode=342 then 1 else 0 end),
 						--加压给氧,加压给氧,呼吸机辅助呼吸,气管插管术,吸氧,无创辅助通气,吸痰护理,前鼻孔填塞,鼻异物取出
 						气道管理数=sum(case when tpm.RescueMeasureCode in (325,338,342,404,408,418,419,420) then 1 else 0 end),

                        静脉输液=sum(case when tpm.RescueMeasureCode=329 then 1 else 0 end),
                        静脉注射=sum(case when tpm.RescueMeasureCode=422 then 1 else 0 end),
 						--静脉输液,静脉注射,微量泵输液
 						静脉数=sum(case when tpm.RescueMeasureCode in(436,329,422) then 1 else 0 end),

 						肌肉注射=sum(case when tpm.RescueMeasureCode=231 then 1 else 0 end),
 						皮下注射=sum(case when tpm.RescueMeasureCode=400 then 1 else 0 end),
 						--心内,肌肉注射,口服,喷雾吸入,皮下,其他,静脉
 						用药方法数=sum(case when tpm.RescueMeasureCode in(229,231,398,399,400,490,436,329,422) then 1 else 0 end),

                        气压治疗=sum(case when tpm.RescueMeasureCode=351 then 1 else 0 end),
                        颈部固定=sum(case when tpm.RescueMeasureCode=407 then 1 else 0 end),
                        止血包扎大中小换药其他 = sum(case when tpm.RescueMeasureCode in(410,332,333,334) then 1 else 0 end),
 						--大换药,中换药,小换药,气压治疗(真空夹板),颈部固定,其他
 						止血包扎固定数=sum(case when tpm.RescueMeasureCode in(332,333,334,351,407,410) then 1 else 0 end),
                        
 						接生数=sum(case when tpm.RescueMeasureCode in (344,352,353) then 1 else 0 end),
 						单胎顺产接生=sum(case when tpm.RescueMeasureCode = 344 then 1 else 0 end),
 						双胎接生 =sum(case when tpm.RescueMeasureCode = 352 then 1 else 0 end),
 						死胎接生=sum(case when tpm.RescueMeasureCode = 353 then 1 else 0 end),
 						--一般物理降温(冰袋冷敷),海默立克手法,搬运,护送,单胎顺产接生,双胎接生,死胎接生
 						其他数=sum(case when tpm.RescueMeasureCode in(344,352,353,335,402,423,424) then 1 else 0 end),

                        病历数 = isnull(count(*),0)
                        from  M_PatientRecord tpr
                        left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                        left join M_PatientRecordRescue tprr on tpr.TaskCode=tprr.TaskCode and tpr.PatientOrder=tprr.PatientOrder 
                        left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder ");
            //连接调度库
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);

            sb.Append(@"and tpr.SubmitLogo='true' 
                        group by tpr.Station,tpr.PatientVersion,tpr.DoctorAndNurse,tpr.AgentWorkID  
                        order by tpr.Station,tpr.PatientVersion,tpr.DoctorAndNurse  
                        set nocount off ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }

        #endregion

        #region 急救耗材统计表

        public DataTable Get_TJ_HC(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                        set nocount on
                        select
                        急救医护人=case when tpr.DoctorAndNurse='' then '无' else tpr.DoctorAndNurse end,
                        留置针=sum(case when tps.SanitationCode in(449,450) then tps.NumberOfTimes else 0 end),
                        气管导管=sum(case when tps.SanitationCode =432 then tps.NumberOfTimes else 0 end),
                        颈托=sum(case when tps.SanitationCode=374 then tps.NumberOfTimes else 0 end),
                        心电电极贴片=sum(case when tps.SanitationCode=392 then tps.NumberOfTimes else 0 end),
                        三角巾=sum(case when tps.SanitationCode=375 then tps.NumberOfTimes else 0 end),
                        绷带=sum(case when tps.SanitationCode=376 then tps.NumberOfTimes else 0 end),
                        敷料=sum(case when tps.SanitationCode=377 then tps.NumberOfTimes else 0 end),
                        耗材总数 = isnull(count(*),0)

                        from M_PatientRecordSanitation tps
                        left join  M_PatientRecord tpr on tps.TaskCode=tpr.TaskCode and tps.PatientOrder=tpr.PatientOrder
                        left join M_PatientRecordRescue tprr on tps.TaskCode=tprr.TaskCode and tps.PatientOrder=tprr.PatientOrder 
                        left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                        left join M_PatientRecordAppend tpra on tpr.TaskCode=tpra.TaskCode and tpr.PatientOrder=tpra.PatientOrder");

            // where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'
            //--and tprr.Sanitations<>''
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);

            sb.Append(@"
                        and tpr.SubmitLogo='true'
                        group by tpr.DoctorAndNurse
                        set nocount off
                    ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region  药品用药量统计表
        public DataTable Get_TJ_YPYYL(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                 select '医护人员'= case when mpr.DoctorAndNurse is null or mpr.DoctorAndNurse='' then '无' else mpr.DoctorAndNurse end,
                        '氯化钠'=sum(case when mprd.DrugCode=275 then mprd.Dosage else '0' end),
                        '葡萄糖'=sum(case when mprd.DrugCode=276 then mprd.Dosage else '0' end),
	                    '甘露醇'=sum(case when mprd.DrugCode=431 then mprd.Dosage else '0' end),
		                '胺碘酮'=sum(case when mprd.DrugCode=260 then mprd.Dosage else '0' end),
		                '纳洛酮'=sum(case when mprd.DrugCode=246 then mprd.Dosage else '0' end),
		                '托拉塞米'=sum(case when mprd.DrugCode=250 then mprd.Dosage else '0' end),
                        '甲泼尼龙琥珀酸钠'=sum(case when mprd.DrugCode=328 then mprd.Dosage else '0' end),  
	                    '注射用血凝酶'=sum(case when mprd.DrugCode=255 then mprd.Dosage else '0' end)
                from M_PatientRecord mpr 
                left join M_PatientRecordDrug mprd on mpr.TaskCode = mprd.TaskCode and mpr.PatientOrder = mprd.PatientOrder                              
                left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append("  where mpr.MedicalRecordGenerationTime >='" + beginTime + "' and  mpr.MedicalRecordGenerationTime<'" + endTime + @"'
                and mpr.SubmitLogo=1 ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by mpr.DoctorAndNurse  ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 器械检查统计表
        public DataTable Get_TJ_QXJC(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                        set nocount on
                      select
                        出车分站= case when mpr.Station=''or mpr.Station is null then '无' else mpr.Station end,
                        事件类型=case when mpr.PatientVersion='' or mpr.PatientVersion is null then '无' else mpr.PatientVersion end,
                        急救医护人=case when mpr.DoctorAndNurse ='' or mpr.DoctorAndNurse is null then '无' else mpr.DoctorAndNurse end,
                        经办人工号=case when mpr.AgentWorkID ='' or mpr.AgentWorkID is null then '无' else mpr.AgentWorkID end,                                 
                        心电监测=sum(case when mprm.RescueMeasureCode=282 then mprm.NumberOfTimes else 0 end),
                        常规心电图检查=sum(case when mprm.RescueMeasureCode=296 then mprm.NumberOfTimes else 0 end),
                        葡萄糖测定=sum(case when mprm.RescueMeasureCode=317 then mprm.NumberOfTimes else 0 end),
                        指脉氧检测=sum(case when mprm.RescueMeasureCode=318 then mprm.NumberOfTimes else 0 end),
                        器械检查=sum(case when mprm.RescueMeasureCode in(282,296,317,318) then mprm.NumberOfTimes else 0 end),
                        病历数= isnull(count(*),0)                                              
                      from  M_PatientRecord mpr
                      left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder
                      left join  M_PatientRecordMeasure mprm on mprm.TaskCode=mpr.TaskCode and mprm.PatientOrder=mpr.PatientOrder
                      ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >='" + beginTime + @"' and mpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(@"and mpr.SubmitLogo=1
                        group by mpr.Station,mpr.PatientVersion,mpr.DoctorAndNurse,mpr.AgentWorkID  
                        order by mpr.Station,mpr.PatientVersion,mpr.DoctorAndNurse  
                        set nocount off ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 急救评分统计表
        public DataTable Get_TJ_JJPF(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                        set nocount on
                        select
						出车分站= case when tpr.Station=''then '无' else tpr.Station end,
                        事件类型=case when tpr.PatientVersion=''then '无' else tpr.PatientVersion end,
                        急救医护人=case when tpr.DoctorAndNurse ='' then '无' else tpr.DoctorAndNurse end,
                        经办人工号=case when tpr.AgentWorkID ='' then '无' else tpr.AgentWorkID end,
						GCS38 =sum(case when tpa.GCSScore>=3 and tpa.GCSScore<=8 then 1 else 0 end),
						GCS815=sum(case when tpa.GCSScore>8 and tpa.GCSScore<=15 then 1 else 0 end),
						GCSScore= sum(case when tpa.GCSScore<>'' then 1 else 0 end),

						TI07=sum(case when tpa.TIScore between 1 and 7 then 1 else 0 end),
						TI818=sum(case when tpa.TIScore between 8 and 18 then 1 else 0 end),
						TIOver18=sum(case when tpa.TIScore>18 then 1 else 0 end),
						TI29=sum(case when tpa.TIScore between 2 and 9 then 1 else 0 end),
						TIOver10=sum(case when tpa.TIScore >10 then 1 else 0 end),
						TI1016=sum(case when tpa.TIScore between 10 and 16 then 1 else 0 end),
						TI1720=sum(case when tpa.TIScore between 17 and 20 then 1 else 0 end),
						TIOver21=sum(case when tpa.TIScore >21 then 1 else 0 end),
						TIScore =sum(case when tpa.TIScore<>'' then 1 else 0 end),

						AP03=sum(case when tpa.ApgarScore between 1 and 3 then 1 else 0 end),
						AP47=sum(case when tpa.ApgarScore between 4 and 7 then 1 else 0 end),
						AP810=sum(case when tpa.ApgarScore between 8 and 10 then 1 else 0 end),
						ApgarScore=sum(case when tpa.ApgarScore<>'' then 1 else 0 end)
						,病历数 = isnull(count(*),0)
						from  M_PatientRecord tpr
                        left join  M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder
                       left join M_PatientRecordRescue tprr on tpr.TaskCode=tprr.TaskCode and tpr.PatientOrder=tprr.PatientOrder 
                    left join  M_PatientRecordAppend tpa on tpa.TaskCode=tpr.TaskCode and tpa.PatientOrder=tpr.PatientOrder");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(" where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <'" + endTime + @"'");
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpa.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);

            sb.Append(@"group by tpr.Station,tpr.PatientVersion,tpr.DoctorAndNurse,tpr.AgentWorkID 
                         order by tpr.Station,tpr.PatientVersion,tpr.DoctorAndNurse  
                        set nocount off
                        ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 出车数,病历数,重大事件数统计表
        public DataTable Get_TJ_BLZDSJ(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                        select
                        出车分站= case when tpr.Station=''then '无' else tpr.Station end,
                        事件类型=case when tpr.PatientVersion=''then '无' else tpr.PatientVersion end,
						人员工号=case when tpr.AgentWorkID ='' then '无' else tpr.AgentWorkID end,
                        姓名=case when tpr.DoctorAndNurse ='' then '无' else tpr.DoctorAndNurse end,
                        出车数量=0,
						病历数=isnull(count(*),0),
						重大事件数=sum(case when tpr.MajorEmergencies ='是' then 1 else 0 end)
						into #temp3 ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.MainConnectionString);
            //sb.Append("left join ").Append(builder.InitialCatalog).Append(".dbo.TTask tt on tt.任务编码=tcrm.任务编码 ");
            sb.Append(@" from ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecord tpr ");
            sb.Append(@" left join ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordMeasure tpm on tpm.TaskCode=tpr.TaskCode and tpm.PatientOrder=tpr.PatientOrder ");
            sb.Append(@"left join ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordRescue tprr on tprr.TaskCode = tpr.TaskCode and tprr.PatientOrder=tpr.PatientOrder");
            sb.Append(@" left join ").Append(builder.InitialCatalog).Append(".dbo.M_PatientRecordAppend tpra on tpra.TaskCode=tpr.TaskCode and tpra.PatientOrder=tpr.PatientOrder ");
            sb.Append(" left join dbo.TStation ts on tpr.OutStationCode=ts.编码 ");
            sb.Append(@"where tpr.MedicalRecordGenerationTime >='" + beginTime + @"' and tpr.MedicalRecordGenerationTime <='" + endTime + @"' 
						and tpr.SubmitLogo='true' ");

            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("tpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("tpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringLike("tprr.Measures", txtMeasures, sb);
            WhereClauseUtility.AddStringEqual("tpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("tpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("tpr.SendAddress", txtSendAddress, sb);
            WhereClauseUtility.AddStringLike("tpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("tpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("tpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tpr.AgentWorkID", agentWorkID, sb);
            sb.Append(@"GROUP BY tpr.Station,tpr.PatientVersion,tpr.AgentWorkID,tpr.DoctorAndNurse
                 
                union all   
				select 出车分站=case when ts.名称='' then '无' else ts.名称 end,
						 事件类型=taet.名称,
						 人员工号=tp.工号,
						 姓名=tp.姓名,
						 出车数量= isnull(count(*),0),
						 病历数=0,
						 重大事件数=0
						 
						from TTask tt 
						inner join dbo.TAlarmEvent tae on tt.事件编码=tae.事件编码 
						left join dbo.TTaskPersonLink tpl on tt.任务编码=tpl.任务编码  
						left join  TPerson tp on tpl.人员编码 =tp.编码 
						left join TStation ts on tt.分站编码 =ts.编码
                        left join TCenter tc on ts.中心编码 =tc.编码
						left join TZAlarmEventType taet on tae.事件类型编码=taet.编码
					
						 where tae.首次受理时刻>='" + beginTime + @"' and tae.首次受理时刻<='" + endTime + @"'
						  and tae.是否测试=0   and tpl.是否有效 =1 and tpl.人员类型编码 in (4,5) and taet.是否有效=1 ");
            WhereClauseUtility.AddStringLike("taet.名称", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("tc.编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("ts.编码", stationCode, sb);
            WhereClauseUtility.AddStringLike("tp.姓名", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("tp.工号", agentWorkID, sb);

            sb.Append(@"
						  group by ts.名称,taet.名称, tp.工号,tp.姓名
						   order by 出车分站,姓名

				select  出车分站,事件类型,人员工号,姓名,出车数量=sum(出车数量),  病历数=sum(病历数) ,重大事件数=sum(重大事件数) 
				from #temp3 
				group by 出车分站,事件类型,人员工号,姓名
				drop table #temp3 ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 拒绝治疗、体检、器械检查、提供病史统计表
        public DataTable Get_TJ_JJZLJC(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 拒绝治疗=sum(case when mpr.IFRefuseTreatment='拒绝治疗' then 1 else 0 end ),
                           拒绝体检=sum(case when  mpra.PhysicalExaminationChoose='拒查' then 1 else 0  end ),
						   拒绝提供病史=sum(case when mpr.PatientVersion='拒绝提供病史' then 1 else 0 end ),
						   拒绝心电图印象=sum(case when mpra.ECGImpressionSelect='拒查' then 1 else 0 end),
						   拒绝血糖=sum(case when mpra.BloodSugarSelect='拒查' then 1 else 0 end),
						   拒绝血氧饱和度=sum(case when mpra.BloodOxygenSaturationSelect='拒查' then 1 else 0 end),
                           病历总数 = isnull(count(*),0)
                    from M_PatientRecord mpr
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder
                     ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码  ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >='" + beginTime + "' and mpr.MedicalRecordGenerationTime <'" + endTime + @"' and SubmitLogo=1 ");
            WhereClauseUtility.AddStringEqual("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 呼救医院统计表
        public DataTable Get_TJ_HJYY(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select  Hospital=substring(LocalAddress,0,Charindex('院',LocalAddress)+1)
                        ,Number1=sum(case when LocalAddress like '%医院%' and 0<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<1  then 1 else 0 end)
		                ,Number2=sum(case when LocalAddress like '%医院%' and 1<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<2  then 1 else 0 end)
		                ,Number3=sum(case when LocalAddress like '%医院%' and 2<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<3  then 1 else 0 end)
		                ,Number4=sum(case when LocalAddress like '%医院%' and 3<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<4  then 1 else 0 end)
		                ,Number5=sum(case when LocalAddress like '%医院%' and 4<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<5  then 1 else 0 end)
		                ,Number6=sum(case when LocalAddress like '%医院%' and 5<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<6  then 1 else 0 end)
		                ,Number7=sum(case when LocalAddress like '%医院%' and 6<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<7   then 1 else 0 end)
		                ,Number8=sum(case when LocalAddress like '%医院%' and 7<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<8  then 1 else 0 end)
		                ,Number9=sum(case when LocalAddress like '%医院%' and 8<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<9  then 1 else 0 end)
		                ,Number10=sum(case when LocalAddress like '%医院%' and 9<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<10 then 1 else 0 end)
		                ,Number11=sum(case when LocalAddress like '%医院%' and 10<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<11 then 1 else 0 end)
		                ,Number12=sum(case when LocalAddress like '%医院%' and 11<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<12 then 1 else 0 end)
		                ,Number13=sum(case when LocalAddress like '%医院%' and 12<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<13  then 1 else 0 end)
		                ,Number14=sum(case when LocalAddress like '%医院%' and 13<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<14  then 1 else 0 end)
		                ,Number15=sum(case when LocalAddress like '%医院%' and 14<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<15  then 1 else 0 end)
                        ,Number16=sum(case when LocalAddress like '%医院%' and 15<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<16  then 1 else 0 end)
		                ,Number17=sum(case when LocalAddress like '%医院%' and 16<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<17 then 1 else 0 end)
		                ,Number18=sum(case when LocalAddress like '%医院%' and 17<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<18 then 1 else 0 end)
                        ,Number19=sum(case when LocalAddress like '%医院%' and 18<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<19  then 1 else 0 end)
		                ,Number20=sum(case when LocalAddress like '%医院%' and 19<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<20  then 1 else 0 end)
		                ,Number21=sum(case when LocalAddress like '%医院%' and 20<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<21  then 1 else 0 end)
		                ,Number22=sum(case when LocalAddress like '%医院%' and 21<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<22  then 1 else 0 end)
		                ,Number23=sum(case when LocalAddress like '%医院%' and 22<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<23  then 1 else 0 end)
		                ,Number24=sum(case when LocalAddress like '%医院%' and 23<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<24  then 1 else 0 end)		               
                        ,时间段合计 = sum(case when LocalAddress like '%医院%' and 0<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<24  then 1 else 0 end)				
                        from M_PatientRecord mpr                                            
                        left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where (PatientVersion='回家' or PatientVersion='一般转院' or PatientVersion='急救转院') and mpr.MedicalRecordGenerationTime>='" + beginTime + "'and mpr.MedicalRecordGenerationTime<'" + endTime + @"' ");
            sb.Append(" and SubmitLogo=1 and substring(LocalAddress,0,Charindex('院',LocalAddress)+1) <>''and substring(LocalAddress,0,Charindex('院',LocalAddress)+1) is not null  ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by substring(LocalAddress,0,Charindex('院',LocalAddress)+1) ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 送达医院统计表
        public DataTable Get_TJ_SDYY(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select  Hospital=substring(SendAddress,0,Charindex('院',SendAddress)+1)
                        ,Number1=sum(case when SendAddress like '%医院%' and 0<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<1  then 1 else 0 end)
		                ,Number2=sum(case when SendAddress like '%医院%' and 1<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<2  then 1 else 0 end)
		                ,Number3=sum(case when SendAddress like '%医院%' and 2<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<3  then 1 else 0 end)
		                ,Number4=sum(case when SendAddress like '%医院%' and 3<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<4  then 1 else 0 end)
		                ,Number5=sum(case when SendAddress like '%医院%' and 4<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<5  then 1 else 0 end)
		                ,Number6=sum(case when SendAddress like '%医院%' and 5<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<6  then 1 else 0 end)
		                ,Number7=sum(case when SendAddress like '%医院%' and 6<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<7   then 1 else 0 end)
		                ,Number8=sum(case when SendAddress like '%医院%' and 7<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<8  then 1 else 0 end)
		                ,Number9=sum(case when SendAddress like '%医院%' and 8<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<9  then 1 else 0 end)
		                ,Number10=sum(case when SendAddress like '%医院%' and 9<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<10 then 1 else 0 end)
		                ,Number11=sum(case when SendAddress like '%医院%' and 10<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<11 then 1 else 0 end)
		                ,Number12=sum(case when SendAddress like '%医院%' and 11<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<12 then 1 else 0 end)
		                ,Number13=sum(case when SendAddress like '%医院%' and 12<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<13  then 1 else 0 end)
		                ,Number14=sum(case when SendAddress like '%医院%' and 13<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<14  then 1 else 0 end)
		                ,Number15=sum(case when SendAddress like '%医院%' and 14<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<15  then 1 else 0 end)
                        ,Number16=sum(case when SendAddress like '%医院%' and 15<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<16  then 1 else 0 end)
		                ,Number17=sum(case when SendAddress like '%医院%' and 16<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<17 then 1 else 0 end)
		                ,Number18=sum(case when SendAddress like '%医院%' and 17<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<18 then 1 else 0 end)
                        ,Number19=sum(case when SendAddress like '%医院%' and 18<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<19  then 1 else 0 end)
		                ,Number20=sum(case when SendAddress like '%医院%' and 19<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<20  then 1 else 0 end)
		                ,Number21=sum(case when SendAddress like '%医院%' and 20<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<21  then 1 else 0 end)
		                ,Number22=sum(case when SendAddress like '%医院%' and 21<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<22  then 1 else 0 end)
		                ,Number23=sum(case when SendAddress like '%医院%' and 22<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<23  then 1 else 0 end)
		                ,Number24=sum(case when SendAddress like '%医院%' and 23<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<24  then 1 else 0 end)		               
                        ,时间段合计 = sum(case when SendAddress like '%医院%' and 0<=datepart(hh,DrivingTime) and datepart(hh,DrivingTime)<24  then 1 else 0 end)	
                        from M_PatientRecord mpr                                              
                        left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码 ");
            sb.Append(" where (PatientVersion='救治' or PatientVersion='一般转院' or PatientVersion='急救转院') and mpr.MedicalRecordGenerationTime>='" + beginTime + "'and mpr.MedicalRecordGenerationTime<'" + endTime + @"' and SendAddress not in ('不送院','不去院','医院') ");
            sb.Append(" and SubmitLogo=1 and substring(SendAddress,0,Charindex('院',SendAddress)+1) <>''and substring(SendAddress,0,Charindex('院',SendAddress)+1) is not null  ");
            WhereClauseUtility.AddStringLike("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);
            sb.Append(" group by substring(SendAddress,0,Charindex('院',SendAddress)+1) ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region 个人业务统计

        #region 个人业务统计优化后 7-21
        //全部调度信息
        public DataTable GetAllStatisticsDD(DateTime beginTime, DateTime endTime)
        {
            string eventCodeB = beginTime.ToString("yyyyMMdd") + "0000000000";
            string eventCodeE = endTime.AddDays(1).ToString("yyyyMMdd") + "0000000000";
            string taskCodeB = eventCodeB + "0000";
            string taskCodeE = eventCodeE + "0000";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                set ansi_warnings off

         select 人员编码,
                分站编码,中心编码,
                count(*) as 当班数 
                into #temp3
                from
                (
                select aps.人员编码
                ,p.分站编码, p.中心编码
                ,上班时间=上次操作时刻,下班时间=操作时刻 
                from dbo.TAmbulancePersonSign aps
                join TPerson p on aps.人员编码=p.编码
                where 人员类型编码 in (3,4,5,6)
                and 上次操作时刻>='" + beginTime + @"' and 上次操作时刻<='" + endTime + @"'
                and 是否上班操作=0
                ) A
                where  datediff(hh,A.上班时间,A.下班时间)>2
                group by A.人员编码,A.分站编码, A.中心编码 
  
     select ttpl.人员编码,
             tp.姓名
            ,tp.工号
			,tp.分站编码
			,ts.名称 as tsName
			,tc.名称 as tcName
			,tp.中心编码
            ,count(*) as 出车次数
            ,sum(case when tae.事件类型编码=0 then 1 else 0 end ) as 急救数
            ,sum(case when tae.事件类型编码=2 then 1 else 0 end ) as 转院数
            ,sum(case when tae.事件类型编码=1 then 1 else 0 end ) as 回家数
            ,avg(case when datediff(second,tt.生成任务时刻,tt.出车时刻)>0 then datediff(second,tt.生成任务时刻,tt.出车时刻) end) as 平均出车时间
            ,avg(case when datediff(second,tt.出车时刻,tt.到达现场时刻)>0 then datediff(second,tt.出车时刻,tt.到达现场时刻) end) as 平均到达现场时间
            ,avg(case when datediff(second,tt.到达现场时刻,tt.离开现场时刻)>0 then datediff(second,tt.到达现场时刻,tt.离开现场时刻) end) as 平均现场抢救时间
            ,avg(case when datediff(second,tt.离开现场时刻,tt.到达医院时刻)>0 then datediff(second,tt.离开现场时刻,tt.到达医院时刻) end) as 平均运送时间
            ,avg(case when datediff(second,tt.到达医院时刻,tt.完成时刻)>0 then datediff(second,tt.到达医院时刻,tt.完成时刻) end) as 平均医院交接时间
            ,avg(case when datediff(second,tt.开始受理时刻,tt.到达现场时刻)>0 then datediff(second,tt.开始受理时刻,tt.到达现场时刻) end) as 平均反应时间
            ,avg(case when datediff(second,tt.出车时刻,tt.完成时刻)>0 then datediff(second,tt.出车时刻,tt.完成时刻) end) as 平均周转时间
             into #temp4
            from TTask tt
            inner join TAlarmEvent tae on tt.事件编码=tae.事件编码
            left join dbo.TAmbulance ta on tt.车辆编码 = ta.车辆编码
            left join dbo.TTaskPersonLink ttpl on tt.任务编码 = ttpl.任务编码 
            left join dbo.TPerson tp on ttpl.人员编码 = tp.编码
            left join dbo.Tstation ts on tp.分站编码 = ts.编码 
            left join dbo.TCenter tc on tc.编码 = ts.中心编码
            where 
	         ttpl.任务编码 >= '" + taskCodeB + @"' and ttpl.任务编码<='" + taskCodeE + @"' 
               and tae.是否测试 = 0  and ttpl.人员编码<='99999' and ttpl.人员类型编码 in (3,4,5,6) 
				and tt.出车时刻 is not null and tt.出车时刻 is not null and tt.到达现场时刻 is not null and 
                 tt.离开现场时刻 is not null and tt.到达医院时刻 is not null and tt.完成时刻 is not null

		        group by ttpl.人员编码,tp.姓名,tp.工号,tp.分站编码,tp.中心编码,ts.名称,tc.名称
	select  
	 PersonCode=t1.人员编码,Name=姓名,WorkCode=工号,station=t1.分站编码,tsName,center=t1.中心编码,tcName
    ,SendCarTimes=isnull(出车次数,0)
	,FirstAidTimes=isnull(急救数,0)
    ,TransTimes=isnull(转院数,0)
    ,GoHomeTimes=isnull(回家数,0),
	Days=isnull(t2.当班数,0),
	AvgDaySendCarTimes=isnull(cast(convert(float,(100.00*出车次数/t2.当班数/100)) as decimal(10,2)),0), 
	AvgSendCarTime=isnull(平均出车时间,0)
    ,AvgArrivingSceneTime=isnull(平均到达现场时间,0) 
	,AvgSceneRescueTime=isnull(平均现场抢救时间,0)
    ,AvgTransportTime=isnull(平均运送时间,0)
    ,AvgHospitalDeliveryTime=isnull(平均医院交接时间,0)
    ,AvgResponseTime=isnull(平均反应时间,0),AvgRevolveTime=isnull(平均周转时间,0) 

    from #temp4 t1 left join #temp3 t2 on t1.人员编码=t2.人员编码 
    order by t1.人员编码

    drop table #temp3
    drop table #temp4
    set ansi_warnings on  
             ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            DataTable dtall = ds.Tables[0];

            return dtall;
        }
        #endregion

        //全部收费相关
        public DataTable GetALLStatisticsCharge(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
          select    mpr.Driver ,mpr.DoctorAndNurse ,mpr.StretcherBearersI,mpr.OutStationCode ,  
                    收费公里数=isnull(sum(chargekm),0) ,车费=isnull(sum(CarFee),0),等候费=isnull(sum(WaitingFee),0),
                    治疗费=sum((isnull(PaidMoney,0)-isnull(CarFee,0)-isnull(WaitingFee,0))),收费金额=isnull(sum(PaidMoney),0)

                    from M_PatientCharge mpc 
                    inner join M_PatientRecord mpr on mpc.TaskCode =mpr.TaskCode and mpc.PatientOrder= mpr.PatientOrder
                    group by mpr.Driver, mpr.DoctorAndNurse ,mpr.StretcherBearersI,mpr.OutStationCode  
            ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            DataTable dtall = ds.Tables[0];
            return dtall;
        }





        #region 个人业务统计_个人管理块
        public PersonalStatisticsInfo GetPersonalStatisticsGL1(DateTime beginTime, DateTime endTime, string name, string workCode, List<int> role)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                          select  收费公里数=isnull(sum(chargekm),0) ,车费=isnull(sum(CarFee),0),等候费=isnull(sum(WaitingFee),0),
                        治疗费=sum((PaidMoney-isnull(CarFee,0)-isnull(WaitingFee,0))),收费金额=isnull(sum(PaidMoney),0)

                         from M_PatientCharge mpc 
                         inner join M_PatientRecord mpr on mpc.TaskCode =mpr.TaskCode and mpc.PatientOrder= mpr.PatientOrder
                         where mpc.Date >= '" + beginTime + "' and mpc.Date <='" + endTime + @"'  
                    ");
            if (role.Contains(10))
            {
                WhereClauseUtility.AddStringEqual("mpr.Driver", name, sb);
            }
            else if (role.Contains(1))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(3))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(28))
            {
                WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", name, sb);
            }
            else
            {
                WhereClauseUtility.AddStringEqual("1", "2", sb);
            }
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();
                if (dr.Read())
                {
                    AEinfo.PKilometer = DBConvert.ConvertStringToString(dr["收费公里数"]);
                    AEinfo.PChargeCarFee = DBConvert.ConvertStringToString(dr["车费"]);
                    AEinfo.PChargeWaitFee = DBConvert.ConvertStringToString(dr["等候费"]);
                    AEinfo.PChargeAidFee = DBConvert.ConvertStringToString(dr["治疗费"]);
                    AEinfo.PCharge = DBConvert.ConvertStringToString(dr["收费金额"]);
                }
                return AEinfo;
            }
        }

        public PersonalStatisticsInfo GetPersonalStatisticsGL2(DateTime beginTime, DateTime endTime, string name, string workCode, List<int> role)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 
                         除去拒绝治疗病历数=sum(case when mpr.IFRefuseTreatment<>'拒绝治疗' then 1 else 0 end), 
                         sum(case when mpr.CPRIFSuccess ='ROSC' then 1 else 0 end ) as ROSC数,
                         count(*) as 病历数 
                         into #temp
                           from M_PatientRecord  mpr  
                            where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            if (role.Contains(10))
            {
                WhereClauseUtility.AddStringEqual("mpr.Driver", name, sb);
            }
            else if (role.Contains(1))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(3))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(28))
            {
                WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", name, sb);
            }
            else
            {
                WhereClauseUtility.AddStringEqual("1", "2", sb);
            }
            sb.Append(@" select isnull(sum(有救治措施除去只有搬运和护送),0) as 有救治措施除去只有搬运和护送
		            into #temp1
		            	from( select  distinct mprm.TaskCode,
					   有救治措施除去只有搬运和护送= (case when mprm.RescueMeasureCode in(229,231,282,296,317,318,320,322,325,329,332,333,334,335,338,341,342,344,351,352,353
						,397,398,399,400,402,404,405,406,407,408,410,418,419,420,422,436,449,450,489,490) 
                        then 1 else 0 end) 
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  

						 where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            if (role.Contains(10))
            {
                WhereClauseUtility.AddStringEqual("mpr.Driver", name, sb);
            }
            else if (role.Contains(1))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(3))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(28))
            {
                WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", name, sb);
            }
            else
            {
                WhereClauseUtility.AddStringEqual("1", "2", sb);
            }
            sb.Append(@"  ) t 

                            select isnull(sum(静脉开通数),0) as 静脉开通数
		                 into #temp2
		                	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in(329,422,436) then 1 else 0 end) as 静脉开通数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            if (role.Contains(10))
            {
                WhereClauseUtility.AddStringEqual("mpr.Driver", name, sb);
            }
            else if (role.Contains(1))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(3))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(28))
            {
                WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", name, sb);
            }
            else
            {
                WhereClauseUtility.AddStringEqual("1", "2", sb);
            }
            sb.Append(@"  ) t1

                    select isnull(sum(心肺复苏数),0) as 心肺复苏数
	                	into #temp3
		            	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in (341,397,449,450) then 1 else 0 end) as  心肺复苏数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            if (role.Contains(10))
            {
                WhereClauseUtility.AddStringEqual("mpr.Driver", name, sb);
            }
            else if (role.Contains(1))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(3))
            {
                WhereClauseUtility.AddStringEqual("mpr.DoctorAndNurse", name, sb);
            }
            else if (role.Contains(28))
            {
                WhereClauseUtility.AddStringEqual("mpr.StretcherBearersI", name, sb);
            }
            else
            {
                WhereClauseUtility.AddStringEqual("1", "2", sb);
            }

            sb.Append(@"  ) t2
			select 除去拒绝治疗病历数,ROSC数,病历数,有救治措施除去只有搬运和护送,静脉开通数,心肺复苏数 
			from  #temp,#temp1,#temp2,#temp3

					 drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp  ");


            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();
                if (dr.Read())
                {
                    AEinfo.PPRNumberExceptRefuseTreatment = DBConvert.ConvertStringToString(dr["除去拒绝治疗病历数"]);
                    AEinfo.PDisposeNumber = DBConvert.ConvertStringToString(dr["有救治措施除去只有搬运和护送"]);
                    AEinfo.PVeinNumber = DBConvert.ConvertStringToString(dr["静脉开通数"]);
                    AEinfo.PROSCNumber = DBConvert.ConvertStringToString(dr["ROSC数"]);
                    AEinfo.PCPRNumber = DBConvert.ConvertStringToString(dr["心肺复苏数"]);
                    AEinfo.PPRNumber = DBConvert.ConvertStringToString(dr["病历数"]);
                }
                return AEinfo;
            }
        }

        #endregion

        #region 个人业务统计_分站调度块

        //根据人员工号取分站编码（TAmbulancePersonSign 最后一次操作的车辆所属的分站）
        public string GetstaionCodeByWorkCodeTAmbulance(string workCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 
	                ta.分站编码
	                from TAmbulance ta
	                where 	车辆编码 = 
	                (	select top 1 aps.[车辆编码]	  
		                from TAmbulancePersonSign aps 
		                where   aps.[人员编码]=(select p.编码 	from  TPerson  p  where p.工号='" + workCode + @"')  
		                order by  aps.上次操作时刻  ) 
                ");
            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            string stationresult = "";
            while (dr.Read())
            {
                CheckModelExt cm = new CheckModelExt();
                cm.ID = dr.GetString(0);

                string stationCode = cm.ID;
                stationresult = stationCode;
            }
            return stationresult;
        }

        #endregion

        #region 个人业务统计_分站管理块


        public StationStatisticsInfo GetStationStatisticsGL2(DateTime beginTime, DateTime endTime, string station)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" 
                        select 
                         除去拒绝治疗病历数=sum(case when mpr.IFRefuseTreatment<>'拒绝治疗' then 1 else 0 end), 
                         sum(case when mpr.CPRIFSuccess ='ROSC' then 1 else 0 end ) as ROSC数,
                         count(*) as 病历数 
                         into #temp
                           from M_PatientRecord  mpr  
                            where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", station, sb);

            sb.Append(@" select isnull(sum(有救治措施除去只有搬运和护送),0) as 有救治措施除去只有搬运和护送
		            into #temp1
		            	from( select  distinct mprm.TaskCode,
					   有救治措施除去只有搬运和护送= (case when mprm.RescueMeasureCode in(229,231,282,296,317,318,320,322,325,329,332,333,334,335,338,341,342,344,351,352,353
						,397,398,399,400,402,404,405,406,407,408,410,418,419,420,422,436,449,450,489,490) 
                        then 1 else 0 end) 
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  

						 where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", station, sb);

            sb.Append(@"  ) t 

                            select isnull(sum(静脉开通数),0) as 静脉开通数
		                 into #temp2
		                	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in(329,422,436) then 1 else 0 end) as 静脉开通数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", station, sb);
            sb.Append(@"  ) t1
                    select isnull(sum(心肺复苏数),0) as 心肺复苏数
	                	into #temp3
		            	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in (341,397,449,450) then 1 else 0 end) as  心肺复苏数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", station, sb);


            sb.Append(@"  ) t2
			select 除去拒绝治疗病历数,ROSC数,病历数,有救治措施除去只有搬运和护送,静脉开通数,心肺复苏数 
			from  #temp,#temp1,#temp2,#temp3

					 drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp  ");

            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                StationStatisticsInfo AEinfo = new StationStatisticsInfo();
                if (dr.Read())
                {
                    AEinfo.SPRNumberExceptRefuseTreatment = DBConvert.ConvertStringToString(dr["除去拒绝治疗病历数"]);
                    AEinfo.SDisposeNumber = DBConvert.ConvertStringToString(dr["有救治措施除去只有搬运和护送"]);
                    AEinfo.SVeinNumber = DBConvert.ConvertStringToString(dr["静脉开通数"]);
                    AEinfo.SROSCNumber = DBConvert.ConvertStringToString(dr["ROSC数"]);
                    AEinfo.SCPRNumber = DBConvert.ConvertStringToString(dr["心肺复苏数"]);
                    AEinfo.SPRNumber = DBConvert.ConvertStringToString(dr["病历数"]);
                }
                return AEinfo;
            }
        }

        #endregion



        #region 个人业务统计_分中心管理块

        public List<CheckModelExt> GetStationCodeByCenter(string selfCenterID)
        {
            List<CheckModelExt> listM = new List<CheckModelExt>();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select 编码 from TStation  where 中心编码= '" + selfCenterID + @"' and 是否有效=1
                 ");
            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
            while (dr.Read())
            {
                CheckModelExt cm = new CheckModelExt();
                cm.ID = dr.GetString(0);
                listM.Add(cm);
            }
            return listM;
        }

        public CenterStatisticsInfo GetCenterStatisticsGL2(DateTime beginTime, DateTime endTime, string ret1)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select   
                           
                         除去拒绝治疗病历数=sum(case when mpr.IFRefuseTreatment<>'拒绝治疗' then 1 else 0 end), 
                         sum(case when mpr.CPRIFSuccess ='ROSC' then 1 else 0 end ) as ROSC数,
                         count(*) as 病历数 
                         into #temp
                           from M_PatientRecord  mpr  
                            where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", ret1, sb);

            sb.Append(@" select isnull(sum(有救治措施除去只有搬运和护送),0) as 有救治措施除去只有搬运和护送
		            into #temp1
		            	from( select  distinct mprm.TaskCode,
					   有救治措施除去只有搬运和护送= (case when mprm.RescueMeasureCode in(229,231,282,296,317,318,320,322,325,329,332,333,334,335,338,341,342,344,351,352,353
						,397,398,399,400,402,404,405,406,407,408,410,418,419,420,422,436,449,450,489,490) 
                        then 1 else 0 end) 
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  

						 where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", ret1, sb);

            sb.Append(@"  ) t 

                            select isnull(sum(静脉开通数),0) as 静脉开通数
		                 into #temp2
		                	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in(329,422,436) then 1 else 0 end) as 静脉开通数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", ret1, sb);
            sb.Append(@"  ) t1
                    select isnull(sum(心肺复苏数),0) as 心肺复苏数
	                	into #temp3
		            	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in (341,397,449,450) then 1 else 0 end) as  心肺复苏数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", ret1, sb);


            sb.Append(@"  ) t2
			select 除去拒绝治疗病历数,ROSC数,病历数,有救治措施除去只有搬运和护送,静脉开通数,心肺复苏数 
			from  #temp,#temp1,#temp2,#temp3

					 drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp  ");

            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                CenterStatisticsInfo AEinfo = new CenterStatisticsInfo();
                if (dr.Read())
                {
                    AEinfo.CPRNumberExceptRefuseTreatment = DBConvert.ConvertStringToString(dr["除去拒绝治疗病历数"]);
                    AEinfo.CDisposeNumber = DBConvert.ConvertStringToString(dr["有救治措施除去只有搬运和护送"]);
                    AEinfo.CVeinNumber = DBConvert.ConvertStringToString(dr["静脉开通数"]);
                    AEinfo.CROSCNumber = DBConvert.ConvertStringToString(dr["ROSC数"]);
                    AEinfo.CCPRNumber = DBConvert.ConvertStringToString(dr["心肺复苏数"]);
                    AEinfo.CPRNumber = DBConvert.ConvertStringToString(dr["病历数"]);
                }
                return AEinfo;
            }
        }
        #endregion


        #region 个人业务统计_中心管理块


        public TotalStatisticsInfo GetTotalStatisticsGL2(DateTime beginTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select    
                         除去拒绝治疗病历数=sum(case when mpr.IFRefuseTreatment<>'拒绝治疗' then 1 else 0 end), 
                         sum(case when mpr.CPRIFSuccess ='ROSC' then 1 else 0 end ) as ROSC数,
                         count(*) as 病历数 
                         into #temp
                           from M_PatientRecord  mpr  
                            where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");


            sb.Append(@" select isnull(sum(有救治措施除去只有搬运和护送),0) as 有救治措施除去只有搬运和护送
		            into #temp1
		            	from( select  distinct mprm.TaskCode,
					   有救治措施除去只有搬运和护送= (case when mprm.RescueMeasureCode in(229,231,282,296,317,318,320,322,325,329,332,333,334,335,338,341,342,344,351,352,353
						,397,398,399,400,402,404,405,406,407,408,410,418,419,420,422,436,449,450,489,490) 
                        then 1 else 0 end) 
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  

						 where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            sb.Append(@"  ) t 

                            select isnull(sum(静脉开通数),0) as 静脉开通数
		                 into #temp2
		                	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in(329,422,436) then 1 else 0 end) as 静脉开通数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");


            sb.Append(@"  ) t1
                    select isnull(sum(心肺复苏数),0) as 心肺复苏数
	                	into #temp3
		            	from
						(select  distinct mprm.TaskCode,(case when mprm.RescueMeasureCode in (341,397,449,450) then 1 else 0 end) as  心肺复苏数
						from M_PatientRecordMeasure mprm 
						inner join M_PatientRecord mpr on mpr.TaskCode = mprm.TaskCode and mpr.PatientOrder = mprm.PatientOrder  
						where mpr.MedicalRecordGenerationTime >= '" + beginTime + "' and mpr.MedicalRecordGenerationTime <='" + endTime + "' ");

            sb.Append(@"  ) t2
			select 除去拒绝治疗病历数,ROSC数,病历数,有救治措施除去只有搬运和护送,静脉开通数,心肺复苏数 
			from  #temp,#temp1,#temp2,#temp3

					 drop table #temp1
                    drop table #temp2
                    drop table #temp3
                    drop table #temp  ");

            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                TotalStatisticsInfo AEinfo = new TotalStatisticsInfo();
                if (dr.Read())
                {
                    AEinfo.TPRNumberExceptRefuseTreatment = DBConvert.ConvertStringToString(dr["除去拒绝治疗病历数"]);
                    AEinfo.TDisposeNumber = DBConvert.ConvertStringToString(dr["有救治措施除去只有搬运和护送"]);
                    AEinfo.TVeinNumber = DBConvert.ConvertStringToString(dr["静脉开通数"]);
                    AEinfo.TROSCNumber = DBConvert.ConvertStringToString(dr["ROSC数"]);
                    AEinfo.TCPRNumber = DBConvert.ConvertStringToString(dr["心肺复苏数"]);
                    AEinfo.TPRNumber = DBConvert.ConvertStringToString(dr["病历数"]);
                }
                return AEinfo;
            }
        }
        #endregion

        #endregion

        #region 回访统计表
        public DataTable Get_TJ_HF(DateTime beginTime, DateTime endTime, string patientVersion, string stationCode, string agentWorkID, string doctorAndnurse,
                 string txtCenter, string txtDiseasesClassification, string txtIllnessClassification, string txtDeathCase, string txtMeasures, string txtFirstAidEffect,
                 string txtFirstImpression, string txtSendAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select 回访数=sum(case when  mprc.DoctorFollowUp <>'' then 1 else 0 end)
                           ,病历数=isnull(count(*),0)
                    from M_PatientRecordCPR mprc                     
                    left join M_PatientRecord mpr on mprc.PatientOrder=mpr.PatientOrder and mprc.TaskCode=mpr.TaskCode 
                    left join M_PatientRecordAppend mpra on mpr.TaskCode=mpra.TaskCode and mpr.PatientOrder=mpra.PatientOrder
                     ");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.AttemperConnectionString);
            sb.Append(" left join ").Append(builder.InitialCatalog).Append(".dbo.TStation ts on mpr.OutStationCode=ts.编码  ");
            sb.Append(" where mpr.MedicalRecordGenerationTime >='" + beginTime + "' and mpr.MedicalRecordGenerationTime <'" + endTime + @"' and SubmitLogo=1 ");
            WhereClauseUtility.AddStringEqual("mpr.PatientVersion", patientVersion, sb);
            WhereClauseUtility.AddInSelectQuery("ts.中心编码", txtCenter, sb);
            WhereClauseUtility.AddInSelectQuery("mpr.OutStationCode", stationCode, sb);
            WhereClauseUtility.AddStringLike("mpr.DoctorAndNurse", doctorAndnurse, sb);
            WhereClauseUtility.AddStringEqual("mpr.AgentWorkID", agentWorkID, sb);
            WhereClauseUtility.AddStringEqual("mpr.DiseasesClassification", txtDiseasesClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.IllnessClassification", txtIllnessClassification, sb);
            WhereClauseUtility.AddStringEqual("mpr.DeathCase", txtDeathCase, sb);
            WhereClauseUtility.AddStringEqual("mpr.FirstAidEffect", txtFirstAidEffect, sb);
            WhereClauseUtility.AddStringLike("mpra.FirstImpression", txtFirstImpression, sb);
            WhereClauseUtility.AddStringLike("mpr.SendAddress", txtSendAddress, sb);

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region  取Balance内容 IM结余相关
        public DataTable Get_BalanceDetial(string MonthTime, string MaterialType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    select  
                        b.ReportTime,t1.Name as TypeName,m .Name , m.MCode,m.Manufacturer,m.Vendor,
                        m.Specification ,t2.Name as Unit, m.GiveMedicineWay,
                        b.BeginningCounts,b.BeginningPrice,b.IncomeCounts,b.IncomePrice,b.PayCounts,
                         b.PayPrice+b.UpdataPrice as PayPrice,
                       b.SurplusCounts,b.SurplusPrice,b.SurplusTime,b.UpdataPrice

                        from I_Balance  b 
                        left join  I_Material m on b.MaterialID = m.ID   
                        left join TDictionary  t1  on m.MTypeID = t1.ID 
                        left join TDictionary t2 on m.Unit = t2.ID 
                        where b.ReportTime = '" + MonthTime + @"' 
                 and  b.MaterialID in (select ID from I_Material im  where im.MTypeID ='" + MaterialType + @"' )
                    and m.IsActive = 1 
                    and t1.IsActive = 1 and t2.IsActive = 1  ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region  根据ParentID取字典表的ID    Balance内容相关。

        public List<CheckModelExt> GetTDictionaryIDByParentID(string ParentID)
        {
            List<CheckModelExt> listM = new List<CheckModelExt>();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        select ID from  TDictionary  where  ParentID = '" + ParentID + @"' and IsActive=1 
                 ");
            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            while (dr.Read())
            {
                CheckModelExt cm = new CheckModelExt();
                cm.ID = dr.GetString(0);
                listM.Add(cm);
            }
            return listM;
        }

        #endregion

        #region 取Balance汇总，IM财务结余相关

        public DataTable Get_BalanceTotal(string MonthTime, string MaterialType)
        {
            StringBuilder sb = new StringBuilder();
            //已弃用SQL
              //select  
              //          b.ReportTime,t1.Name as TypeName, 
              //          sum(isnull(b.BeginningCounts,0)) as BeginningCounts 
              //          ,sum(isnull(b.BeginningPrice,0)) as BeginningPrice 
              //          ,sum(isnull(b.IncomeCounts,0)) as IncomeCounts 
              //          ,sum(isnull(b.IncomePrice,0)) as IncomePrice 
              //          ,sum(isnull(b.PayCounts,0)) as PayCounts 
              //          ,sum(isnull(b.PayPrice,0) +isnull(b.UpdataPrice,0)) as PayPrice 
              //          ,sum(isnull(b.SurplusCounts,0)) as SurplusCounts 
              //          ,sum(isnull(b.SurplusPrice,0)) as SurplusPrice 
					
              //          from I_Balance  b 
              //          left join  I_Material m on b.MaterialID = m.ID   
              //          left join TDictionary  t1  on m.MTypeID = t1.ID 
                      
              //          where b.ReportTime = '" + MonthTime + @"' 
              //   and  b.MaterialID in (select ID from I_Material im  where im.MTypeID  in ('" + MaterialType + @"') )
              //      and m.IsActive = 1 
              //      and t1.IsActive = 1 
              //      group by  b.ReportTime,t1.Name 

            sb.Append(@"
                 
	                    select  * 	
	                    into #temp1  
	                     from TDictionary where ID in  ('" + MaterialType + @"')  and  IsActive =1
	                     select  
                                   ReportTime='" + MonthTime + @"' 
			                       ,#temp1.Name  as TypeName 
				                    ,sum(isnull(b.BeginningCounts,0)) as BeginningCounts 
				                    ,sum(isnull(b.BeginningPrice,0)) as BeginningPrice 
				                    ,sum(isnull(b.IncomeCounts,0)) as IncomeCounts 
				                    ,sum(isnull(b.IncomePrice,0)) as IncomePrice 
				                    ,sum(isnull(b.PayCounts,0)) as PayCounts 
				                    ,sum(isnull(b.PayPrice,0) +isnull(b.UpdataPrice,0)) as PayPrice 
				                    ,sum(isnull(b.SurplusCounts,0)) as SurplusCounts  
				                    ,sum(isnull(b.SurplusPrice,0)) as SurplusPrice   
					                     from  #temp1  
					
                                    left join  I_Material m on m.MTypeID = #temp1.ID     
				                    left join   I_Balance  b  on m.ID = b.MaterialID   and   b.ReportTime = '" + MonthTime + @"' 
				                    where   
				
				                     m.IsActive = 1 
				                    group by    #temp1.Name
				                    drop table	 #temp1         ");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

        #region IM财务，取个人或者分站的结余相关
        public DataTable Get_BalancePersonal(DateTime startMoneth, DateTime endMoneth, string materialType, string storageCode,string balancemonth)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select 
                        d.Name as 分类, b.ReportTime as 月份, m.Name as 物品名称, 
                         m.MCode as 代码,m.Manufacturer as 生产厂家,m.Vendor as 供应商,
                         m.Specification  as 规格,t2.Name  as 度量单位,
                         s.Name as 仓库名称, sum(ChangeSurplus) as 收入数量
                         ,(isnull(b.PayPrice,0)/isnull(b.PayCounts,1)) as 单价, 
                         (sum(ChangeSurplus)*(isnull(b.PayPrice,0)/isnull(b.PayCounts,1))) as 收入合计

                        FROM I_InventoryRecord ir
                        left join I_Material m  on ir.MaterialID  = m.ID  
                        left join TDictionary d on d.ID = m.MTypeID
                        left join TDictionary t2 on m.Unit = t2.ID
                        left join I_Storage  s on ir.StorageCode  =s.StorageID 
                        left join I_Balance b on b.MaterialID = ir.MaterialID 
                        where Is_Entry = 1");
            WhereClauseUtility.AddStringEqual("ir.StorageCode", storageCode, sb);
            sb.Append(@"
                    and b.ReportTime = '"+balancemonth +@"'
                    and OperatorDateTime < '" + endMoneth + @"' and OperatorDateTime>='" + startMoneth + @"' 
                    and ir.MaterialID in (select ID from I_Material im  where 1=1 ");
            WhereClauseUtility.AddStringEqual("im.MTypeID", materialType, sb);
            sb.Append(@"
         )  group by d.Name,m.MTypeID, m.Name, m.MCode,m.Manufacturer,m.Vendor,m.Specification, t2.Name, 
                    b.ReportTime, ir.MaterialID, s.Name, StorageCode, (isnull(b.PayPrice,0)/isnull(b.PayCounts,1))

");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            return ds.Tables[0];
        }
        #endregion

    }
}
