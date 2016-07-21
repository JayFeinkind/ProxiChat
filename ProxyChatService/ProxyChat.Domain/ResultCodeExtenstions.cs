using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ProxyChat.Domain
{
    public static class ResultCodeExtenstions
    {
      
            public static HttpStatusCode ToHttpStatusCode(this ResultCode code)
            {
                HttpStatusCode? status = null;

                switch (code)
                {
                    case ResultCode.Ok:
                        status = HttpStatusCode.OK;
                        break;
                    case ResultCode.NoContent:
                        status = HttpStatusCode.NoContent;
                        break;
                    case ResultCode.Created:
                        status = HttpStatusCode.Created;
                        break;
                    case ResultCode.NotModified:
                        status = HttpStatusCode.NotModified;
                        break;
                    case ResultCode.InvalidData:
                        status = HttpStatusCode.BadRequest;
                        break;
                    case ResultCode.Forbidden:
                        status = HttpStatusCode.Forbidden;
                        break;
                    case ResultCode.NotFound:
                        status = HttpStatusCode.NotFound;
                        break;
                    case ResultCode.DataConflict:
                        status = HttpStatusCode.Conflict;
                        break;
                    case ResultCode.Error:
                        status = HttpStatusCode.InternalServerError;
                        break;
                }

                return status.Value;
            }
        
    }
}
