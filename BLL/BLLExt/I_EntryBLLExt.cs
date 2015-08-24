using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Model;
using Anke.SHManage.IDAL;
using Anke.SHManage.Utility;

namespace Anke.SHManage.BLL
{
    public partial class I_EntryBLL : BaseBLL<I_Entry>
    {
        private I_EntryDAL dal = new I_EntryDAL();

        /// <summary>
        /// 获取入库信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="EntryCode"></param>
        /// <param name="EntryType"></param>
        /// <returns></returns>
        public object GetEntryList(int page, int rows, ref int total, DateTime startTime, DateTime endTime, string EntryCode, string EntryType, List<int> listEntryStorageIDs)
        {
            return dal.GetEntryList(page, rows, ref total, startTime, endTime, EntryCode, EntryType, listEntryStorageIDs);
        }

        /// <summary>
        /// 入库操作
        ///  红充 入库注意 entryInfo 中的EntryType 为 红充入库  以及  entryDetailInfos的 RedEntryDetailCode 为被红充的单号
        /// </summary>
        /// <param name="entryInfo">入库主表</param>
        /// <param name="entryDetailInfos">入库从表</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public bool EntryOperate(I_Entry entryInfo, List<I_EntryDetail> entryDetailInfos, ref string errorMsg)
        {
            return dal.EntryOperate(entryInfo, entryDetailInfos, ref errorMsg);
        }


        /// <summary>
        /// 红充入库操作
        /// </summary>
        /// <param name="originalID">原入库ID</param>
        /// <param name="newID">新入库ID</param>
        /// <param name="newRemark">新备注</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns>返回值</returns>
        public bool EntryRedOperate(string oriEntryDetailCode, string newRemark, int StroageCode, ref string errorMsg)
        {

            I_Entry entryInfo = new I_Entry();
            entryInfo.EntryCode = DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss") + UserOperateContext.Current.Session_UsrInfo.ID; //生成入库编码
            entryInfo.EntryDate = DateTime.Now;         //当前时间
            entryInfo.EntryType = "MatertalInType-3";   //红冲入库
            entryInfo.OperationTime = DateTime.Now;     //当前时间
            entryInfo.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            entryInfo.EntryStorageID = StroageCode;
            entryInfo.Remark = "红冲入库";


            I_EntryDetail orientryDetailInfo = base.DALContext.II_EntryDetailDAL.GetModelWithOutTrace(e => e.EntryDetailCode == oriEntryDetailCode);
            if (orientryDetailInfo != null)
            {
                orientryDetailInfo.RedEntryDetailCode = oriEntryDetailCode;  //被红充入库编码 = 自己本身的入库编码  标识为“被红充单据”
            }
            else
            {
                errorMsg = "没有查到此入库单从表，操作失败！";
                return false;
            }

            I_EntryDetail newdetailinfo = new I_EntryDetail();
            newdetailinfo.EntryDetailCode = entryInfo.EntryCode + entryInfo.OperatorCode + "1";
            newdetailinfo.BatchNo = orientryDetailInfo.BatchNo;
            newdetailinfo.RealBatchNo = orientryDetailInfo.RealBatchNo;
            newdetailinfo.EntryCode = entryInfo.EntryCode;
            newdetailinfo.EntryCounts = -(orientryDetailInfo.EntryCounts);  //入库数量为负数
            newdetailinfo.EntryDate = DateTime.Now;
            newdetailinfo.MaterialID = orientryDetailInfo.MaterialID;
            newdetailinfo.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            newdetailinfo.RedEntryDetailCode = oriEntryDetailCode;   //红充入库编码
            newdetailinfo.RelatedOrderNum = orientryDetailInfo.RelatedOrderNum;
            newdetailinfo.Remark = newRemark;   //新备注
            newdetailinfo.Specification = orientryDetailInfo.Specification;
            newdetailinfo.StorageCode = orientryDetailInfo.StorageCode;
            newdetailinfo.TotalPrice = -(orientryDetailInfo.TotalPrice);  //钱为负数
            newdetailinfo.Unit = orientryDetailInfo.Unit;
            newdetailinfo.ValidityDate = orientryDetailInfo.ValidityDate;

            return dal.EntryRedOperate(entryInfo, orientryDetailInfo, newdetailinfo, ref errorMsg);
        }

        public bool EditEntryDetail(I_EntryDetail info, ref string errorMsg)
        {
            return dal.EditEntryDetail(info, ref errorMsg);
        }

    }
}
