using Anke.SHManage.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class DepartmentController : Controller
    {
        #region 部门列表 视图
        /// <summary>
        /// 部门列表 视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 部门列表 数据
        /// <summary>
        /// 部门列表 数据
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

            var listData = new P_DepartmentBLL().GetDynamicPagedList(pageIndex, pageSize, ref rowCount, d => d.IsActive == true, d => d.ID, d => new { d.ID, d.Name, d.Remark, d.SN, PdepName = d.P_Department2.Name }, true).ToList();

            //2.返回数据
            return Json(new Model.EasyUIModel.DataGridModel() { rows = listData, total = rowCount });
        }
        #endregion

        #region 获取所有部门
        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllDepart()
        {
            var list = new P_DepartmentBLL().GetListBy(d => d.IsActive == true).ToList();
            List<Model.EasyUIModel.TreeNode> listTree = Model.P_Department.ToTreeNodes(list);
            return Json(listTree, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设置用户角色
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetUserRole()
        {
            try
            {
                //获取要修改部门的用户id
                int usrId = int.Parse(Request.Form["uId"]);
                //获取部门id
                int depId = int.Parse(Request.Form["depId"]);
                //获取角色ids
                string[] strRoleIds = Request.Form["rIds"].Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);//1,2,3,4,
                //先跟新用户的部门id
                new P_UserBLL().Modify(new Model.P_User() { ID = usrId, DepID = depId }, "DepID");


                //查当前用户的 角色id
                var listOldRole = new P_UserRoleBLL().GetListBy(r => r.UserID == usrId).Select(r => r.RoleID).ToList();
                //查新的 角色id
                var listNewRole = strRoleIds.ToList();
                //两个集合比较，去掉重复元素
                for (int i = listOldRole.Count() - 1; i >= 0; i--)
                {
                    int oldRoleId = listOldRole[i];
                    if (listNewRole.Contains(oldRoleId.ToString()))
                    {
                        listOldRole.Remove(oldRoleId);
                        listNewRole.Remove(oldRoleId.ToString());
                    }
                }
                //新增新角色
                listNewRole.ForEach(newR =>
                {
                    new P_UserRoleBLL().Add(new Model.P_UserRole() { UserID = usrId, RoleID = int.Parse(newR), IsActive = true });
                });

                //删除旧角色
                listOldRole.ForEach(oldR =>
                {
                    new P_UserRoleBLL().DelBy(ur => ur.UserID == usrId && ur.RoleID == oldR);
                });

            }
            catch (Exception ex)
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "设置权限失败!" + ex.Message, null, null);
            }
            return this.JsonResult(Utility.E_JsonResult.OK, "设置权限成功!", null, null);
        }
        #endregion



       //*************新增修改视图***************************

        #region  获取部门页面

        /// <summary>
        /// 显示 新增权限 表单代码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDepView()
        {
            int id = int.Parse(Request.Form["id"]);

            var viewModel = new Model.ViewModel.Department();
            if (id == -1)
            {
                viewModel.IsActive = true;
                viewModel.SN = 255;
                return PartialView("DepartmentForm", viewModel);
            }
            else
            {
                var tempModel = new P_DepartmentBLL().GetListBy(p => p.ID == id).FirstOrDefault();

                if (tempModel != null)
                {
                    viewModel.ID = tempModel.ID;
                    viewModel.IsActive = tempModel.IsActive;
                    viewModel.Name = tempModel.Name;
                    viewModel.ParentID = tempModel.ParentID;
                    viewModel.Remark = tempModel.Remark;
                    viewModel.SN = tempModel.SN;
                    viewModel.DispatchSationID = tempModel.DispatchSationID;
                    viewModel.DispatchSubCenterID = tempModel.DispatchSubCenterID;
                }
                return PartialView("DepartmentForm", viewModel);
            }
        }

        #endregion

       //*************CURD方法*******************************

        #region 保存部门

        /// <summary>
        ///保存部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDepartment(Model.ViewModel.Department model)
        {
            //先查部门是否存在  如果没有新增 反正修改
            if (new P_DepartmentBLL().GetModelWithOutTrace(d => d.ID == model.ID) != null)
            {
                int res = new P_DepartmentBLL().Modify(model.ToPOCO(), "Name", "ParentID", "IsActive", "Remark", "SN", "DispatchSationID", "DispatchSubCenterID");
                if (res > 0)
                    return this.JsonResult(Utility.E_JsonResult.OK, "修改成功!", null, null);
                else
                    return this.JsonResult(Utility.E_JsonResult.Error, "修改失败！", null, null);
            }
            else
            {
                int res = new P_DepartmentBLL().Add(model.ToPOCO());
                if (res > 0)
                    return this.JsonResult(Utility.E_JsonResult.OK, "新增成功!", null, null);
                else
                    return this.JsonResult(Utility.E_JsonResult.Error, "新增失败！", null, null);
            }
        }
        #endregion

        #region 删除部门
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelDepartment()
        {
            try
            {
                int id = int.Parse(Request.Form["id"]);
                if (new P_DepartmentBLL().GetListBy(p => p.ParentID == id).ToList().Count > 0)
                {
                    return this.JsonResult(Utility.E_JsonResult.Error, "您现在删除的部门正在被使用，请先取消此部门关联!", null, null);
                }
                else
                {
                    int res = new P_DepartmentBLL().DelBy(p => p.ID == id);
                    if (res > 0)
                        return this.JsonResult(Utility.E_JsonResult.OK, "删除成功!", null, null);
                    else
                        return this.JsonResult(Utility.E_JsonResult.OK, "删除失败!", null, null);
                }
            }
            catch (Exception ex)
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "链接数据库失败!" + ex.Message, null, "");
            }
        } 
        #endregion

    }
}
