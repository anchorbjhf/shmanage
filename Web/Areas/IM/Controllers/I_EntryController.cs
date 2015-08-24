
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System.Reflection;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class I_EntryController : Controller
    {
        // GET: /IM/I_Entry/

        public ActionResult I_EntryList()
        {
            ViewBag.RKHC = UserOperateContext.Current.getGongneng(E_IMPermisson.RKHC);
            ViewBag.ZJXG = UserOperateContext.Current.getGongneng(E_IMPermisson.ZJXG);
            ViewBag.RK = UserOperateContext.Current.getGongneng(E_IMPermisson.RK);
            ViewBag.RKBDZJE = UserOperateContext.Current.getGongneng(E_IMPermisson.RKnoZJ);
            return View();

        }


        public ActionResult I_EntryDetail()
        {
            return View();
        }
        //int page = Request.Form["page"];
        /// <summary>
        /// 查询入库流水信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadEntryList()
        {

            //统计时间
            DateTime StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
            DateTime EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
            string EntryCode = Request.Form["EntryCode"].ToString();
            string EntryType = Request.Form["EntryType"].ToString();
            string StroageID = Request.Form["StroageID"].ToString();
            List<int> StroageIDs = new List<int>();
            if (StroageID.Length > 0)
                StroageIDs.Add(int.Parse(StroageID));
            else
                StroageIDs = UserOperateContext.Current.Session_StorageRelated.listUserStorage;

            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);

            I_EntryBLL bll = new I_EntryBLL();
            int total = 0;
            var list = bll.GetEntryList(pageIndex, pageSize, ref total, StartDate, EndDate, EntryCode, EntryType, StroageIDs);
            return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
            //return Json(null, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadCheckEntryDetailList()
        {
            string EntryCode = Request.Form["EntryCode"].ToString();
            var list = new I_EntryDetailBLL().GetListBy(e => e.EntryCode == EntryCode).Select(e => e.ToExtModel());
            return Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  直接入库方法 MatertalInType-1
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveEntry()
        {
            string sEntryDetailInfo = Request.Form["sEntryDetailInfo"].ToString();
            List<EntryDetailInfo> relist = JsonHelper.GetJsonInfoBy<List<EntryDetailInfo>>(sEntryDetailInfo);
            I_Entry entryInfo = new I_Entry();
            entryInfo.EntryCode = DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss") + entryInfo.OperatorCode;    //生成入库编码
            entryInfo.EntryDate = Convert.ToDateTime(Request.Form["EntryDate"]);
            entryInfo.EntryType = "MatertalInType-1";
            entryInfo.OperationTime = DateTime.Now;
            entryInfo.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
            entryInfo.EntryStorageID = int.Parse(Request.Form["StorageCode"]);
            entryInfo.Remark = Request.Form["Remark"].ToString().Trim();
            List<I_EntryDetail> list = new List<Model.I_EntryDetail>();
            int ind = 0;
            foreach (EntryDetailInfo item in relist)
            {
                ind++;
                I_EntryDetail detailinfo = new I_EntryDetail();
                detailinfo.EntryDetailCode = entryInfo.EntryCode + entryInfo.OperatorCode + ind.ToString();
                detailinfo.BatchNo = item.BatchNo;
                detailinfo.RealBatchNo = item.RealBatchNo;
                detailinfo.EntryCode = entryInfo.EntryCode;
                detailinfo.EntryCounts = item.EntryCounts;
                detailinfo.EntryDate = item.EntryDate;
                detailinfo.MaterialID = item.MaterialID;
                detailinfo.OperatorCode = UserOperateContext.Current.Session_UsrInfo.ID;
                detailinfo.RedEntryDetailCode = "";
                detailinfo.RelatedOrderNum = item.RelatedOrderNum;
                detailinfo.Remark = item.Remark.Trim();
                detailinfo.Specification = item.Specification;
                detailinfo.StorageCode = item.StorageCode;
                detailinfo.TotalPrice = item.TotalPrice;
                detailinfo.Unit = item.Unit;
                if (item.ValidityDateStr == "无有效期")
                    detailinfo.ValidityDate = DateTime.MaxValue;
                else
                    detailinfo.ValidityDate = Convert.ToDateTime(item.ValidityDateStr);
                list.Add(detailinfo);
            }
            string errorMsg = "";
            if (new I_EntryBLL().EntryOperate(entryInfo, list, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "入库信息成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }


        public ActionResult SaveRedEntryDetiail()
        {
            string EntryDetailCode = Request.Form["EntryDetailCode"].ToString();
            string EntryRemark = Request.Form["EntryRemark"].ToString();
            int StroageCode = int.Parse(Request.Form["StroageCode"]);
            string errorMsg = "";
            if (new I_EntryBLL().EntryRedOperate(EntryDetailCode, EntryRemark, StroageCode, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "红冲入库信息成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }

        public ActionResult EditEntryTotalPriceDetiail()
        {
            string EntryDetailCode = Request.Form["EntryDetailCode"].ToString();
            decimal TotalPrice = Convert.ToDecimal(Request.Form["TotalPrice"]);
            string EntryRemark = Request.Form["EntryRemark"].ToString();
            string errorMsg = "";
            Model.I_EntryDetail info = new I_EntryDetail();
            info.EntryDetailCode = EntryDetailCode;
            info.Remark = EntryRemark;
            info.TotalPrice = TotalPrice;
            if (new I_EntryBLL().EditEntryDetail(info, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "修改价格成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }


    }
}
