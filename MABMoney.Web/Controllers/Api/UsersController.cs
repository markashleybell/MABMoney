using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MABMoney.Services;
using MABMoney.Domain;

namespace MABMoney.Web.Controllers.Api
{
    public class UsersController : ApiController
    {
        private IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // GET api/people
        public IEnumerable<User> Get()
        {
            return _userServices.GetAllUsers();
        }

        // GET api/people/5
        public User Get(int id)
        {
            return _userServices.GetUserByID(id);
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
