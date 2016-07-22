using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public interface IBaseRepository<IDBContext, TEntity, TDto>
    {
        IRepositoryResult<TDto> Create(TDto dto);

        IRepositoryResult<TDto> Read(Func<TEntity, bool> whereClause);
        IRepositoryResult<TDto> Read(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities);

        IRepositoryResult<IList<TDto>> ReadAll();
        IRepositoryResult<IList<TDto>> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities);
        IRepositoryResult<IList<TDto>> ReadAll(Func<TEntity, bool> whereClause);
        IRepositoryResult<IList<TDto>> ReadAll(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities);
    }
}
