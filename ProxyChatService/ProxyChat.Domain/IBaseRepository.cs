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
        Task<IRepositoryResult<TDto>> Create(TDto dto);

        Task<IRepositoryResult<TDto>> Read(Expression<Func<TEntity, bool>> whereClause);
        Task<IRepositoryResult<TDto>> Read(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities);

        Task<IRepositoryResult<IList<TDto>>> ReadAll();
        Task<IRepositoryResult<IList<TDto>>> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities);
        Task<IRepositoryResult<IList<TDto>>> ReadAll(Func<TEntity, bool> whereClause);
        Task<IRepositoryResult<IList<TDto>>> ReadAll(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities);
    }
}
