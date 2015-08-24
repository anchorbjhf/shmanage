using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;

namespace Anke.SHManage.IDAL
{
   public partial  interface II_StorageRoleDAL
    {
       //查询角色物资关系
       object GetLists(int pageSize, int pageIndex);

       //取角色物资关联
       List<RoleMaterialLinkInfo> GetRoleMaterialList(int roleID);

       //保存修改后的角色物资关系
       bool Update(int roleID, List<I_StorageRole> lstSR);
    }
}
