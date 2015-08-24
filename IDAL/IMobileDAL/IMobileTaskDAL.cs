using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public interface IMobileTaskDAL
    {
        List<MobileTaskInfo> GetMobileTaskListBy(string userCode);
    }
}
