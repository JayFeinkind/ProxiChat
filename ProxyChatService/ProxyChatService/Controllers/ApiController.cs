using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;

namespace ProxyChatService.Controllers
{
    public class ApiController : Controller
    {
        public HttpResponseException CreateResponseException(HttpStatusCode statusCode, string reason)
        {
            var response = new HttpResponseMessage(statusCode);

            try
            {
                if (!string.IsNullOrEmpty(reason))
                {
                    response.ReasonPhrase = reason;
                }

                return new HttpResponseException(response);
            }
            catch
            {
                if (response != null)
                {
                    response.Dispose();
                }

                throw;
            }
        }
    }
}