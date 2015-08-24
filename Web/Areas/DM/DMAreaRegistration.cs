using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.DM
{
    public class DMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DM_default",
                "DM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
