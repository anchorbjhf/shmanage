using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Anke.SHManage.Model;

namespace Anke.SHManage.IDAL
{
    public interface IM_DictionaryTreeDAL
    {
        List<CommonTree> GetMSDictionaryTreeInfoList(string tableName, string ParentID, string TypeID);

        List<M_ChargeItemInfo> GetMSChargeItemTreeInfoList(string ParentID, string TypeID);
    }
}
