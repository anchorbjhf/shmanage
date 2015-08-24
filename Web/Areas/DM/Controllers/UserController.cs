using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /DM/User/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            //0.接收参数 page=1&rows=5
            int pageIndex = int.Parse(form["page"]);
            int pageSize = int.Parse(form["rows"]);

            string strWorkCode = form["WorkCode"];
            string strName = form["Name"];

            #region 拼接查询条件
            Expression<Func<P_User, bool>> where = p => p.IsActive == true;
            if (strWorkCode != null && strWorkCode.Length > 0)
            {
                where = where.And(r => r.WorkCode == strWorkCode);
            }
            if (strName != null && strName.Length > 0)
            {
                where = where.And(s => s.Name.Contains(strName));
            }
            #endregion

            //1.读取数据
            var rowCount = 0;
            var list = new P_UserBLL().GetPagedList(pageIndex, pageSize, ref rowCount, where, u => u.ID, true).Select(u => u.ToExtModle());
            //2.返回数据
            return Json(new Model.EasyUIModel.DataGridModel() { total = rowCount, rows = list });
        }

        [HttpPost]
        public ActionResult GetUserDepAndRole()
        {
            int userid = Convert.ToInt32(Request.Form["UserId"]);
            List<int> userRole = new P_UserRoleBLL().GetListBy(ur => ur.UserID == userid).Select(ur => ur.RoleID).ToList();
            P_User userInfo = new P_UserBLL().GetModelWithOutTrace(u => u.ID == userid);
            string Depid = "";
            if (userInfo != null)
            {
                Depid = userInfo.DepID.ToString();
            }
            string uRole = "";
            foreach (int item in userRole)
            {
                uRole = uRole + "," + item;
            }
            string reUR = "";
            if (uRole != "")
            {
                reUR = uRole.Substring(1);
            }
            return Json(new { UserRole =reUR , Dep = Depid }, "appliction/json", JsonRequestBehavior.AllowGet);
        }
    }
}
