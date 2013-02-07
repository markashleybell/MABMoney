using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
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

            // Ignore this if we're working locally
            if (request.IsLocal)
                return;

            // Reject all non https requests
            base.HandleNonHttpsRequest(filterContext);

            // TODO: The code below doesn't work on AppHarbor - it redirects to an odd port, something to do with the load balancers?
            /*
            // Recreate the url with a https protocol prefix
            UriBuilder uri = new UriBuilder(request.Url);
            uri.Scheme = Uri.UriSchemeHttps;

            // If it's a GET or HEAD request, just redirect to the https equivalent
            if (request.HttpMethod == "GET" || request.HttpMethod == "HEAD")
                filterContext.Result = new RedirectResult(uri.ToString());
            else // Don't allow POST, PUT, PATCH or DELETE requests to non-https URLs
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotFound);
            */
        }
    }
}