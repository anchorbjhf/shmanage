using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DD
{
    public class DDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DD_default",
                "DD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
