using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    /// <summary>
    /// 获取登录人员信息
    /// </summary>
    public class M_UserLoginInfo
    {
        public string LoginName { get; set; }
        public string Name { get; set; }
        public string WorkCode { get; set; }
        public string DispatchSationID { get; set; }
        public string DispatchSubCenterID { get; set; }
        public string RoleID { get; set; }
    }
}
