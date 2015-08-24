using Anke.SHManage.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute()); 
            filters.Add(new LoginValidateAttribute()); //登录验证
        }
    }
}