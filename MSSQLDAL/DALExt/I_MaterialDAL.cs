using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_MaterialDAL : BaseDAL<I_Material>, II_MaterialDAL
    {

        IDALContext dalContext = new DALContextFactory().GetDALContext();

        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;


        #region 根据字典表上级编码来获取管理库字典表信息(物资分类，细分类使用 已弃用 )  朱传海 2015-3-27
        /// <summary>
        /// 根据字典表上级编码来获取管理库字典表信息(物资分类，细分类使用)  朱传海 2015-3-27
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IList<M_CheckModel> GetCheckBoxModelByparentID(string parentID)
        {
            using (AKSHManageEntities dbContext = new AKSHManageEntities())
            {
                string list = @"select Id =t.Name,Name =t.Name,Tags =t.Name  from dbo.TDictionary t where t.IsActive =1 and t.ParentID='" + parentID + "' order by t.SN";

                //<M_CheckModel>只是一个模型，获取的list，以<M_CheckModel>模型对应的字段传给Templist list需要与（<M_CheckModel>）模型的属性，一一对应，名称和类型也要相同，必要是需要转换(int 转string)。
                IList<M_CheckModel> Templist = dbContext.Database.SqlQuery<M_CheckModel>(list).ToList();
                return Templist;
            }
        }
        #endregion

        #region 获取物资仓库的ID 和Name
        /// <summary>
        /// 获取物资仓库的ID 和Name
        /// </summary>
        /// <returns></returns>
        public IList<CheckModelExt> GetStorage()
        {
            using (AKSHManageEntities dbContext = new AKSHManageEntities())
            {
                string list = @"select ID =t.StorageID,Name =t.Name from dbo.I_Storage t";

                //IList<CheckModelExt>只是一个模型，获取的list，以<M_CheckModel>模型对应的字段传给Templist list需要与（<M_CheckModel>）模型的属性，一一对应，名称和类型也要相同，必要是需要转换(int 转string)。
                IList<CheckModelExt> Templist = dbContext.Database.SqlQuery<CheckModelExt>(list).ToList();
                return Templist;
            }
        }
        #endregion

        #region 出库单查询  朱传海
        /// <summary>
        /// 出库单查询  朱传海
        /// </summary>
        /// <returns></returns>
        public object GetDeliveryOrder(int page, int rows, DateTime startTime, DateTime endTime, string deliveryType,
         string deliveryCode, string entryStorageCode, string operatorName, string mName, string receivingStoreID, string consigneeName, string mCode)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.MainConnectionString);//取管理库的链接字符串
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"select identity(int,1,1) as 行号, 
                                     mName=tm.Name, 
                                     DeliveryCode =d.DeliveryCode, 
                                     DeliveryTime = d.DeliveryTime, 
                                     operatorName=tp1.Name, 
                                     consigneeName=tp2.Name, 
                                     BatchNo =dd.BatchNo, 
                                     EntryStorageName =s1.Name, 
                                     ReceivingStoreName=s.Name, 
                                     DeliveryCounts = dd.DeliveryCounts, 
                                     DeliveryType=td.Name, 
                                     Remark =d.Remark 
                                     into #temp 
                                     from I_Delivery d 
                                     left join I_DeliveryDetail dd on dd.DeliveryCode=d.DeliveryCode 
                                     left join I_Material tm on tm.ID=dd.MaterialID 
                                     left join P_User tp1 on tp1.ID=d.OperatorCode 
                                     left join P_User tp2 on tp2.ID=d.ConsigneeID 
                                     left join I_Storage s on s.StorageID=d.ReceivingStoreID 
                                     left join I_Storage s1 on s1.StorageID=dd.EntryStorageCode 
                                     left join TDictionary td on td.ID=d.DeliveryType
                                     where 1 = 1 
                                        ");
            WhereClauseUtility.AddDateTimeGreaterThan("d.DeliveryTime", startTime, sbSQL);
            WhereClauseUtility.AddDateTimeLessThan("d.DeliveryTime", endTime, sbSQL);

            WhereClauseUtility.AddStringEqual("d.DeliveryCode", deliveryCode, sbSQL);

            WhereClauseUtility.AddStringLike("tp1.Name", operatorName, sbSQL);

            WhereClauseUtility.AddStringLike("tp2.Name", consigneeName, sbSQL);

            WhereClauseUtility.AddStringEqual("dd.ReceivingStoreID", receivingStoreID, sbSQL);

            WhereClauseUtility.AddStringEqual("dd.EntryStorageCode", entryStorageCode, sbSQL);

            WhereClauseUtility.AddStringLike("tm.Name", mName, sbSQL);

            WhereClauseUtility.AddStringEqual("tm.MCode", mCode, sbSQL);

            WhereClauseUtility.AddStringEqual("d.DeliveryType", deliveryType, sbSQL);

            sbSQL.Append(" group by d.DeliveryCode,tm.Name,d.DeliveryTime,tp1.Name,tp2.Name,dd.BatchNo,s1.Name,s.Name,dd.DeliveryCounts,td.Name,d.Remark ");
            sbSQL.Append("order by d.DeliveryCode desc ");
            sbSQL.Append(" select top " + rows + " A.*  from #temp A where 行号>" + (page - 1) * rows + " order by 行号 ");
            sbSQL.Append(" SELECT COUNT(*) FROM #temp ");
            sbSQL.Append(" drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sbSQL.ToString(), null);
            List<I_DeliveryOrder> list = new List<I_DeliveryOrder>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                I_DeliveryOrder info = new I_DeliveryOrder();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new I_DeliveryOrder();

                    info.mName = dr["mName"].ToString();
                    info.DeliveryCode = dr["DeliveryCode"].ToString();
                    info.DeliveryTime = Convert.ToDateTime(dr["DeliveryTime"]);
                    info.OperatorName = dr["operatorName"].ToString();
                    info.ConsigneeName = dr["consigneeName"].ToString();
                    info.Remark = dr["Remark"].ToString();
                    info.BatchNo = dr["BatchNo"].ToString();
                    info.TotalDelivery = dr["DeliveryCounts"].ToString();
                    info.ReceivingStoreName = dr["ReceivingStoreName"].ToString();
                    info.EntryStorageName = dr["EntryStorageName"].ToString();
                    info.DeliveryType = dr["DeliveryType"].ToString();
                    ////info. = Convert.ToInt32(dr["Code"]);
                    //info.DeliveryType = Convert.ToInt32(dr["DeliveryType"]);
                    //info.ItemCode = dr["ItemCode"].ToString() == "" ? -1 : Convert.ToInt32(dr["ItemCode"]);
                    list.Add(info);
                }
            }
            else
            {
                list = null;
                var resultK = new { total = 0, rows = 0 };//当查询没有数据返回;
                return resultK;
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            return result;
        }
        #endregion

        #region 物资基本信息查询 朱传海

        public object GetMaterialList(int page, int rows, DateTime startTime, DateTime endTime,
            string vender, string isActive, string mTypeId, string mName, string mCode)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.MainConnectionString);//取管理库的链接字符串
            StringBuilder sbSQL = new StringBuilder();
            //bool isActive1 = Convert.ToBoolean("isActive");
            sbSQL.Append(@"select identity(int,1,1) as 行号, 
                                    ID= u.ID,
                                     CreatorDate = u.CreatorDate, 
                                     Name = u.Name, 
                                     MCode = u.MCode, 
                                     Unit = isnull(td1.Name,u.Unit),
                                     Specification = u.Specification, 
                                     Manufacturer = u.Manufacturer, 
                                     Vendor = u.Vendor, 
                                     AlarmCounts = u.AlarmCounts,
                                     RealPrice = u.RealPrice, 
                                     TransferPrice = u.TransferPrice, 
                                     Remark = u.Remark, 
                                     MTypeID = td.name, 
                                     isActive = u.IsActive 
                                     into #temp 
                                     from I_Material u 
                                     left join TDictionary td on td.ID=u.MTypeID 
                                     left join TDictionary td1 on td1.ID =u.Unit 
                                     where 1 = 1 ");
            WhereClauseUtility.AddDateTimeGreaterThan("u.CreatorDate", startTime, sbSQL);
            WhereClauseUtility.AddDateTimeLessThan("u.CreatorDate", endTime, sbSQL);
            WhereClauseUtility.AddStringEqual("u.isActive", isActive, sbSQL);
            WhereClauseUtility.AddStringLike("u.Vendor", vender, sbSQL);
            //WhereClauseUtility.AddStringLikeOr("u.MTypeID", mTypeId,"", sbSQL);
            WhereClauseUtility.AddStringLike("u.Name", mName, sbSQL);
            WhereClauseUtility.AddStringLike("u.MCode", mCode, sbSQL);
            if (!string.IsNullOrEmpty(mTypeId))
            {
                sbSQL.Append(" AND (u.MTypeID = '" + mTypeId + "' or td.ParentID ='" + mTypeId + "') ");
            }
            sbSQL.Append("order by u.CreatorDate desc ");
            sbSQL.Append(" select top " + rows + " A.*  from #temp A where 行号>" + (page - 1) * rows + " order by 行号 ");
            sbSQL.Append(" SELECT COUNT(*) FROM #temp ");
            sbSQL.Append(" drop table #temp");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sbSQL.ToString(), null);
            List<MaterialInfo> list = new List<MaterialInfo>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                MaterialInfo info = new MaterialInfo();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new MaterialInfo();
                    info.ID = Convert.ToInt32(dr["ID"]);
                    info.CreatorDate = Convert.ToDateTime(dr["CreatorDate"]);
                    info.Name = dr["Name"].ToString();
                    info.MCode = dr["MCode"].ToString();
                    info.Unit = dr["Unit"].ToString();
                    info.Specification = dr["Specification"].ToString();
                    info.Manufacturer = dr["Manufacturer"].ToString();
                    info.Vendor = dr["Vendor"].ToString();
                    info.AlarmCounts = Convert.ToInt32(dr["AlarmCounts"]);
                    info.RealPrice = Convert.ToDecimal(dr["RealPrice"]);
                    info.TransferPrice = Convert.ToDecimal(dr["TransferPrice"]);
                    info.MTypeID = dr["MTypeID"].ToString();
                    info.IsActive = Convert.ToBoolean(dr["isActive"]);
                    info.Remark = dr["Remark"].ToString();
                    list.Add(info);
                }

                int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                var result = new { total = total, rows = list };
                return result;
            }
            else
            {
                list = null;
                var resultK = new { total = 0, rows = 0 };//当查询没有数据返回;
                return resultK;
            }

        }
        #endregion

        #region 过期物资查询 朱传海
        public object GetOverdue(int page, int rows, DateTime startTime, DateTime endTime, string mTypeId, string mName, int remainTime, string mCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select identity(int,1,1) as 行号,
		                            MTypeID = td1.Name,
		                            Name = m.Name,
		                            MCode = m.MCode,
		                            ValidDate = s.ValidityDate,
		                            EntryCode = ed.EntryCode,
		                            EntryData = ed.EntryDate,
		                            Manufacturer = m.Manufacturer,
		                            Vendor = m.Vendor,
		                            Unit = m.Unit,
		                            Specification = m.Specification,
		                            Storage = td2.Name,
		                            Surplus = s.Surplus,
                                    isOverdue = (case when s.ValidityDate < convert(char(10),getdate(),120) then 1 else 0 end)
		                         into #temp
	                            from  I_Surplus s
	                            left join I_Material m  on m.ID = s.MaterialID
	                            left join I_EntryDetail ed on  ed.MaterialID = m.ID
	                            left join TDictionary td1 on td1.ID = m.MTypeID
	                            left join TDictionary td2 on td2.ID = s.StorageCode ");

            WhereClauseUtility.AddDateTimeGreaterThan("ed.EntryDate", startTime, sb);
            WhereClauseUtility.AddDateTimeLessThan("ed.EntryDate", endTime, sb);
            WhereClauseUtility.AddStringLike("m.Name", mName, sb);
            //WhereClauseUtility.AddStringEqual("m.MTypeID", mTypeId, sb);
            if (!string.IsNullOrEmpty(mTypeId))
            {
                sb.Append(" AND m.MTypeID = '" + mTypeId + "' or td1.ParentID = '" + mTypeId + "' ");
            }

            WhereClauseUtility.AddStringEqual("m.mCode", mCode, sb);

            //将现在获取的时间DateTime.Now，加上告警天数的值，与有效期做比较，有效期小于这个值。
            DateTime times = Convert.ToDateTime(DateTime.Now.AddDays(remainTime).ToString("yyyy-MM-dd"));
            WhereClauseUtility.AddDateTimeLessThan("s.ValidityDate", times, sb);

            sb.Append(@"select top " + rows + " A.*  from #temp A where 行号>" + (page - 1) * rows + " order by 行号 ");
            sb.Append(@"SELECT count(*) FROM #temp t ");
            sb.Append("drop table #temp ");

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            List<I_OverdueExt> list = new List<I_OverdueExt>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                I_OverdueExt info = new I_OverdueExt();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new I_OverdueExt();
                    info.MTypeID = dr["MTypeID"].ToString();
                    info.Name = dr["Name"].ToString();
                    info.MCode = dr["MCode"].ToString();
                    info.ValidDate = Convert.ToDateTime(dr["ValidDate"]);
                    info.EntryCode = dr["EntryCode"].ToString();
                    info.EntryData = Convert.ToDateTime(dr["EntryData"]);
                    info.Manufacturer = dr["Manufacturer"].ToString();
                    info.Vendor = dr["Vendor"].ToString();
                    info.Unit = dr["Unit"].ToString();
                    info.Specification = dr["Specification"].ToString();
                    info.Storage = dr["Storage"].ToString();
                    info.Surplus = Convert.ToDouble(dr["Surplus"]);
                    info.isOverdue = Convert.ToBoolean(dr["isOverdue"]);
                    list.Add(info);
                }
            }
            else
            {
                list = null;
                var resultK = new { total = 0, rows = 0 };//当查询没有数据返回;
                return resultK;
            }
            int total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            var result = new { total = total, rows = list };
            //return list;
            return result;
        }
        #endregion

        /// <summary>
        /// 获取基础物资
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="rowCounts">返回数据总数</param>
        /// <param name="manufacturer">生产商</param>
        /// <param name="vender">供应商</param>
        /// <param name="strIsActive">是否有效</param>
        /// <param name="listmTypeId">物资分类编码List</param>
        /// <param name="mCode">物资编码</param>
        /// <returns></returns>
        public object GetMaterialList(int page, int rows, ref int rowCounts, string manufacturer, string vender, string strIsActive, List<string> listmTypeId, string mID)
        {
            var q = (from m in db.I_Material
                     select new
                           {
                               ID = m.ID,
                               Name = m.Name,
                               MTypeID = m.MTypeID,
                               OtherTypeID = m.OtherTypeID,
                               Manufacturer = m.Manufacturer,
                               Vendor = m.Vendor,
                               Unit = m.Unit,
                               Specification = m.Specification,
                               QRCode = m.QRCode,
                               Remark = m.Remark,
                               CreatorName = m.CreatorName,
                               CreatorDate = m.CreatorDate,
                               PinYin = m.PinYin,
                               IsActive = m.IsActive,
                               RealPrice = m.RealPrice,
                               AlarmCounts = m.AlarmCounts,
                               MCode = m.MCode,
                               FeeScale = m.FeeScale,
                               LimitMaxPrice=m.LimitMaxPrice,
                               SN=m.SN,
                               GiveMedicineWay=m.GiveMedicineWay,

                               MTypeName = m.TDictionary1.Name, //翻译MTypeID 名称
                               UnitName = m.TDictionary2.Name,//翻译Unit 名称
                           });

            //q = q.Where(m => m.IsActive == isActive && m.MCode.Equals(mCode));

            if (!string.IsNullOrEmpty(manufacturer))
            {
                q = q.Where(m => m.Manufacturer.Contains(manufacturer));
            }
            if (!string.IsNullOrEmpty(vender))
            {
                q = q.Where(m => m.Vendor.Contains(vender));
            }
            if (!string.IsNullOrEmpty(strIsActive))
            {
                bool isActive = bool.Parse(strIsActive);
                q = q.Where(m => m.IsActive==isActive);
            }
            if (listmTypeId != null)
            {
                q = q.Where(m => listmTypeId.Contains(m.MTypeID));
            }
            if (!string.IsNullOrEmpty(mID))
            {
                int id = int.Parse(mID);
                q = q.Where(m => m.ID.Equals(id));
            }

            rowCounts = q.Count();
            var r = q.OrderBy(u => u.CreatorDate).Skip((page - 1) * rows).Take(rows).ToList();
            return r;

        }

        /// <summary>
        /// 查询救治措施    尤浩
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="rowCounts">返回数据总数</param>
        /// <param name="strIsActive">是否有效</param>
        /// <param name="measureType">措施类型</param>
        /// <param name="measureID">措施编码</param>
        /// <returns></returns>
        public object GetMaterialList(int page, int rows, ref int rowCounts, string strIsActive, string measureType, string measureID)
        {
            var q = (from m in db.I_Material
                     where m.TDictionary.TypeID == "PRMeasureType"
                     //where m.MTypeID == "NULLTYPE"
                     select new
                     {
                         ID = m.ID,
                         Name = m.Name,
                         MeasureTypeID = m.OtherTypeID,
                         OtherTypeID = m.OtherTypeID,
                         MTypeID = m.MTypeID,
                         Manufacturer = m.Manufacturer,
                         Vendor = m.Vendor,
                         Unit = m.Unit,
                         Specification = m.Specification,
                         QRCode = m.QRCode,
                         Remark = m.Remark,
                         CreatorName = m.CreatorName,
                         CreatorDate = m.CreatorDate,
                         PinYin = m.PinYin,
                         IsActive = m.IsActive,
                         RealPrice = m.RealPrice,
                         AlarmCounts = m.AlarmCounts,
                         MCode = m.MCode,
                         FeeScale = m.FeeScale,
                         LimitMaxPrice = m.LimitMaxPrice,
                         SN = m.SN,
                         GiveMedicineWay = m.GiveMedicineWay,

                         MeasureTypeName = m.TDictionary.Name, //翻译MeasureTypeID 名称
                         UnitName = m.TDictionary2.Name,//翻译Unit 名称
                     });

            //q = q.Where(m => m.IsActive == isActive && m.MCode.Equals(mCode));
           
            if (!string.IsNullOrEmpty(strIsActive))
            {
                bool isActive = bool.Parse(strIsActive);
                q = q.Where(m => m.IsActive == isActive);
            }
            if (!string.IsNullOrEmpty(measureType))
            {               
                q = q.Where(m => m.MeasureTypeID == measureType);
            }            
            if (!string.IsNullOrEmpty(measureID))
            {
                int id = int.Parse(measureID);
                q = q.Where(m => m.ID.Equals(id));
            }

            rowCounts = q.Count();
            var r = q.OrderBy(u => u.CreatorDate).Skip((page - 1) * rows).Take(rows).ToList();
            return r;

        }

        /// <summary>
        /// 根据物资类型获取 物资流水   尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMaterialListBy(string mtype)
        {

            string sql = @"select m.*,UnitName=tu.Name from I_Material m
                           left join TDictionary t on m.MTypeID = t.ID 
						   left join TDictionary tu on m.Unit = tu.ID 
                           where MTypeID='" + mtype + @"'   ";
                         
            List<I_MaterialExt> list = db.Database.SqlQuery<I_MaterialExt>(sql).ToList();
            return list;
        }

        /// <summary>
        /// 根据措施类型获取 措施流水    尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMeasureListBy(string measuretype)
        {

            string sql = @"select m.*,UnitName=tu.Name from I_Material m
                           left join TDictionary t on m.MTypeID = t.ID 
						   left join TDictionary tu on m.Unit = tu.ID 
                           where OtherTypeID='" + measuretype + "'  ";
                           
            List<I_MaterialExt> list = db.Database.SqlQuery<I_MaterialExt>(sql).ToList();
            return list;
        }

        //取所有措施流水  尤浩
        public List<I_MaterialExt> GetMeasureList()
        {
            
            
             string sql = @"select m.*,UnitName=tu.Name from I_Material m
                           left join TDictionary t on m.MTypeID = t.ID 
						   left join TDictionary tu on m.Unit = tu.ID 
                           where MTypeID='NULLTYPE' ";

             List<I_MaterialExt> list = db.Database.SqlQuery<I_MaterialExt>(sql).ToList();
             return list;
        }

        //取所有物资流水  尤浩
        public List<I_MaterialExt> GetMaterialList()
        {


            string sql = @"select m.*,UnitName=tu.Name from I_Material m
                           left join TDictionary t on m.OtherTypeID = t.ID 
						   left join TDictionary tu on m.Unit = tu.ID 
                           where t.TypeID='PRDrugType' or t.TypeID='PRConsumableType' ";

            List<I_MaterialExt> list = db.Database.SqlQuery<I_MaterialExt>(sql).ToList();
            return list;
        }          
 
    }
}
