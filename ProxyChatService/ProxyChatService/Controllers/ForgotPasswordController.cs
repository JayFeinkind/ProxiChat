using ProxyChat.Accounts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProxyChatService.Controllers
{
    public class ForgotPasswordController : Controller
    {
        UserRepository _userRepository = new UserRepository();

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ResetPassword(string email, Guid token)
        {
            return View();
        }

        private void SendEmail(string email, Guid token)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("JayFeinkind@gmail.com", "nemisis1986"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;

            string url = Request.Url.AbsoluteUri.Replace("SendForgotPasswordEmail", "ResetPassword") + "&token=" + token.ToString();

            mail.Body = string.Format("<a href='{0}'>Reset Password</a>", url);

            //Setting From , To and CC
            mail.From = new MailAddress("JayFeinkind@gmail.com", "MyWeb Site");
            mail.To.Add(new MailAddress(email));

            client.Send(mail);
        }

        [HttpPost]
        public async Task<JsonResult> SendForgotPasswordEmail(string email)
        {
            object result = null;

            try
            {
                var user = await _userRepository.Read(u => u.EmailAddress == email);

                if (user.ResultCode != ProxyChat.Domain.ResultCode.Ok)
                {
                    result = new { Success = false, Message = "Email not on file" };
                }
                else
                {
                    var newToken = Guid.NewGuid();
                    user.ResultData.ResetPasswordToken = newToken;

                    var updatedUser = await _userRepository.Update(user.ResultData);

                    SendEmail(email, newToken);

                    result = new { Success = true, Message = string.Empty };
                }
            }
            catch (Exception e)
            {
                result = new { Success = false, Message = "An error occurred" };
            }

            return Json(result);
        }
    }
}