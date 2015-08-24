using Anke.SHManage.IDAL;
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
    public class AlarmEventDAL : IAlarmEventDAL
    //: IAlarmEventDAL
    {
        public AlarmEventInfo GetAlarmEventInfoByCode(string EventCode)
        {
            AlarmEventInfo aeInfo = new AlarmEventInfo();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"SELECT 事件编码,首次呼救电话,首次受理时刻,事件名称=(现场地址+患者姓名),
            首次调度员编码,受理次数,区域,执行任务总数,当前执行任务数,等车地址,
            是否挂起,TZET.名称 as 事件类型,TZEO.名称 as 事件来源 
            ,TZAT.名称 as 事故类型,TZAL.名称 as 事故等级,TP.姓名 as 首次调度员, 
			首次调度员工号=tp.工号,TAE.是否测试 
            ,TAE.中心编码,中心名称=tc.名称 
             FROM TAlarmEvent TAE 
             left join TZAlarmEventType TZET on TZET.编码 = TAE.事件类型编码
             left join TZAlarmEventOrigin TZEO on TZEO.编码 =TAE.事件来源编码
             left join TZAccidentType TZAT on TZAT.编码 = TAE.事故类型编码
             left join TZAccidentLevel TZAL on TZAL.编码 =TAE.事故等级编码
             left join TPerson TP on TP.编码=TAE.首次调度员编码
            left join TCenter tc on tc.编码=TAE.中心编码
             where tae.事件编码 = '" + EventCode + @"'");
            using (SqlDataReader sdr = SqlHelper.ExecuteReader(SqlHelper.AttemperConnectionString, CommandType.Text, strSQL.ToString(), null))
            {
                if (sdr.Read())
                {
                    aeInfo.EventCode = sdr["事件编码"].ToString();
                    aeInfo.FirstAlarmCall = sdr["首次呼救电话"].ToString();
                    aeInfo.EvetnName = sdr["事件名称"].ToString();
                    aeInfo.FirstAcceptTime = DBConvert.ConvertNullableToNullableTime(sdr["首次受理时刻"].ToString());
                  //  aeInfo.FirstDisptcher = sdr["首次调度员编码"].ToString();
                    aeInfo.FirstDisptcherWorkID = sdr["首次调度员工号"].ToString();//2013.04.11 刘爱青 add
                    aeInfo.AcceptCount = Convert.ToInt32(sdr["受理次数"]);
                   // aeInfo.FirstSendAmbTime = DBConvert.ConvertNullableToNullableTime(sdr["首次派车时刻"].ToString());
                    aeInfo.Area = sdr["区域"].ToString();
                    aeInfo.TransactTaskCount = Convert.ToInt32(sdr["执行任务总数"]);
                    aeInfo.NonceTransactTaskCount = Convert.ToInt32(sdr["当前执行任务数"]);
                    aeInfo.IsHangUp = Convert.ToBoolean(sdr["是否挂起"]);
                    aeInfo.EventType = sdr["事件类型"].ToString();
                   // aeInfo.EventTypeCode = Convert.ToInt32(sdr["事件类型编码"]);
                    aeInfo.EventSource = sdr["事件来源"].ToString();
                   // aeInfo.EventSourceCode = Convert.ToInt32(sdr["事件来源编码"]);
                    aeInfo.AccidentType = sdr["事故类型"].ToString();
                  //  aeInfo.AccidentTypeCode = Convert.ToInt32(sdr["事故类型编码"]);
                    aeInfo.AccidentLevel = sdr["事故等级"].ToString();
                 //   aeInfo.AccidentLevelCode = Convert.ToInt32(sdr["事故等级编码"]);
                    aeInfo.FirstDisptcherName = sdr["首次调度员"].ToString();
                    aeInfo.IsTest = Convert.ToBoolean(sdr["是否测试"]);
                    aeInfo.CenterCode = Convert.ToInt32(sdr["中心编码"]);
                    aeInfo.CenterName = sdr["中心名称"].ToString();
                }
            }
            return aeInfo;
        }

    }
}
