﻿using ProxyChat.Accounts.Dtos;
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
        UserRepository _userRepository = new UserRepository();

        public ActivationController()
        {
        }

        [HttpPost]
        public HttpResponseMessage LogIn(string UserName, string Password)
        {
            return null;
        }

        [HttpPost]
        public HttpResponseMessage Activation(UserDto User)
        {
            try
            {
                var createResult = _userRepository.Create(User);

                if (createResult.ResultCode != ResultCode.Created)
                {
                    return Request.CreateResponse(createResult.ResultCode.ToHttpStatusCode(), createResult.ResultDescription);
                }

                return Request.CreateResponse(HttpStatusCode.Created, createResult.ResultData, "application/json");
            }
            catch (Exception e)
            {
                throw CreateResponseException(HttpStatusCode.InternalServerError, "Error encountered: " + e.Message);
            }
        }
    }
}