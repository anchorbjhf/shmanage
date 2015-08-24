using Anke.SHManage.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class MaterialDeliveryOrderController : Controller
    {
        //
        // GET: /IM/MaterialDeliveryOrder/

        public ActionResult MaterialDeliveryOrder()
        {
            return View();
        }

        /// <summary>
        /// 页面初始化，需要将出库类型，物资来源，物资去向分别从字典表、仓库分类表中提取出来
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryOrder()
            //string menuName, string personCode, int? pageNumber, string startTime, string endTime)
        {
            //ViewData["MenuName"] = menuName;
            //ViewData["personid"] = personCode;
            //if (pageNumber == null || pageNumber == 0)
            //    this.ViewData["pageNumber"] = "1";
            //else
            //    this.ViewData["pageNumber"] = pageNumber;
            this.ViewData["startTime"] =  DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            this.ViewData["endTime"] = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");

            return View();
        }
        //// 查询出库单操作
       [HttpPost]
        public ActionResult DataLoad()
            
           // int page, int rows, string order, string sort, DateTime startTime, DateTime endTime, string deliveryType,
           //string deliveryCode, string entryStorageCode, string operatorName, string mName, string receivingStoreID, string consigneeName, string mCode)
      
        {
            DateTime startTime = Convert.ToDateTime(Request.Form["startTime"]);
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"]);
            string deliveryType = Request.Form["deliveryType"].ToString();
            string deliveryCode = Request.Form["deliveryCode"].ToString();
            string entryStorageCode = Request.Form["entryStorageCode"].ToString();
            string operatorName = Request.Form["operatorName"].ToString();
            string mName = Request.Form["mName"].ToString();
            string receivingStoreID = Request.Form["receivingStoreID"].ToString();
            string consigneeName = Request.Form["consigneeName"].ToString();
            string mCode = Request.Form["mCode"].ToString();

            //获取页容量
            int rows = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int page = int.Parse(Request.Form["page"]);

            try
            {
                I_MaterialBLL dBLL = new I_MaterialBLL();
                var list = dBLL.GetDeliveryOrder(page, rows,startTime, endTime, deliveryType,
                  deliveryCode, entryStorageCode, operatorName, mName, receivingStoreID, consigneeName, mCode);

                return this.Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("InfoID", "0");
                dict.Add("InfoMessage", ex.Message);
                return this.Json(dict);
            }
        }
        /// <summary>
        /// 获取仓库基本信息,ID 和Name，用于物资来源，物资去向下拉框的选项
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStorage()
        {
            I_MaterialBLL dBLL = new I_MaterialBLL();
          var result =  dBLL.GetStorage();
          return Json(result);
         
        }
    }
}
