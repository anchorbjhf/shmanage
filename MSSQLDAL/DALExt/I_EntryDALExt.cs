using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_EntryDAL
    {

        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        /// <summary>
        /// 获取入库表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="EntryCode"></param>
        /// <param name="EntryType"></param>
        /// <returns></returns>
        public object GetEntryList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string EntryCode, string EntryType, List<int> listEntryStorageIDs)
        {
            var q = (from u in db.I_Entry
                     select new
                           {
                               EntryCode = u.EntryCode,
                               EntryDate = u.EntryDate,
                               EntryType = u.EntryType,
                               OperationTime = u.OperationTime,
                               OperatorCode = u.OperatorCode,
                               EntryStorageID = u.EntryStorageID,
                               Remark = u.Remark,
                               OperatorName = u.P_User.Name, //用导航属性 查询操作人员
                               EntryTypeName = u.TDictionary.Name,
                               EntryStorageName = u.I_Storage.Name
                           });

            q = q.Where(u => u.EntryDate >= startTime && u.EntryDate <= endTime);
            if (!string.IsNullOrEmpty(EntryCode))
            {
                q = q.Where(p => p.EntryCode == EntryCode);
            }
            if (!string.IsNullOrEmpty(EntryType))
            {
                q = q.Where(p => p.EntryType == EntryType);
            }
            if (listEntryStorageIDs != null)
            {
                q = q.Where(p => listEntryStorageIDs.Contains(p.EntryStorageID));
            }
            rowCounts = q.Count();
            var r = q.OrderBy(u => u.OperationTime).Skip((page - 1) * rows).Take(rows).ToList();
            return r;
        }


        /// <summary>
        /// 入库操作
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="entryInfo"></param>
        /// <param name="entryDetailInfos"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool EntryOperate(I_Entry entryInfo, List<I_EntryDetail> entryDetailInfos, ref string errorMsg)
        {
            //创建事务
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    this.DoEntry(entryInfo, entryDetailInfos, ref errorMsg);

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "入库操作失败！ 操作已取消！~~" + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// 红充操作
        /// </summary>
        /// <param name="newEntryInfo">新入库单</param>
        /// <param name="orientryDetailInfo">原入单从表</param>
        /// <param name="newEntryDetailInfo">新入库单从表</param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool EntryRedOperate(I_Entry newEntryInfo, I_EntryDetail orientryDetailInfo, I_EntryDetail newEntryDetailInfo, ref string errorMsg)
        {
            //创建事务
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //修改红冲入库单号  orientryDetailInfo 不能被EF框架跟踪
                    dalContext.II_EntryDetailDAL.Modify(orientryDetailInfo, "RedEntryDetailCode"); //更新被红充编码

                    List<I_EntryDetail> entryDetailInfos = new List<I_EntryDetail>();
                    entryDetailInfos.Add(newEntryDetailInfo);
                    this.DoEntry(newEntryInfo, entryDetailInfos, ref errorMsg); //做入库操作

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "入库操作失败！ 操作已取消！~~" + ex.Message;
                    return false;
                }
            }
        }


        /// <summary>
        /// 入库操作
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <param name="entryDetailInfos"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        internal void DoEntry(I_Entry entryInfo, List<I_EntryDetail> entryDetailInfos, ref string errorMsg)
        {

            //1 先插入主表
            I_Entry tempEntry = (from e in db.I_Entry
                                 where e.EntryCode == entryInfo.EntryCode
                                 select e).FirstOrDefault();
            if (tempEntry == null)
                dalContext.II_EntryDAL.Add(entryInfo);


            foreach (I_EntryDetail info in entryDetailInfos)
            {
                I_EntryDetail tempEntryDetail = (from e in db.I_EntryDetail
                                                 where e.EntryDetailCode == info.EntryDetailCode
                                                 select e).FirstOrDefault();
                if (tempEntryDetail != null)
                {
                    info.EntryDetailCode = (Convert.ToInt32(info.EntryDetailCode) + 1).ToString();
                }
                //2. 再插入从表信息   红充流程注意记录RedEntryDetailCode信息
                dalContext.II_EntryDetailDAL.Add(info);

                //3. 保存剩余流水表 
                dalContext.II_SurplusDAL.SaveEntrySurplusInfo(info, entryInfo.EntryType, ref errorMsg);
            }
        }

        /// <summary>
        /// 修改入库价格信息
        /// </summary>
        /// <param name="entryInfo"></param>
        /// <returns></returns>
        public bool EditEntryDetail(I_EntryDetail info, ref string errorMsg)
        {
            //创建事务
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    new I_EntryDetailDAL().Modify(info, "Remark", "TotalPrice");
                    I_InventoryRecord inInfo = new I_InventoryRecord();
                    inInfo.EntryDetailCode = info.EntryDetailCode;
                    inInfo.SurplusPrice = info.TotalPrice;
                    inInfo.ID = (from u in db.I_InventoryRecord
                                 where u.EntryDetailCode == info.EntryDetailCode
                                 select u.ID).First();
                    new I_InventoryRecordDAL().Modify(inInfo, "SurplusPrice");

                    tran.Commit(); //提交事务

                    return true;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "修改入库价格失败！ 操作已取消！~~" + ex.Message;
                    return false;
                }
            }
        }

    }
}
