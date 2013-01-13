using MABMoney.Services;
using MABMoney.Data;
using MABMoney.Domain;
using MABMoney.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace MABMoney.Web.Infrastructure
{
    public class AuthenticateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;

            if (context.Request.Cookies["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("/Users/Login");
            }
            else
            {
                var userId = Convert.ToInt32(EncryptionHelpers.DecryptStringAES(context.Request.Cookies["UserId"].Value, ConfigurationManager.AppSettings["SharedSecret"]));

                var unitOfWork = new UnitOfWork(new DataStoreFactory());
                var userServices = new UserServices(new Repository<User, int>(unitOfWork), unitOfWork);

                var user = userServices.Get(userId);

                if(user == null)
                    filterContext.Result = new RedirectResult("/Users/Login");

                context.Items.Add("UserID", user.UserID);
                base.OnActionExecuting(filterContext);
            }
        }
    }
}