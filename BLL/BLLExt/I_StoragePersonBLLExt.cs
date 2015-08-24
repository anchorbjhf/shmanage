using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
   public partial class I_StoragePersonBLL
    {
        private I_StoragePersonDAL dal = new I_StoragePersonDAL();      

       //查询人员仓库关系
        public object GetList(int pageSize, int pageIndex,  string name, string workCode, string stationName, string roletypeName)
        {
            return dal.GetList(pageSize, pageIndex, name, workCode, stationName, roletypeName);
        }
       //取人员仓库关系
        public List<PersonStorageLinkInfo> GetPersonStorageList(int userID)
        {
            return dal.GetPersonStorageList(userID);
        }
       //保存
        public bool Update(int userID, List<I_StoragePerson> lstSR)
        {
            return dal.Update(userID, lstSR);
        }
        //取姓名
        public object GetNameList()
        {
            return dal.GetNameList();
        }
        //取工号
        public object GetWorkCodeList()
        {
            return dal.GetWorkCodeList();
        }
       //取角色类型
        public object GetRoleTypeList()
        {
            return dal.GetRoleTypeList();
        }
       //取部门名称
        public object GetDepList()
        {
            return dal.GetDepList();
        }
        /// <summary>
        /// 返回仓储人员
        /// </summary>
        /// <returns></returns>
        public object GetALLStoragePersons()
        {
            return dal.GetDynamicListWithOrderBy(s => s.UserID != -1,  s => new { s.UserID, s.P_User.Name },s => s.UserID).Distinct();
        }

    }
}
