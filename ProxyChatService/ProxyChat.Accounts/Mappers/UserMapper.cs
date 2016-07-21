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
    public class UserMapper : IResourceMapper<User, UserDto>
    {
        public IList<User> MapDtosToEntities(IEnumerable<UserDto> dtos)
        {
            return dtos.Select(dto => MapDtoToEntity(dto)).ToList();
        }

        public User MapDtoToEntity(UserDto dto)
        {
            User entity = new User();

            entity.Id = dto.Id;
            entity.CreatedUTC = dto.CreatedUTC;
            entity.EmailAddress = dto.EmailAddress;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.ModifiedUTC = dto.ModifiedUTC;
            entity.UserName = dto.UserName;

            return entity;
        }

        public IList<UserDto> MapEntitiesToDtos(IEnumerable<User> entities)
        {
            return entities.Select(e => MapEntityToDto(e)).ToList();
        }

        public UserDto MapEntityToDto(User entity)
        {
            UserDto dto = new UserDto();
            dto.CreatedUTC = DateTime.SpecifyKind(entity.CreatedUTC, DateTimeKind.Utc);
            dto.DeviceTokens = entity.DeviceTokens;
            dto.Id = entity.Id;
            dto.EmailAddress = entity.EmailAddress;
            dto.FirstName = entity.FirstName;
            dto.LastName = entity.LastName;
            dto.ModifiedUTC = entity.ModifiedUTC;
            dto.UserName = entity.UserName;
            dto.VersionNumber = entity.VersionNumber;

            return dto;
        }
    }
}
