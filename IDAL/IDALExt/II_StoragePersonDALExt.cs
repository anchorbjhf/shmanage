using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
   public partial interface II_StoragePersonDAL
    {
       //查询人员仓库关系
       object GetList(int pageSize, int pageIndex, string name, string workCode, string stationName, string roletypeName);
       //取人员仓库关系
       List<PersonStorageLinkInfo> GetPersonStorageList(int userID);
       //保存
       bool Update(int userID, List<I_StoragePerson> lstSR);
       //取角色类型
       object GetRoleTypeList();
       //取部门名称
       object GetDepList();
    }
}
