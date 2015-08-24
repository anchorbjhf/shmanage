using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;

namespace Anke.SHManage.IDAL
{
    public partial interface IM_DictionaryDAL
    {
        List<M_Dictionary> GetMSDictionaryInfos(string typeCode, string isPatient);
        List<M_Dictionary> GetMainDictionary(string tableName, string code);

        List<M_Dictionary> GetManagerDictionary(string tableName);

        IList<M_CheckModel> GetCheckBoxModelByTableName(string tableName);
        IList<M_CheckModel> GetCheckBoxModel(string typeCode);

        object GetCheckBoxOrRadioButtonList(string Prefix, string TypeID, string Type);
        object GetCheckBoxListByTableName(string tableName, string Prefix, string Type);
    }
}
