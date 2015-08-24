using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_ApplyDAL
    {

        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        /// <summary>
        /// 申请操作
        /// </summary>
        /// <param name="apply">申请主表</param>
        /// <param name="applyDetailInfos">申请从表</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public bool ApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetailInfos, ref string errorMsg)
        {
            //创建事务
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //1 先插入主表
                    dalContext.II_ApplyDAL.Add(apply);

                    foreach (I_ApplyDetail info in applyDetailInfos)
                    {
                        //2. 再插入从表信息 
                        dalContext.II_ApplyDetailDAL.Add(info);
                    }

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (DbEntityValidationException dbEx)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务

                    errorMsg = "申请操作失败 失败！  原因:" + dbEx.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                    return false;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "申请操作失败！ 操作已取消！~~" + ex.InnerException.Message;
                    return false;
                }
            }
        }



        /// <summary>
        /// 获取申请列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="EntryCode"></param>
        /// <param name="EntryType"></param>
        /// <returns></returns>
        public object GetApplyList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string ApplyUserID, string ApplyType)
        {
            var q = (from a in db.I_Apply
                     select new
                     {
                         ApplyCode = a.ApplyCode,
                         ApplyReceivingStoreID = a.ApplyReceivingStoreID,
                         ApplyTime = a.ApplyTime,
                         ApplyType = a.ApplyType,
                         ApplyUserID = a.ApplyUserID,
                         ApprovalTime = a.ApprovalTime,
                         ApprovalUserID = a.ApprovalUserID,
                         Remark = a.Remark,

                         ApplyUserName = a.P_User.Name, //用导航属性 申请人姓名
                         ApprovalUserName = a.P_User1.Name,  //批准人姓名
                         ApplyTypeName = a.TDictionary.Name,   //申请类型
                         ApplyReceivingStoreName = a.I_Storage.Name   //接受仓库名称
                     });

            q = q.Where(a => a.ApplyTime >= startTime && a.ApplyTime <= endTime);
            if (!string.IsNullOrEmpty(ApplyUserID))
            {
                int usrID = int.Parse(ApplyUserID);
                q = q.Where(a => a.ApplyUserID == usrID);
            }
            if (!string.IsNullOrEmpty(ApplyType))
            {
                q = q.Where(a => a.ApplyType == ApplyType);
            }

            rowCounts = q.Count();
            var r = q.OrderBy(a => a.ApplyTime).Skip((page - 1) * rows).Take(rows).ToList();
            return r;
        }

        public List<I_ApplyDetailExt> getApplyDetailListBy(string strApplyCode)
        {
            string sql = @"SELECT ApplyDetailCode
                          ,ApplyCode
                          ,apd.MaterialID
                          ,apd.RealBatchNo
                          ,apd.BatchNo
                          ,apd.ApplyTime
                          ,apd.ApplyCounts
                          ,apd.ApplyUserID
                          ,apd.SelfStorageCode
                          ,apd.ApplyTargetStorageCode
                          ,apd.Remark
                          ,apd.ApprovalCounts
                          ,sur.Surplus
	                      ,sur.ValidityDate
	                      ,MaterialName=mat.Name
	                      ,ApplyUserName=usr.Name
	                      ,SelfStorageName=selfsto.Name
	                      ,ApplyTargetStorageName = targetsto.Name
                          FROM I_ApplyDetail apd
                          LEFT JOIN I_Surplus sur on apd.MaterialID= sur.MaterialID and apd.ApplyTargetStorageCode=sur.StorageCode  and apd.BatchNo=sur.BatchNo
                          LEFT JOIN I_Material mat on mat.ID =apd.MaterialID
                          LEFT JOIN P_User usr on usr.ID = apd.ApplyUserID
                          LEFT JOIN I_Storage selfsto on selfsto.StorageID = apd.SelfStorageCode
                          LEFT JOIN I_Storage targetsto on targetsto.StorageID =  apd.ApplyTargetStorageCode
                          WHERE apd.ApplyCode='" + strApplyCode + "'";

            List<I_ApplyDetailExt> list = db.Database.SqlQuery<I_ApplyDetailExt>(sql).ToList();
            return list;
        }


        /// <summary>
        /// 同意申请
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="applyDetail"></param>
        /// <param name="deliveryInfo"></param>
        /// <param name="deliveryDetailInfos"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ApproveApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetail, I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg)
        {
            //创建事务
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //1.0 更新申请表
                    new I_ApplyDAL().Modify(apply, "ApplyType", "ApprovalUserID", "ApprovalTime");
                    foreach (var info in applyDetail)
                    {
                        new I_ApplyDetailDAL().Modify(info, "ApprovalCounts");
                    }

                    //2.0 做出库操作
                    #region 出库操作

                    //1 先插入出库主表
                    dalContext.II_DeliveryDAL.Add(deliveryInfo);

                    foreach (I_DeliveryDetail info in deliveryDetailInfos)
                    {
                        //2. 再插入从表信息 
                        dalContext.II_DeliveryDetailDAL.Add(info);

                        //3. 保存剩余流水表 
                        dalContext.II_SurplusDAL.SaveDeliverySurplusInfo(info, deliveryInfo.DeliveryType, ref errorMsg);
                    } 

                    #endregion

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "同意申请操作失败！ 操作已取消！~~" + ex.Message;
                    return false;
                }
            }
        }
    }
}
