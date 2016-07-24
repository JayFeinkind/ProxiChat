using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Models;
using ProxyChat.Accounts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Data.Entity;
using ProxyChat.Domain;
using System.Web.Mvc;
using Newtonsoft.Json;

using System.Net;
using System.Text;
using ProxyChatService.Models;
using System.Threading.Tasks;

namespace ProxyChatService.Controllers
{
    public class ActivationController : ProxyChatApiController
    {
        UserRepository _userRepository = new UserRepository();
        MembershipRepository _membershipRepository = new MembershipRepository();

        public ActivationController()
        {
        }

        [HttpPost]
        public  async Task<HttpResponseMessage> LogIn(LogInModel model)
        {
            try
            {
                var user = await _userRepository.Read(u => u.UserName == model.UserName, u => u.DeviceTokens);

                // TODO add password authenticate
                if (
                    user.ResultCode != ResultCode.Ok || 
                    user.ResultData == null ||
                    // leave this last so it only validates if necessary
                    (await _membershipRepository.ValidatePassword(user.ResultData.Id, model.Password)) == false) 
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                return Request.CreateResponse(user.ResultCode.ToHttpStatusCode(), user.ResultData, "application/json");
            }
            catch (Exception e)
            {
                throw CreateResponseException(HttpStatusCode.InternalServerError, "Error encountered: " + e.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Activation(UserDto User)
        {
            try
            {
                var createResult = await _userRepository.Create(User);

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