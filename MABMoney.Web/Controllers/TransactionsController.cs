using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MABMoney.Services;
using MABMoney.Web.Models.Transactions;
using MABMoney.Services.DTO;
using mab.lib.SimpleMapper;
using MABMoney.Web.Helpers;
using Microsoft.VisualBasic.FileIO;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;

namespace MABMoney.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        public TransactionsController(IUserServices userServices,
                                      IAccountServices accountServices,
                                      ICategoryServices categoryServices,
                                      ITransactionServices transactionServices,
                                      IBudgetServices budgetServices,
                                      HttpContextBase context,
                                      ISiteConfiguration config) : base(userServices,
                                                                        accountServices,
                                                                        categoryServices,
                                                                        transactionServices, 
                                                                        budgetServices,
                                                                        context,
                                                                        config) { }

        //
        // GET: /Transaction/
        [Authenticate]
        public ActionResult Index(ProfileViewModel profile)
        {
            return View(new IndexViewModel {
                Transactions = _transactionServices.All(profile.UserID)
                                                   .OrderByDescending(x => x.Date)
                                                   .ThenByDescending(x => x.TransactionID)
                                                   .ToList()
            });
        }

        //
        // GET: /Transaction/Details/5
        [Authenticate]
        public ActionResult Details(ProfileViewModel profile, int id)
        {
            return View();
        }

        //
        // GET: /Transaction/Create
        [Authenticate]
        public ActionResult Create(ProfileViewModel profile)
        {
            return View(new CreateViewModel {
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID)
            });
        }

        //
        // POST: /Transaction/Create
        [Authenticate]
        [HttpPost]
        public ActionResult Create(ProfileViewModel profile, CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(profile.UserID, dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Transaction/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _transactionServices.Get(profile.UserID, id).MapTo<EditViewModel>();
            model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID);
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
            return View(model);
        }

        //
        // POST: /Transaction/Edit/5
        [Authenticate]
        [HttpPost]
        public ActionResult Edit(ProfileViewModel profile, EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices, profile.UserID);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(profile.UserID, dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Transaction/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id)
        {
            _transactionServices.Delete(profile.UserID, id);
            return RedirectToAction("Index");
        }

        [Authenticate]
        public ActionResult Import(ProfileViewModel profile)
        {
            return View(new ImportViewModel { 
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID)
            });
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Import(ProfileViewModel profile, ImportViewModel model)
        {
            var transactions = new List<TransactionDTO>();

            using (var myCsvFile = new TextFieldParser(model.File.InputStream))
            {
                myCsvFile.TextFieldType = FieldType.Delimited;
                myCsvFile.SetDelimiters(",");
                // myCsvFile.CommentTokens = new[] { "HEADER", "COMMENT: ", "TRAILER" };

                myCsvFile.ReadFields();

                while (!myCsvFile.EndOfData)
                {
                        
                    try
                    {
                        string[] fieldArray = myCsvFile.ReadFields();

                        transactions.Add(new TransactionDTO { 
                            Account_AccountID = model.Account_AccountID,
                            Date = DateTime.ParseExact(fieldArray[0], "dd/MM/yyyy", null),
                            Description = fieldArray[1].Trim(new char[] { '"' }),
                            Amount = Convert.ToDecimal(fieldArray[2])
                        });
                    }
                    catch (MalformedLineException ex)
                    {
                        // not a valid delimited line - log, terminate, or ignore
                        continue;
                    }
                    // process values in fieldArray
                }
            }

            // model.File.SaveAs(@"E:\Inetpub\myapps\MABMoney\MABMoney.Web\Content");

            // Reverse the list because transactions are in reverse chronological order in CSV
            transactions.Reverse();

            foreach (var transaction in transactions)
                _transactionServices.Save(profile.UserID, transaction);

            return View(new ImportViewModel {
                RecordsImported = transactions.Count,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices, profile.UserID)
            });
        }
    }
}
