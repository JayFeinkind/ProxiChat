using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public interface IResourceMapper<TEntity, TDto>
    {
        IList<TEntity> MapDtosToEntities(IEnumerable<TDto> dtos);
        IList<TDto> MapEntitiesToDtos(IEnumerable<TEntity> entities);
        TEntity MapDtoToEntity(TDto dto);
        TDto MapEntityToDto(TEntity entity);
    }
}
