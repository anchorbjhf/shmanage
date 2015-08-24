using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BM.Controllers
{
    [SkipLoginAttribute]
    public class LoginController : Controller
    {
        //
        // GET: /BM/Login/

        public ActionResult Index()
        {
            string name = Request.Form["username"].ToString();
            string pw = Request.Form["passWord"].ToString();

            //首先检查用户名密码
            LoginUserBLL bll = new LoginUserBLL();
            if (bll.Login(name, pw))
            {
                string username = UserOperateContext.Current.Session_UsrInfo.Name;
                string workCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
                //登录成功，跳转到主页面
                return Json(new { msg = "OK", UName = username, WorkCode = workCode }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
            else
            {
                //登录失败,发送错误错误给用户
                return Json(new { msg = "Error" }, "appliction/json", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult text()
        {
            return View();
        }

    }
}
