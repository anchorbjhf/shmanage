using Anke.SHManage.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{

    /// <summary>
    ///  数据操作工程类
    /// </summary>
    public class DALContextFactory:IDALContextFactory
    {
        /// <summary>
        /// 此方法的作用: 在线程中 共用一个 DALContext
        /// </summary>
        /// <returns></returns>
        public IDALContext GetDALContext()
        {
            //从当前线程中 获取 DBContext 数据仓储 对象
            IDALContext dalContext = CallContext.GetData(typeof(DALContextFactory).Name) as  DALContext;
            if (dalContext == null)
            {
                dalContext = new DALContext();
                CallContext.SetData(typeof(DALContextFactory).Name, dalContext); //存入线程槽
            }
            return dalContext;
        }
    }
}
