using Anke.SHManage.IDAL;
using Anke.SHManage.Model.BasicEventInfo;
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
    public class AcceptEventDAL : IAcceptEventDAL
    {
        public AcceptEventInfo GetAcceptEventInfoByCode(string code, int orderNum)
        {
            AcceptEventInfo AEinfo;
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT 事件编码=tae.事件编码,受理序号=tae.受理序号,受理类型编码,受理类型子编码,责任受理人编码,呼救电话=tae.呼救电话
            ,电话振铃时刻=tae.电话振铃时刻,开始受理时刻=tae.开始受理时刻,结束受理时刻=tae.结束受理时刻,发送指令时刻,现场地址,等车地址
            ,送往地点,往救地点类型编码,送往地点类型编码,联系人,联系电话,分机,患者姓名,性别
            ,年龄,国籍,主诉,病种判断,病情编码=tale.病情编码,是否需要担架,伤亡人数,特殊要求,是否标注
            ,X坐标,Y坐标,派车列表, 备注=tale.备注,TZET.名称 as 受理类型,TP.姓名 as 责任受理人,病情=tzis.名称 
            ,往救地点类型=tzlat.名称,送往地点类型=tzsat.名称,责任受理人工号=tp.工号

             FROM TAcceptEvent TAE
             inner join TZAcceptEventType TZET on TZET.编码 = TAE.受理类型编码
             inner join TPerson TP on TP.编码 = TAE.责任受理人编码
             left join TAlarmEvent tale on tae.事件编码= tale.事件编码
             left join TZIllState tzis on tzis.编码 = tale.病情编码
             left join TZLocalAddrType tzlat on tzlat.编码 = tale.往救地点类型编码
             left join TZSendAddrType tzsat on tzsat.编码 = tale.送往地点类型编码
            left join TTask tk on tae.事件编码 =tk.事件编码
             where tae.事件编码 = '" + code + @"' and tae.受理序号='" + orderNum + @"'");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null))
            {
                AEinfo = new AcceptEventInfo();
                if (dr.Read())
                {
                    AEinfo.EventCode = DBConvert.ConvertStringToString(dr["事件编码"]);
                    AEinfo.AcceptOrder = Convert.ToInt32(dr["受理序号"]);
                    AEinfo.TypeId = Convert.ToInt32(dr["受理类型编码"]);
                    AEinfo.DetailReasonId = Convert.ToInt32(dr["受理类型子编码"]);
                    AEinfo.AcceptPersonCode = DBConvert.ConvertStringToString(dr["责任受理人编码"]);
                    AEinfo.AcceptPersonWorkID = DBConvert.ConvertStringToString(dr["责任受理人工号"]);
                    AEinfo.AlarmTel = DBConvert.ConvertStringToString(dr["呼救电话"]);
                    //  AEinfo.HangUpTime = DBConvert.ConvertNullableToNullableTime(dr["挂起时刻"].ToString());
                    AEinfo.RingTime = DBConvert.ConvertNullableToNullableTime(dr["电话振铃时刻"].ToString());
                    AEinfo.AcceptBeginTime = DBConvert.ConvertNullableToNullableTime(dr["开始受理时刻"].ToString());
                    AEinfo.AcceptEndTime = DBConvert.ConvertNullableToNullableTime(dr["结束受理时刻"].ToString());
                    AEinfo.CommandTime = DBConvert.ConvertNullableToNullableTime(dr["发送指令时刻"].ToString());
                    AEinfo.LocalAddr = DBConvert.ConvertStringToString(dr["现场地址"]);
                    AEinfo.WaitAddr = DBConvert.ConvertStringToString(dr["等车地址"]);
                    AEinfo.SendAddr = DBConvert.ConvertStringToString(dr["送往地点"]);
                    AEinfo.LocalAddrTypeId = Convert.ToInt32(dr["往救地点类型编码"]);
                    AEinfo.SendAddrTypeId = Convert.ToInt32(dr["送往地点类型编码"]);
                    AEinfo.LinkMan = DBConvert.ConvertStringToString(dr["联系人"]);
                    AEinfo.LinkTel = DBConvert.ConvertStringToString(dr["联系电话"]);
                    AEinfo.Extension = DBConvert.ConvertStringToString(dr["分机"]);
                    AEinfo.PatientName = DBConvert.ConvertStringToString(dr["患者姓名"]);
                    AEinfo.Sex = DBConvert.ConvertStringToString(dr["性别"]);
                    AEinfo.Age = DBConvert.ConvertStringToString(dr["年龄"]);
                    // AEinfo.Folk = DBConvert.ConvertStringToString(dr["民族"]);
                    AEinfo.National = DBConvert.ConvertStringToString(dr["国籍"]);
                    AEinfo.AlarmReason = DBConvert.ConvertStringToString(dr["主诉"]);
                    AEinfo.Judge = DBConvert.ConvertStringToString(dr["病种判断"]);
                    AEinfo.IllStateId = Convert.ToInt32(dr["病情编码"]);
                    AEinfo.IsNeedLitter = Convert.ToBoolean(dr["是否需要担架"]);
                    AEinfo.PatientCount = Convert.ToInt32(dr["伤亡人数"]);
                    AEinfo.SpecialNeed = DBConvert.ConvertStringToString(dr["特殊要求"]);
                    AEinfo.IsLabeled = Convert.ToBoolean(dr["是否标注"]);
                    AEinfo.X = Convert.ToDouble(dr["X坐标"]);
                    AEinfo.Y = Convert.ToDouble(dr["Y坐标"]);
                    AEinfo.AmbulanceList = DBConvert.ConvertStringToString(dr["派车列表"]);
                    AEinfo.Remark = DBConvert.ConvertStringToString(dr["备注"]);
                    AEinfo.AcceptType = DBConvert.ConvertStringToString(dr["受理类型"]);
                    AEinfo.Dispatcher = DBConvert.ConvertStringToString(dr["责任受理人"]);
                    AEinfo.IllState = DBConvert.ConvertStringToString(dr["病情"]);
                    AEinfo.LocalAddrType = DBConvert.ConvertStringToString(dr["往救地点类型"]);
                    AEinfo.SendAddrType = DBConvert.ConvertStringToString(dr["送往地点类型"]);
                    //AEinfo.AcceptPersonWorkID = DBConvert.ConvertStringToString(dr["责任受理人工号"]);
                    // AEinfo.BackUpOne = DBConvert.ConvertStringToString(dr["保留字段1"]);
                    //AEinfo.BackUpTwo = DBConvert.ConvertStringToString(dr["保留字段2"]);
                    // AEinfo.HangUpTime = DBConvert.ConvertNullableToNullableTime(dr["挂起时刻"].ToString());
                }
                return AEinfo;
            }
        }

        public AcceptEventInfo GetEventNode(string code)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(@"  select A.事件编码,受理序号
                        from TAcceptEvent A
                         where A.事件编码='" + code + @"'
                    order by 受理序号 ");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null))
            { // DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.AttemperConnectionString, CommandType.Text, sb.ToString(), null);
                AcceptEventInfo ENinfo = new AcceptEventInfo();
                if (dr.Read())
                {
                    ENinfo.EventCode = DBConvert.ConvertStringToString(dr["事件编码"]);
                    ENinfo.AcceptOrder = Convert.ToInt32(dr["受理序号"]);
                }
                return ENinfo;
            }
        }
        //return ds.Tables[0];

    }
}
