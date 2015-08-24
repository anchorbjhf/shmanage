using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BB.Controllers
{
    public class TJRedirectLinkController : Controller
    {
        //
        // GET: /BB/TJRedirectLink/

        #region  统计页面跳转
        public ActionResult Index()
        {
            return View();
        }
        //调度业务数据统计
        public ActionResult LinkDD()
        {
            string LinkDD = "LinkDD";
            P_PermissionBLL bll = new P_PermissionBLL();
            //在Permission表中先根据ActionName取出对应的info的ID，赋值给parentID  在DataLoad方法中取出所有"孩子"
            Model.P_Permission info = bll.GetListBy(p => p.ActionName == LinkDD && p.IsActive == true).FirstOrDefault();
            ViewData["parentID"] = info.ID;
            return View();
        }
        //质量监控业务数据统计
        public ActionResult LinkZJ()
        {
            string LinkZJ = "LinkZJ";
            P_PermissionBLL bll = new P_PermissionBLL();
            Model.P_Permission info = bll.GetListBy(p => p.ActionName == LinkZJ && p.IsActive == true).FirstOrDefault();
            ViewData["parentID"] = info.ID;
            return View();
        }
        //领导台统计
        public ActionResult LinkLD()
        {
            string LinkLD = "LinkLD";
            P_PermissionBLL bll = new P_PermissionBLL();
            Model.P_Permission info = bll.GetListBy(p => p.ActionName == LinkLD && p.IsActive == true).FirstOrDefault();
            ViewData["parentID"] = info.ID;
            return View();
        }
        //急救科统计
        public ActionResult LinkJJ()
        {
            string LinkJJ = "LinkJJ";
            P_PermissionBLL bll = new P_PermissionBLL();
            Model.P_Permission info = bll.GetListBy(p => p.ActionName == LinkJJ && p.IsActive == true).FirstOrDefault();
            ViewData["parentID"] = info.ID;
            return View();
        }

#endregion

        #region 个人业务统计
        public ActionResult LinkPersonal()
        {
            return View();
        }
        //获取个人业务
        //实例化,整个页面只NEW一次，该方法内可反复调用。
        private PersonalStatisticsBLL psbll = new PersonalStatisticsBLL();
        public ActionResult GetPersonalStatistics()
        {
            DateTime beginTime = Convert.ToDateTime(Request.Form["beginTime"]);
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"]);

            var listdd = psbll.GetPersonalStatisticsDD(beginTime, endTime);
            var listcharge = psbll.GetPersonalStatisticsGLCharge(beginTime, endTime);
            var listpr = new PersonalStatisticsBLL().GetPersonalStatisticsGLPR(beginTime, endTime);
       

            return Json(new { PSInfo = listdd, PSChargeInfo = listcharge, PSPRInfo = listpr }, "appliction/json", JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetStationStatistics()
        {
            DateTime beginTime = Convert.ToDateTime(Request.Form["beginTime"]);
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"]);


            var listdd = psbll.GetStationStatisticsDD(beginTime, endTime);
            var listcharge = psbll.GetStationStatisticsGL1(beginTime, endTime);
            var listpr = new PersonalStatisticsBLL().GetStationStatisticsGL2(beginTime, endTime);

            return Json(new { SSInfo = listdd, SSChargeInfo = listcharge, SSPRInfo = listpr }, "appliction/json", JsonRequestBehavior.AllowGet);

        }

      
        public ActionResult GetCenterStatistics()
        {
            DateTime beginTime = Convert.ToDateTime(Request.Form["beginTime"]);
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"]);

            var listdd = psbll.GetCenterStatisticsDD(beginTime, endTime);
            var listcharge = psbll.GetCenterStatisticsGL1(beginTime, endTime);
            var listpr = new PersonalStatisticsBLL().GetCenterStatisticsGL2(beginTime, endTime);
            return Json(new { CSInfo = listdd, CSChargeInfo = listcharge, CSPRInfo = listpr }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTotalStatistics()
        {
            DateTime beginTime = Convert.ToDateTime(Request.Form["beginTime"]);
            DateTime endTime = Convert.ToDateTime(Request.Form["endTime"]);

            var listdd = psbll.GetTotalStatisticsDD(beginTime, endTime);
            var listcharge = psbll.GetTotalStatisticsGL1(beginTime, endTime);
            var listpr = new PersonalStatisticsBLL().GetTotalStatisticsGL2(beginTime, endTime);
            return Json(new { CSInfo = listdd, CSChargeInfo = listcharge, CSPRInfo = listpr }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  个人业务统计简版，用于主页下挂
        //做了判断，根据个人工号查询最后一次操作的车辆所在的分站，如果没有，就查询session中个人所在分中心，显示分中心，
        //如果没有分中心，就显示仅显示全中心内容。
        public ActionResult LinkPersonalSimple()
        {
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfStationID = new PersonalStatisticsBLL().GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
            if (selfStationID != null && selfStationID != "")
            {
                ViewBag.IFHasStation = true;
            }
            else { ViewBag.IFHasStation = false; }

            if (selfCenterID != null && selfCenterID != "")
            {
                ViewBag.IFHasCenter = true;
            }
            else
            {
                ViewBag.IFHasCenter = false;
            }
            return View();
        }
        #endregion


        //根据父ID查询
        public ActionResult DataLoad(int parentID)
        {
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int rowCount = 0;
            P_PermissionBLL bll = new P_PermissionBLL();

            //取权限list<Model>，new list<ID> 使用循环，将List<Model>中的ID 取出ADD进list<ID>中 再查询时，where 添加List<ID> contains(p.id)
            List<Model.P_Permission> mlist = UserOperateContext.Current.Session_UsrPermission.ToList();
            List<int> listid = new List<int>();
            for (int i = 0; i < mlist.Count; i++)
            {
                Model.P_Permission m = (Model.P_Permission)mlist[i];
                listid.Add(m.ID);
            }
            // 查询分页数据
            var list = bll.GetPagedList(pageIndex, pageSize, ref rowCount, p => p.ParentID == parentID && p.IsActive == true && listid.Contains(p.ID), p => p.SN).Select(p => p.ToPOCO());
            // 生成规定格式的 json字符串发回 给异步对象
            Model.EasyUIModel.DataGridModel dgModel = new Model.EasyUIModel.DataGridModel()
            {
                total = rowCount,
                rows = list,
                footer = null
            };
            return Json(dgModel);
        }
    }
}
