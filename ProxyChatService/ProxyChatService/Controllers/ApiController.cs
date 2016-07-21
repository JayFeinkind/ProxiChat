using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Text;

namespace ProxyChatService.Controllers
{
    public class ProxyChatApiController : ApiController
    {
        public string GetRequestBody(HttpRequestBase Request)
        {
            string documentContents;

            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

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