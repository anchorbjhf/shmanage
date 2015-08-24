using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class I_SurplusController : Controller
    {
        //
        // GET: /IM/I_Surplus/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult I_SurplusList()
        {
            return View();
        }
        public ActionResult I_SurplusSee()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetSurplusList()
        {
            List<int> listStorageCode = new List<int>();

            string SurplusCode = Request.Form["SurplusCode"].ToString();
            string MaterialType = Request.Form["MaterialType"].ToString();
            List<string> mTypeList = UserOperateContext.Current.getMaterialTypeList(MaterialType);
            string MaterialID = Request.Form["MaterialID"].ToString();
            if (SurplusCode.Length > 0)
                listStorageCode.Add(int.Parse(SurplusCode));
            else
                listStorageCode = UserOperateContext.Current.Session_StorageRelated.listUserStorage;

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_SurplusBLL().GetSurplusList(pageIndex, pageSize, ref total, MaterialID, listStorageCode, mTypeList);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSurplusGroupList()
        {
            List<int> listStorageCode = new List<int>();

            string SurplusCode = Request.Form["SurplusCode"].ToString();
            string MaterialType = Request.Form["MaterialType"].ToString();
            List<string> mTypeList = UserOperateContext.Current.getMaterialTypeList(MaterialType);
            string MaterialID = Request.Form["MaterialID"].ToString();
            if (SurplusCode.Length > 0)
                listStorageCode.Add(int.Parse(SurplusCode));
            else
                listStorageCode = UserOperateContext.Current.Session_StorageRelated.listUserStorage;

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_SurplusBLL().GetSurplusListGroupBy(pageIndex, pageSize, ref total, MaterialID, listStorageCode, mTypeList);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetMaterialSurplusList()
        {
            List<int> listStorageCode = new List<int>();

            int SurplusCode = int.Parse(Request.Form["SurplusCode"]);
            string MaterialID = Request.Form["MaterialID"].ToString();
            listStorageCode.Add(SurplusCode);


            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_SurplusBLL().GetSurplusList(pageIndex, pageSize, ref total, MaterialID, listStorageCode, null, -1, 0, false);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 盘库操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveStocking()
        {
            bool StockingType = Convert.ToBoolean(Request.Form["StockingType"]); //true为盘盈  false为盘亏
            int DeliveryCounts = Convert.ToInt32(Request.Form["DeliveryCounts"]);
            int StorageCode = -2; //盘库入库为盘点库-2
            string StockingTypeInfo = "";
            if (StockingType)
            {
                StockingTypeInfo = "MatertalOutType-4";  //盘盈出库单
                DeliveryCounts = -DeliveryCounts;
            }
            else
            {
                StockingTypeInfo = "MatertalOutType-5";  //盘亏出库单
            }
            List<I_DeliveryDetail> relist = new List<I_DeliveryDetail>();

            I_DeliveryDetail info = new I_DeliveryDetail();
            info.RealBatchNo = Request.Form["RealBatchNo"].ToString();
            info.BatchNo = Request.Form["BatchNo"].ToString();
            info.MaterialID = Convert.ToInt32(Request.Form["MaterialID"]);
            info.DeliveryCounts = DeliveryCounts;
            info.EntryStorageCode = int.Parse(Request.Form["StorageCode"]); //来源仓库编码
            info.Remark = Request.Form["Remark"].ToString();
            info.TargetStorageCode = StorageCode;
            relist.Add(info);

            I_Delivery delivery = new I_Delivery();
            delivery.DeliveryCode = DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss") + UserOperateContext.Current.Session_UsrInfo.ID;    //生成入库编码
            delivery.ConsigneeID = UserOperateContext.Current.Session_UsrInfo.ID.ToString();
            delivery.DeliveryTime = DateTime.Now;
            delivery.Remark = info.Remark;
            delivery.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            delivery.OperationTime = DateTime.Now;
            delivery.ReceivingStoreID = StorageCode;
            delivery.DeliveryType = StockingTypeInfo;

            string errorMsg = "";
            if (new I_DeliveryBLL().DeliveryOerate(delivery, relist, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "盘点物资仓库成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }
    }

}
