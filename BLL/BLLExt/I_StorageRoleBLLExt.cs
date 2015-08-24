using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;

namespace Anke.SHManage.BLL
{
   public partial  class I_StorageRoleBLL
    {
          private I_StorageRoleDAL dal = new I_StorageRoleDAL();

       //查询角色物资关系
        public object GetLists(int pageSize, int pageIndex)
       {
           return dal.GetLists(pageSize, pageIndex);
       }

       //取角色物资关联
        public List<RoleMaterialLinkInfo> GetRoleMaterialList(int roleID)
        {
            return dal.GetRoleMaterialList(roleID);
        }
       //保存修改后的角色物资关系
        public bool Update(int roleID, List<I_StorageRole> lstSR )
        {
            return dal.Update(roleID, lstSR);
        }
    }
}
