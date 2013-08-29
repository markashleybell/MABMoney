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
                               IHttpContextProvider context,
                               ISiteConfiguration config,
                               IDateTimeProvider dateProvider,
                               ICacheProvider cacheProvider,
                               ICryptoProvider crypto,
                               IUrlHelper urlHelper) : base(userServices,
                                                            accountServices,
                                                            categoryServices,
                                                            transactionServices, 
                                                            budgetServices,
                                                            context,
                                                            config,
                                                            dateProvider,
                                                            cacheProvider,
                                                            urlHelper) 
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
            return View(new CreateViewModel {
                RedirectAfterSubmitUrl = _url.Action("Index")
            });
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

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // GET: /Users/Edit/5
        [Authenticate]
        public ActionResult Edit(int id)
        {
            var model = _userServices.Get(id).MapTo<EditViewModel>();
            model.RedirectAfterSubmitUrl = _url.Action("Index");
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

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Users/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(int id, string redirectAfterSubmitUrl)
        {
            _userServices.Delete(id);

            return Redirect(redirectAfterSubmitUrl);
        }

        public ActionResult Signup()
        {
            return View(new SignupViewModel { 
                RedirectAfterSubmitUrl = _url.Action("Index", "Home")
            });
        }

        [HttpPost]
        public ActionResult Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = model.MapTo<UserDTO>();
            _userServices.Save(dto);

            // Encrypt the ID before storing it in a cookie
            var encryptedUserId = EncryptionHelpers.EncryptStringAES(dto.UserID.ToString(), _config.SharedSecret);
            _context.SetCookie(_config.CookieKey, encryptedUserId);

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel { 
                RedirectAfterSubmitUrl = _url.Action("Index", "Home")
            });
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

            if (model.RememberMe)
                _context.SetCookie(_config.CookieKey, encryptedUserId, _dateProvider.Now.AddDays(7));
            else
                _context.SetCookie(_config.CookieKey, encryptedUserId);

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _context.SetCookie(_config.CookieKey, "", _dateProvider.Now.AddDays(-1));

            // TODO: Is there ever going to be a need to define this on the fly with RedirectAfterSubmitUrl?
            return RedirectToAction("Login");
        }

        public ActionResult PasswordResetRequest()
        {
            return View(new PasswordResetRequestViewModel { 
                RedirectAfterSubmitUrl = _url.Action("PasswordResetRequestSent")
            });
        }

        [HttpPost]
        public ActionResult PasswordResetRequest(PasswordResetRequestViewModel model)
        {
            var user = _userServices.GetByEmailAddress(model.Email);

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

            return Redirect(model.RedirectAfterSubmitUrl);
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
                GUID = id,
                RedirectAfterSubmitUrl = _url.Action("PasswordResetComplete")
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

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        public ActionResult PasswordResetComplete()
        {
            return View();
        }
    }
}
