﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Controllers
{
    public class TransactionsController : Controller
    {
        //
        // GET: /Transactions/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Transactions/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Transactions/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Transactions/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Transactions/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Transactions/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Transactions/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Transactions/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
