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
    public class TaskDAL
    {
        /// <summary>
        ///  给移动终端用的任务数据
        /// </summary>
        /// <param name="personCode"></param>
        /// <returns></returns>
        public List<MobileTaskInfo> getMobileTaskListBy(string userCode)
        {
            DateTime BeginTime= new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            DateTime EndTime = BeginTime.AddDays(1);

            StringBuilder sbSQL = new StringBuilder();//sql语句
            sbSQL.Append("declare @BeginTime datetime ");
            sbSQL.Append("declare @EndTime datetime ");
            sbSQL.Append("declare @UserCode varchar(50) ");
            sbSQL.Append("set @BeginTime='" + BeginTime + "' ");
            sbSQL.Append("set @EndTime='" + EndTime + "' ");
            sbSQL.Append("set @UserCode='" + userCode + "' ");
            sbSQL.Append("declare @TaskCodeB char(22) ");
            sbSQL.Append("declare @TaskCodeE char(22) ");
            sbSQL.Append("set @TaskCodeB = convert(char(8),@BeginTime,112)+'00000000000000' ");
            sbSQL.Append("set @TaskCodeE = convert(char(8),dateadd(day,1,@EndTime),112)+'00000000000000' ");
          
            string sqlStr = @"  select *
                                from
                                (select
                                tt.用户流水号,
                                tae.现场地址,
                                tt.到达现场时刻,
                                tae.患者姓名,
                                司机编码= sj.人员编码,
                                司机=sj.姓名,
                                医生编码= isnull(ys.人员编码,''),
                                医生=isnull(ys.姓名,''),
                                护士编码= isnull(hs.人员编码,''),
                                护士=isnull(hs.姓名,'')
                                from TTask tt
                                left join TAlarmEvent tae on tae.事件编码= tt.事件编码
                                Left join TTaskPersonLink sj on sj.任务编码 = tt.任务编码 and sj.人员类型编码=3
                                Left join TTaskPersonLink ys on ys.任务编码 = tt.任务编码 and ys.人员类型编码=4
                                Left join TTaskPersonLink hs on hs.任务编码 = tt.任务编码 and hs.人员类型编码=5
                                where tt.任务编码>@TaskCodeB and tt.任务编码<=@TaskCodeE 
                                ) temp1
                                where temp1.司机编码=@UserCode or temp1.护士编码=@UserCode or temp1.医生编码=@UserCode ";

            sbSQL.Append(sqlStr);

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sbSQL.ToString(), null);
            List<MobileTaskInfo> list = new List<MobileTaskInfo>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MobileTaskInfo info = new MobileTaskInfo();
                    info.YongHuLiuShuiHao = dr["用户流水号"].ToString();
                    info.XianChangDiZhi = dr["现场地址"].ToString();
                    info.DaoDaXianChangShiKe = dr["到达现场时刻"].ToString();
                    info.HuanZheXingMing = dr["患者姓名"].ToString();
                    info.SiJi = dr["司机"].ToString();
                    info.YiSheng = dr["医生"].ToString();
                    info.HuShi = dr["护士"].ToString();
                    list.Add(info);
                }
            }

            return list;

           
        }
    }
}
