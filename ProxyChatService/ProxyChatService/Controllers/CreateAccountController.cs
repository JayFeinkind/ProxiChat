using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Repositories;
using ProxyChatService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProxyChatService.Controllers
{
    public class CreateAccountController : Controller
    {
        UserRepository _userRepository = new UserRepository();
        MembershipRepository _membershipRepository = new MembershipRepository();

        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccountCreated(UserDto user)
        {
            return PartialView("~/Views/CreateAccount/AccountCreatedPartial.cshtml", user);
        }

        private UserDto CreateUser(CreateAccountModel model)
        {
            UserDto user = new UserDto();
            user.CreatedUTC = DateTime.UtcNow;
            user.EmailAddress = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;

            var newUser = _userRepository.Create(user);

            MembershipDto membership = new MembershipDto();
            membership.CreatedUTC = DateTime.UtcNow;
            membership.TextPassword = model.Password;
            membership.Id = newUser.ResultData.Id;

            _membershipRepository.Create(membership);

            return newUser.ResultData;
        }
   
        public JsonResult CreateNewAccount(CreateAccountModel model)
        {
            bool success = true;
            string message = string.Empty;
            UserDto user = null;

            var existingUser = _userRepository.Read(u => u.UserName == model.UserName || u.EmailAddress == model.Email);

            if (existingUser.ResultCode == ProxyChat.Domain.ResultCode.Ok && existingUser.ResultData != null)
            {
                success = false;

                if (existingUser.ResultData.UserName == model.UserName)
                {
                    message = "User name already exists, try another";
                }
                if (existingUser.ResultData.EmailAddress == model.Email)
                {
                    message = "Email already in use, if you forgot your password click the link below";
                }
            }
            else
            {
                user = CreateUser(model);
            }

            return Json(new { Success = success , Message = message, Value = user});
        }

       
    }
}