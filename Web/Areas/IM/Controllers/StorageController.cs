using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;

namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class StorageController : Controller
    {
        //
        // GET: /IM/Storage/

        public ActionResult StorageList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                List<Model.EasyUIModel.TreeNode> list = Model.I_Storage.ToTreeNodes(new I_StorageBLL().GetStorageListby("StorageID>0"));
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }
        }
        public ActionResult AddStroage()
        {
            string pid = Request.Form["PID"].ToString();
            int lCode = int.Parse(Request.Form["LevelC"]) + 1;
            string name = Request.Form["StroageName"].ToString();
            I_Storage storage = new I_Storage();
            storage.ParentStorageID = pid;
            storage.Name = name;
            storage.LevelCode = lCode;
            if (new I_StorageBLL().Add(storage) > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "新增仓库成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增仓库失败！", null, null);
        }
        public ActionResult EditStroage()
        {
            int sid = int.Parse(Request.Form["SID"]);
            string name = Request.Form["StroageName"].ToString();
            I_Storage storage = new I_Storage();
            storage.StorageID = sid;
            storage.Name = name;
            if (new I_StorageBLL().Modify(storage,"Name") > 0)
                return this.JsonResult(Utility.E_JsonResult.OK, "修改仓库成功！", null, null);
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "修改仓库失败！", null, null);
        }

    }
}
