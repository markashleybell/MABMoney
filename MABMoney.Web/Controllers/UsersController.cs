using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Users;
using mab.lib.SimpleMapper;
using MABMoney.Services.DTO;

namespace MABMoney.Web.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IUserServices userServices,
                               IAccountServices accountServices,
                               ICategoryServices categoryServices,
                               ITransactionServices transactionServices,
                               IBudgetServices budgetServices) : base(userServices,
                                                                      accountServices,
                                                                      categoryServices,
                                                                      transactionServices, 
                                                                      budgetServices) { }

        //
        // GET: /Users/

        public ActionResult Index()
        {
            return View(new IndexViewModel { 
                Users = _userServices.All().ToList()
            });
        }

        //
        // GET: /Users/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
            return View(new CreateViewModel());
        }

        //
        // POST: /Users/Create

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

        public ActionResult Edit(int id)
        {
            var model = _userServices.Get(id).MapTo<EditViewModel>();
            return View(model);
        }

        //
        // POST: /Users/Edit/5

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

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _userServices.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
