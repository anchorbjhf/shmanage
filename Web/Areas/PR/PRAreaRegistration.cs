using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.PR
{
    public class PRAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PR_default",
                "PR/{controller}/{action}/{id}",
                new {   id = UrlParameter.Optional }
            );
        }
    }
}
