using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_DeliveryDAL : BaseDAL<I_Delivery>, II_DeliveryDAL
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        /// <summary>
        /// 出库操作
        /// </summary>
        public bool DeliveryOperate(I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg)
        {

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //1 先插入出库主表
                    dalContext.II_DeliveryDAL.Add(deliveryInfo);

                    foreach (I_DeliveryDetail info in deliveryDetailInfos)
                    {
                        //2. 再插入从表信息 
                        dalContext.II_DeliveryDetailDAL.Add(info);

                        //3. 保存剩余流水表 
                        dalContext.II_SurplusDAL.SaveDeliverySurplusInfo(info, deliveryInfo.DeliveryType, ref errorMsg);
                    }

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "出库操作失败！ 操作已取消！~~" + ex.Message;
                    return false;
                }
            }
        }



        /// <summary>
        /// 出库单查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="DeliveryCode"></param>
        /// <param name="DeliveryType"></param>
        /// <param name="listSourceStorageIDs"></param>
        /// <param name="listTargetStorageIDs"></param>
        /// <returns></returns>
        public object GetDeliveryList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string DeliveryCode, string DeliveryType, List<int> listSourceStorageID, List<int> listTargetStorageID, List<string> listMaterialType, string MaterialID)
        {
            var q = (from dd in db.I_DeliveryDetail
                  select new
                  {
                      //物资信息
                      MaterialMTypeID = dd.I_Material.MTypeID, //物资分类编码
                      MaterialName = dd.I_Material.Name, //物资名称
                      MaterialManufacturer = dd.I_Material.Manufacturer, //制造商
                      MaterialVendor = dd.I_Material.Vendor, //供应商
                      MaterialAlarmCounts = dd.I_Material.AlarmCounts, //报警数量
                      MaterialSpecification = dd.I_Material.Specification, //规格
                      MaterialRealPrice = dd.I_Material.RealPrice, //销售价格
                      MaterialUnit = dd.I_Material.Unit, //单位
                      MaterialUnitName = dd.I_Material.TDictionary2.Name, //单位名称
                      EntryStorageName = dd.I_Storage.Name,     //翻译仓库名称
                      TargetStorageName = dd.I_Storage1.Name,   //目标仓库名称
                      OperatorName = dd.P_User.Name,            //操作人员名称

                      //物资出库主表信息
                      ConsigneeID = dd.I_Delivery.ConsigneeID, // 收货人编码
                      Remark = dd.I_Delivery.Remark,  //出货单备注
                      DeliveryType = dd.I_Delivery.DeliveryType, //出库类型编码
                      DeliveryTypeName = dd.I_Delivery.TDictionary.Name,  //出库单类型名称

                      //出库详细信息
                      DeliveryDetailCode = dd.DeliveryDetailCode, //物资出库编码
                      DeliveryCode = dd.DeliveryCode,    //出库单编码
                      RealBatchNo = dd.RealBatchNo, //实际批次号
                      BatchNo = dd.BatchNo,  //系统批次号
                      DeliveryCounts = dd.DeliveryCounts,   //出库数量
                      DeliveryTime = dd.DeliveryTime,  //出库时间
                      OperatorCode = dd.OperatorCode,  //操作人员编码
                      EntryStorageCode = dd.EntryStorageCode,     //来源仓库编码
                      TargetStorageCode = dd.TargetStorageCode,   //目标仓库编码
                      TargetEntryDetailCode = dd.TargetEntryDetailCode,  //来源入库编码
                      DetailRemark = dd.Remark,                          //备注
                      RedDeliveryDetailCode = dd.RedDeliveryDetailCode,  // 红冲编码
                      MaterialID = dd.MaterialID                         // 物资编码
                  });

            q = q.Where(d => d.DeliveryTime >= startTime && d.DeliveryTime <= endTime);

            if (!string.IsNullOrEmpty(DeliveryCode))  //出库编码
            {
                q = q.Where(d => d.DeliveryCode == DeliveryCode);
            }
            if (!string.IsNullOrEmpty(DeliveryType)) //出库类型
            {
                q = q.Where(d => d.DeliveryType == DeliveryType);
            }
            if (!string.IsNullOrEmpty(MaterialID)) //物资ID
            {
                int id = int.Parse(MaterialID);
                q = q.Where(d => d.MaterialID == id);
            }
            if (listSourceStorageID != null)
            {
                q = q.Where(d => listSourceStorageID.Contains(d.EntryStorageCode));
            }
            if (listTargetStorageID != null)
            {
                q = q.Where(d => listTargetStorageID.Contains(d.TargetStorageCode));
            }
            if (listMaterialType != null)
            {
                q = q.Where(d => listMaterialType.Contains(d.MaterialMTypeID));
            }

            rowCounts = q.Count();
            var r = q.OrderBy(d => d.DeliveryTime).Skip((page - 1) * rows).Take(rows).ToList();

            return r;
        }


    }
}
