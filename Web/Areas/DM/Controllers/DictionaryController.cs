using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class DictionaryController : Controller
    {
        //
        // GET: /DM/Dictionary/
       
        public ActionResult DictionaryList()
        {
            return View();
        }

        //加载字典子目录树内容
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                string TTypeID = Request.Form["TTypeID"].ToString();

                List<Model.EasyUIModel.TreeNode> list = Model.TDictionary.ToTreeNodes(new TDictionaryBLL().GetListBy(p => p.TypeID == TTypeID, p => p.SN));
                return Json(list);
            }
            catch
            {
                return this.Json("");
            }

        }

        //加载DictionaryType基本数据
         [HttpPost]
        public ActionResult LoadDictionaryType()
        {
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int rowCount = 0;
            TDictionaryTypeBLL dtbll = new TDictionaryTypeBLL();

            // 查询分页数据
            var list = dtbll.GetPagedList(pageIndex, pageSize, ref rowCount, p => p.TypeID.Length > 0, p => p.ID).Select(p => p.ToPOCO());
            // 生成规定格式的 json字符串发回 给异步对象
            Model.EasyUIModel.DataGridModel dgModel = new Model.EasyUIModel.DataGridModel()
            {
                total = rowCount,
                rows = list,
                footer = null
            };
            return Json(dgModel);
        }

        //新增Dictionary信息
         [HttpPost]
        public ActionResult AddDictionary()
        {

            string TDictionaryType = Request.Form["TDictionaryType"].ToString();
            int maxID = new TDictionaryBLL().GetMaxDictionaryID(TDictionaryType, "TDictionary");

            string newID = TDictionaryType + "-" + (maxID).ToString();

            int newSN = int.Parse(Request.Form["SN"]);
            string newName = Request.Form["DinctionaryName"].ToString();
            string pid = Request.Form["PID"].ToString();
            TDictionary dic = new TDictionary();
            dic.ID = newID;
            dic.Name = newName;
            dic.TypeID = TDictionaryType;
            dic.SN = newSN;
            dic.IsActive = true;
            dic.ParentID = pid;

            if (new TDictionaryBLL().Add(dic) > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "新增字典信息成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增字典信息失败！", null, null);
        }
        //修改Dictionary信息


        public ActionResult GetSNByID(string ID)
        {
            string DictionaryID = Request.Form["DID"].ToString();
            var list = new TDictionaryBLL().GetDictionarySNByID(DictionaryID);
           return Json(new { SNInfo = list }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

         [HttpPost]
        public ActionResult EditDictionary()
        {
            string DictionaryID = Request.Form["DID"].ToString();
            string DictionaryName = Request.Form["DictionaryName"].ToString();
            int SN = int.Parse(Request.Form["SN"]);
            TDictionary dic = new TDictionary();
            dic.ID = DictionaryID;
            dic.Name = DictionaryName;
            dic.SN = SN;
            if (new TDictionaryBLL().Modify(dic, "Name" ,"SN") > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "修改字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "修改字典信息失败！", null, null);
            }
        }

        //启用，禁用字典信息
        [HttpPost]
        public ActionResult updataActive()
        {
            string DictionaryID = Request.Form["DID"].ToString();
            bool isActive = Convert.ToBoolean(Request.Form["isActive"]);
            string DictionaryName = Request.Form["DinctionaryNameActive"].ToString();
            TDictionary dic = new TDictionary();
            dic.ID = DictionaryID;
            dic.IsActive = isActive;
            if (isActive == false)
            {
                dic.Name = DictionaryName + " 【 已禁用！ 】";
            }
               
            else { 
                
              string newDN=  DictionaryName.Substring(0,DictionaryName.Length-9);
                dic.Name = newDN ; }

            if (new TDictionaryBLL().Modify(dic, "IsActive", "Name") > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "修改字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "修改字典信息失败！", null, null);
            }
        }

         [HttpPost]
        public ActionResult AddDictonaryType()
        {
            string TypeID = Request.Form["TypeID"].ToString();
            string Description = Request.Form["Description"].ToString();
            int maxID = new TDictionaryBLL().GetMaxDictionaryTypeID();
            int newID = maxID + 1;

            TDictionary dic = new TDictionary();
            dic.ID = TypeID + "-1";
            dic.Name = Description;
            dic.TypeID = TypeID;
            dic.SN = 1;
            dic.IsActive = true;
            dic.ParentID = "-1";

            TDictionaryType dictype = new TDictionaryType();
            dictype.TypeID = TypeID;
            dictype.ID = newID;
            dictype.Description = Description;
            dictype.IsActive = true;
            if (new TDictionaryTypeBLL().Add(dictype) > 0 && new TDictionaryBLL().Add(dic) > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "新增字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "新增字典信息失败！", null, null);
            }
        }

         [HttpPost]
        public ActionResult EditDictonaryType()
        {
            int DID = int.Parse(Request.Form["DID"]);
            string TypeID = Request.Form["TypeID"].ToString();
            string Description = Request.Form["Description"].ToString();
            TDictionary dic = new TDictionary();
            dic.ID = TypeID + "-1";
            dic.TypeID = TypeID;
            dic.Name = Description;


            TDictionaryType dictype = new TDictionaryType();
            dictype.ID = DID;
            dictype.TypeID = TypeID;
            dictype.Description = Description;

            if (new TDictionaryTypeBLL().Modify(dictype, "TypeID", "Description") > 0 && new TDictionaryBLL().Modify(dic, "TypeID", "Name")>0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "修改字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "修改字典信息失败！", null, null);
            }
        }

         [HttpPost]
        public ActionResult updataTypeActive()
        {
            int DTID = int.Parse(Request.Form["DTID"]);
            bool isActive = Convert.ToBoolean(Request.Form["isActive"]);
            TDictionaryType dic = new TDictionaryType();
            dic.ID = DTID;
            dic.IsActive = isActive;
            if (new TDictionaryTypeBLL().Modify(dic, "IsActive") > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "修改字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "修改字典信息失败！", null, null);
            }
        }



    }
}
