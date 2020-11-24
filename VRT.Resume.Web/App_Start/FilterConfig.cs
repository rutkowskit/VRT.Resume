
using System.Web.Mvc;
namespace VRT.Resume.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {            
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomAuthorizeAttribute());
            filters.Add(new AllowPartialRenderingAttribute());            
        }
    }
}
