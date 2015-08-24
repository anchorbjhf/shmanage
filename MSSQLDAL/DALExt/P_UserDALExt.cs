using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class P_UserDAL : BaseDAL<P_User>, IP_UserDAL
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        /// <summary>
        /// 测试更新  
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool TestTransaction(P_User user)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {

                    P_User temp = dalContext.IP_UserDAL.GetModelWithOutTrace(p => p.ID == 5);
                    temp.SN = 222;

                    P_User u = new P_User();
                    u.ID = temp.ID;
                    u.SN = 222;

                    dalContext.IP_UserDAL.Modify(u, "SN");

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception )
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务

                    return false;
                }
            }
        }

    }
}
