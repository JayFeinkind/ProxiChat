using ProxyChat.Accounts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ProxyChatService.Controllers
{
    public class ActivationController : ApiController
    {
        public ActivationController()
        { }


        [HttpPost]
        public HttpResponseMessage Activation(UserDto newUser)
        {
            var response = new HttpResponseMessage();


            return response;
        }
    }
}