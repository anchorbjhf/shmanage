using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Utility;

namespace Anke.SHManage.Web.Areas.PR.Controllers
{
    public class DictionaryController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        //
        // GET: /PR/Dictionary/

        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult GetChargeItemDictionaryTrees()
        {
            try
            {
                M_DictionaryTreeBLL bll = new M_DictionaryTreeBLL();
                //string ParentID, string TypeID, string treeState
                string ParentID = Request.Form["ParentID"].ToString();
                string TypeID = Request.Form["TypeID"].ToString();
                string treeState = Request.Form["treeState"].ToString();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache(TypeID);
                    if (result == null)
                    {
                        result = bll.GetModelChargeItemList(ParentID, TypeID,treeState);
                        CacheHelper.SetCache(TypeID, result);
                    }
                }
                JsonResult j = this.Json(result, "appliction/json", JsonRequestBehavior.AllowGet);
                return j;
            }
            catch
            {
                return this.Json("");
            }
        }
    }
}
