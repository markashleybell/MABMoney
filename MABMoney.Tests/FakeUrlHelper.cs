using MABMoney.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Tests
{
    public class FakeUrlHelper : IUrlHelper
    {
        public string Action(string actionName)
        {
            return "http://localhost";
        }

        public string Action(string actionName, object routeValues)
        {
            return "http://localhost";
        }

        public string Action(string actionName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            return "http://localhost";
        }

        public string Action(string actionName, string controllerName)
        {
            return "http://localhost";
        }

        public string Action(string actionName, string controllerName, object routeValues)
        {
            return "http://localhost";
        }

        public string Action(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            return "http://localhost";
        }

        public string Action(string actionName, string controllerName, object routeValues, string protocol)
        {
            return "http://localhost";
        }

        public string Action(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues, string protocol, string hostName)
        {
            return "http://localhost";
        }
    }
}
