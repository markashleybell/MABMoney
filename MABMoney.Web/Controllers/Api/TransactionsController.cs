﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MABMoney.Web.Controllers.Api
{
    public class TransactionsController : ApiController
    {
        // GET api/transaction
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/transaction/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/transaction
        public void Post([FromBody]string value)
        {
        }

        // PUT api/transaction/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/transaction/5
        public void Delete(int id)
        {
        }
    }
}
