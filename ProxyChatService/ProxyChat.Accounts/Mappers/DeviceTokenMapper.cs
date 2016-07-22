using ProxyChat.Accounts.Dtos;
using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Mappers
{
    public class DeviceTokenMapper : IResourceMapper<DeviceToken, DeviceTokenDto>
    {
        public IList<DeviceToken> MapDtosToEntities(IEnumerable<DeviceTokenDto> dtos)
        {
            return dtos.Select(dto => MapDtoToEntity(dto)).ToList();
        }

        public DeviceToken MapDtoToEntity(DeviceTokenDto dto)
        {
            DeviceToken entity = new DeviceToken();

            entity.CreatedUTC = dto.CreatedUTC;
            entity.DeviceId = dto.DeviceId;
            entity.Id = dto.Id;
            entity.ModifiedUTC = dto.ModifiedUTC;
            entity.Token = dto.Token;
            entity.UserId = dto.UserId;

            return entity;
        }

        public IList<DeviceTokenDto> MapEntitiesToDtos(IEnumerable<DeviceToken> entities)
        {
            return entities.Select(e => MapEntityToDto(e)).ToList();
        }

        public DeviceTokenDto MapEntityToDto(DeviceToken entity)
        {
            DeviceTokenDto dto = new DeviceTokenDto();

            dto.CreatedUTC = entity.CreatedUTC;
            dto.Device = entity.Device;
            dto.DeviceId = entity.DeviceId;
            dto.Id = entity.Id;
            dto.ModifiedUTC = entity.ModifiedUTC;
            dto.Token = entity.Token;
            dto.User = entity.User;
            dto.UserId = entity.UserId;
            dto.VersionNumber = entity.VersionNumber;

            return dto;
        }
    }
}
