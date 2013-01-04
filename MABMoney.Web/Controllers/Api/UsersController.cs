using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MABMoney.Services;
using MABMoney.Services.DTO;

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
        public IEnumerable<UserDTO> Get()
        {
            return _userServices.All();
        }

        // GET api/people/5
        public UserDTO Get(int id)
        {
            return _userServices.Get(id);
        }

        // POST api/people
        public void Post(UserDTO person)
        {
            
        }

        // PUT api/people/5
        public void Put(int id, UserDTO person)
        {
            
        }

        // DELETE api/people/5
        public void Delete(int id)
        {
            
        }
    }
}
