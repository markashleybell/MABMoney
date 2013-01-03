using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MABMoney.Services;
using MABMoney.Domain;

namespace MABMoney.Web.Controllers
{
    public class PeopleController : ApiController
    {
        private IUserServices _personServices;

        public PeopleController(IUserServices personServices)
        {
            _personServices = personServices;
        }

        // GET api/people
        public IEnumerable<User> Get()
        {
            return _personServices.GetAllUsers();
        }

        // GET api/people/5
        public User Get(int id)
        {
            return _personServices.GetUserByID(id);
        }

        // POST api/people
        public void Post(User person)
        {
            
        }

        // PUT api/people/5
        public void Put(int id, User person)
        {
            
        }

        // DELETE api/people/5
        public void Delete(int id)
        {
            
        }
    }
}
