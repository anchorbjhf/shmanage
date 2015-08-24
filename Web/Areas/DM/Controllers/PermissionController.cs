using Anke.SHManage.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class PermissionController : Controller
    {
        #region 请求页面视图

        /// <summary>
        /// 父权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

   


        [HttpGet]
        /// <summary>
        /// 子权限列表 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PermissionSon()
        {
            //获取父权限id
            //int parentMenuId = int.Parse(Request.QueryString["pid"]);
            return View();
        }
        #endregion

        #region 请求列表数据

        #region 获取父权限列表数据
        [HttpPost]
        /// <summary>
        /// 权限列表 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPermData()
        {
            //获取页容量   
            int pageSize = int.Parse(Request.Form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(Request.Form["page"]);

            int rowCount = 0;

            P_PermissionBLL bll = new P_PermissionBLL();

            // 查询分页数据
            var list = bll.GetPagedList(pageIndex, pageSize, ref rowCount, p => p.ParentID == 0 && p.IsActive == true, p => p.SN).Select(p => p.ToPOCO());
            // 生成规定格式的 json字符串发回 给异步对象
            Model.EasyUIModel.DataGridModel dgModel = new Model.EasyUIModel.DataGridModel()
            {
                total = rowCount,
                rows = list,
                footer = null
            };
            return Json(dgModel);
        }
        #endregion

        #region 获取子权限列表数据

        [HttpPost]
        /// <summary>
        /// 查询子权限列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult PermissionSon(FormCollection form)
        {
            //获取页容量
            int pageSize = int.Parse(form["rows"]);
            //获取请求的页码
            int pageIndex = int.Parse(form["page"]);
            //获取父权限id
            int pid = int.Parse(Request.QueryString["pid"]);
            int rowCount = 0;
            // 查询分页数据
            var list = new P_PermissionBLL().GetPagedList(pageIndex, pageSize, ref rowCount, p => p.ParentID == pid && p.IsActive == true, p => p.SN).Select(p => p.ToPOCO());
            //2 生成规定格式的 json字符串发回 给 异步对象
            Model.EasyUIModel.DataGridModel dgModel = new Model.EasyUIModel.DataGridModel()
            {
                total = rowCount,
                rows = list,
                footer = null
            };
            return Json(dgModel);
        }

        #endregion

        #region 生成下拉框两种写法
        /// <summary>
        /// 生成 父菜单 下拉框 html字符串
        /// </summary>
        /// <returns></returns>
        string GetParentMenuDrowDownListHtml(int id)
        {
            ////查询所有的父菜单
            //var list = new P_PermissionBLL().GetListBy(p => p.ID == id).Select(p => p.ToViewModel()).ToList();

            //StringBuilder sb = new StringBuilder("<select name='ParentID'><option value=0>父菜单</option>");
            //list.ForEach(p =>
            //{
            //    if (id == p.ID)
            //        sb.AppendLine("<option selected value='" + p.ID + "'>" + p.Name + "</option>");
            //    else
            //        sb.AppendLine("<option value='" + p.ID + "'>" + p.Name + "</option>");
            //});
            //sb.AppendLine("</select>");

            //return sb.ToString();
            return "";
        }

        /// <summary>
        /// 设置新增和修改通用的下拉框数据
        /// </summary>
        void SetDropDonwList()
        {
            //准备请求方式下拉框数据
            ViewBag.httpMethodList = new List<SelectListItem>() { 
             new SelectListItem(){ Text="Get",Value="1"},
             new SelectListItem(){ Text="Post",Value="2"},
             new SelectListItem(){ Text="Both",Value="3"}
            };
        }

        #endregion 

        #endregion

        //公用方法

        #region  获取修改页面

        /// <summary>
        /// 获取权限页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEditPerView()
        {

            //根据id查询要修改的权限
            int id = int.Parse(Request.Form["id"]);

            var viewModel = new P_PermissionBLL().GetListBy(p => p.ID == id).FirstOrDefault().ToViewModel();
            if (viewModel != null)
            {
                ViewBag.ID = viewModel.ID;

                var parent = new P_PermissionBLL().GetListBy(p => p.ID == viewModel.ParentID).FirstOrDefault();
                if (parent != null)
                {
                    ViewBag.ParentID = parent.ID;

                    ViewBag.ParentName = parent.Name;
                    viewModel.ParentName = parent.Name;
                }
                else
                {
                    ViewBag.ParentID = 0;
                    ViewBag.ParentName = "无";
                    viewModel.ParentName = "无";
                }

                SetDropDonwList(); //设置下拉菜单

                //将 权限对象 传给 视图 的 Model属性
                return View("EditPermission", viewModel);
            }
            else
            {
                return this.JsonResult(Utility.E_JsonResult.Error, "没有查到此权限信息!", null, null);
            }

        }
        #endregion

        #region  获取新增页面

        /// <summary>
        /// 显示 新增权限 表单代码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAddPerView(int id)
        {
            var viewModel = new Model.ViewModel.Permission();
            SetDropDonwList();

            ViewBag.ID = viewModel.ID;
            if (id > 0)
            {
                //查一下父ID (传过来是父权限的ID)
                var model = new P_PermissionBLL().GetListBy(p => p.ID == id).FirstOrDefault();
                if (model != null)
                {
                    ViewBag.ParentID = model.ID; //父节点的ID
                    ViewBag.ParentName = model.Name;
                    viewModel.ParentName = model.Name;
                }
            }
            else
            {
                ViewBag.ParentID = 0;
                ViewBag.ParentName = "无";
                viewModel.ParentName = "无";
            }

            return PartialView("EditPermission", viewModel);
        }

        #endregion

        #region 功能操作  (保存,删除)
        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePermission(Model.ViewModel.Permission viewModel)
        {
            int res = -1;

            Model.P_Permission model = new Model.P_Permission();
            model.ID = viewModel.ID;
            model.ParentID = viewModel.ParentID;
            model.Name = viewModel.Name;
            model.AreaName = viewModel.AreaName;
            model.ControllerName = viewModel.ControllerName;
            model.ActionName = viewModel.ActionName;
            model.FormMethod = viewModel.FormMethod;
            model.IsShow = viewModel.IsShow;
            model.Remark = viewModel.Remark;
            model.SN = viewModel.SN;

            if (new P_PermissionBLL().GetModelWithOutTrace(p => p.ID == model.ID) != null)
            {
                //修改修改
                res = new P_PermissionBLL().Modify(model, "Name", "AreaName", "ControllerName", "ActionName", "FormMethod", "SN", "IsShow", "Remark");
            }
            else
            {
                //增加权限
                model.IsActive = true;
                res = new P_PermissionBLL().Add(model);
            }

            if (res > 0)
            {
                if (model.ParentID > 1)
                    return Redirect("/DM/Permission/PermissionSon?pid=" + model.ParentID);
                else//如果新增的是 父权限，则跳到父权限列表
                    return Redirect("/DM/Permission/Index");
            }
            else
                return Redirect("/DM/Permission/Permission");
        }



        /// <summary>
        /// 1.4 删除权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelPemission()
        {
            try
            {
                int id = int.Parse(Request.Form["id"]);
                if (new P_PermissionBLL().GetListBy(p => p.ParentID == id).ToList().Count > 0)
                {
                    return this.JsonResult(Utility.E_JsonResult.Error, "您现在删除的权限正在被角色使用，请先取消角色中的这个权限!!", null, "");
                }
                else
                {
                    new P_PermissionBLL().DelBy(p => p.ID == id);
                    return this.JsonResult(Utility.E_JsonResult.OK, "删除成功!", null, "");
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
