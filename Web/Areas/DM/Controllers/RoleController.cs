using Anke.SHManage.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class RoleController : Controller
    {
        #region 显示 角色视图 
        /// <summary>
        /// 显示 角色视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region  加载 角色数据
        /// <summary>
        /// 加载 角色数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            //0.接收参数 page=1&rows=5
            int pageIndex = int.Parse(form["page"]);
            int pageSize = int.Parse(form["rows"]);

            //1.读取数据
            var rowCount = 0;
            var list = new P_RoleBLL().GetDynamicPagedList(pageIndex, pageSize, ref rowCount, r => r.IsActive == true, r => r.ID, r => new { r.IsActive, r.ID, r.DepID, DepartmentName = r.P_Department.Name, r.Name,r.Remark });
            //2.将list转成 json 字符串发回浏览器
            return Json(new Model.EasyUIModel.DataGridModel() { rows = list, total = rowCount });
        }
        #endregion

        #region  获取角色页面

        /// <summary>
        /// 显示 新增角色 表单代码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRoleView()
        {
            int id = int.Parse(Request.Form["id"]);

            var viewModel = new Model.ViewModel.Role();
            if (id == -1)
            {
                viewModel.DepID = 1;
                viewModel.IsActive = true;
                viewModel.SN = 255;
                return PartialView("RoleForm", viewModel);
            }
            else
            {
                var tempModel = new P_RoleBLL().GetListBy(p => p.ID == id).FirstOrDefault();

                if (tempModel != null)
                {
                    viewModel.ID = tempModel.ID;
                    viewModel.IsActive = tempModel.IsActive;
                    viewModel.Name = tempModel.Name;
                    viewModel.DepID = tempModel.DepID;
                    viewModel.Remark = tempModel.Remark;
                    viewModel.SN = tempModel.SN;
                   
                }
                return PartialView("RoleForm", viewModel);
            }
        }

        #endregion

        #region 新增角色
        [HttpPost]
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRole(Model.P_Role model)
        {
            model.IsActive = true;

            int res = new P_RoleBLL().Add(model);
            if (res > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "新增权限成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "新增权限失败~！", null,null);
        }
        #endregion

        #region  修改角色
        [HttpPost]
        /// <summary>
        /// 角色修改
        /// </summary>
        /// <returns></returns>
        public ActionResult EditRole(Model.P_Role model)
        {
            int res = new P_RoleBLL().Modify(model, "DepID", "Name", "Remark", "IsActive", "SN");
            if (res > 0)
            {
                return this.JsonResult(Utility.E_JsonResult.OK, "编辑权限成功！", null, null);
            }
            else
                return this.JsonResult(Utility.E_JsonResult.Error, "编辑权限失败~！", null, null);
        }
        #endregion

        #region 删除角色
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelRole()
        {
            try
            {
                int id = int.Parse(Request.Form["id"]);
                new P_RoleBLL().DelBy(p => p.ID == id);
                return this.JsonResult(Utility.E_JsonResult.OK, "删除成功!", null, "");
            }
            catch (Exception ex)
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "删除角色错误！" + ex.Message, null, "");
            }
        }

        #endregion

        #region 保存角色

        /// <summary>
        ///保存角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRole(Model.ViewModel.Role model)
        {
            //先查角色是否存在  如果没有新增 反正修改
            if (new P_RoleBLL().GetModelWithOutTrace(d => d.ID == model.ID) != null)
            {
                int res = new P_RoleBLL().Modify(model.ToPOCO(), "Name", "ID", "IsActive", "Remark", "SN", "DepID");
                if (res > 0)
                    return this.JsonResult(Utility.E_JsonResult.OK, "修改成功!", null, null);
                else
                    return this.JsonResult(Utility.E_JsonResult.Error, "修改失败！", null, null);
            }
            else
            {
                //model.DepID = 1;
                int res = new P_RoleBLL().Add(model.ToPOCO());
                if (res > 0)
                    return this.JsonResult(Utility.E_JsonResult.OK, "新增成功!", null, null);
                else
                    return this.JsonResult(Utility.E_JsonResult.Error, "新增失败！", null, null);
            }
        }
        #endregion

        #region 加载 所有的权限树（把角色对应的权限选中）
        /// <summary>
        /// 3.0 加载 所有的权限树（把角色对应的权限选中）
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetRoleTree(int id)
        {
            //获取角色 权限ID
            var listRolePerIDs = new P_RolePermissionBLL().GetListBy(rp=>rp.RoleID==id).Select(rp=>rp.PermissionID);

            //获取所有 权限
            var listAllPer = new P_PermissionBLL().GetListBy(p => p.IsActive == true).ToList();

            //获取角色权限
            var listRolePer = (from p in listAllPer where listRolePerIDs.Contains(p.ID) select p).ToList();

            //获取父权限集合
            var listParentPer = (from p in listAllPer where p.ParentID == 0 select p).ToList();
            //var listParentPer = (from p in listAllPer select p).ToList();
            //准备一个 角色id，传给视图
            ViewBag.roleId = id;

            return PartialView(new Model.ViewModel.RolePemissionTree()
            {
                RolePers = listRolePer,
                AllPers = listAllPer,
                AllParentPers = listParentPer
            });
        }
        #endregion

        #region 设置角色权限 + SetRolePer() 
        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
 
        public ActionResult SetRolePer()
        {
            try
            {
                //获取角色id
                int roleId = int.Parse(Request.Form["rId"]);
                //获取新的权限 id字符串
                string[] perIds = Request.Form["rolePerids"].Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //原来的所有权限id 3
                var listOldPer = new P_RolePermissionBLL().GetListBy(rp => rp.RoleID == roleId).Select(r => r.PermissionID).ToList();

                //新权限id 3,5,6
                var listNewPer = perIds.ToList();
                //将两个集合中 相同的元素 都删除，之后，新权限集合里的权限就是要新增到数据库的，旧权限集合里的权限 就是要从数据库删除的
                for (int i = listOldPer.Count - 1; i >= 0; i--)
                {
                    int old = listOldPer[i];
                    if (listNewPer.Contains(old.ToString()))
                    {
                        listOldPer.Remove(old);
                        listNewPer.Remove(old.ToString());
                    }
                }

                //新增新的权限
                listNewPer.ForEach(newP =>
                {
                    new P_RolePermissionBLL().Add(new Model.P_RolePermission() { RoleID = roleId, PermissionID = int.Parse(newP), IsActive = true });
                });

                //移除旧的权限
                listOldPer.ForEach(oldP =>
                {
                    new P_RolePermissionBLL().DelBy(p => p.RoleID == roleId && p.PermissionID == oldP);
                });
            }
            catch (Exception ex)
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "异常原因：" + ex.Message, null, null);
            }

            return this.JsonResult(Utility.E_JsonResult.OK, "修改权限成功~~~！", null, null);
        }
        #endregion

        /// <summary>
        /// 获取部门角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRoleByDepId()
        {
            var list = new P_RoleBLL().GetListBy(r => r.IsActive==true).Select(r => r.ToPOCO());
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
