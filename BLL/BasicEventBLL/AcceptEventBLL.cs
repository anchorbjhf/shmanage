using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.BasicEventInfo;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
   public class AcceptEventBLL
    {
       private static readonly IAcceptEventDAL m_DAL = DataAccess.CreateAcceptEventDAL();
       private static readonly IAlarmEventDAL aeDAL = DataAccess.CreateAlarmEventDAL();
       //根据事件编码，受理序号获取受理基本信息
       public AcceptEventInfo GetAcceptEventInfoByCode(string code, int orderNum)
       {
            return  m_DAL.GetAcceptEventInfoByCode(code, orderNum);

       }


       //获取事件节点（事件编码和受理序号）目的：母页面用来分配tabs
       public AcceptEventInfo GetEventNode(string code)
       {
           return m_DAL.GetEventNode(code);
       }
       public AlarmEventInfo GetAlarmEventInfoByCode(string code)
       {
           return aeDAL.GetAlarmEventInfoByCode(code);

       }

    }
}
