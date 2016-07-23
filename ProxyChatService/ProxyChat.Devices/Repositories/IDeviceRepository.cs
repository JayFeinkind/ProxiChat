using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Repositories
{
    interface IDeviceRepository<DeviceContext, Device, DeviceDto> : IBaseRepository<DeviceContext, Device, DeviceDto>
    {
    }
}
