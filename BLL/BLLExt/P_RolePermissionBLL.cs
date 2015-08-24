using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class P_PermissionBLL : BaseBLL<P_Permission>
    {
        /// <summary>
        /// 根据角色 查询 其权限集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<Model.P_Permission> GetPermissionByRoleId(int roleId)
        {
            //查询出 角色id 和参数相等，并且 排除掉 权限1
            return null;
        }

    }
}
