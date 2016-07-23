using ProxyChat.Devices.Dtos;
using ProxyChat.Devices.Repositories;
using ProxyChat.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace ProxyChatService.Controllers
{
    public class DeviceController : ProxyChatApiController
    {
        DeviceTokenRepository _deviceTokenRepository = new DeviceTokenRepository();


        [HttpPost]
        public HttpResponseMessage UpdateDeviceToken(DeviceTokenDto deviceToken)
        {
            try
            {
                var createResult = _deviceTokenRepository.Create(deviceToken);

                return Request.CreateResponse(createResult.ResultCode.ToHttpStatusCode(), createResult.ResultData, "application/json");
            }
            catch (Exception e)
            {
                throw CreateResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}