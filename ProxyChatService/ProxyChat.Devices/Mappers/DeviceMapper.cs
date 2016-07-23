using ProxyChat.Devices.Dtos;
using ProxyChat.Devices.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Mappers
{
    public class DeviceMapper : IResourceMapper<Device, DeviceDto>
    {
        public IList<Device> MapDtosToEntities(IEnumerable<DeviceDto> dtos)
        {
            return dtos.Select(dto => MapDtoToEntity(dto)).ToList();
        }

        public Device MapDtoToEntity(DeviceDto dto)
        {
            Device entity = new Device();
            entity.CreatedUTC = dto.CreatedUTC;
            entity.DeviceIdentifier = dto.DeviceIdentifier;
            entity.Id = dto.Id;
            entity.ModifiedUTC = dto.ModifiedUTC;

            return entity;
        }

        public IList<DeviceDto> MapEntitiesToDtos(IEnumerable<Device> entities)
        {
            return entities.Select(e => MapEntityToDto(e)).ToList();
        }

        public DeviceDto MapEntityToDto(Device entity)
        {
            DeviceDto dto = new DeviceDto();

            dto.CreatedUTC = entity.CreatedUTC;
            dto.DeviceIdentifier = entity.DeviceIdentifier;
            dto.Id = entity.Id;
            dto.ModifiedUTC = entity.ModifiedUTC;
            dto.VersionNumber = entity.VersionNumber;

            return dto;
        }
    }
}
