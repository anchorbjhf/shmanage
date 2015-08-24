using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class M_ZICDNewController : Controller
    {
        //
        // GET: /DM/M_ZICDNew/

        public ActionResult M_ZICDNewList()
        {
            return View();
        }
        //查出树状结构
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                List<Model.EasyUIModel.TreeNode> list = Model.M_ZICDNew.ToTreeNodes(new M_ZICDNewBLL().GetImpressionList());
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
            M_ZICDNewExt model = new M_ZICDNewBLL().GetImpressionList(id);


            return Json(new { model }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        //新增
        public ActionResult AddImpression()
        {
            string pid = Request.Form["PID"].ToString();         
            List<M_ZICDNew> list = new M_ZICDNewBLL().GetMaxID();
            int id = list[0].ID + 1;
            string name = Request.Form["ImpressionName"].ToString();
            bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
            int sn = int.Parse(Request.Form["SN"]);
            string pinYin = Request.Form["PinYin"].ToString();
            M_ZICDNew impression = new M_ZICDNew();
            impression.ID = id;
            impression.ParentID = Convert.ToInt32(pid);
            impression.Name = name;
            impression.IsActive = isActive;
            impression.SN = sn;
            impression.PinYin = pinYin;
            if (new M_ZICDNewBLL().Add(impression) > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "新增成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增失败！", null, null);
        }

        //修改
        public ActionResult EditImpression()
        {
            int sid = int.Parse(Request.Form["SID"]);
            string name = Request.Form["ImpressionName"].ToString();
            int pid = int.Parse(Request.Form["PID"]);
            bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
            int sn = int.Parse(Request.Form["SN"]);
            bool isc = Convert.ToBoolean(Request.Form["isAct"]); 
            M_ZICDNew impression = new M_ZICDNew();
            impression.ID = sid;
            impression.Name = name;
            impression.ParentID = pid;
            impression.IsActive = isActive;
            impression.SN = sn;
            if (Request.Form["PinYin"] == null)
            { impression.PinYin = ""; }
            else
            { impression.PinYin = Request.Form["PinYin"]; }
            if (new M_ZICDNewBLL().Modify(impression, "ID", "Name", "ParentID", "IsActive", "SN", "PinYin") > 0)
            {
                if (isc != isActive)
                {
                    bool bol = new M_ZICDNewBLL().Update(sid, isActive);


                };
              
                return this.JsonResult(Utility.E_JsonResult.OK, "修改成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改失败！", null, null);
        }


    }
}
