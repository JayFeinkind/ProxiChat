using ProxyChat.Devices.Dtos;
using ProxyChat.Devices.Repositories;
using ProxyChat.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProxyChatService.Controllers
{
    public class DeviceController : ProxyChatApiController
    {
        DeviceTokenRepository _deviceTokenRepository = new DeviceTokenRepository();

        //[HttpPost]
        //public HttpResponseMessage CreateDevice(DeviceDto device)
        //{

        //}

        [HttpPost]
        public async Task<HttpResponseMessage> CreateDeviceToken(DeviceTokenDto deviceToken)
        {
            try
            {
                var createResult = await _deviceTokenRepository.Create(deviceToken);

                return Request.CreateResponse(createResult.ResultCode.ToHttpStatusCode(), createResult.ResultData, "application/json");
            }
            catch (Exception e)
            {
                throw CreateResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}