using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Anke.SHManage.MSSQLDAL
{
    //人员与仓库关系    尤浩
    public partial class I_StoragePersonDAL
    {

        #region 查询人员仓库关系
        public object GetList(int pageSize, int pageIndex, string name, string workCode, string depID, string roletypeName)
        {
            List<I_StoragePersonExt> list = new List<I_StoragePersonExt>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select distinct identity(int,1,1) as 行号,  ID=u.ID, Dep=d.Name, roletypeName=dbo.fmerg_personrole(u.ID),Name=u.Name,WorkCode=u.WorkCode,
	                      storage=dbo.fmerg_personstorage(u.ID) into #temp       
                          from P_UserRole pur 
                          left join  P_User u on pur.UserID = u.ID
                          left join  P_Role r on r.ID = pur.RoleID
                          left join  P_Department d on d.ID = r.DepID
                          where pur.IsActive = 1");            
            WhereClauseUtility.AddStringEqual("u.Name", name, strSQL);
            WhereClauseUtility.AddStringEqual("u.WorkCode", workCode, strSQL);
            WhereClauseUtility.AddStringEqual("d.Name", depID, strSQL);
            WhereClauseUtility.AddStringEqual("r.ID", roletypeName, strSQL);
            strSQL.Append(" order by u.ID ");
            strSQL.Append("select top " + pageSize + " a.* from #temp a where 行号>" + (pageIndex - 1) * pageSize + " order by 行号 ");
            strSQL.Append(" SELECT COUNT(*) FROM #temp t");
            strSQL.Append(" drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    I_StoragePersonExt info = new I_StoragePersonExt();                   
                    info.uid = Convert.ToInt32(dr["ID"]);
                    info.roleName = dr["roletypeName"].ToString();
                    info.userName = dr["Name"].ToString();
                    info.storage = dr["storage"].ToString();
                    info.workCode = dr["WorkCode"].ToString();
                    info.depName = dr["Dep"].ToString();
                    list.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;
        }
        #endregion

        #region 取人员仓库关系
        public List<PersonStorageLinkInfo> GetPersonStorageList(int uid)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select s.StorageID, s.Name ,IsSelected = (select COUNT(*) from I_StoragePerson isp where s.StorageID = isp.StorageID and isp.UserID=@uid) 
                          from I_Storage s 
                          ");
            SqlParameter[] prams = new SqlParameter[] { 
                new SqlParameter("@uid",SqlDbType.Int)
            };
            prams[0].Value = uid;
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), prams);
            List<PersonStorageLinkInfo> lstInfo = new List<PersonStorageLinkInfo>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    PersonStorageLinkInfo info = new PersonStorageLinkInfo();
                    info.ID = dr["StorageID"].ToString();
                    info.Name = dr["Name"].ToString();
                    info.IsSelected = Convert.ToBoolean(dr["IsSelected"]);
                    lstInfo.Add(info);
                }
            }
            int total = Convert.ToInt32(ds.Tables[0].Rows.Count);
            return lstInfo;
        }
        #endregion

        #region 保存
        public bool Update(int uid, List<I_StoragePerson> lstSR)
        {
            bool ret = false;
            if (uid == -1)
                return ret;

            using (SqlConnection con = new SqlConnection(SqlHelper.MainConnectionString))
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        StringBuilder strSQL = new StringBuilder();
                        strSQL.Append("delete from I_StoragePerson where UserID = " + uid + " ");
                        foreach (I_StoragePerson info in lstSR)
                        {
                            strSQL.Append(" insert into I_StoragePerson (UserID,StorageID) values(" +
                                info.UserID + ",'" + info.StorageID + "') ");
                        }
                        int count = SqlHelper.ExecuteNonQuery(con, CommandType.Text, strSQL.ToString(), null);
                        ret = true;
                    }
                    catch (Exception )
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

        #region 获取combobox的值

        //获取combobox姓名
        public object GetNameList()
        {
            List<P_User> list = new List<P_User>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select ID,Name from P_User where IsActive = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_User info = new P_User();
                    info.ID = Convert.ToInt32(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取combobox工号
        public object GetWorkCodeList()
        {
            List<P_User> list = new List<P_User>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select ID,WorkCode from P_User where IsActive = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_User info = new P_User();
                    info.ID = Convert.ToInt32(dr["ID"]);
                    info.Name = dr["WorkCode"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取combobox角色类型
        public object GetRoleTypeList()
        {
            List<P_Role> list = new List<P_Role>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select ID,Name from P_Role where IsActive = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Role info = new P_Role();
                    info.ID = Convert.ToInt32(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        //获取combobox部门名称
        public object GetDepList()
        {
            List<P_Department> list = new List<P_Department>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(@"select ID,Name from P_Department where IsActive = 1");
            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, strSQL.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    P_Department info = new P_Department();
                    info.ID = Convert.ToInt32(dr["ID"]);
                    info.Name = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }
        #endregion
    }
}
