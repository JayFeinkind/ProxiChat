using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ProxyChat.Domain
{
    public abstract class BaseRepository<TDBContext, TEntity, TDto> : IBaseRepository<TDBContext, TEntity, TDto> 
        where TEntity : class, new()
        where TDBContext : DbContext, new()
    {
        protected abstract IResourceMapper<TEntity, TDto> Mapper { get; }

        #region Read

        public virtual IRepositoryResult<IList<TDto>> ReadAll()
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().AsQueryable();

                    result.ResultData = Mapper.MapEntitiesToDtos(entities);
                    result.ResultCode = ResultCode.Ok;
                }
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
                result.Exception = e;
            }

            return result;
        }

        public virtual IRepositoryResult<IList<TDto>> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().AsQueryable();

                    // include each related entity supplied in parameter list
                    foreach (var relation in relatedEntities)
                    {
                        entities = entities.Include(relation);
                    }

                    result.ResultData = Mapper.MapEntitiesToDtos(entities);
                    result.ResultCode = ResultCode.Ok;
                }
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
                result.Exception = e;
            }

            return result;
        }

        public virtual IRepositoryResult<IList<TDto>> ReadAll(Func<TEntity, bool> whereClause)
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().Where(whereClause).AsQueryable();

                    result.ResultData = Mapper.MapEntitiesToDtos(entities);
                    result.ResultCode = ResultCode.Ok;
                }
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
                result.Exception = e;
            }

            return result;
        }

        public virtual IRepositoryResult<IList<TDto>> ReadAll(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> entities = context.Set<TEntity>().AsNoTracking().Where(whereClause).AsQueryable();

                    // include each related entity supplied in parameter list
                    foreach (var relation in relatedEntities)
                    {
                        entities = entities.Include(relation);
                    }

                    result.ResultData = Mapper.MapEntitiesToDtos(entities);
                    result.ResultCode = ResultCode.Ok;
                }
            }
            catch (Exception e)
            {
                result.ResultData = null;
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
                result.Exception = e;
            }

            return result;
        }

        #endregion

        public virtual IRepositoryResult<TDto> Create(TDto dto)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                using (var context = new TDBContext())
                {
                    // convert dto to an entity
                    var entity = Mapper.MapDtoToEntity(dto);

                    var create = context.Set<TEntity>().Add(entity);
                    context.SaveChanges();

                    // convert created entity back to dto for return object
                    result.ResultData = Mapper.MapEntityToDto(create);
                    result.ResultCode = ResultCode.Created;
                }
            }
            catch (Exception e)
            {
                result.ResultData = default(TDto);
                result.ResultCode = ResultCode.Error;
                result.ResultDescription = e.Message;
                result.Exception = e;
            }

            return result;
        }
    }
}
