using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class M_ZCaseTemplateBLL : BaseBLL<M_ZCaseTemplate>
    {
        private M_ZCaseTemplateDAL dal = new M_ZCaseTemplateDAL();

        // 获取所有病种 （combobox用）   
        public List<M_ZCaseTemplate> GetDiseasesList()
        {
            return dal.GetDiseasesList();
        }

        // 查询所有病种     
        public object GetDiseaseList(int page, int rows, ref int rowCounts, string strIsActive, string DiseaseID)
        {
            return dal.GetDiseaseList(page, rows, ref rowCounts, strIsActive, DiseaseID);
        }

        //取M_ZCaseTemplate表中id最大的一条数据
        public List<M_ZCaseTemplate> GetMaxID()
        {
            string sql = @" select * from M_ZCaseTemplate where id in (select max(id) from M_ZCaseTemplate)";
            return base.DALContext.IM_ZCaseTemplateDAL.ExcuteSqlToList(sql);
        }

    }
}
