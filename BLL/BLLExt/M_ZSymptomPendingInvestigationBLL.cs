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
    public partial class M_ZSymptomPendingInvestigationBLL : BaseBLL<M_ZSymptomPendingInvestigation>
    {
        /// <summary>
        /// 查症状待查
        /// </summary>       
        /// <returns></returns>
        public List<M_ZSymptomPendingInvestigation> GetSymptomList()
        {
            string sql = @"select * from M_ZSymptomPendingInvestigation   order by SN ";
            return base.DALContext.IM_ZSymptomPendingInvestigationDAL.ExcuteSqlToList(sql);
        }

        //通过id查到此id对应的IsActive，SN，PinYin字段的值
        public M_ZSymptomPendingInvestigationExt GetSymptomList(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select IsActive,SN,PinYin  from M_ZSymptomPendingInvestigation where ID='" + id + "'");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                M_ZSymptomPendingInvestigationExt info = new M_ZSymptomPendingInvestigationExt();
                if (dr.Read())
                {
                    info.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    info.SN = Convert.ToInt32(dr["SN"]);
                    info.PinYin = DBConvert.ConvertStringToString(dr["PinYin"]);

                }
                return info;
            }
        }

        // 取最大ID的数据
        public List<M_ZSymptomPendingInvestigation> GetMaxID()
        {
            string sql = @" select * from M_ZSymptomPendingInvestigation where id in (select max(id) from M_ZSymptomPendingInvestigation)";
            return base.DALContext.IM_ZSymptomPendingInvestigationDAL.ExcuteSqlToList(sql);
        }

        //  禁用/启用
        public bool Update(int id, bool isActive)
        {

            bool ret = false;
            using (SqlConnection con = new SqlConnection(SqlHelper.MainConnectionString))
            {

                StringBuilder sb = new StringBuilder();
                sb.Append(@" with cte as 
                           (
                            select * from M_ZSymptomPendingInvestigation where id='" + id + @"'
                            union all
                            select m.* from M_ZSymptomPendingInvestigation m inner join cte c  on m.ParentID=c.ID
                            and m.ID<>c.ID
                            )
                            update M_ZSymptomPendingInvestigation set IsActive='" + isActive + "'  where ID in(select ID from cte) ");
                using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
                {

                    if (dr.Read())
                    {

                        int count = SqlHelper.ExecuteNonQuery(con, CommandType.Text, sb.ToString(), null);
                    }
                    ret = true;
                }
            }

            return ret;

        }
    }
}

