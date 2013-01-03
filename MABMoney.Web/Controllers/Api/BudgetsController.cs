using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MABMoney.Web.Controllers.Api
{
    public class BudgetsController : ApiController
    {
        // GET api/budget
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/budget/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/budget
        public void Post([FromBody]string value)
        {
        }

        // PUT api/budget/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/budget/5
        public void Delete(int id)
        {
        }
    }
}
