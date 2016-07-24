using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public abstract class BaseRepository<TDBContext, TEntity, TDto> : IBaseRepository<TDBContext, TEntity, TDto>
        where TDto : ResourceDto, new()
        where TEntity : class, IResource, new()
        where TDBContext : DbContext, new()
    {
        // used to convert from entities to dtos and dtos to entities
        protected abstract IResourceMapper<TEntity, TDto> Mapper { get; }

        // checks required fields to avoid sql exceptions
        protected abstract bool ValidateDto(TDto dto);

        // copies resource specific values from one entity to another
        protected abstract void UpdateEntityProperties(TEntity from, TEntity to);

        #region Read

        public virtual async Task<IRepositoryResult<TDto>> Read(Expression<Func<TEntity, bool>> whereClause)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                using (var context = new TDBContext())
                {
                    var entity = await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(whereClause);

                    if (entity == null)
                    {
                        result.ResultData = null;
                        result.ResultCode = ResultCode.NotFound;
                    }
                    else
                    {
                        result.ResultData = Mapper.MapEntityToDto(entity);
                        result.ResultCode = ResultCode.Ok;
                    }
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

        public virtual async Task<IRepositoryResult<TDto>> Read(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                using (var context = new TDBContext())
                {
                     IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();

                    // include each related entity supplied in parameter list
                    foreach (var relation in relatedEntities)
                    {
                        query = query.Include(relation);
                    }

                    var entity = await query.FirstOrDefaultAsync(whereClause);

                    if (entity == null)
                    {
                        result.ResultData = null;
                        result.ResultCode = ResultCode.NotFound;
                    }
                    else
                    {
                        result.ResultData = Mapper.MapEntityToDto(entity);
                        result.ResultCode = ResultCode.Ok;
                    }
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

        public virtual async Task<IRepositoryResult<IList<TDto>>> ReadAll()
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();
                    var entities = await query.ToListAsync();

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

        public virtual async Task<IRepositoryResult<IList<TDto>>> ReadAll(params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

            try
            {
                using (var context = new TDBContext())
                {
                    IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking().AsQueryable();

                    // include each related entity supplied in parameter list
                    foreach (var relation in relatedEntities)
                    {
                        query = query.Include(relation);
                    }

                    var entities = await query.ToListAsync();

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

        public virtual Task<IRepositoryResult<IList<TDto>>> ReadAll(Func<TEntity, bool> whereClause)
        {
            return Task.Run(() =>
            {
                IRepositoryResult<IList<TDto>> result = new RepositoryResult<IList<TDto>>();

                try
                {
                    using (var context = new TDBContext())
                    {
                        var query = context.Set<TEntity>().AsNoTracking();

                        var entities = context.Set<TEntity>().AsNoTracking().Where(whereClause).AsQueryable();

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
            });
        }

        // returns a task because adding in the where clause then calling async read after throws an exceptioin because it
        //    gets invoked on a collectioin that doesn't implement async methods.  Currently no WhereAsync is available
        public virtual Task<IRepositoryResult<IList<TDto>>> ReadAll(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            return Task.Run(() =>
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
            });
        }

        #endregion

        #region Update
        public virtual async Task<IRepositoryResult<TDto>> Update(TDto dto)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                using (var context = new TDBContext())
                {
                    var entity = await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == dto.Id);

                    if (entity == null)
                    {
                        result.ResultCode = ResultCode.NotModified;
                        result.ResultData = null;
                    }
                    else
                    {
                        UpdateEntityProperties(Mapper.MapDtoToEntity(dto), entity);
                        await context.SaveChangesAsync();

                        result.ResultCode = ResultCode.Ok;
                        result.ResultData = Mapper.MapEntityToDto(entity);
                    }
                }
            }
            catch (Exception e)
            {
                result.ResultCode = ResultCode.Error;
                result.ResultData = null;
                result.ResultDescription = e.Message;
            }

            return result;
        }
        #endregion

        #region Create
        public virtual async Task<IRepositoryResult<TDto>> Create(TDto dto)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                if (ValidateDto(dto))
                {
                    using (var context = new TDBContext())
                    {
                        // convert dto to an entity
                        var entity = Mapper.MapDtoToEntity(dto);

                        var create = context.Set<TEntity>().Add(entity);
                        await context.SaveChangesAsync();

                        // convert created entity back to dto for return object
                        result.ResultData = Mapper.MapEntityToDto(create);
                        result.ResultCode = ResultCode.Created;
                    }
                }
                else
                {
                    result.ResultData = null;
                    result.ResultCode = ResultCode.InvalidData;
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
    }
}
