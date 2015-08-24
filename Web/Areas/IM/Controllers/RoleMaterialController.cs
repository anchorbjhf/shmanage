using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;


namespace Anke.SHManage.Web.Areas.IM.Controllers
{
    public class RoleMaterialController : Controller
    {
        //
        // GET: /IM/RoleMaterial/

        public ActionResult RoleMaterial()
        {
            return View();
        }
       
        #region 取角色物资关联
        public ActionResult RoleMaterialEditNew(int id)
        {

            I_StorageRoleBLL sbll = new I_StorageRoleBLL();
            try
            {
                List<RoleMaterialLinkInfo> list = sbll.GetRoleMaterialList(id);
                ViewData["RoleID"] = id;
                return View(list);
            }
            catch
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "取角色关联失败", null, "");
            }

        }
        #endregion

        #region 保存
        [HttpPost]        
        public ActionResult RoleMaterialEditNew()
        {
            int roleID = -1;
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            for (int i = 0; i < Request.Form.Keys.Count; i++)
            {
                string key = Request.Form.Keys[i];
                if (key == "RoleID")
                {
                    roleID = int.Parse(Request.Form["RoleID"]);
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

            List<I_StorageRole> lst = new List<I_StorageRole>();
            foreach (string materialType in dic.Keys)
            {
                bool isSelected = dic[materialType];
                if (isSelected)
                {
                    I_StorageRole info = new I_StorageRole();
                    info.MaterialType = materialType;
                    info.RoleID = roleID;
                    lst.Add(info);
                }
            }
            I_StorageRoleBLL bll = new I_StorageRoleBLL();
            bool dbret = bll.Update(roleID, lst);
            //可以优化，dbret为true时转到主页面，false时转到错误业务提醒用户
            return RedirectToAction("RoleMaterial");
        }
        #endregion

        #region 查询角色物资关系
        public ActionResult DataLoad()
        {
            //获取页容量
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);

            I_StorageRoleBLL bll = new I_StorageRoleBLL();
            var list = bll.GetLists(pageSize, pageIndex);
            JsonResult j = this.Json(list, "appliction/json", JsonRequestBehavior.AllowGet);
            return j;
        }
        #endregion
      
    }
}
