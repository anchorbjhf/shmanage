using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DD.Controllers
{
    public class EventInfoController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        //
        // GET: /DD/EventInfo/

        public ActionResult EventInfoList()
        {
            this.ViewData["start"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            this.ViewData["end"] = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
            return View();
        }

        public ActionResult DataLoad()
        {
            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            //获取查询条件
            DateTime start = Convert.ToDateTime(Request.Form["start"]);
            DateTime end = Convert.ToDateTime(Request.Form["end"]);
            string mainSuit = Request.Form["mainSuit"];
            string telephoneNumber = Request.Form["telephoneNumber"];
            string localAddress = Request.Form["localAddress"];
            string patientName = Request.Form["patientName"];
            string sendAddress = Request.Form["sendAddress"];
            string dispatcher = Request.Form["dispatcher"];
            string driver = Request.Form["driver"];
            string doctor = Request.Form["doctor"];
            string nurse = Request.Form["nurse"];
            string stretcher = Request.Form["stretcher"];
            string eventType = Request.Form["eventType"];
            string illnessJudgment = Request.Form["illnessState"];
            string eventCode = Request.Form["eventCode"];
            string station = Request.Form["station"];
            string carCode = Request.Form["ambulanceCode"];
            string eventSource = Request.Form["eventSource"];
            E_StatisticsPermisson em = UserOperateContext.Current.getMaxPerForStatistics();
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
            string selfStationID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSationID;
            EventInfoBLL bll = new EventInfoBLL();
            var list = bll.GetEventInfoList(pageSize,pageIndex,start,end,mainSuit,telephoneNumber,localAddress,patientName,sendAddress,dispatcher,
                                           driver, doctor, nurse,stretcher, eventType, illnessJudgment, eventCode, station, carCode, eventSource,
                                           em,selfWorkCode,selfCenterID,selfStationID);
            JsonResult j = this.Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
            return j;

        }

        #region 获取combobox信息
        
        // 获取事件类型      
        [HttpPost]
        public ActionResult GetEventType()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetEventType");
                    if (result == null)
                    {
                        result = bll.GetEventTypeList();
                        CacheHelper.SetCache("GetEventType", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        //获取事件来源
        [HttpPost]
        public ActionResult GetEventSource()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetEventSource");
                    if (result == null)
                    {
                        result = bll.GetEventSourceList();
                        CacheHelper.SetCache("GetEventSource", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取病情判断
        [HttpPost]
        public ActionResult GetIllnessState()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetIllnessState");
                    if (result == null)
                    {
                        result = bll.GetIllnessStateList();
                        CacheHelper.SetCache("GetIllnessState", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取车辆分站
        [HttpPost]
        public ActionResult GetStation()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetStation");
                    if (result == null)
                    {
                        result = bll.GetStationList();
                        CacheHelper.SetCache("GetStation", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取车辆编码
        [HttpPost]
        public ActionResult GetAmbulanceCode()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetAmbulanceCode");
                    if (result == null)
                    {
                        result = bll.GetAmbulanceCodeList();
                        CacheHelper.SetCache("GetAmbulanceCode", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取调度员
        [HttpPost]
        public ActionResult GetDispatcher()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetDispatcher");
                    if (result == null)
                    {
                        result = bll.GetDispatcherList();
                        CacheHelper.SetCache("GetDispatcher", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取司机
        [HttpPost]
        public ActionResult GetDriver()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetDriver");
                    if (result == null)
                    {
                        result = bll.GetDriverList();
                        CacheHelper.SetCache("GetDriver", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取医生
        [HttpPost]
        public ActionResult GetDoctor()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetDoctor");
                    if (result == null)
                    {
                        result = bll.GetDoctorList();
                        CacheHelper.SetCache("GetDoctor", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取护士
        [HttpPost]
        public ActionResult GetNurse()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetNurse");
                    if (result == null)
                    {
                        result = bll.GetNurseList();
                        CacheHelper.SetCache("GetNurse", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        // 获取担架员
        [HttpPost]
        public ActionResult GetStretcher()
        {
            try
            {
                EventInfoBLL bll = new EventInfoBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetStretcher");
                    if (result == null)
                    {
                        result = bll.GetStretcherList();
                        CacheHelper.SetCache("GetStretcher", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }

        #endregion
    }
}
