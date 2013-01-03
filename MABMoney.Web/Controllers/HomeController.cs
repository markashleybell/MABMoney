using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Domain;
using MABMoney.Services;

namespace MABMoney.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserServices _personServices;

        public HomeController(IUserServices personServices)
        {
            _personServices = personServices;
        }

        public ActionResult Index()
        {
            var people = _personServices.GetAllUsers();

            return View();
        }
    }
}
