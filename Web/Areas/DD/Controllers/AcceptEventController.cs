using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DD.Controllers
{
    public class AcceptEventController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        //
        // GET: /DD/AcceptEvent/

        //受理事件任务列表母页 根据事件编码查出基本事件信息
        public ActionResult AcceptEventTaskList()
        //  string EventCode)
        {
            string EventCode = "201209010725180009";
            ViewBag.EventCode = EventCode;
            //ViewBag.OrderNumber = OrderNumber;
            return View();
        }

        /// <summary>
        /// 获取事件信息节点，分配tabs使用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEventNode()
        {
            string code = Request.Form["code"].ToString();

            
           var list =   new AcceptEventBLL().GetEventNode(code);
            return Json(new { PRInfo = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AcceptEventTaskInfo()
            //string EventCode, int AcceptOrder)
        {
            //ViewBag.EventCode = EventCode;
            //ViewBag.AcceptOrder = AcceptOrder;
            return View();
        }
        //获取受理详细信息
        public ActionResult GetAcceptEventInfo()
        {
           // string EventCode = Request.Form["EventCode"].ToString();
            //int AcceptOrder = int.Parse(Request.Form["AcceptOrder"]);
            string EventCode = "201209010725180009";
            int AcceptOrder = 1;
            
            // dynamic editinfo = null;
         var list=   new AcceptEventBLL().GetAcceptEventInfoByCode(EventCode, AcceptOrder);
            return Json(new { PRInfo = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        //获取事件详细信息
        public ActionResult GetAlarmEventInfo()
        {
            //string EventCode = Request.Form["EventCode"].ToString();
           // int AcceptOrder = int.Parse(Request.Form["AcceptOrder"]);
            string EventCode = "201209010725180009";
           
            var list = new AcceptEventBLL().GetAlarmEventInfoByCode(EventCode);
            return Json(new { PLInfo = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }


    }
}
