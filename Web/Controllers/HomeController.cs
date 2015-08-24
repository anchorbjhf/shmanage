using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.BLL;
using Anke.SHManage.Model.EasyUIModel;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;

namespace Anke.SHManage.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 用户登录Get请求页面
        /// </summary>
        /// <returns></returns>
        [SkipLoginAttribute]
        public ActionResult UserLogin()
        {
            return View();
        }
        /// <summary>
        /// 用户登录首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Home()
        {
            ViewBag.lname = UserOperateContext.Current.Session_UsrInfo.Name;

            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfStationID = new PersonalStatisticsBLL().GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
              //new PersonalStatisticsBLL()
            if (selfStationID != null && selfStationID != "")
            {
                ViewBag.selfName = UserOperateContext.Current.Session_UsrInfo.Name +"月工作信息";
            }
            else if (selfCenterID != null && selfCenterID != "")
            {
                string selfCenterName = "";
                switch (selfCenterID)
                {
                    case "1": selfCenterName = "中心";
                        break;
                    case "2": selfCenterName = "中区";
                        break;
                    case "3": selfCenterName = "东区";
                        break;
                    case "4": selfCenterName = "南区";
                        break;
                    case "5": selfCenterName = "西区";
                        break;
                    case "6": selfCenterName = "北区";
                        break;
                    default: break;
                }
                ViewBag.selfName = selfCenterName+"月工作信息";
            }
            else {
                ViewBag.selfName = "全中心月工作信息";
            }

            ViewBag.ltime = DateTime.Now;
            return View();
        }
        /// <summary>
        /// 根据双杨要求workcode实际传为LoginName
        /// </summary>
        /// <param name="workcode"></param>
        /// <returns></returns>
        [HttpGet]
        [SkipLoginAttribute]
        public ActionResult UrlLogin(string loginname, string urllogin)
        {
            LoginUserBLL bll = new LoginUserBLL();
            if (bll.Login(loginname))
            {
                //登录成功，跳转到主页面
                return Redirect("~/Home/Index?urllogin=" + urllogin);
            }
            else
            {
                JavaScriptResult js = new JavaScriptResult();
                js.Script = "alert('业务系统未找到[" + loginname + "]登录人！')";
                return js;
            }
        }

        /// <summary>
        /// 提交登录请求
        /// </summary>
        /// <param name="info">登录实体</param>
        /// <returns></returns>
        [HttpPost]
        [SkipLoginAttribute]
        public ActionResult UserLoginByInfo(LoginInfo info)
        {
            //首先检查用户名密码
            LoginUserBLL bll = new LoginUserBLL();
            if (bll.Login(info.LoginName, info.PassWord))
            {
                //登录成功，跳转到主页面
                return this.JsonResult(E_JsonResult.OK, "", null, "~/Home/Index");
            }
            else
            {
                //登录失败,发送错误错误给用户
                return this.JsonResult(E_JsonResult.Error, "登录失败！用户名密码错误！", null, null);
            }
        }

        /// <summary>
        /// 加载主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string urllogin)
        {
            if (urllogin == null)
                ViewBag.urllogin = Url.Content("~/Home/RedirectToLogin");
            else
                ViewBag.urllogin = urllogin;
            ViewBag.LoginName = UserOperateContext.Current.Session_UsrInfo.Name;
            return View();
        }

        /// <summary>
        /// 获取权限菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult getMenuData()
        {
            /*
             *  ToDo：
                    1.把需要显示的权限菜单显示到UI左侧菜单树中   
                    2.把url 属性添加到attribute的 url 属性中 用 tabs 容器包裹 iframe 标签实现 多tab页面展现 
             */
            return Content(UserOperateContext.Current.Session_UserTreeJsonStr);  //把用户权限菜单
        }

        /// <summary>
        /// Filters跳转到登录页面
        /// </summary>
        /// <returns></returns>
        [SkipLoginAttribute]
        public ActionResult RedirectToLogin()
        {
            return View();
        }
    }
}
