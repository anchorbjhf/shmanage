using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;
using Anke.SHManage.IDAL;
using Anke.SHManage.Utility;
using System.Data.SqlClient;

namespace Anke.SHManage.BLL
{
    /// <summary>
    /// 出入库物资业务
    /// </summary>
    public partial class I_MaterialBLL

    {
        public List<Model.I_Material> GetList(int pageindex,int pageSize, DateTime startTime, DateTime endTime, string vender,
            bool isActive, string mTypeId, string mTypeDetailId, string mName, string mCode)
        {
            I_MaterialBLL mbll = new I_MaterialBLL();
            return mbll.GetPagedList(pageindex, pageSize, p => p.Name == mName && p.MCode == mCode && p.MTypeID == mTypeId  && p.IsActive == isActive && p.CreatorDate > startTime && p.CreatorDate < endTime, p => p.Name == mName);
        }

        /// <summary>
        /// 原始查询，传统sql语句执行方法
        /// </summary>
        /// <returns></returns>

        //private static readonly IBaseDAL<II_MaterialDAL> mDAL = new IBaseDAL<II_MaterialDAL>;
        public object GetMaterialList(int pageindex,int pageSize, DateTime startTime, DateTime endTime, I_Material materialmodel)
        {
            return true;
        }

        /// <summary>
        /// 下拉框获取字典表  
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        //public static IList<CheckModelExt> GetCheckBoxModel(string tableName)
        //{
        //    using (akshmanageentities dbcontext = new akshmanageentities())
        //    {
        //        string list = @"select id =t.id,name =t.name,tags =t.name  from  " + tablename + " t where t.isactive =1";

        //        ilist<checkmodelext> templist = dbcontext.database.sqlquery<checkmodelext>(list).tolist();
        //        return templist;
        //    }
        //}

        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
