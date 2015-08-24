using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class M_ZECGImpressionController : Controller
    {
        //
        // GET: /DM/M_ZECGImpression/

        public ActionResult M_ZECGImpressionList()
        {
            return View();
        }

        //查询节点
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                List<Model.EasyUIModel.TreeNode> list = Model.M_ZECGImpression.ToTreeNodes(new M_ZECGImpressionBLL().GetImpressionList());
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        //通过id取此id对应的数据
        public ActionResult GetListByID(int id)
        {
            M_ZECGImpressionExt model = new M_ZECGImpressionBLL().GetImpressionList(id);

          
            return Json(new { model}, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        //新增
        public ActionResult AddImpression()
        {
            string pid = Request.Form["PID"].ToString();
            //int lCode = int.Parse(Request.Form["LevelC"]) + 1;
            List<M_ZECGImpression> list = new M_ZECGImpressionBLL().GetMaxID();          
            int id = list[0].ID + 1;
            string name = Request.Form["ImpressionName"].ToString();
            bool isActive= Convert.ToBoolean(Request.Form["IsActive"]);
            int sn = int.Parse(Request.Form["SN"]);
            string pinYin= Request.Form["PinYin"].ToString();
            M_ZECGImpression impression = new M_ZECGImpression();
            impression.ID = id;
            impression.ParentID = Convert.ToInt32(pid);
            impression.Name = name;
           // storage.LevelCode = lCode;
            impression.IsActive = isActive;
            impression.SN = sn;
            impression.PinYin = pinYin;
            if (new M_ZECGImpressionBLL().Add(impression) > 0)
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
            //string pinYin = Request.Form["PinYin"].ToString();
            M_ZECGImpression impression = new M_ZECGImpression();
            impression.ID = sid;
            impression.Name = name;
            impression.ParentID = pid;
            impression.IsActive = isActive;
            impression.SN = sn;
            if (Request.Form["PinYin"] == null)
            { impression.PinYin = ""; }
            else
            { impression.PinYin = Request.Form["PinYin"]; }
            
            if (new M_ZECGImpressionBLL().Modify(impression, "ID", "Name", "ParentID", "IsActive", "SN", "PinYin") > 0)
            {
                if (isc != isActive)
                {
                    bool bol = new M_ZECGImpressionBLL().Update(sid, isActive);
                   

                };         
                return this.JsonResult(Utility.E_JsonResult.OK, "修改成功！", null, null);
               
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改失败！", null, null);
        }
       

    }
}

