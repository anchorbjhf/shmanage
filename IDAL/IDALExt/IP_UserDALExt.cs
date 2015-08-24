using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface IP_UserDAL : IBaseDAL<P_User>
    {
        /// <summary>
        /// 测试专用
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool TestTransaction(P_User user);

    }
}
