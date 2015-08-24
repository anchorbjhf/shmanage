﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IBLL
{
    /// <summary>
    ///  业务父接口
    /// </summary>
    public interface IBaseBLL<T> where T: class,new()
    {
        #region 新增实体 int Add(T model)
        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Add(T model);

        #endregion

        /***************************删除*********************/
        #region 根据Model删除对象  int Del(T model);
        /// <summary>
        /// 根据 id 删除
        /// </summary>
        /// <param name="model">包含要删除id的对象</param>
        /// <returns></returns>
        int Del(T model);

        #endregion

        #region  根据条件删除  int DelBy(Expression<Func<T, bool>> delWhere);
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="delWhere"></param>
        /// <returns></returns>
        int DelBy(Expression<Func<T, bool>> delWhere);

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
        int Modify(T model, params string[] proNames);

        #endregion

        #region 批量修改（反射方式 性能较低慎用） int Modify(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        /// <summary>
        /// 批量修改  效率较低慎用
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames);

        #endregion


        /***************************查询**********************/
        #region 根据条件查询    List<T> GetListBy(Expression<Func<T,bool>> whereLambda)
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">传入条件Lambda表达式</param>
        /// <returns></returns>
        List<T> GetListBy(Expression<Func<T, bool>> whereLambda);

        #endregion

        #region 根据条件查询并排序   List<T> GetListBy<TKey>
        /// <summary>
        /// 根据条件查询并排序
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <param name="orderLambda">排序条件 lambda表达式</param>
        /// <returns></returns>
        List<T> GetListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda);

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
        List<T> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy);

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
        List<T> GetPagedList<TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, bool isAsc = true);

        #endregion

        /************************不用EF缓存方式查询 *******************/
        #region 根据条件获取一个    T GetModelWithOutTrace(Expression<Func<T, bool>> whereLambda)
        /// <summary>
        /// 根据条件查询一个不被EF上下文跟踪的对象 
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        T GetModelWithOutTrace(Expression<Func<T, bool>> whereLambda);

        #endregion
    }
}