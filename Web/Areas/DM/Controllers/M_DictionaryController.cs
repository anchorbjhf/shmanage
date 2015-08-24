using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class M_DictionaryController : Controller
    {
        //
        // GET: /DM/M_Dictionary/

        public ActionResult M_DictionaryList()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LoadMDictionaryType()
        {
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            int rowCount = 0;
            M_DictionaryTypeBLL dtbll = new M_DictionaryTypeBLL();

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


        //加载字典子目录树内容
        [HttpPost]
        public ActionResult DataLoad()
        {
            try
            {
                string TTypeID = Request.Form["TTypeID"].ToString();
                int pageSize = int.Parse(Request.Form["rows"]);
                //获取请求的页码
                int pageIndex = int.Parse(Request.Form["page"]);
                int rowCount = 0;
               
                // 查询分页数据
                var list = new M_DictionaryBLL().GetPagedList(pageIndex, pageSize, ref rowCount, p => p.TypeID == TTypeID, p => p.SN).Select(p => p.ToPOCO());
                Model.EasyUIModel.DataGridModel dgModel = new Model.EasyUIModel.DataGridModel()
                {
                    total = rowCount,
                    rows = list,
                    footer = null
                };
                return Json(dgModel);
            }
            catch
            {
                return this.Json("");
            }

        }
        //新增字典信息，ID为TypeID和最大TypeID数量+1的拼出结果。
        public ActionResult AddDictionary()
        {
            string TDictionaryType = Request.Form["TDictionaryType"].ToString();
            int maxID = new TDictionaryBLL().GetMaxDictionaryID(TDictionaryType, "M_Dictionary");
            string newID = TDictionaryType + "-" + (maxID).ToString();

            int newSN = int.Parse(Request.Form["SN"]);
            string newName = Request.Form["DinctionaryName"].ToString();
          
            M_Dictionary dic = new M_Dictionary();
            dic.ID = newID;
            dic.Name = newName;
            dic.TypeID = TDictionaryType;
            dic.SN = newSN;
            dic.IsActive = true;
           

            if (new M_DictionaryBLL().Add(dic) > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "新增字典信息成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增字典信息失败！", null, null);
        }

        //修改
        public ActionResult EditDictionary() 
        {
            string DictionaryID = Request.Form["DID"].ToString();
            string DictionaryName = Request.Form["DictionaryName"].ToString();
            int SN = int.Parse(Request.Form["SN"]);
            M_Dictionary dic = new M_Dictionary();
            dic.ID = DictionaryID;
            dic.Name = DictionaryName;
            dic.SN = SN;
            if (new M_DictionaryBLL().Modify(dic, "Name", "SN") > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "修改字典信息成功！", null, null);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "修改字典信息失败！", null, null);
            }
        }

        public ActionResult updataActive()
        {
            string DictionaryID = Request.Form["DID"].ToString();
            bool isActive = Convert.ToBoolean(Request.Form["isActive"]);
            M_Dictionary dic = new M_Dictionary();
            dic.ID = DictionaryID;
            dic.IsActive = isActive;
            if (new M_DictionaryBLL().Modify(dic, "IsActive") > 0)
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
