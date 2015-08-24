using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class I_ApplyController : Controller
    {
        //
        // GET: /IM/I_Apply/

        public ActionResult ApplyList()
        {
            ViewBag.userName = UserOperateContext.Current.Session_UsrInfo.Name;
            ViewBag.userID = UserOperateContext.Current.Session_UsrInfo.ID;
            return View();
        }
        public ActionResult ApprovalApplyList()
        {
            ViewBag.Edit = UserOperateContext.Current.getGongneng(E_IMPermisson.SLXG);  //68申领单修改功能
            ViewBag.Approval = UserOperateContext.Current.getGongneng(E_IMPermisson.SLSP);  //69申领单审批功能
            ViewBag.CheckAll = UserOperateContext.Current.getGongneng(E_IMPermisson.CKALLSL); //71查看所有申领
            ViewBag.userId = UserOperateContext.Current.Session_UsrInfo.ID;
            return View();
        }
        /// <summary>
        /// 保存申领单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveApply()
        {
            string ApplyUserID = Request.Form["ApplyUserID"].ToString();
            DateTime ApplyDateTime = Convert.ToDateTime(Request.Form["ApplyDateTime"]);
            string ApplyStorage = Request.Form["ApplyStorage"].ToString();
            string Remark = Request.Form["Remark"].ToString();
            string sApplyDetailInfo = Request.Form["sApplyDetailInfo"].ToString();
            I_Apply applyInfo = new I_Apply();
            applyInfo.ApplyCode = DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss") + ApplyUserID;
            applyInfo.ApplyUserID = int.Parse(ApplyUserID);
            applyInfo.ApplyReceivingStoreID = int.Parse(ApplyStorage);
            applyInfo.ApplyTime = ApplyDateTime;
            applyInfo.ApplyType = "ApplyType-1";
            applyInfo.Remark = Remark;
            List<I_DeliveryDetail> relist = JsonHelper.GetJsonInfoBy<List<I_DeliveryDetail>>(sApplyDetailInfo);
            List<I_ApplyDetail> adlist = new List<I_ApplyDetail>();
            int ind = 0;
            foreach (I_DeliveryDetail item in relist)
            {
                I_ApplyDetail info = new I_ApplyDetail();
                info.ApplyCode = applyInfo.ApplyCode;
                info.ApplyDetailCode = applyInfo.ApplyCode + ind.ToString();
                info.MaterialID = item.MaterialID;
                info.RealBatchNo = item.RealBatchNo;
                info.BatchNo = item.BatchNo;
                info.ApplyTime = applyInfo.ApplyTime;
                info.ApplyCounts = item.DeliveryCounts;
                info.ApprovalCounts = item.DeliveryCounts;
                info.ApplyUserID = applyInfo.ApplyUserID;
                info.SelfStorageCode = applyInfo.ApplyReceivingStoreID;
                info.ApplyTargetStorageCode = item.TargetStorageCode;
                info.Remark = item.Remark;
                adlist.Add(info);
                ind++;
            }
            string errorMsg = "";
            if (new I_ApplyBLL().ApplyOperate(applyInfo, adlist, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "申领物资成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);

        }
        /// <summary>
        /// 获取申领单list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetApplyList()
        {
            DateTime StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
            DateTime EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
            string userID = Request.Form["SelectUsers"].ToString();
            string ApplyType = Request.Form["ApplyType"].ToString();
            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_ApplyBLL().GetApplyList(pageIndex, pageSize, ref total, StartDate, EndDate, userID, ApplyType);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetApplyDetail()
        {
            string ApplyCode = Request.Form["ApplyID"].ToString();
            var list = new I_ApplyBLL().getApplyDetailList(ApplyCode);
            return Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CancelApply()
        {
            string ApprovalId = Request.Form["ApprovalId"].ToString();
            I_Apply model = new I_Apply();
            model.ApplyCode = ApprovalId;
            model.ApplyType = "ApplyType-4";
            int res = new I_ApplyBLL().Modify(model, "ApplyCode", "ApplyType");
            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "作废申领单成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "作废申领单失败!", null, null);
        }
        /// <summary>
        /// 修改申领单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditApply()
        {
            return Json("");
        }
        /// <summary>
        /// 拒绝申领
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectApply()
        {
            string ApprovalId = Request.Form["ApprovalId"].ToString();
            I_Apply model = new I_Apply();
            model.ApplyCode = ApprovalId;
            model.ApplyType = "ApplyType-3";
            int res = new I_ApplyBLL().Modify(model, "ApplyCode", "ApplyType");
            if (res > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "拒绝申领单成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "拒绝申领单失败!", null, null);
        }
        /// <summary>
        /// 批准申领
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RatifyApply()
        {
            string ConsigneeID = Request.Form["ConsigneeID"].ToString();
            string DeliveryCode = Request.Form["DeliveryCode"].ToString();
            DateTime DeliveryDate = Convert.ToDateTime(Request.Form["DeliveryDate"]);
            int ReceivingStoreID = Convert.ToInt32(Request.Form["ReceivingStoreID"]);
            string Remark = Request.Form["Remark"].ToString();
            I_Delivery deliveryInfo = new I_Delivery();
            deliveryInfo.DeliveryCode = DeliveryCode;
            deliveryInfo.ConsigneeID = ConsigneeID;
            deliveryInfo.DeliveryTime = DeliveryDate;
            deliveryInfo.Remark = Remark;
            deliveryInfo.OperationTime = DateTime.Now;
            deliveryInfo.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            deliveryInfo.ReceivingStoreID = ReceivingStoreID;
            deliveryInfo.DeliveryType = "MatertalInType-2";

            string sDeliveryDetailInfo = Request.Form["sDeliveryDetailInfo"].ToString();
            List<I_DeliveryDetail> deliveryDetailList = JsonHelper.GetJsonInfoBy<List<I_DeliveryDetail>>(sDeliveryDetailInfo);

            I_Apply applyInfo = new I_Apply();
            applyInfo.ApplyCode = DeliveryCode;
            applyInfo.ApprovalUserID = UserOperateContext.Current.Session_UsrInfo.ID;
            applyInfo.ApprovalTime = DeliveryDate;
            applyInfo.ApplyType = "ApplyType-2";

            List<I_ApplyDetail> applyDetailList = new List<I_ApplyDetail>();
            foreach (I_DeliveryDetail item in deliveryDetailList)
            {
                item.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;

                I_ApplyDetail adInfo = new I_ApplyDetail();
                adInfo.ApplyDetailCode = item.DeliveryDetailCode;
                adInfo.ApprovalCounts = item.DeliveryCounts;

                applyDetailList.Add(adInfo);
            }
            string errorMsg = "";
            if (new I_ApplyBLL().ApproveApplyOperate(applyInfo, applyDetailList, deliveryInfo
                , deliveryDetailList, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "审批物资申领单成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }

        [HttpPost]
        public ActionResult GetLastDaySurplusList()
        {
            List<int> listStorageCode = new List<int>();

            string SurplusCode = Request.Form["SurplusCode"].ToString();
            string MaterialType = Request.Form["MaterialType"].ToString();
            List<string> mTypeList = UserOperateContext.Current.getMaterialTypeList(MaterialType);
            string MaterialID = Request.Form["MaterialID"].ToString();
            if (SurplusCode.Length > 0)
                listStorageCode.Add(int.Parse(SurplusCode));
            else
            {
                string listCode = AppConfig.GetConfigString("ApplyStorageCode");
                listStorageCode = listCode.Split(',').Select<string, int>(e => Convert.ToInt32(e)).ToList();
            }

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_SurplusBLL().GetLastDaySurplusList(pageIndex, pageSize, ref total, MaterialID, listStorageCode, mTypeList);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

    }
}
