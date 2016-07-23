using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ProxyChat.Domain
{
    public abstract class BaseRepository<TDBContext, TEntity, TDto> : IBaseRepository<TDBContext, TEntity, TDto>
        where TDto : class, new()
        where TEntity : class, new()
        where TDBContext : DbContext, new()
    {
        // Implemented by inherited classes, used to convert from entities to dtos and dtos to entities
        protected abstract IResourceMapper<TEntity, TDto> Mapper { get; }

        // Implemented by inherited classes, checks required fields to avoid sql exceptions
        protected abstract bool ValidateDto(TDto dto);

        #region Read

        public virtual IRepositoryResult<TDto> Read(Func<TEntity, bool> whereClause)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

            try
            {
                using (var context = new TDBContext())
                {
                   var entity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(whereClause);

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

        public virtual IRepositoryResult<TDto> Read(Func<TEntity, bool> whereClause, params Expression<Func<TEntity, object>>[] relatedEntities)
        {
            IRepositoryResult<TDto> result = new RepositoryResult<TDto>();

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

                    var entity = query.FirstOrDefault(whereClause);

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

        #region Create
        public virtual IRepositoryResult<TDto> Create(TDto dto)
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
                        context.SaveChanges();

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
