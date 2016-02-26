using mab.lib.SimpleMapper;
using MABMoney.Caching;
using MABMoney.Services;
using MABMoney.Services.DTO;
using MABMoney.Web.Helpers;
using MABMoney.Web.Infrastructure;
using MABMoney.Web.Models;
using MABMoney.Web.Models.Transactions;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MABMoney.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        public TransactionsController(IUserServices userServices,
                                      IAccountServices accountServices,
                                      ICategoryServices categoryServices,
                                      ITransactionServices transactionServices,
                                      IBudgetServices budgetServices,
                                      IHttpContextProvider context,
                                      ISiteConfiguration config,
                                      IUrlHelper urlHelper,
                                      IModelCache cache,
                                      ICachingHelpers cachingHelpers) : base(userServices,
                                                                             accountServices,
                                                                             categoryServices,
                                                                             transactionServices, 
                                                                             budgetServices,
                                                                             context,
                                                                             config,
                                                                             urlHelper,
                                                                             cache,
                                                                             cachingHelpers) { }

        private IndexViewModel GetTransactionIndexViewModel(int userId, int? accountId, int? categoryId, DateTime? from, DateTime? to)
        {
            var accountList = _accountServices.All();

            var accounts = accountList.Select(x => new SelectListItem {
                Value = x.AccountID.ToString(),
                Text = x.Name
            });

            var account = (accountId.HasValue) ? accountList.FirstOrDefault(x => x.AccountID == accountId.Value) : accountList.FirstOrDefault(x => x.Default);

            if (account == null)
                account = accountList.First();

            var allCategories = _categoryServices.All();

            var categoryList = (account != null) ? allCategories.Where(c => c.Account_AccountID == account.AccountID) : allCategories.Where(c => c.Account_AccountID == accountList.First().AccountID);

            var categories = categoryList.Select(x => new SelectListItem {
                Value = x.CategoryID.ToString(),
                Text = x.Name
            });

            // Default to last 30 days
            var now = DateTime.Now;
            var defaultTo = new DateTime(now.Year, now.Month, now.Day);
            var defaultFrom = defaultTo.AddDays(-30);

            var model = new IndexViewModel {
                Accounts = accounts,
                AccountID = account.AccountID,
                Categories = categories,
                CategoryID = categoryId,
                From = (from.HasValue) ? from.Value : defaultFrom,
                To = (to.HasValue) ? to.Value : defaultTo
            };

            if (accountId.HasValue)
            {
                if (categoryId.HasValue)
                {
                    model.Transactions = _transactionServices.GetForAccount(model.AccountID.Value, model.CategoryID.Value, model.From, model.To)
                                                                 .OrderByDescending(x => x.Date)
                                                                 .ToList();
                }
                else
                {
                    model.Transactions = _transactionServices.GetForAccount(model.AccountID.Value, model.From, model.To)
                                                                 .OrderByDescending(x => x.Date)
                                                                 .ToList();
                }
            }

            return model;
        }

        //
        // GET: /Transaction/
        [Authenticate]
        public ActionResult Index(ProfileViewModel profile, int? id)
        {
            return View(GetTransactionIndexViewModel(profile.UserID, id, null, null, null));
        }

        [Authenticate]
        [HttpPost]
        public ActionResult Index(ProfileViewModel profile, IndexViewModel model)
        {
            return View(GetTransactionIndexViewModel(profile.UserID, model.AccountID, model.CategoryID, model.From, model.To));
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
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                Date = DateTime.Now,
                RedirectAfterSubmitUrl = _url.Action("Index")
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
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // GET: /Transaction/Edit/5
        [Authenticate]
        public ActionResult Edit(ProfileViewModel profile, int id)
        {
            var model = _transactionServices.Get(id).MapTo<EditViewModel>();
            model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            model.RedirectAfterSubmitUrl = _url.Action("Index");
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
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            return Redirect(model.RedirectAfterSubmitUrl);
        }

        //
        // POST: /Transaction/Delete/5
        [Authenticate]
        [HttpPost]
        public ActionResult Delete(ProfileViewModel profile, int id, string redirectAfterSubmitUrl)
        {
            _transactionServices.Delete(id);

            // Clear the cache of anything that depends upon transactions
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.Transaction));
            // Clear the user because current balance comes from User.Accounts property
            _cache.InvalidateAllWithDependency(_cachingHelpers.GetDependencyKey(CachingDependency.User));

            return Redirect(redirectAfterSubmitUrl);
        }

        [Authenticate]
        public ActionResult Import(ProfileViewModel profile)
        {
            return View(new ImportViewModel { 
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices),
                RedirectAfterSubmitUrl = _url.Action("Index")
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
                        var exception = ex;
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
                _transactionServices.Save(transaction);

            return View(new ImportViewModel {
                RecordsImported = transactions.Count,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices)
            });
        }
    }
}
