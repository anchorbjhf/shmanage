using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{

    public partial class M_ZCaseTemplateDAL : BaseDAL<M_ZCaseTemplate>, IM_ZCaseTemplateDAL
    {

        IDALContext dalContext = new DALContextFactory().GetDALContext();

        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        //取所有病种(combobox取值)
        public List<M_ZCaseTemplate> GetDiseasesList()
        {

            string sql = @"select *  from  M_ZCaseTemplate  order by SN ";
                          
            List<M_ZCaseTemplate> list = db.Database.SqlQuery<M_ZCaseTemplate>(sql).ToList();
            return list;
        }


        // 查询病种分类

        public object GetDiseaseList(int page, int rows, ref int rowCounts, string strIsActive, string DiseaseID)
        {
            var q = (from m in db.M_ZCaseTemplate

                     select new
                     {
                         ID = m.ID,
                         Name = m.Name,
                         IsActive = m.IsActive,
                         SN = m.SN,
                         AlarmReason = m.AlarmReason,
                         HistoryOfPresentIllness = m.HistoryOfPresentIllness
                     });
                        
                        
            if (!string.IsNullOrEmpty(strIsActive))
            {
                bool isActive = bool.Parse(strIsActive);
                q = q.Where(m => m.IsActive == isActive);
            }
            if (!string.IsNullOrEmpty(DiseaseID))
            {
                int diseaseID = int.Parse(DiseaseID);
                q = q.Where(m => m.ID == diseaseID);
            }
           
            rowCounts = q.Count();
            var r = q.OrderBy(u => u.SN).Skip((page - 1) * rows).Take(rows).ToList();
            return r;

        }
    }
}
