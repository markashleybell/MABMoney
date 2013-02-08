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
using MABMoney.Data;
using System.Text.RegularExpressions;
using System.Net.Mail;

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
                               ICacheProvider cacheProvider,
                               ICryptoProvider crypto) : base(userServices,
                                                              accountServices,
                                                              categoryServices,
                                                              transactionServices, 
                                                              budgetServices,
                                                              context,
                                                              config,
                                                              dateProvider,
                                                              cacheProvider) 
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
            var cookie = new HttpCookie(_config.CookieKey, encryptedUserId);
            if (model.RememberMe)
                cookie.Expires = _dateProvider.Now.AddDays(7);
            _context.Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var cookie = new HttpCookie(_config.CookieKey, "");
            cookie.Expires = _dateProvider.Now.AddDays(-1);
            _context.Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }

        public ActionResult PasswordResetRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordResetRequest(string userEmail)
        {
            var user = _userServices.GetByEmailAddress(userEmail);

            if (user != null)
            {
                var guid = Guid.NewGuid().ToString();

                user.PasswordResetGUID = guid;
                user.PasswordResetExpiry = DateTime.Now.AddHours(1);

                _userServices.Save(user);

                var regexOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline;

                var templateHtml = "";

                using (var file = System.IO.File.OpenText(Server.MapPath("~/Content/EmailTemplates") + "\\password-reset.html"))
                {
                    templateHtml = file.ReadToEnd();
                }

                templateHtml = Regex.Replace(templateHtml, @"\[\{SITE_URL\}\]", _config.SiteUrl, regexOptions);
                templateHtml = Regex.Replace(templateHtml, @"\[\{RESET_GUID\}\]", guid, regexOptions);

                var smtp = new SmtpClient();

                var msg = new MailMessage(new MailAddress(_config.NoReplyEmailAddress, _config.NoReplyEmailDisplayName), new MailAddress(user.Email));
                msg.Subject = "Reset your Password";
                msg.IsBodyHtml = true;
                msg.Body = templateHtml;

                smtp.Send(msg);
            }

            return RedirectToAction("PasswordResetRequestSent");
        }

        public ActionResult PasswordResetRequestSent()
        {
            return View();
        }

        public ActionResult PasswordReset(string id)
        {
            var user = _userServices.GetByPasswordResetGUID(id);

            if (user == null)
                return View("PasswordResetRequestExpired");

            return View(new PasswordResetViewModel { 
                GUID = id
            });
        }

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userServices.GetByPasswordResetGUID(model.GUID);

            user.Password = _crypto.HashPassword(model.Password);
            user.PasswordResetExpiry = null;
            user.PasswordResetGUID = null;

            _userServices.Save(user);

            return RedirectToAction("Login");
        }
    }
}
