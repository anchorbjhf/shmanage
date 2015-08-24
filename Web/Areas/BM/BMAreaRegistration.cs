using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BM
{
    public class BMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BM_default",
                "BM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
