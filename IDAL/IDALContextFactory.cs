using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    /// <summary>
    /// 数据访问上下文工厂接口
    /// </summary>
    public interface IDALContextFactory
    {
        IDALContext GetDALContext();
    }
}
