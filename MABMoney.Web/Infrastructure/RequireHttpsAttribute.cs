using System;
using System.Configuration;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace MABMoney.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsAttribute : RequireHttpsAttributeBase
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            var request = filterContext.HttpContext.Request;

            // If the request was made over https
            if (request.IsSecureConnection)
                return;

            // Handle AppHarbor load balancer header
            if (string.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
                return;

            // Respect the override setting in Web.config
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["RequireHttps"]) == false)
                return;

            // Reject all non https requests
            HandleNonHttpsRequest(filterContext);
        }
    }
}