using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using Anke.SHManage.Model.ViewModel;


namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class PersonStorageController : Controller
    {
        private static object m_SyncRoot = new Object();//互斥对象
        //
        // GET: /IM/MaterialStoragePerson/

        public ActionResult PersonStorage()
        {
            return View();
        }

        #region 取人员仓库关系
        public ActionResult PersonStorageNewEdit(int id)
        {

            I_StoragePersonBLL sbll = new I_StoragePersonBLL();
            try
            {
                List<PersonStorageLinkInfo> list = sbll.GetPersonStorageList(id);
                ViewData["UserID"] = id;
                return View(list);
            }
            catch
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "取人员仓库关系失败", null, "");
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        public ActionResult PersonStorageSave()
        {
            int uid = -1;
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            for (int i = 0; i < Request.Form.Keys.Count; i++)
            {
                string key = Request.Form.Keys[i];
                if (key == "UserID")
                {
                    uid = int.Parse(Request.Form["UserID"]);
                }
                else if (!dic.Keys.Contains(key))
                {
                    string isChecked = Request.Form[key];
                    string[] isCheckedArr = isChecked.Split(',');
                    if (isCheckedArr.Length > 0)
                        dic[key] = Convert.ToBoolean(isCheckedArr[0]);
                    else
                        dic[key] = false;
                }
            }
            List<I_StoragePerson> lst = new List<I_StoragePerson>();
            foreach (string StorageID in dic.Keys)
            {
                bool isSelected = dic[StorageID];
                if (isSelected)
                {
                    I_StoragePerson info = new I_StoragePerson();
                    info.StorageID = int.Parse(StorageID);
                    info.UserID = uid;
                    lst.Add(info);
                }
            }
            I_StoragePersonBLL bll = new I_StoragePersonBLL();
            bool dbret = bll.Update(uid, lst);
            if (dbret)
                return Json(new { Message = "更新成功", Result = "OK", RedirecURL = "null", Data = "null" }, "text/html", JsonRequestBehavior.AllowGet);
                    //this.JsonResult(Utility.E_JsonResult.OK, "更新成功！", null, null);
            else
                return Json(new { Message = "更新失败", Result = "Error", RedirecURL = "null", Data = "null" }, "text/html", JsonRequestBehavior.AllowGet); 
                    //this.JsonResult(Utility.E_JsonResult.Error, "更新失败", null, null);
            //可以优化，dbret为true时转到主页面，false时转到错误业务提醒用户
            //return RedirectToAction("PersonStorage");
        }
        #endregion

        #region 查询人员仓库关系
        public ActionResult DataLoad()
        {
            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);
            //获取查询条件
            string name = Request.Form["name"];
            string workCode = Request.Form["workCode"];
            string stationName = Request.Form["stationName"];
            string roletypeName = Request.Form["roletypeName"];
            I_StoragePersonBLL bll = new I_StoragePersonBLL();
            var list = bll.GetList(pageSize, pageIndex, name, workCode, stationName, roletypeName);
            JsonResult j = this.Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
            return j;

        }
        #endregion

        #region 获取combobox信息
        //获取姓名
        [HttpPost]
        public ActionResult GetName()
        {
            try
            {
                I_StoragePersonBLL bll = new I_StoragePersonBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetName");
                    if (result == null)
                    {
                        result = bll.GetNameList();
                        CacheHelper.SetCache("GetName", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        //获取工号
        [HttpPost]
        public ActionResult GetWorkCode()
        {
            try
            {
                I_StoragePersonBLL bll = new I_StoragePersonBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetWorkCode");
                    if (result == null)
                    {
                        result = bll.GetWorkCodeList();
                        CacheHelper.SetCache("GetWorkCode", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }       
        //获取角色类型
        [HttpPost]
        public ActionResult GetRoleType()
        {
            try
            {
                I_StoragePersonBLL bll = new I_StoragePersonBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetRoleType");
                    if (result == null)
                    {
                        result = bll.GetRoleTypeList();
                        CacheHelper.SetCache("GetRoleType", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        // 获取部门名称
        [HttpPost]
        public ActionResult GetDep()
        {
            try
            {
                I_StoragePersonBLL bll = new I_StoragePersonBLL();
                var result = new object();
                //增加缓存
                lock (m_SyncRoot)
                {
                    result = CacheHelper.GetCache("GetDep");
                    if (result == null)
                    {
                        result = bll.GetDepList();
                        CacheHelper.SetCache("GetDep", result);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return this.Json("");
            }
        }
        #endregion

    }
}
