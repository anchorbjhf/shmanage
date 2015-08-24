using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Anke.SHManage.Model;

namespace Anke.SHManage.MSSQLDAL
{
    public class MaterialListDAL
    {

        /// <summary>
        /// 用于多表相连取数据，left join
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="vender"></param>
        /// <param name="isActive"></param>
        /// <param name="mTypeId"></param>
        /// <param name="mTypeDetailId"></param>
        /// <param name="mName"></param>
        /// <param name="mCode"></param>
        /// <returns></returns>
        //public object GetMaterialList(int page, int rows, string order, string sort, DateTime startTime, DateTime endTime, string vender, bool isActive, string mTypeId, string mTypeDetailId, string mName, string mCode)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select CreatorDate,MTypeID,Name,MCode,Unit,Specification,Manufacturer,Vendor,TransferPrice,IsActive")
        //    strSql.Append("from 
                
        //        )
        //}

    }
}
