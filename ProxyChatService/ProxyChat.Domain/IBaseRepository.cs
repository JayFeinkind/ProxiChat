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
        IQueryable<TEntity> ReadAll();
        IQueryable<TEntity> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities);
    }
}
