using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Anke.SHManage.Utility;
using Anke.SHManage.Model.ViewModel;
using System.Transactions;

namespace Anke.SHManage.MSSQLDAL
{
    // 角色与物资关系   尤浩 
    
    public partial class I_StorageRoleDAL
    {
        #region 查询角色物资关系
        public object GetLists(int pageSize, int pageIndex)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@" select  ID , RoleName = Name, MaterialType = dbo.fmerg(ID)                       
                          from P_Role");
            //strSQL.Append(" select top " + pageSize + " a.* from #temp a where ID>" + (pageIndex - 1) * pageSize + " order by ID ");
            //strSQL.Append(" SELECT COUNT(*) FROM #temp ");
            //strSQL.Append(" drop table #temp");
            
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            List<I_StorageRoleExt> list = new List<I_StorageRoleExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    I_StorageRoleExt info = new I_StorageRoleExt();
                    info.roleID = Convert.ToInt32(dr["ID"]);
                    info.roleName = dr["RoleName"].ToString();
                    info.materialTypeName = dr["MaterialType"].ToString();
                    list.Add(info);
                }
            }
            //int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            //var result = new { total = total, rows = list };
            return list;
        }
        #endregion

        #region 取角色物资关联
        public List<RoleMaterialLinkInfo> GetRoleMaterialList(int roleID)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"SELECT dic.ID ,dic.Name ,IsSelected = (select COUNT(*) from I_StorageRole isr where dic.ID = isr.MaterialType and isr.RoleID = @roleID) 
                           FROM TDictionary dic
                           where TypeID = 'MaterialType' and ParentID='-1' and IsActive = 1");
            SqlParameter[] prams = new SqlParameter[] { 
                new SqlParameter("@roleID",SqlDbType.Int)
            };
            prams[0].Value = roleID;
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), prams);


            List<RoleMaterialLinkInfo> lstInfo = new List<RoleMaterialLinkInfo>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    RoleMaterialLinkInfo info = new RoleMaterialLinkInfo();
                    info.ID = dr["ID"].ToString();
                    info.Name = dr["Name"].ToString();
                    info.IsSelected = Convert.ToBoolean(dr["IsSelected"]);
                    lstInfo.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[0].Rows.Count);
            //var result = new { total = total, rows = lstInfo };

            return lstInfo;
        }
        #endregion

        #region 保存
        public bool Update(int roleID, List<I_StorageRole> lstSR)
        {
            bool ret = false;
            if (roleID == -1)
                return ret;

            using (SqlConnection con = new SqlConnection(SqlHelper.MainConnectionString))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        StringBuilder strSQL = new StringBuilder();
                        strSQL.Append("delete from I_StorageRole where RoleID = " + roleID + " ");
                        foreach (I_StorageRole info in lstSR)
                        {
                            strSQL.Append(" insert into I_StorageRole (RoleID,MaterialType) values(" +
                                info.RoleID + ",'" + info.MaterialType + "') ");
                        }
                        int count = SqlHelper.ExecuteNonQuery(con, CommandType.Text, strSQL.ToString(), null);
                        ret = true;
                    }
                    catch (Exception)
                    {
                        ret = false;
                        //日志
                        //错误处理
                    }
                    ts.Complete();
                }
            }
            return ret;
        }
        #endregion
    }
}
