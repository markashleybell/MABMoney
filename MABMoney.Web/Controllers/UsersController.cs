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
using MABMoney.Caching;
using System.Text;

namespace MABMoney.Web.Controllers
{
    public class UsersController : BaseController
    {
        private ICryptoProvider _crypto;
        private ISessionServices _sessionServices;

        public UsersController(IUserServices userServices,
                               IAccountServices accountServices,
                               ICategoryServices categoryServices,
                               ITransactionServices transactionServices,
                               IBudgetServices budgetServices,
                               IHttpContextProvider context,
                               ISiteConfiguration config,
                               ICryptoProvider crypto,
                               IUrlHelper urlHelper,
                               IModelCache cache,
                               ICachingHelpers cachingHelpers,
                               ISessionServices sessionServices) : base(userServices,
                                                                        accountServices,
                                                                        categoryServices,
                                                                        transactionServices, 
                                                                        budgetServices,
                                                                        context,
                                                                        config,
                                                                        urlHelper,
                                                                        cache,
                                                                        cachingHelpers) 
        {
            _crypto = crypto;
            _sessionServices = sessionServices;
        }

        ////
        //// GET: /Users/
        //[Authenticate]
        //public ActionResult Index()
        //{
        //    return View(new IndexViewModel { 
        //        Users = _userServices.All().ToList()
        //    });
        //}

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
            var model = _userServices.Get().MapTo<EditViewModel>();
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
            _userServices.Delete();

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

            // Create a session record
            var uniqueKey = System.Web.Security.Membership.GeneratePassword(16, 8);
            var sessionKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.UserID + "-" + uniqueKey + "-" + _config.Get<string>("SharedSecret")));
            var sessionExpiry = DateTime.Now.AddDays(7);

            _sessionServices.Save(new SessionDTO {
                User_UserID = dto.UserID,
                Key = sessionKey,
                Expiry = sessionExpiry
            });

            // Set the auth cookie
            _context.SetCookie(_config.Get<string>("CookieKey"), sessionKey);

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

            // Create a session record
            var uniqueKey = System.Web.Security.Membership.GeneratePassword(16, 8);
            var sessionKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserID + "-" + uniqueKey + "-" + _config.Get<string>("SharedSecret")));
            var sessionExpiry = DateTime.Now.AddDays(7);

            // The record in the database expires in 7 days regardless
            _sessionServices.Save(new SessionDTO {
                User_UserID = user.UserID,
                Key = sessionKey,
                Expiry = sessionExpiry
            });

            // If remember me is checked
            if (model.RememberMe)
            {
                // The auth cookie expires in 7 days
                _context.SetCookie(_config.Get<string>("CookieKey"), sessionKey, sessionExpiry);
            }
            else
            {
                // The auth cookie will expire when the browser is closed
                // This does mean there is a valid session left in the database for up to 7 days, 
                // but an attacker would have to guess a 16-character random strong password in 
                // order to forge a cookie to retrieve it
                _context.SetCookie(_config.Get<string>("CookieKey"), sessionKey);
            }

            _sessionServices.DeleteExpired();

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var cookieKey = _config.Get<string>("CookieKey");
            // Get the session key value from the auth cookie
            var sessionKey = _context.GetCookieValue<string>(cookieKey);
            // Delete the session
            _sessionServices.DeleteByKey(sessionKey);
            // Remove the cookie
            _context.SetCookie(cookieKey, "", DateTime.Now.AddDays(-1));

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

                templateHtml = Regex.Replace(templateHtml, @"\[\{SITE_URL\}\]", _config.Get<string>("SiteUrl"), regexOptions);
                templateHtml = Regex.Replace(templateHtml, @"\[\{RESET_GUID\}\]", guid, regexOptions);

                var smtp = new SmtpClient();

                var msg = new MailMessage(new MailAddress(_config.Get<string>("NoReplyEmailAddress"), _config.Get<string>("NoReplyEmailDisplayName")), new MailAddress(user.Email));
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

            _userServices.ResetPassword(model.GUID, _crypto.HashPassword(model.Password));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        public ActionResult PasswordResetComplete()
        {
            return View();
        }
    }
}
