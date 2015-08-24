using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model;
using Anke.SHManage.BLL.MobileBLL;

namespace Anke.SHManage.Web.Areas.BM.Controllers
{
    [SkipLoginAttribute]
    public class MobileTaskController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        [HttpPost]
        public ActionResult Index()
        {
            string userCode = Request.Form["userCode"].ToString();

            string taskCode = Request.Form["TaskCode"].ToString();

            string errorMsg = "成功";


            var list = new MobileTaskBLL().GetMobileTaskListBy(userCode, taskCode, ref errorMsg);

            if (list != null)
                return Json(new { success = true, errorMsg = errorMsg, list = list }, "appliction/json", JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false, errorMsg = errorMsg, list = "" }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据任务编码和病历序号查询病历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPatientRecordInfo()
        {
            string TaskCode = Request.Form["TaskCode"].ToString();
            int PatientOrder = Convert.ToInt32(Request.Form["PatientOrder"]);
            M_PatientRecord pr = null;
            M_PatientRecordAppend pra = null;
            new MobilePatientRecordBLL().GetPatientInfo(TaskCode, PatientOrder, out pr, out pra);
            if (pr != null)
                return Json(new { success = true, pr = pr, pra = pra }, "appliction/json", JsonRequestBehavior.AllowGet);
            else
            {
                M_AttemperData result = new M_PatientRecordBLL().GetAttemperData(TaskCode, "");//根据任务编码获取调度信息
                pr = new M_PatientRecord();
                pr.CallOrder = result.CallOrder;
                pr.PatientOrder = 1;
                pr.LocalAddress = result.LocalAddress;
                pr.Name = result.Name;
                pr.Sex = result.Sex;
                pr.Age = result.Age;
                pr.AgeType = result.AgeType;
                pr.Station = result.Station;
                pr.OutStationCode = result.StationCode;
                pr.DrivingTime = result.DrivingTime;
                pr.ArriveSceneTime = result.ArriveSceneTime;
                pr.LeaveSceneTime = result.LeaveSceneTime;
                pr.ArriveDestinationTime = result.ArriveDestinationTime;
                pr.SendAddress = result.SendAddress;
                pr.Driver = result.Driver;
                pr.DoctorAndNurse = result.Doctor + result.Nurse;
                pr.StretcherBearersI = result.StretcherBearers;
                pr.ContactTelephone = result.ContactTelephone;
                return Json(new { success = true, pr = pr, pra = pra }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 提交病历
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitPatientRecord()
        {
            try
            {
                string TaskCode = Request.Form["TaskCode"].ToString();
                int PatientOrder = Convert.ToInt32(Request.Form["PatientOrder"]);
                string EvenType = Request.Form["EvenType"].ToString();
                string TaskOrder = Request.Form["TaskOrder"].ToString();
                string DispatcherInfo = Request.Form["DispatcherInfo"].ToString();
                string TiJianInfo = Request.Form["TiJianInfo"].ToString();
                string WorkCode = Request.Form["WorkCode"].ToString();
                string PersonName = Request.Form["PersonName"].ToString();
                M_PatientRecord pr = JsonHelper.GetJsonInfoBy<M_PatientRecord>(DispatcherInfo);

                M_PatientRecordAppend pra = JsonHelper.GetJsonInfoBy<M_PatientRecordAppend>(TiJianInfo);

                string errorMsg = "";
                if (new MobilePatientRecordBLL().PADAddPatientRecord(TaskCode, PatientOrder, EvenType, TaskOrder, pr, pra, WorkCode, PersonName, ref errorMsg))
                    return Json(new { success = true, msg = TaskOrder }, "appliction/json", JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, msg = "上传病历失败！原因：" + errorMsg }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "上传病历失败！原因：" + ex.Message }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取分站
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStation()
        {
            try
            {
                string code = "";
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                if (code == "")
                { code = "-1"; }
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetStation");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("TStation", code);
                        CacheHelper.SetCache("GetStation", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }
        /// <summary>
        /// 获取医院列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetHospitalInfo()
        {
            try
            {
                M_DictionaryBLL bll = new M_DictionaryBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetHospitalInfo");
                    if (result == null)
                    {
                        result = bll.GetMainDictionary("THospitalInfo", "");
                        CacheHelper.SetCache("GetHospitalInfo", result);
                    }
                }
                return Json(result);
            }
            catch (Exception e)
            {
                return this.Json("");
            }
        }
    }
}
