using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
   public class DropDownListBLL
    {
       LSDAL ls = new LSDAL();
       public DataTable GetCenterOnaspx()
       {
           return ls.GetCenterOnaspx();
       }
       public DataTable GetStationOnaspx(string centerID)
       {
           return ls.GetStationOnaspx(centerID);
       }
    }
}
