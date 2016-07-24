using ProxyChat.Devices.Dtos;
using ProxyChat.Devices.Mappers;
using ProxyChat.Devices.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Repositories
{
    public class DeviceRepository : BaseRepository<DevicesContext, Device, DeviceDto>, IDeviceRepository<DevicesContext, Device, DeviceDto>
    {
        DeviceMapper _mapper = new DeviceMapper();

        protected override IResourceMapper<Device, DeviceDto> Mapper
        {
            get
            {
                return _mapper;
            }
        }

        protected override void UpdateEntityProperties(Device from, Device to)
        {
            to.DeviceIdentifier = from.DeviceIdentifier;
            to.DeviceLocations = from.DeviceLocations;
            to.DeviceTokens = from.DeviceTokens;
            to.ModifiedUTC = DateTime.UtcNow;
        }

        protected override bool ValidateDto(DeviceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceIdentifier)) return false;

            return true;
        }
    }
}
