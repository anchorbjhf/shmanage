using Anke.SHManage.Model;
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
    /// 获取任务数据
    /// </summary>
    public class MobileTaskDAL
    {
        /// <summary>
        ///  给移动终端用的任务数据
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public List<MobileTaskInfo> GetMobileTaskListBy(string userCode, string taskCode, ref string errorMsg)
        {
            try
            {

                StringBuilder sbSQL = new StringBuilder();//sql语句
                sbSQL.Append("declare @taskCode char(22) ");
                sbSQL.Append("set @taskCode='" + taskCode + "' ");

                sbSQL.Append("declare @UserCode varchar(50) ");
                sbSQL.Append("set @UserCode=(select 编码 from TPerson where 工号='" + userCode + "') ");

                string sqlStr = @"   select top 20
                                tt.任务编码,
                                tt.用户流水号,
                                case when pr.LocalAddress is null then tae.现场地址 else pr.LocalAddress end as '现场地址',
                                tt.到达现场时刻,
                                case when pr.[Name] is null then tae.患者姓名 else pr.Name end as '患者姓名',
                                tae.事件类型编码,    
								case when pr.PatientVersion is null then tzaet.名称  else pr.PatientVersion end as '事件类型',                               
                                司机=isnull(dbo.GetStr(tt.任务编码,3),''),                                
                                医生=isnull(dbo.GetStr(tt.任务编码,4),''),                              
                                护士=isnull(dbo.GetStr(tt.任务编码,5),''),
								case when pr.[TaskCode] is null then '未填' else '已填' end as '状态'
                                from TTask tt
                                LEFT JOIN TAlarmEvent tae on tae.事件编码= tt.事件编码
                                LEFT JOIN TZAlarmEventType tzaet on tae.事件类型编码 = tzaet.编码
                                LEFT JOIN dbo.TTaskPersonLink  tpl on tpl.任务编码=tt.任务编码
								left join [AKSHManage].[dbo].[M_PatientRecord] pr on pr.[TaskCode]=tt.任务编码

            
                                where  ";
                sbSQL.Append(sqlStr);

                if (taskCode != "")
                {
                    sbSQL.Append(" tt.任务编码<@taskCode And");
                }
                sbSQL.Append(" tpl.人员编码=@UserCode ");
                sbSQL.Append(" order by tt.任务编码 desc");

                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sbSQL.ToString(), null);
                List<MobileTaskInfo> list = new List<MobileTaskInfo>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MobileTaskInfo info = new MobileTaskInfo();
                        info.RenWuBianMa = dr["任务编码"].ToString();
                        info.YongHuLiuShuiHao = dr["用户流水号"].ToString();
                        info.XianChangDiZhi = dr["现场地址"].ToString();
                        info.DaoDaXianChangShiKe = dr["到达现场时刻"].ToString();
                        info.HuanZheXingMing = dr["患者姓名"].ToString();
                        info.EventType = dr["事件类型编码"].ToString();
                        info.EventTypeName = dr["事件类型"].ToString();
                        info.SiJi = dr["司机"].ToString();
                        info.YiSheng = dr["医生"].ToString();
                        info.HuShi = dr["护士"].ToString();
                        info.State = dr["状态"].ToString();
                        list.Add(info);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                List<MobileTaskInfo> listError = null;
                errorMsg = "系统错误：" + ex.Message;
                return listError;
            }
        }
        /// <summary>
        ///  给移动终端用的任务数据
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public void GetMobileTaskInfo(string taskCode, out string HJTel, out string Area, out string EventType)
        {
            try
            {
                HJTel = "";
                Area = "";
                EventType = "";
                StringBuilder sbSQL = new StringBuilder();//sql语句
                sbSQL.Append("declare @taskCode char(22) ");
                sbSQL.Append("set @taskCode='" + taskCode + "' ");


                string sqlStr = @"   select   
                                tt.任务编码,tzaet.名称 as 事件类型 ,tae.呼救电话,tae.区域                       from TTask tt
                                LEFT JOIN TAlarmEvent tae on tae.事件编码= tt.事件编码
                                LEFT JOIN TZAlarmEventType tzaet on tae.事件类型编码 = tzaet.编码                where tt.任务编码=@taskCode ";


                sbSQL.Append(sqlStr);
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sbSQL.ToString(), null);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    HJTel = ds.Tables[0].Rows[0]["呼救电话"].ToString();
                    Area = ds.Tables[0].Rows[0]["区域"].ToString();
                    EventType = ds.Tables[0].Rows[0]["事件类型"].ToString();
                }


            }
            catch (Exception ex)
            {
                HJTel = "";
                Area = "";
                EventType = "";
            }
        }
    }
}
