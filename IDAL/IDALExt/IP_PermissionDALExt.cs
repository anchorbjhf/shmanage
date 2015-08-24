using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface IP_PermissionDAL
    {
        object getViewPermission(int id);
    }
}
