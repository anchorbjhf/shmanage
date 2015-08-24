using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.IM
{
    public class IMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "IM_default",
                "IM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
