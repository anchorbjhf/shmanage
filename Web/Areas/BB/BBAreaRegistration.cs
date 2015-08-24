using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BB
{
    public class BBAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BB";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BB_default",
                "BB/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
