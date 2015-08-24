using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_SurplusDAL : BaseDAL<I_Surplus>, II_SurplusDAL
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;
        /// <summary>
        /// 保存入库结余信息 
        /// TODO： 1.先查询入库结余  2.如果有则更新结余  3.如没有则新增结余  4.插入出库入库流水  
        /// </summary>
        /// <param name="info">入库从表</param>
        public void SaveEntrySurplusInfo(I_EntryDetail info, string strEntryType, ref string errorMsg)
        {
            try
            {

                //查结余 注意查原剩余信息时候 要用非追踪查询 否则修改失败
                I_Surplus surplusinfo = base.GetModelWithOutTrace(s => s.BatchNo == info.BatchNo && s.MaterialID == info.MaterialID && s.StorageCode == info.StorageCode && s.RealBatchNo == info.RealBatchNo);
                if (surplusinfo != null) //有结余数据
                {
                    surplusinfo.EntryCounts = surplusinfo.EntryCounts + info.EntryCounts;
                    surplusinfo.Surplus = surplusinfo.Surplus + info.EntryCounts;
                    surplusinfo.SurplusPrice = surplusinfo.SurplusPrice + info.TotalPrice;

                    //更新结余信息
                    base.Modify(surplusinfo, "EntryCounts", "Surplus", "SurplusPrice");
                }
                else
                {
                    surplusinfo = new I_Surplus();
                    surplusinfo.BatchNo = info.BatchNo;           //批次号
                    surplusinfo.RealBatchNo = info.RealBatchNo;   //真实批次号
                    surplusinfo.EntryCounts = info.EntryCounts;   //入库数量
                    surplusinfo.MaterialID = info.MaterialID;     //物资ID
                    surplusinfo.StorageCode = info.StorageCode;   //仓储ID
                    surplusinfo.Surplus = info.EntryCounts;       // 剩余数量 = 入库数量
                    surplusinfo.SurplusPrice = info.TotalPrice;   // 剩余金额 = 入库金额 
                    surplusinfo.ValidityDate = info.ValidityDate; //有效期

                    base.Add(surplusinfo);  //新增结余信息
                }

                #region 2.记录入库信息
                I_InventoryRecord record = new I_InventoryRecord();
                record.BatchNo = info.BatchNo;                   //批次号
                record.RealBatchNo = info.RealBatchNo;           //真实批次号
                record.DeliveryEntryType = strEntryType;       //出入库类型
                record.EntryDetailCode = info.EntryDetailCode;  //入库详细编码
                record.Is_Entry = true;                    //入库标识
                record.MaterialID = info.MaterialID;       //物资编码
                record.OperatorDateTime = info.EntryDate;  //入库日期
                record.StorageCode = info.StorageCode;  //仓储编码

                record.OriginalSurplus = surplusinfo.Surplus - info.EntryCounts;   // 原结余数量= 当前库存结余数量-入库数量
                record.ChangeSurplus = info.EntryCounts;     //改变数量=入库数量
                record.Surplus = surplusinfo.Surplus;       //当前库存结余数量
                record.SurplusPrice = surplusinfo.SurplusPrice; //当前库存结余价格
                record.OperatorCode = info.OperatorCode;  //操作人员编码

                //插入出入流水
                dalContext.II_InventoryRecordDAL.Add(record);
                #endregion

            }
            catch (Exception e)
            {
                errorMsg = "结余操作失败！原因: " + e.Message;
                throw new Exception(errorMsg);

            }
        }


        /// <summary>
        /// 保存出库结余信息
        /// TODO：1.先查询出库结余  2.如果有则更新结余  3.如没有则提示错误  4.插入出库入库流水  
        /// </summary>
        /// <param name="info"></param>
        /// <param name="strEntryType"></param>
        public void SaveDeliverySurplusInfo(I_DeliveryDetail info, string strDeliveryType, ref string errorMsg)
        {
            decimal price = 0;

            try
            {
                //查结余
                I_Surplus surplusinfo = base.GetModelWithOutTrace(s => s.BatchNo == info.BatchNo && s.MaterialID == info.MaterialID && s.StorageCode == info.EntryStorageCode && s.RealBatchNo == info.RealBatchNo);
                if (surplusinfo != null) //有结余数据
                {
                    double surplus = surplusinfo.Surplus - info.DeliveryCounts;

                    if (surplusinfo.Surplus > 0) //剩余必须大于 0  才能计算出平均单价
                        price = surplusinfo.SurplusPrice / Convert.ToDecimal(surplusinfo.Surplus);

                    if (surplus >= 0)
                    {
                        surplusinfo.Surplus = surplusinfo.Surplus - info.DeliveryCounts;
                    }
                    else
                    {
                        errorMsg = "该批次号库存不足，出库失败！";
                        throw new Exception(errorMsg);
                    }

                    surplusinfo.SurplusPrice = surplusinfo.SurplusPrice - price * Convert.ToDecimal(info.DeliveryCounts);

                    base.Modify(surplusinfo, "Surplus", "SurplusPrice"); //更新结余信息
                }
                else
                {
                    errorMsg = "未找到该批次号商品信息，出库失败！";
                    throw new Exception(errorMsg);
                }

                #region  2.记录出库信息

                I_InventoryRecord record = new I_InventoryRecord();
                record.BatchNo = info.BatchNo;                   //批次号
                record.RealBatchNo = info.RealBatchNo;           //真实批次号
                record.DeliveryEntryType = strDeliveryType;       //出入库类型
                record.DeliveryDetailCode = info.DeliveryDetailCode;  //出库详细编码
                record.Is_Entry = false;                    //入库标识
                record.MaterialID = info.MaterialID;       //物资编码
                record.OperatorDateTime = info.DeliveryTime;  //出库日期
                record.StorageCode = info.EntryStorageCode;  //仓储编码

                record.OriginalSurplus = surplusinfo.Surplus + info.DeliveryCounts;   // 原结余数量= 当前库存结余数量+ 出库数量
                record.ChangeSurplus = -info.DeliveryCounts;     //改变数量=出库数量

                record.Surplus = surplusinfo.Surplus; //当前库存结余数量
                record.SurplusPrice = surplusinfo.SurplusPrice; //当前库存结余价格
                record.OperatorCode = info.OperatorCode;  //操作人员编码
                //插入出入库流水
                dalContext.II_InventoryRecordDAL.Add(record);
                #endregion

                #region 3.自动生成转库入库单

                I_Entry entry = new I_Entry(); // 入库主表
                entry.EntryCode = info.DeliveryCode; //出库编码
                entry.EntryDate = info.DeliveryTime; // 出库时间
                entry.EntryStorageID = info.TargetStorageCode; //入库仓库编码=出库目的编码

                if (info.RedDeliveryDetailCode != null)
                    entry.EntryType = "MatertalInType-3";  //红充入库单
                else
                    entry.EntryType = "MatertalInType-2";  //转库入库单

                entry.OperationTime = DateTime.Now;  // 操作时刻
                entry.OperatorCode = info.OperatorCode;
                entry.Remark = "";

                I_EntryDetail entryDetail = new I_EntryDetail();  //入库从表
                entryDetail.BatchNo = info.BatchNo;
                entryDetail.RealBatchNo = info.RealBatchNo;    //真实批次号
                entryDetail.EntryCode = info.DeliveryCode; //入库编码=出库编码 
                entryDetail.EntryCounts = info.DeliveryCounts;   //入库数量 = 出库数量
                entryDetail.EntryDate = info.DeliveryTime;   // 入库时间 = 出库时间
                entryDetail.EntryDetailCode = info.DeliveryDetailCode; //出库子编码
                entryDetail.MaterialID = info.MaterialID;
                entryDetail.OperatorCode = info.OperatorCode;
                entryDetail.StorageCode = info.TargetStorageCode;
                entryDetail.TotalPrice = price; //当前库存结余价格;
                entryDetail.ValidityDate = surplusinfo.ValidityDate; //有效期 

                //如果被红充的出库信息不为空 
                if (info.RedDeliveryDetailCode != null)
                {
                    entryDetail.RedEntryDetailCode = info.RedDeliveryDetailCode; //被红充入库编码 = 被红充出库编码
                }

                List<I_EntryDetail> list = new List<I_EntryDetail>();
                list.Add(entryDetail);

                //入库操作
                new I_EntryDAL().DoEntry(entry, list, ref errorMsg); //调用入库流程  

                #endregion
            }
            catch (Exception e)
            {
                errorMsg = "结余操作失败！原因: " + e.Message;
                throw new Exception(errorMsg);
            }
        }



        /// <summary>
        /// 获取剩余列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="materialID"></param>
        /// <param name="storageCode"></param>
        /// <returns></returns>
        public object GetSurplusList(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType, int alarmCounts = -1, int overDays = 0, bool IsShowOver = true)
        {
            var q = (from sur in db.I_Surplus
                     select new
                     {
                         ID = sur.ID,
                         MaterialID = sur.MaterialID,

                         BatchNo = sur.BatchNo,
                         RealBatchNo = sur.RealBatchNo,
                         EntryCounts = sur.EntryCounts,
                         Surplus = sur.Surplus,
                         SurplusPrice = sur.SurplusPrice,
                         StorageCode = sur.StorageCode,
                         ValidityDate = sur.ValidityDate,  //有效期

                         MaterialMTypeID = sur.I_Material.MTypeID, //物资分类编码
                         MaterialName = sur.I_Material.Name, //物资名称
                         MaterialManufacturer = sur.I_Material.Manufacturer, //制造商
                         MaterialVendor = sur.I_Material.Vendor, //供应商
                         MaterialAlarmCounts = sur.I_Material.AlarmCounts, //报警数量
                         MaterialSpecification = sur.I_Material.Specification, //规格
                         MaterialRealPrice = sur.I_Material.RealPrice, //销售价格
                         MaterialUnit = sur.I_Material.Unit, //单位
                         MaterialUnitName = sur.I_Material.TDictionary2.Name, //单位名称

                         StorageName = sur.I_Storage.Name //翻译仓库名称

                     });
            q = q.Where(p => p.Surplus > 0);//剩余数量大于0
            if (!string.IsNullOrEmpty(materialID))
            {
                int i = int.Parse(materialID);
                q = q.Where(p => p.MaterialID == i);
            }
            if (listStorageCode != null)
            {
                q = q.Where(p => listStorageCode.Contains(p.StorageCode));
            }
            if (listMaterialType != null)
            {
                q = q.Where(p => listMaterialType.Contains(p.MaterialMTypeID));
            }
            if (alarmCounts != -1)
            {
                q = q.Where(p => p.MaterialAlarmCounts <= alarmCounts);
            }
            if (overDays != 0)
            {
                DateTime dt = DateTime.Now.AddDays(overDays);
                q = q.Where(p => p.ValidityDate <= dt);
            }
            if (IsShowOver == true) //是否查过期物资
            {
                q = q.Where(p => p.ValidityDate >= DateTime.Now);
            }
            rowCounts = q.Count();
            var r = q.OrderBy(u => u.ValidityDate).Skip((page - 1) * rows).Take(rows).ToList();

            return r;
        }
        /// <summary>
        /// 获取最近到期剩余列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="materialID"></param>
        /// <param name="storageCode"></param>
        /// <returns></returns>
        public object GetLastDaySurplusList(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType)
        {
            var q = (from sur in db.I_Surplus
                     select new
                     {
                         ID = sur.ID,
                         MaterialID = sur.MaterialID,

                         BatchNo = sur.BatchNo,
                         RealBatchNo = sur.RealBatchNo,
                         EntryCounts = sur.EntryCounts,
                         Surplus = sur.Surplus,
                         SurplusPrice = sur.SurplusPrice,
                         StorageCode = sur.StorageCode,
                         ValidityDate = sur.ValidityDate,  //有效期

                         MaterialMTypeID = sur.I_Material.MTypeID, //物资分类编码
                         MaterialName = sur.I_Material.Name, //物资名称
                         MaterialManufacturer = sur.I_Material.Manufacturer, //制造商
                         MaterialVendor = sur.I_Material.Vendor, //供应商
                         MaterialAlarmCounts = sur.I_Material.AlarmCounts, //报警数量
                         MaterialSpecification = sur.I_Material.Specification, //规格
                         MaterialRealPrice = sur.I_Material.RealPrice, //销售价格
                         MaterialUnit = sur.I_Material.Unit, //单位
                         MaterialUnitName = sur.I_Material.TDictionary2.Name, //单位名称

                         StorageName = sur.I_Storage.Name //翻译仓库名称

                     });
            q = q.Where(p => p.Surplus > 0);//剩余数量大于0
            q = q.Where(p => p.ID == (from s in db.I_Surplus
                                      where s.MaterialID == p.MaterialID
                                      && s.StorageCode == p.StorageCode
                                      && s.ValidityDate >= DateTime.Now
                                      orderby s.ValidityDate
                                      select s.ID
                                       ).FirstOrDefault());
            if (!string.IsNullOrEmpty(materialID))
            {
                int i = int.Parse(materialID);
                q = q.Where(p => p.MaterialID == i);
            }
            if (listStorageCode != null)
            {
                q = q.Where(p => listStorageCode.Contains(p.StorageCode));
            }
            if (listMaterialType != null)
            {
                q = q.Where(p => listMaterialType.Contains(p.MaterialMTypeID));
            }
            q = q.Where(p => p.ValidityDate >= DateTime.Now);
            rowCounts = q.Count();
            var r = q.OrderBy(u => u.ValidityDate).Skip((page - 1) * rows).Take(rows).ToList();

            return r;
        }


        public object GetSurplusListGroupBy(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType)
        {
            var q = (from sur in db.I_Surplus
                     group sur by new
                     {
                         MaterialMTypeID = sur.I_Material.MTypeID, //物资分类编码
                         MaterialName = sur.I_Material.Name, //物资名称
                         MaterialManufacturer = sur.I_Material.Manufacturer, //制造商
                         MaterialVendor = sur.I_Material.Vendor, //供应商
                         MaterialAlarmCounts = sur.I_Material.AlarmCounts, //报警数量
                         MaterialSpecification = sur.I_Material.Specification, //规格
                         MaterialRealPrice = sur.I_Material.RealPrice, //销售价格
                         MaterialUnit = sur.I_Material.Unit, //单位
                         MaterialUnitName = sur.I_Material.TDictionary2.Name, //单位名称
                         StorageName = sur.I_Storage.Name, //翻译仓库名称

                         StorageCode = sur.StorageCode,
                         MaterialID = sur.MaterialID
                     }
                         into surGroup
                         select new
                         {

                             MaterialID = surGroup.Key.MaterialID,
                             StorageCode = surGroup.Key.StorageCode,

                             EntryCounts = surGroup.Sum(s => s.EntryCounts),
                             Surplus = surGroup.Sum(s => s.Surplus),

                             MaterialMTypeID = surGroup.Key.MaterialMTypeID, //物资分类编码
                             MaterialName = surGroup.Key.MaterialName, //物资名称
                             MaterialManufacturer = surGroup.Key.MaterialManufacturer, //制造商
                             MaterialVendor = surGroup.Key.MaterialVendor, //供应商
                             MaterialAlarmCounts = surGroup.Key.MaterialAlarmCounts, //报警数量
                             MaterialSpecification = surGroup.Key.MaterialSpecification, //规格
                             MaterialRealPrice = surGroup.Key.MaterialRealPrice, //销售价格
                             MaterialUnit = surGroup.Key.MaterialUnit, //单位
                             MaterialUnitName = surGroup.Key.MaterialUnitName,//单位名称
                             StorageName = surGroup.Key.StorageName //翻译仓库名称

                         });
            q = q.Where(p => p.Surplus > 0);//剩余数量大于0
            if (!string.IsNullOrEmpty(materialID))
            {
                int i = int.Parse(materialID);
                q = q.Where(p => p.MaterialID == i);
            }
            if (listStorageCode != null)
            {
                q = q.Where(p => listStorageCode.Contains(p.StorageCode));
            }
            if (listMaterialType != null)
            {
                q = q.Where(p => listMaterialType.Contains(p.MaterialMTypeID));
            }

            rowCounts = q.Count();
            var r = q.OrderBy(p => p.MaterialID).Skip((page - 1) * rows).Take(rows).ToList();

            return r;
        }


    }
}
