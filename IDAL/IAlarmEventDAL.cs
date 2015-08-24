using Anke.SHManage.Model;
using Anke.SHManage.Model.BasicEventInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
   public interface IAlarmEventDAL
    {
       AlarmEventInfo GetAlarmEventInfoByCode(string code);
    }
}
