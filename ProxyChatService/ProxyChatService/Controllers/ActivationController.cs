using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Models;
using ProxyChat.Accounts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ProxyChat.Domain;
using System.Web.Mvc;
using Newtonsoft.Json;

using System.Net;
using System.Text;

namespace ProxyChatService.Controllers
{
    public class ActivationController : ProxyChatApiController
    {
        public ActivationController()
        { }

        [HttpPost]
        public HttpResponseMessage Activation(UserDto User)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Created;

            IUserRepository<AccountsContext, User, UserDto> repository = new UserRepository();

            var createResult = repository.Create(User);
        

            if (createResult.ResultCode != ResultCode.Created)
            {
                return Request.CreateResponse(createResult.ResultCode.ToHttpStatusCode(), createResult.ResultDescription);
            }

            return Request.CreateResponse(HttpStatusCode.OK, createResult.ResultData, "application/json");
        }
    }
}