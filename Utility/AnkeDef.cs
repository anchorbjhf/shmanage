using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Utility
{
    public enum E_JsonResult
    {
        OK = 0,
        Error = 1,
        NoLogin = 2
    }

    public enum E_IMPermisson
    {
        RKHC = 47,
        ZJXG = 48,
        RK = 49,
        RKnoZJ = 54,
        SLXG = 68,
        SLSP = 69,
        CKALLSL = 71

    }
    /// <summary>
    /// 统计权限枚举
    /// </summary>
    public enum E_StatisticsPermisson
    {
        None = -1,
        SELF = 57,
        STATION = 58,
        CENTER = 59,
        ALL = 60,
        ROLE = 61,
        Name = 62,
        LINKDD = 72,
        LINKZJ = 73,
        LINKLD = 74
    }




}
