
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    /// <summary>
    /// 数据库访问层模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDAL<T> where T : class,new()
    {
       
        //创建DbContext对象 这里为了 性能考虑用工厂模式为每个请求线程创建一个DbContext
        DbContext db = DBContextFactory.GetDBContext();

        #region 新增实体 int Add(T model)
        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int Add(T model)
        {
            db.Set<T>().Add(model);
            return db.SaveChanges();//保存成功后，会将自增的id设置给 model的 主键属性，并返回受影响行数
        }
        #endregion 

        /***************************删除*********************/
        #region 根据Model删除对象  int Del(T model)
        /// <summary>
        /// 根据 id 删除
        /// </summary>
        /// <param name="model">包含要删除id的对象</param>
        /// <returns></returns>
        public virtual int Del(T model)
        {
            //先附加给上下文
            db.Set<T>().Attach(model);
            //再将附加的model标识为“删除”状态
            db.Set<T>().Remove(model);
            return db.SaveChanges();
        }
        #endregion

        #region  根据条件删除  int DelBy(Expression<Func<T, bool>> delWhere)
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="delWhere"></param>
        /// <returns></returns>
        public virtual int DelBy(Expression<Func<T, bool>> delWhere)
        {
            //查询要删除的数据
            List<T> listDeleting = db.Set<T>().Where(delWhere).ToList();
            //将要删除的数据 用删除方法添加到 EF 容器中
            listDeleting.ForEach(u =>
            {
                db.Set<T>().Attach(u);//先附加到 EF容器
                db.Set<T>().Remove(u);//标识为 删除 状态
            });
            //一次性 生成sql语句到数据库执行删除
            return db.SaveChanges();
        }
        #endregion 

        /***************************修改**********************/
        #region 修改单个实体   int Modify(T model, params string[] proNames)
        /// <summary>
        /// 修改  调用方式如下：
        /// T u = new T() { uId = 1, uLoginName = "asdfasdf" };
        /// this.Modify(u, "uLoginName");
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="proNames">要修改的 属性 名称数组</param>
        /// <returns></returns>
        public virtual int Modify(T model, params string[] proNames)
        {
            //将 对象 添加到 EF中
            DbEntityEntry entry = db.Entry<T>(model);
            //先设置 对象的包装 状态为 Unchanged
            entry.State = System.Data.Entity.EntityState.Unchanged;
            //循环 被修改的属性名 数组
            foreach (string proName in proNames)
            {
                //将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                entry.Property(proName).IsModified = true;
            }
            //一次性 生成sql语句到数据库执行
            return db.SaveChanges();
        }



        public virtual int Modify(T model)
        {

            DbEntityEntry entry = db.Entry<T>(model);

            entry.State = System.Data.Entity.EntityState.Unchanged;


            Type t = typeof(T); // model.GetType();
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            //循环 被修改的属性名 数组
            foreach (PropertyInfo proInfo in proInfos)
            {
                //将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                entry.Property(proInfo.ToString()).IsModified = true;
            }

            //一次性 生成sql语句到数据库执行
            return db.SaveChanges();
        }

        #endregion

        #region 批量修改（反射方式 性能较低慎用） int Modify(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        /// <summary>
        /// 批量修改  效率较低慎用
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        public virtual int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        {
            //查询要修改的数据
            List<T> listModifing = db.Set<T>().Where(whereLambda).ToList();

            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
            //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //创建 实体属性 字典集合
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
            proInfos.ForEach(p =>
            {
                if (modifiedProNames.Contains(p.Name))
                {
                    dictPros.Add(p.Name, p);
                }
            });

            //循环 要修改的属性名
            foreach (string proName in modifiedProNames)
            {
                //判断 要修改的属性名是否在 实体类的属性集合中存在
                if (dictPros.ContainsKey(proName))
                {
                    //如果存在，则取出要修改的 属性对象
                    PropertyInfo proInfo = dictPros[proName];
                    //取出 要修改的值
                    object newValue = proInfo.GetValue(model, null); //object newValue = model.某属性;

                    //批量设置 要修改 对象的 属性
                    foreach (T tempobj in listModifing)
                    {
                        //为 要修改的对象 的 要修改的属性 设置新的值
                        proInfo.SetValue(tempobj, newValue, null); //tempobj.某属性 = newValue;
                    }
                }
            }
            //一次性 生成sql语句到数据库执行
            return db.SaveChanges();
        }
        #endregion


        /***************************查询**********************/
        #region 根据条件查询    List<T> GetListBy(Expression<Func<T,bool>> whereLambda)
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">传入条件Lambda表达式</param>
        /// <returns></returns>
        public virtual List<T> GetListBy(Expression<Func<T, bool>> whereLambda)
        {
            //List<int> listIDs = new List<int>() { 1, 2, 3 };
            //new MODEL.UserInfo.Where(u => listIDs.Contains(u.ID)).OrderBy(u=>u.ID);  
            return db.Set<T>().Where(whereLambda).ToList();
        }
        #endregion

        #region 根据条件查询并排序   List<T> GetListBy<TKey>
        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <param name="orderLambda">排序条件 lambda表达式</param>
        /// <returns></returns>
        public virtual List<T> GetListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda)
        {
            //List<int> listIDs = new List<int>() { 1, 2, 3 };
            //new MODEL.UserInfo.Where(u => listIDs.Contains(u.ID)).OrderBy(u=>u.ID);  
            return db.Set<T>().Where(whereLambda).OrderBy(orderLambda).ToList();
        }
        #endregion

        #region  分页查询   List<T> GetPagedList<TKey>
        /// <summary>
        /// 分页查询 
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public virtual List<T> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion

        #region 分页查询并返回总记录条数    List<T> GetPagedList<TKey>
        /// <summary>
        ///  分页查询并返回总记录条数
        /// </summary>
        /// <typeparam name="TKey">查询实体类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="rowCount">总记录数</param>
        /// <param name="whereLambda">条件lambda</param>
        /// <param name="orderBy">排序 lambda</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public virtual List<T> GetPagedList<TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, bool isAsc = true)
        {
            rowCount = db.Set<T>().Where(whereLambda).Count();

            if (isAsc)
            {
                return db.Set<T>().OrderBy(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return db.Set<T>().OrderByDescending(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        #endregion 



        #region 根据条件查询 -进化版   List<T> GetListBy<TKey>
        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <returns></returns>
        public virtual List<dynamic> GetDynamicListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, dynamic>> selector)
        {
            return db.Set<T>().Where(whereLambda).Select(selector).ToList();
        }
        #endregion

        #region 根据条件查询并排序 -进化版   List<T> GetListBy<TKey>
        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <param name="orderLambda">排序条件 lambda表达式</param>
        /// <returns></returns>
        public virtual List<dynamic> GetDynamicListWithOrderBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, dynamic>> selector, Expression<Func<T, TKey>> orderLambda)
        {
            return db.Set<T>().Where(whereLambda).OrderBy(orderLambda).Select(selector).ToList();
        }
        #endregion

        #region  分页查询- 进化版 List<T> GetPagedList<TKey>
        /// <summary>
        /// 分页查询 
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public virtual List<dynamic> GetDynamicPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, Expression<Func<T, dynamic>> selector)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().Where(whereLambda).OrderBy(orderBy).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion

        #region 分页查询并返回总记录条数 - 进化版，返回List 数据

        /// <summary>
        /// 分页查询按照Select器，返回List 数据
        /// </summary>
        /// <typeparam name="TKey">查询实体类型</typeparam>
        /// <param name="selector"></param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        public List<dynamic> GetDynamicPagedList<TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, Expression<Func<T, dynamic>> selector, bool isAsc = true)
        {
            rowCount = db.Set<T>().Where(whereLambda).Count();
            if (isAsc)
            {
                return db.Set<T>().OrderBy(orderBy).Where(whereLambda).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return db.Set<T>().OrderByDescending(orderBy).Where(whereLambda).Select(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        #endregion

        /************************传统SQL语句方式***********************/

        #region 传统sql语句执行方法   int ExcuteSql(string strSql, params object[] paras)
        /// <summary>
        /// 传统sql语句执行方法
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual int ExcuteSql(string strSql, params object[] paras)
        {
            return db.Database.ExecuteSqlCommand(strSql, paras);
        }


        /// <summary>
        /// 获取List方法
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual List<T> ExcuteSqlToList(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<T>(strSql, paras).ToList();
        }


        /// <summary>
        /// 查看指定类型
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual List<TElement> ExcuteSqlToModelListBy<TElement>(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<TElement>(strSql, paras).ToList();
        }

        #endregion

        /************************不用EF缓存方式查询 *******************/
        #region 根据条件获取一个    T GetModelWithOutTrace(Expression<Func<T, bool>> whereLambda)
        /// <summary>
        /// 根据条件查询一个不被EF上下文跟踪的对象 
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T GetModelWithOutTrace(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().AsNoTracking().FirstOrDefault(whereLambda);
        }
        #endregion

    }
}
