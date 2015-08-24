using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class OverdueMaterialController : Controller
    {
        //
        // GET: /IM/OverdueMaterial/
        #region 页面初始化
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
        public ActionResult OverdueMaterial()
        {
            return View();
        }
        #endregion

        #region 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public ActionResult DataLoad()
        {
            int remainTime = int.Parse(Request.Form["remainTime"]);
            string MaterialType = Request.Form["MaterialType"].ToString();
            List<string> mTypeList = getMaterialTypeList(MaterialType);
            string MaterialID = Request.Form["MaterialID"].ToString();
            List<int> listStorageCode = new List<int>();
            string StorageCode = Request.Form["StorageCode"].ToString();
            if (StorageCode.Length > 0)
                listStorageCode.Add(int.Parse(StorageCode));
            else
                listStorageCode = UserOperateContext.Current.Session_StorageRelated.listUserStorage;

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int total = 0;
            var list = new I_SurplusBLL().GetSurplusList(pageIndex, pageSize, ref total, MaterialID, listStorageCode, mTypeList, -1, remainTime, false);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);

        }
        private List<string> getMaterialTypeList(string Mtype)
        {
            List<string> mTypeList = new List<string>();
            if (Mtype.Length > 0)
            {
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion("'" + Mtype + "'");
                mTypeList = listD.Select(t => t.ID).ToList();
            }
            else
            {
                List<string> mlist = UserOperateContext.Current.Session_StorageRelated.listUserStorageMaterialType;
                string instr = "";
                foreach (string item in mlist)
                {
                    instr += "'" + item + "',";
                }
                instr = instr.Substring(0, instr.Length - 1);
                List<TDictionary> listD = new TDictionaryBLL().getTDicRecursion(instr);
                mTypeList = listD.Select(t => t.ID).ToList();
            }
            return mTypeList;
        }
        #endregion
        /// <summary>
        /// 记录报废物资
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveScrapOverdueMaterial()
        {
            int StorageCode = -3; //报废入库为报废库-3

            List<I_DeliveryDetail> relist = new List<I_DeliveryDetail>();

            I_DeliveryDetail info = new I_DeliveryDetail();
            info.RealBatchNo = Request.Form["RealBatchNo"].ToString();
            info.BatchNo = Request.Form["BatchNo"].ToString();
            info.MaterialID = Convert.ToInt32(Request.Form["MaterialID"]);
            info.DeliveryCounts = Convert.ToInt32(Request.Form["DeliveryCounts"]);
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
            delivery.DeliveryType = "MatertalOutType-6";


            string errorMsg = "";
            if (new I_DeliveryBLL().DeliveryOerate(delivery, relist, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "报废物资成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }
    }

}
