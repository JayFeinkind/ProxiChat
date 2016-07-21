using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ProxyChat.Domain
{
    public abstract class BaseRepository<IDBContext, TEntity, TDto> : IBaseRepository<IDBContext, TEntity, TDto> where TEntity : class where IDBContext : DbContext, new()
    {

        public virtual IQueryable<TEntity> ReadAll()
        {
            using (var context = new IDBContext())
            {
                IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().AsQueryable();

                return entities;
            }
        }

        public virtual IQueryable<TEntity> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            using (var context = new IDBContext())
            {
                IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().AsQueryable();

                foreach (var relation in relatedEntities)
                {
                    entities = entities.Include(relation);
                }

                return entities;
            }
        }

        public virtual IQueryable<TEntity> ReadAll(Func<TEntity, bool> whereClause)
        {
            using (var context = new IDBContext())
            {
                IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().Where(whereClause).AsQueryable();

                return entities;
            }
        }

        public virtual IQueryable<TEntity> ReadAll(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            using (var context = new IDBContext())
            {
                IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().Where(whereClause).AsQueryable();

                foreach (var relation in relatedEntities)
                {
                    entities = entities.Include(relation);
                }

                return entities;
            }
        }
    }
}
