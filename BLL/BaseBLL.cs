
using Anke.SHManage.IDAL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public abstract class BaseBLL<T> where T:class,new()
    {

        /// <summary>
        /// 构造函数实例化对象时候 强制实例化父类 idal属性
        /// </summary>
        public BaseBLL()
        {
            SetDAL();
        }

        //数据层接口对象: 设计用来被子类重写
        protected IBaseDAL<T> idal;// = new I_EntryDAL(); 根据工厂获取具体的实现类
 
        /// <summary>
        /// 抽象方法：业务父类里的idal对象赋值 模板模式
        /// </summary>
        public abstract void SetDAL();

        /// <summary>
        ///  数据访问上下文接口（可以获取所有的数据实现类对象）
        /// </summary>
        private IDALContext iDAlContext;

        #region 数据访问上下文属性   IDALContext DALContext
        /// <summary>
        /// 数据仓储 属性
        /// </summary>
        public IDALContext DALContext
        {
            get
            {
                if (iDAlContext == null)
                {
                    // 读取web.config文件 appSetting部分 获取DLL路径获取DAL
                    string strDALPath = AppConfig.GetConfigString("DALPath");
                    string strDALFactroyClassName = AppConfig.GetConfigString("DALFactroyClassName");
                    //通过反射创建 DALContextFactory 工厂对象
                    Assembly dalDLL = Assembly.Load(strDALPath);
                    string classFullName = strDALPath + "." + strDALFactroyClassName;
                    Type typeDBSessionFatory = dalDLL.GetType(classFullName);
                    IDALContextFactory iDAlContextFac = Activator.CreateInstance(typeDBSessionFatory) as IDALContextFactory;

                    //通过 数据访问上下文工厂 创建 DALContext对象
                   iDAlContext = iDAlContextFac.GetDALContext();
                }
                return iDAlContext;
            }
        }
        #endregion

        /***************************新增*********************/
        #region 新增实体 int Add(T model)
        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int Add(T model)
        {
            try
            {
                return idal.Add(model);
            }
            catch (Exception)
            {

                return -1;
            }
            
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
            try
            {
                return idal.Del(model);
            }
            catch (Exception)
            {
                return -1;
            }
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
            try
            {
                return idal.DelBy(delWhere);
            }
            catch (Exception)
            {
                return -1;
            }
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
            try
            {
                return idal.Modify(model, proNames);
            }
            catch (Exception)
            {
                return -1;
            }
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
            try
            {
                return idal.ModifyBy(model, whereLambda, modifiedProNames);
            }
            catch (Exception)
            {
                return -1;
            }
            
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
            try
            {
                return idal.GetListBy(whereLambda);
            }
            catch (Exception)
            {
                return null;
            }
          
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
            try
            {
                return idal.GetListBy(whereLambda, orderLambda);
            }
            catch (Exception)
            {
                return null;
            }
           
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
            try
            {
                return idal.GetPagedList(pageIndex, pageSize, whereLambda, orderBy);
            }
            catch (Exception)
            {
                return null;
            }
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
            try
            {
                return idal.GetPagedList<TKey>(pageIndex, pageSize, ref rowCount, whereLambda, orderBy, isAsc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        
        #region 根据条件查询 -进化版   List<T> GetListBy<TKey>
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="selector">映射器</param>
        /// <returns></returns>
        public List<dynamic> GetDynamicListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, dynamic>> selector)
        {
            try
            {
                return idal.GetDynamicListBy<TKey>(whereLambda, selector);
            }
            catch (Exception)
            {
                return null;
            }
           
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
        public List<dynamic> GetDynamicListWithOrderBy<TKey>(Expression<Func<T, bool>> whereLambda,  Expression<Func<T, dynamic>> selector,Expression<Func<T, TKey>> orderLambda)
        {
            try
            {
                return idal.GetDynamicListWithOrderBy<TKey>(whereLambda, selector, orderLambda);
            }
            catch (Exception)
            {
                return null;
            }
           
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
        public List<dynamic> GetDynamicPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, Expression<Func<T, dynamic>> selector)
        {
            try
            {
                return idal.GetDynamicPagedList<TKey>(pageIndex, pageSize, whereLambda, orderBy, selector);
            }
            catch (Exception)
            {
                return null;
            }
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
            try
            {
                return idal.GetDynamicPagedList<TKey>(pageIndex, pageSize, ref rowCount, whereLambda, orderBy, selector, isAsc);
            }
            catch (Exception)
            {
                return null;
            }
            
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
            try
            {
                return idal.GetModelWithOutTrace(whereLambda);
            }
            catch (Exception)
            {

                return null;
            }
          
        }
        #endregion

    }
}
