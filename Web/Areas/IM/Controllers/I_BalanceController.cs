using Anke.SHManage.BLL;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class I_BalanceController : Controller
    {
        //
        // GET: /IM/I_Balance/

        public ActionResult BalanceList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DataLoad()
        {
            string MonthTime = Request.Form["MonthTime"].ToString();
            string BalanceType = Request.Form["BalanceType"].ToString();
            int pageSize = int.Parse(Request.Form["rows"]);
            int pageIndex = int.Parse(Request.Form["page"]);
            int rowCounts = 0;
            var list = new I_BalanceBLLExt().GetBalanceList(pageIndex, pageSize, MonthTime, BalanceType, ref rowCounts);
            return Json(new { total = rowCounts, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetMaxReportTime()
        {
            string MonthTime = Request.Form["MonthTime"].ToString();
            string reportTime = "";
            if (new I_BalanceBLLExt().GetBalanceMax(MonthTime, ref reportTime))
            {
                return Json(new { success = true, reportTime = reportTime }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, reportTime = reportTime }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SelectBalanceReport()
        {
            string mType = Request.Form["MType"].ToString();
            string reportTime = Request.Form["reportTime"].ToString();
            string errorMsg = "";
            var list = new I_BalanceBLLExt().GetI_BalanceNewList(reportTime, mType, ref errorMsg);
            if (list != null)
            {
                if (list.Count > 0)
                    return Json(new { success = true, list = list, msg = "" }, "appliction/json", JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, list = list, msg = errorMsg }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, list = list, msg = errorMsg }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SubmitReport()
        {
            string ReportStr = Request.Form["ReportStr"].ToString();
            List<I_Balance> list = JsonHelper.GetJsonInfoBy<List<I_Balance>>(ReportStr);
            string MType = Request.Form["MType"].ToString();
            string ReportTime = Request.Form["ReportTime"].ToString();
            string errorMsg = "";
            if (new I_BalanceBLLExt().AddBalance(list, MType, ReportTime, ref errorMsg))
                return this.JsonResult(Utility.E_JsonResult.OK, "报表生成成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, errorMsg, null, null);
        }

    }
}
