using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{
  /// <summary>
    /// 角色权限树 视图实体
    /// </summary>
    public class RolePemissionTree
    {
        /// <summary>
        /// 某角色的权限
        /// </summary>
        public List<Model.P_Permission> RolePers { get; set; }
        /// <summary>
        /// 系统中所有权限
        /// </summary>
        public List<Model.P_Permission> AllPers { get; set; }
        /// <summary>
        /// 所有父权限
        /// </summary>
        public List<Model.P_Permission> AllParentPers { get; set; }
    }
}
