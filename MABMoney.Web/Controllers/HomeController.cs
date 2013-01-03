using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Domain;
using MABMoney.Services;
using MABMoney.Web.Models;

namespace MABMoney.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserServices _userServices;

        public HomeController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public ActionResult Index()
        {
            var users = _userServices.GetAllUsers().Select(x => new { 
                Forename = x.Forename,
                Surname = x.Surname,
                Email = x.Email
            });

            // return Json(users, JsonRequestBehavior.AllowGet);

            return View(new IndexViewModel());
        }
    }
}
