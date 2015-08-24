using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    /// <summary>
    /// 权限扩展方法
    /// </summary>
    public partial class P_PermissionDAL : BaseDAL<P_Permission>, IP_PermissionDAL
    {
        AKSHManageEntities dbContext = DBContextFactory.GetDBContext() as AKSHManageEntities;
        public object getViewPermission(int id)
        {
            return 1;
        }
    }

}
