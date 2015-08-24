using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{

    public partial class I_StorageBLL : BaseBLL<I_Storage>
    {

        /// <summary>
        /// 按照SQL仓储信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<I_StorageExt> GetStorageExtListby(string strWhere)
        {
            string sql = @"select s.*, ID=S.StorageID from I_Storage s where " + strWhere;
            return base.DALContext.II_StorageDAL.ExcuteSqlToModelListBy<I_StorageExt>(sql);
        }

        public List<I_Storage> GetStorageListby(string strWhere)
        {
            string sql = @"select * from I_Storage where " + strWhere;
            return base.DALContext.II_StorageDAL.ExcuteSqlToList(sql);
        }

        /// <summary>
        /// 根据用户ID查询所属仓储
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public List<I_Storage> GetStorageListby(int userID)
        {
            string sql = @"select * from I_Storage s where  s.StorageID in (select StorageID from dbo.I_StoragePerson sp where sp.UserID=@UserID)";
            return base.DALContext.II_StorageDAL.ExcuteSqlToList(sql, new SqlParameter("@UserID", userID));
        }

    }
}
