using System.Web.Mvc;
using Deploy.Pawn.Api;
using Deploy.Utilities;

namespace Deploy.Pawn.Host.Infrastructure
{
    public class AuthorizeWithSecretKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var key = filterContext.HttpContext.Request.Headers[PawnClient.AuthenticationKeyHeaderName];
            if (key != AppSettings.GetString("AuthenticationSecretKey"))
            {
                var res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.End();
            }
        }
    }
}