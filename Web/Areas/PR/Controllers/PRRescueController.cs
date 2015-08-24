using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using Anke.SHManage.BLL;

namespace Anke.SHManage.Web.Areas.PR.Controllers
{
    public class PRRescueController : Controller
    {
        //
        // GET: /PR/PRRescue/

        public ActionResult Index()
        {
            return View();
        }


        #region 获取救治记录列表
        [HttpPost]
        public ActionResult GetPatientRecordRescue()
        {
            try
            {
                //string taskCode, int patientOrder, int pageSize, int pageIndex
                string taskCode = Request.Form["TaskCode"].ToString();
                int patientOrder = int.Parse(Request.Form["PatientOrder"]);

                //获取页容量
                int pageSize = int.Parse(Request.Form["rows"]);
                //获取请求的页码
                int pageIndex = int.Parse(Request.Form["page"]);

                M_PatientRecordBLL bll = new M_PatientRecordBLL();
                int total = 0;
                List<M_PatientRecordRescue> list = bll.GetPRRescueList(pageIndex, pageSize, ref total, taskCode, patientOrder);
                return Json(new { total = total, rows = list }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogUtility.Error("PRRescueController/GetPatientRecordRescue()", ex.ToString());
                return this.Json("");
            }
        }
        #endregion

        #region 保存病历--救治记录
        /// <summary>
        /// 保存病历--救治记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool SavePRRescue()
        {
            string state = "";
            M_PatientRecordBLL bll = new M_PatientRecordBLL();
            M_PatientRecordRescue prRescue = null;//救治记录主表信息
            List<M_PatientRecordMeasure> prMeasure = null;//救治记录--救治措施
            List<M_PatientRecordDrug> prDrug = null;//救治记录--药品
            List<M_PatientRecordSanitation> prSanitation = null;//救治记录--耗材
            List<M_PatientRecordLossDrug> prLossDrug = null;//救治记录--损耗药品
            List<M_PatientRecordLossSanitation> prLossSanitation = null;//救治记录--损耗耗材

            string PRRescue = Request.Form["PRRescue"].ToString();
            M_AddPatientRecordRescue add = new M_AddPatientRecordRescue();
            add = JsonHelper.GetJsonInfoBy<M_AddPatientRecordRescue>(PRRescue);
            if (add != null)
            {
                string TaskCode = add.TaskCode;
                int PatientOrder = add.PatientOrder;
                state = add.state;
                prRescue = add.prRescue;//救治记录主表信息

                if (add.prMeasure.Count > 0)
                {
                    prMeasure = add.prMeasure;//救治记录--救治措施
                }
                else
                { prMeasure = null; }
                if (add.prDrug.Count > 0)
                {
                    prDrug = add.prDrug;//救治记录--药品
                }
                else
                { prDrug = null; }

                if (add.prSanitation.Count > 0)
                {
                    prSanitation = add.prSanitation;//救治记录--耗材
                }
                else
                { prSanitation = null; }

                if (add.prLossDrug.Count > 0)
                {
                    prLossDrug = add.prLossDrug;//救治记录--损耗药品
                }
                else
                { prLossDrug = null; }

                if (add.prLossSanitation.Count > 0)
                {
                    prLossSanitation = add.prLossSanitation;//救治记录--损耗耗材
                }
                else
                { prLossSanitation = null; }
            }

            bool save = false;
            if (prRescue != null)
            {
                try
                {
                    if (state == "new")
                    {
                        save = bll.InsertPRRescue(prRescue, prMeasure, prDrug, prSanitation, prLossDrug, prLossSanitation);//新增救治记录主表、子表
                    }
                    else if (state == "edit")
                    {
                        save = bll.UpdatePRRescue(prRescue, prMeasure, prDrug, prSanitation, prLossDrug, prLossSanitation);//修改救治记录主表、子表
                    }
                }
                catch
                {
                    save = false;
                }
            }
            return save;
        }
        #endregion


        #region 删除病历--救治记录
        [HttpPost]
        public bool DeletePRRescue()
        {
            bool save = false;
            try
            {
                M_PatientRecordBLL bll = new M_PatientRecordBLL();
                string TaskCode = Request.Form["TaskCode"].ToString();
                int PatientOrder = Convert.ToInt32(Request.Form["PatientOrder"]);
                string RescueRecordCode = Request.Form["RescueRecordCode"].ToString();
                int DisposeOrder = Convert.ToInt32(Request.Form["DisposeOrder"]);
                if (TaskCode != null)
                {
                    save = bll.DeletePRRescue(TaskCode, PatientOrder, RescueRecordCode, DisposeOrder);
                }
            }
            catch
            {
                save = false;
            }
            return save;
        }
        #endregion

        
        #region 获取救治记录
        [HttpPost]
        public ActionResult GetPRRescue()
        {
            try
            {
                M_PatientRecordBLL prBLL = new M_PatientRecordBLL();
                M_PatientRecordRescue prrInfo;//病历--救治记录主表
                string TaskCode = Request.Form["TaskCode"].ToString();
                int PatientOrder = Convert.ToInt32(Request.Form["PatientOrder"]);
                string RescueRecordCode = Request.Form["RescueRecordCode"].ToString();
                int DisposeOrder = Convert.ToInt32(Request.Form["DisposeOrder"]);
                prBLL.GetPRRescueInfo(TaskCode, PatientOrder, RescueRecordCode, DisposeOrder, out prrInfo);

                var model = prrInfo;
                return Json(new { IsSuccess = true, Model = model });
            }
            catch (Exception ex)
            {
                LogUtility.Error("PRRescueController/GetPRRescue()", ex.ToString());
                return this.Json("");
            }
        }
        #endregion
    }
}
