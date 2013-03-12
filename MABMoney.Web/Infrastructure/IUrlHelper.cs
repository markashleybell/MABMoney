using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace MABMoney.Web.Infrastructure
{
    public interface IUrlHelper
    {
        string Action(string actionName);
        string Action(string actionName, object routeValues);
        string Action(string actionName, RouteValueDictionary routeValues);
        string Action(string actionName, string controllerName);
        string Action(string actionName, string controllerName, object routeValues);
        string Action(string actionName, string controllerName, RouteValueDictionary routeValues);
        string Action(string actionName, string controllerName, object routeValues, string protocol);
        string Action(string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName);
    }
}
