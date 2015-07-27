using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MABMoney.Web.Infrastructure
{
    public class UrlHelperAdapter : IUrlHelper
    {
        protected UrlHelper _helper;

        public UrlHelperAdapter(HttpContextBase context)
        {
            _helper = new UrlHelper(context.Request.RequestContext);
        }

        public string Action(string actionName)
        {
            return _helper.Action(actionName).TrimEnd('/');
        }

        public string Action(string actionName, object routeValues)
        {
            return _helper.Action(actionName, routeValues).TrimEnd('/');
        }

        public string Action(string actionName, RouteValueDictionary routeValues)
        {
            return _helper.Action(actionName, routeValues).TrimEnd('/');
        }

        public string Action(string actionName, string controllerName)
        {
            return _helper.Action(actionName, controllerName).TrimEnd('/');
        }

        public string Action(string actionName, string controllerName, object routeValues)
        {
            return _helper.Action(actionName, controllerName, routeValues).TrimEnd('/');
        }

        public string Action(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return _helper.Action(actionName, controllerName, routeValues).TrimEnd('/');
        }

        public string Action(string actionName, string controllerName, object routeValues, string protocol)
        {
            return _helper.Action(actionName, controllerName, routeValues, protocol).TrimEnd('/');
        }

        public string Action(string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return _helper.Action(actionName, controllerName, routeValues, protocol, hostName).TrimEnd('/');
        }
    }
}