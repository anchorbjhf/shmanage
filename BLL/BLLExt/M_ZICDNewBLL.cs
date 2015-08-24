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
    public partial class M_ZICDNewBLL : BaseBLL<M_ZICDNew>
    {
      
        // 查初步印象
        public List<M_ZICDNew> GetImpressionList()
        {
            string sql = @"select * from M_ZICDNew   order by SN ";
            return base.DALContext.IM_ZICDNewDAL.ExcuteSqlToList(sql);
        }

        //通过id查到此id对应的IsActive，SN，PinYin字段的值
        public M_ZICDNewExt GetImpressionList(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select IsActive,SN,PinYin  from M_ZICDNew where ID='" + id + "'");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                M_ZICDNewExt info = new M_ZICDNewExt();
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
        public List<M_ZICDNew> GetMaxID()
        {
            string sql = @" select * from M_ZICDNew where id in (select max(id) from M_ZICDNew)";
            return base.DALContext.IM_ZICDNewDAL.ExcuteSqlToList(sql);
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
                            select * from M_ZICDNew where id='" + id + @"'
                            union all
                            select m.* from M_ZICDNew m inner join cte c  on m.ParentID=c.ID
                            and m.ID<>c.ID
                            )
                            update M_ZICDNew set IsActive='" + isActive + "'  where ID in(select ID from cte) ");
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

