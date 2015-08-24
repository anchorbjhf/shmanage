using Anke.SHManage.IDAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Anke.SHManage.MSSQLDAL
{
    /// <summary>
    ///  数据库访问上下文工厂类
    /// </summary>
    public class DBContextFactory
    {
        /// <summary>
        /// 创建 EF上下文 对象，在线程中共享上下文对象（线程单实例）
        /// </summary>
        public static DbContext GetDBContext()
        {
            //从当前线程中 获取 EF上下文对象
            DbContext dbContext = CallContext.GetData(typeof(DBContextFactory).Name) as DbContext;
            if (dbContext == null)
            {
                dbContext = new Model.AKSHManageEntities();
                dbContext.Configuration.ValidateOnSaveEnabled = false; //关闭自动验证提升性能
                //将新创建的 ef上下文对象 存入线程
                CallContext.SetData(typeof(DBContextFactory).Name, dbContext);
            }
            return dbContext;
        }
    }
}
