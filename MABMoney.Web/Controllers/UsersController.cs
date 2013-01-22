using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Users;
using mab.lib.SimpleMapper;
using MABMoney.Services.DTO;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Helpers;
using System.Configuration;

namespace MABMoney.Web.Controllers
{
    public class UsersController : BaseController
    {
        private ICryptoProvider _crypto;

        public UsersController(IUserServices userServices,
                               IAccountServices accountServices,
                               ICategoryServices categoryServices,
                               ITransactionServices transactionServices,
                               IBudgetServices budgetServices,
                               HttpContextBase context,
                               ISiteConfiguration config,
                               IDateTimeProvider dateProvider,
                               ICryptoProvider crypto) : base(userServices,
                                                              accountServices,
                                                              categoryServices,
                                                              transactionServices, 
                                                              budgetServices,
                                                              context,
                                                              config,
                                                              dateProvider) 
        {
            _crypto = crypto;
        }

        //
        // GET: /Users/
        [Authenticate]
        public ActionResult Index()
        {
            return View(new IndexViewModel { 
                Users = _userServices.All().ToList()
            });
        }

        //
        // GET: /Users/Create
        [Authenticate]
        public ActionResult Create()
        {
            return View(new CreateViewModel());
        }

        //
        // POST: /Users/Create
        [Authenticate]
        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<UserDTO>();
            _userServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Users/Edit/5
        [Authenticate]
        public ActionResult Edit(int id)
        {
            var model = _userServices.Get(id).MapTo<EditViewModel>();
            return View(model);
        }

        //
        // POST: /Users/Edit/5
        [Authenticate]
        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<UserDTO>();
            _userServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Users/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            _userServices.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            // Fetch the user
            var user = _userServices.GetByEmailAddress(model.Email);

            if(user == null || !_crypto.VerifyHashedPassword(user.Password, model.Password))
                return View(model);

            // Encrypt the ID before storing it in a cookie
            var encryptedUserId = EncryptionHelpers.EncryptStringAES(user.UserID.ToString(), _config.SharedSecret);
            _context.Response.Cookies.Add(new HttpCookie("UserID", encryptedUserId));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var cookie = new HttpCookie("UserID", "");
            cookie.Expires = _dateProvider.Date.AddDays(-1);
            _context.Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }
    }
}
