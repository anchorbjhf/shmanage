using Anke.SHManage.BLL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Filters
{
    /// <summary>
    /// 登录校验过滤器
    /// </summary>
    public class LoginValidateAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        /// <summary>
        ///  验证方法 - 在 ActionExcuting过滤器之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (UserOperateContext.Current.Session_UsrInfo == null)
            {
                if (DoesSkip<SkipLoginAttribute>(filterContext))
                {
                    return;
                }

                filterContext.Result = new RedirectResult("~/Home/RedirectToLogin");
            }
        }

        /// <summary>
        /// 是否跳过验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool DoesSkip<T>(System.Web.Mvc.AuthorizationContext filterContext) where T : Attribute
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(T), false) &&
                    !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(T), false))
            {
                return false;
            }
            return true;
        }

    }
}