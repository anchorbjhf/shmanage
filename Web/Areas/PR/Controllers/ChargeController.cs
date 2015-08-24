using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;

namespace Anke.SHManage.Web.Areas.PR.Controllers
{
    public class ChargeController : Controller
    {
        //
        // GET: /PR/Charge/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PatinetCharge( string TaskCode, int PatientOrder, string AlarmeventType, string ChargeType)
        {
            ViewBag.TaskCode = TaskCode;
            ViewBag.PatientOrder = PatientOrder;
            ViewBag.AlarmeventType = AlarmeventType;
            ViewBag.ChargeType = ChargeType;
            return View();
        }
        [HttpPost]
        public ActionResult GetPatientCharge()
        {
            string taskCode = Request.Form["TaskCode"].ToString();
            string patientOrder = Request.Form["PatientOrder"].ToString();
            // return Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
            List<PatientChargeInfo> listYP = null;
            List<PatientChargeInfo> listCL = null;
            List<PatientChargeInfo> listJC = null;
            List<PatientChargeInfo> listZL = null;
            dynamic info = null;
            dynamic editinfo = null;
            new M_PatientChargeBLL().getPatientCharge(taskCode, patientOrder, out listYP, out listCL, out listJC, out listZL, out info, out editinfo);
            return Json(new { PRInfo = info, EditPRInfo = editinfo,listYP = listYP, listCL = listCL, listJC = listJC, listZL = listZL }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SavePatientCharge(M_PatientCharge info)
        {
            info.ChargePerson = UserOperateContext.Current.Session_UsrInfo.Name;
            info.IFArrearage = false;

            if (new M_PatientChargeBLL().Add(info) > 0)

                return Json(new { Result = "OK", Message = "收费信息记录成功！"}, "text/plain;charset=UTF-8", JsonRequestBehavior.AllowGet);
                //return this.JsonResult(Utility.E_JsonResult.OK, "收费信息记录成功！", null, null);
            else
                return Json(new { Result = "Error", Message = "收费信息记录失败！" }, "text/plain;charset=UTF-8", JsonRequestBehavior.AllowGet);
                //return this.JsonResult(Utility.E_JsonResult.Error, "收费信息记录失败！", null, null);

        }
        //病历收费修改
        [HttpPost]
        public ActionResult EditPatientCharge(M_PatientCharge info)
        {
            info.ChargePerson = UserOperateContext.Current.Session_UsrInfo.Name;
            info.IFArrearage = false;
            int res = new M_PatientChargeBLL().Modify(info, "Date", "InvoiceNumber", "PatientName", "AddressStart", "AddressEnd", "OutStationRoadCode", "PointRoadCode", "ArriveHospitalRoadCode", "ReturnStationRoadCode", "OneWayKM",
                "ChargeKM", "CarFee", "WaitingFee", "EmergencyFee", "DrugFeeTotal", "ExamineFeeTotal", "ConsumableFeeTotal", "MeasureFeeTotal", "ReceivableTotal", "PaidMoney", "ChargePerson", "IFArrearage", "TaskSD");
            if (res > 0)
                return Json(new { Result = "OK", Message = "收费信息记录成功！"}, "text/plain;charset=UTF-8", JsonRequestBehavior.AllowGet);
                //return this.JsonResult(Utility.E_JsonResult.OK, "收费信息记录成功！", null, null); 
            else
                return Json(new { Result = "Error", Message = "收费信息记录失败！" }, "text/plain;charset=UTF-8", JsonRequestBehavior.AllowGet);
               // return this.JsonResult(Utility.E_JsonResult.Error, "收费信息记录失败！", null, null);

        }

    }
}
