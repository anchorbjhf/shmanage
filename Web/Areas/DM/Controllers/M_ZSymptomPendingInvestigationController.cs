using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class M_ZSymptomPendingInvestigationController : Controller
    {
        //
        // GET: /DM/M_ZSymptomPendingInvestigation/

        public ActionResult M_ZSymptomPendingInvestigationList()
        {
            return View();
        }
        //查树状页面
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                List<Model.EasyUIModel.TreeNode> list = Model.M_ZSymptomPendingInvestigation.ToTreeNodes(new M_ZSymptomPendingInvestigationBLL().GetSymptomList());
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        //根据id取其对应的model
        public ActionResult GetListByID(int id)
        {
            M_ZSymptomPendingInvestigationExt model = new M_ZSymptomPendingInvestigationBLL().GetSymptomList(id);


            return Json(new { model }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        //新增
        public ActionResult AddSymptom()
        {
            string pid = Request.Form["PID"].ToString();
            List<M_ZSymptomPendingInvestigation> list = new M_ZSymptomPendingInvestigationBLL().GetMaxID();
            int id = list[0].ID + 1;
            string name = Request.Form["SymptomName"].ToString();
            bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
            int sn = int.Parse(Request.Form["SN"]);
            string pinYin = Request.Form["PinYin"].ToString();
            M_ZSymptomPendingInvestigation symptom = new M_ZSymptomPendingInvestigation();
            symptom.ID = id;
            symptom.ParentID = Convert.ToInt32(pid);
            symptom.Name = name;
            symptom.IsActive = isActive;
            symptom.SN = sn;
            symptom.PinYin = pinYin;
            if (new M_ZSymptomPendingInvestigationBLL().Add(symptom) > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "新增成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增失败！", null, null);
        }

        //修改
        public ActionResult EditSymptom()
        {
            int sid = int.Parse(Request.Form["SID"]);
            string name = Request.Form["SymptomName"].ToString();
            int pid = int.Parse(Request.Form["PID"]);
            bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
            int sn = int.Parse(Request.Form["SN"]);
            bool isc = Convert.ToBoolean(Request.Form["isAct"]); 
            string pinYin = Request.Form["PinYin"].ToString();
            M_ZSymptomPendingInvestigation symptom = new M_ZSymptomPendingInvestigation();
            symptom.ID = sid;
            symptom.Name = name;
            symptom.ParentID = pid;
            symptom.IsActive = isActive;
            symptom.SN = sn;
            if (Request.Form["PinYin"] == null)
            { symptom.PinYin = ""; }
            else
            { symptom.PinYin = Request.Form["PinYin"]; }
            if (new M_ZSymptomPendingInvestigationBLL().Modify(symptom, "ID", "Name", "ParentID", "IsActive", "SN", "PinYin") > 0)
            {
                if (isc != isActive)
                {
                    bool bol = new M_ZSymptomPendingInvestigationBLL().Update(sid, isActive);


                };

                return this.JsonResult(Utility.E_JsonResult.OK, "修改成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改失败！", null, null);
        }

    }

}

