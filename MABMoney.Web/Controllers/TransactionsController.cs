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

namespace MABMoney.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        public TransactionsController(IUserServices userServices,
                                      IAccountServices accountServices,
                                      ICategoryServices categoryServices,
                                      ITransactionServices transactionServices,
                                      IBudgetServices budgetServices) : base(userServices,
                                                                             accountServices,
                                                                             categoryServices,
                                                                             transactionServices, 
                                                                             budgetServices) { }

        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View(new IndexViewModel {
                Transactions = _transactionServices.All()
                                                   .OrderByDescending(x => x.Date)
                                                   .ThenByDescending(x => x.TransactionID)
                                                   .ToList()
            });
        }

        //
        // GET: /Transaction/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Transaction/Create

        public ActionResult Create()
        {
            return View(new CreateViewModel {
                Categories = DataHelpers.GetCategorySelectOptions(_categoryServices),
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices)
            });
        }

        //
        // POST: /Transaction/Create

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // GET: /Transaction/Edit/5

        public ActionResult Edit(int id)
        {
            var model = _transactionServices.Get(id).MapTo<EditViewModel>();
            model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
            model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
            return View(model);
        }

        //
        // POST: /Transaction/Edit/5

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = DataHelpers.GetCategorySelectOptions(_categoryServices);
                model.Accounts = DataHelpers.GetAccountSelectOptions(_accountServices);
                return View(model);
            }

            var dto = model.MapTo<TransactionDTO>();
            _transactionServices.Save(dto);

            return RedirectToAction("Index");
        }

        //
        // POST: /Transaction/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _transactionServices.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Import()
        {
            return View(new ImportViewModel { 
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices)
            });
        }

        [HttpPost]
        public ActionResult Import(ImportViewModel model)
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
                _transactionServices.Save(transaction);

            return View(new ImportViewModel {
                RecordsImported = transactions.Count,
                Accounts = DataHelpers.GetAccountSelectOptions(_accountServices)
            });
        }
    }
}
