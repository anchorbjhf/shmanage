using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;

namespace Anke.SHManage.BLL
{
    public partial class M_DictionaryBLL : BaseBLL<M_Dictionary>
    {
        private readonly IM_DictionaryDAL m_DAL = DataAccess.CreateM_DictionaryDAL();
        private const string m_M_DictionaryTreeKey = "M_DictionaryKey";
        private static object m_SyncRoot = new object();
        public List<M_Dictionary> GetMSDictionaryInfos(string typeCode, string isPatient)
        {
            return m_DAL.GetMSDictionaryInfos(typeCode, isPatient);
        }
        public List<M_Dictionary> GetMainDictionary(string tableName, string code)
        {
            return m_DAL.GetMainDictionary(tableName,code);
        }
        public List<M_Dictionary> GetManagerDictionary(string tableName)
        {
            return m_DAL.GetManagerDictionary(tableName);
        }
        public IList<M_CheckModel> GetCheckBoxModelByTableName(string tableName)
        {
            return m_DAL.GetCheckBoxModelByTableName(tableName);
        }
        public IList<M_CheckModel> GetCheckBoxModel(string typeCode)
        {
            return m_DAL.GetCheckBoxModel(typeCode);
        }
        public object GetCheckBoxOrRadioButtonList(string Prefix, string TypeID, string Type)
        {
            return m_DAL.GetCheckBoxOrRadioButtonList(Prefix, TypeID, Type);
        }
        public object GetCheckBoxListByTableName(string tableName, string Prefix, string Type)
        {
            return m_DAL.GetCheckBoxListByTableName(tableName, Prefix, Type);
        }
    }
}
