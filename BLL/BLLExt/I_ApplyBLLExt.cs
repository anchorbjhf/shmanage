using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    /// <summary>
    /// 提交申请
    /// </summary>
    public partial class I_ApplyBLL : BaseBLL<I_Apply>
    {
        private I_ApplyDAL dal = new I_ApplyDAL();
        public bool ApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetail, ref string errorMsg)
        {
            return dal.ApplyOperate(apply, applyDetail, ref errorMsg);
        }


        /// <summary>
        /// 获取申请列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="ApplyUserID"></param>
        /// <param name="ApplyType"></param>
        /// <returns></returns>
        public object GetApplyList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string ApplyUserID, string ApplyType)
        {
             return dal.GetApplyList(page, rows, ref rowCounts, startTime, endTime, ApplyUserID, ApplyType);
        }


        /// <summary>
        /// 获取申请从表信息
        /// </summary>
        /// <param name="ApplyCode"></param>
        /// <returns></returns>
        public List<I_ApplyDetailExt> getApplyDetailList(string ApplyCode)
        {
            return dal.getApplyDetailListBy(ApplyCode);
        }

        /// <summary>
        /// 同意申请
        /// </summary>
        /// <param name="apply">主表</param>
        /// <param name="applyDetail">从表</param>
        /// <param name="deliveryInfo">出库主</param>
        /// <param name="deliveryDetailInfos">出库从</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public bool ApproveApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetail, I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg)
        {
            return dal.ApproveApplyOperate(apply, applyDetail, deliveryInfo, deliveryDetailInfos, ref errorMsg);
        }
    }
}
