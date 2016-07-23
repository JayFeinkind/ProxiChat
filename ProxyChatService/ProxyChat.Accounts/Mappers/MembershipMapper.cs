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
    public class MembershipMapper : IResourceMapper<Membership, MembershipDto>
    {
        public IList<Membership> MapDtosToEntities(IEnumerable<MembershipDto> dtos)
        {
            return dtos.Select(dto => MapDtoToEntity(dto)).ToList();
        }

        public Membership MapDtoToEntity(MembershipDto dto)
        {
            Membership entity = new Membership();
            entity.CreatedUTC = dto.CreatedUTC;
            entity.Id = dto.Id;
            entity.Password = dto.HashedPassword;
            entity.Salt = dto.Salt;

            return entity;
        }

        public IList<MembershipDto> MapEntitiesToDtos(IEnumerable<Membership> entities)
        {
            return entities.Select(e => MapEntityToDto(e)).ToList();
        }

        public MembershipDto MapEntityToDto(Membership entity)
        {
            MembershipDto dto = new MembershipDto();

            dto.CreatedUTC = entity.CreatedUTC;
            dto.Id = entity.Id;
            dto.HashedPassword = entity.Password;
            dto.Salt = entity.Salt;

            return dto;
        }
    }
}
