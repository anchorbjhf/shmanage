using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model;
using Anke.SHManage.BLL;
using Anke.SHManage.Utility;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class I_DeliveryController : Controller
    {
        //
        // GET: /IM/I_Delivery/


        public ActionResult I_DeliveryList()
        {
            return View();
        }

        public ActionResult DataLoad()
        {
            //统计时间
            DateTime StartDate = Convert.ToDateTime(Request.Form["StartDate"]); //查询起时间 单一参数
            DateTime EndDate = Convert.ToDateTime(Request.Form["EndDate"]); //查询止时间 单一参数
            string DeliveryCode = Request.Form["DeliveryCode"].ToString();//出库单号 单一参数
            string DeliveryType = Request.Form["DeliveryType"].ToString();  //出库类型 单一参数


            string SelectStorage = Request.Form["SelectStorage"].ToString(); //来源仓库编码
            List<int> listSourceStroageIDs = new List<int>();
            if (SelectStorage.Length > 0)
                listSourceStroageIDs.Add(int.Parse(SelectStorage));
            else
                listSourceStroageIDs = UserOperateContext.Current.Session_StorageRelated.listUserStorage;

            string TargetStorage = Request.Form["TargetStorage"].ToString(); //目标仓库编码
            List<int> listTargetStorageIDs = null;
            if (TargetStorage != "")
                listTargetStorageIDs = TargetStorage.Split(',').Select<string, int>(e => Convert.ToInt32(e)).ToList();

            string MaterialType = Request.Form["MaterialType"].ToString();
            List<string> mTypeList = UserOperateContext.Current.getMaterialTypeList(MaterialType);
            string MaterialID = Request.Form["MaterialID"].ToString();

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_DeliveryBLL().GetDeliveryList(pageIndex, pageSize, ref total, StartDate, EndDate, DeliveryCode, DeliveryType, listSourceStroageIDs, listTargetStorageIDs, mTypeList, MaterialID);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult I_DeliveryDetail()
        {
            return View();
        }

        /// <summary>
        /// 记录出库信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDeliveryDetail()
        {
            string ConsigneeID = Request.Form["ConsigneeID"].ToString();
            DateTime DeliveryDate = Convert.ToDateTime(Request.Form["DeliveryDate"]);
            int ReceivingStoreID = int.Parse(Request.Form["ReceivingStoreID"]);
            string Remark = Request.Form["Remark"].ToString();
            string sDeliveryDetailInfo = Request.Form["sDeliveryDetailInfo"].ToString();
            List<I_DeliveryDetail> relist = JsonHelper.GetJsonInfoBy<List<I_DeliveryDetail>>(sDeliveryDetailInfo);
            I_Delivery delivery = new I_Delivery();
            delivery.DeliveryCode = DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss") + UserOperateContext.Current.Session_UsrInfo.ID;    //生成入库编码
            delivery.ConsigneeID = ConsigneeID;
            delivery.DeliveryType = "MatertalOutType-1";
            delivery.OperationTime = DeliveryDate;
            delivery.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            delivery.DeliveryTime = DateTime.Now;
            delivery.ReceivingStoreID = ReceivingStoreID;
            delivery.Remark = Remark;
            //delivery.SourceStoreID

            string errorMsg = "";
            if (new I_DeliveryBLL().DeliveryOerate(delivery, relist, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "出库信息成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }
    }
}
