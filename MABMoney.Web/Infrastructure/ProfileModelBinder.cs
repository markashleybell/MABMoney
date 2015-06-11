using MABMoney.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Infrastructure
{
    public class ProfileModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext");

            if(controllerContext.HttpContext == null
            || controllerContext.HttpContext.Items == null
            || controllerContext.HttpContext.Items["UserID"] == null 
            || controllerContext.HttpContext.Items["Email"] == null)
            {
                return null;
            }

            return new ProfileViewModel {
                UserID = Convert.ToInt32(controllerContext.HttpContext.Items["UserID"]),
                Email = controllerContext.HttpContext.Items["Email"].ToString()
            };
        }
    }
}