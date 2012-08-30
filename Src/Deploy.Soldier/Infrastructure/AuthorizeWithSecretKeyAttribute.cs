using System.Web.Mvc;
using Deploy.Utilities;

namespace Deploy.Soldier.Infrastructure
{
    public class AuthorizeWithSecretKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var key = filterContext.HttpContext.Request.Headers["pawnKey"];
            if (key != AppSettings.GetString("AuthenticationSecretKey"))
            {
                var res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.End();
            }
        }
    }
}