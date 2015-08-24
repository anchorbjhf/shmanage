using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public class MobileTaskBLL
    {
        MobileTaskDAL idal = new MobileTaskDAL();

        //IMobileTaskDAL idal = DataAccess.GetMobileTaskDAL();
        public List<MobileTaskInfo> GetMobileTaskListBy(string userCode, string taskCode,ref string errorMsg)
        {
            return idal.GetMobileTaskListBy(userCode, taskCode,ref errorMsg);
        }

    }
}
