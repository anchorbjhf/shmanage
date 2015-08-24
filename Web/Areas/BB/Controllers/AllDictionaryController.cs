using Anke.SHManage.BLL;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BB.Controllers
{
    public class AllDictionaryController : Controller
    {
        //
        // GET: /BB/AllDictionary/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 取人员姓名（调度员工作状态流水用）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPersonName()
        {
            try
            {
                var list = new LSDAL().Get_LS_DDYGZZT_RYXM();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取分中心(统计流水表)
        [HttpPost]
        public ActionResult GetCenterName()
        {
            try
            {
                var list = new LSDAL().GetCenter();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取分站（统计流水表）
        [HttpPost]
        public ActionResult GetSationName(string centerID)
        {
            try
            {
                //List<string> centerId = centerID.Split(',').ToList();

                var list = new LSDAL().GetStation(centerID);
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取要车性质 （呼救事件流水表）
        public ActionResult GetAlarmEventType()
        {
            try
            {


                var list = new LSDAL().GetAlarmEventType();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取车辆状态 （呼救事件流水表）
        [HttpPost]
        public ActionResult GetCarState()
        {
            try
            {


                var list = new LSDAL().GetCarState();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        ////取受理类型（受理记录流水表）
        //[HttpPost]
        //public ActionResult GetCallType()
        //{
        //    try
        //    {


        //        var list = new LSDAL().GetCallType();
        //        return Json(list);
        //    }
        //    catch (Exception)
        //    {
        //        return this.Json("");
        //    }
        //}
        //取分站（病历流水表）
        [HttpPost]
        public ActionResult GetStations()
        {
            try
            {


                var list = new LSDAL().GetStations();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取病种分类（病历流水表）
        [HttpPost]
        public ActionResult GetDiseasesClassifications()
        {
            try
            {


                var list = new LSDAL().GetDiseasesClassifications();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取病历中的事件类型（救治措施统计表）
        public ActionResult GetPatientVersion()
        {
            try
            {

                string PatientVersion = "PatientVersion";
                var list = new M_DictionaryBLL().GetCheckBoxModel(PatientVersion);
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取病情分类（病历流水表）
        [HttpPost]
        public ActionResult GetIllnessClassification()
        {
            try
            {


                var list = new LSDAL().GetIllnessClassification();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取病情预报（病历流水表）
        [HttpPost]
        public ActionResult GetIllnessForecast()
        {
            try
            {


                var list = new LSDAL().GetIllnessForecast();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取急救效果（病历流水表）
        [HttpPost]
        public ActionResult GetFirstAidEffect()
        {
            try
            {


                var list = new LSDAL().GetFirstAidEffect();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取病家合作度（病历流水表）
        [HttpPost]
        public ActionResult GetDiseaseCooperation()
        {
            try
            {


                var list = new LSDAL().GetDiseaseCooperation();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取事件类型（病历流水表）
        [HttpPost]
        public ActionResult GetEventType()
        {
            try
            {


                var list = new LSDAL().GetEventType();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取死亡类型
        [HttpPost]
        public ActionResult GetDeathCase()
        {
            try
            {


                var list = new LSDAL().GetDeathCase();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取死亡证明类型
        [HttpPost]
        public ActionResult GetDeathCertificate()
        {
            try
            {


                var list = new LSDAL().GetDeathCertificate();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取受理类型
        [HttpPost]
        public ActionResult GetAcceptType()
        {
            try
            {


                var list = new LSDAL().GetAcceptType();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取电话类型
        [HttpPost]
        public ActionResult GetCallType()
        {
            try
            {


                var list = new LSDAL().GetCallType();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取车辆状态类型
        [HttpPost]
        public ActionResult GetAmbulanceState()
        {
            try
            {


                var list = new LSDAL().GetAmbulanceState();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //取角色类型
        [HttpPost]
        public ActionResult GetRole()
        {
            try
            {


                var list = new LSDAL().GetRole();
                return Json(list);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
    }
}
