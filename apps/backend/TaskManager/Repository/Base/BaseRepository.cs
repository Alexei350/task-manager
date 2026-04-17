using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models.Base;
using TaskManager.Models.Base.Entities;
using TaskManager.Models.Base.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TaskManager.Repository.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Adiciona um registro
        /// </summary>
        /// <param name="entity">Entidade a ser criada</param>
        Task CreateAsync(T entity);

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada</param>
        void Update(T entity);

        /// <summary>
        /// Deleta um registro
        /// </summary>
        /// <param name="entity">Entidade a ser deletada</param>
        void Delete(T entity);

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Query();

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <returns></returns>
        IQueryable<T> Query(FilterBy<T> filter);

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <returns></returns>
        IQueryable<T> Query(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Verifica se existe algum registro com o filtro indicado
        /// </summary>
        /// <param name="filter">Filtro</param>
        /// <returns></returns>
        Task<bool> AnyAsync(FilterBy<T> filter);

        /// <summary>
        /// Verifica se existe algum registro com o filtro indicado
        /// </summary>
        /// <param name="filter">Filtro</param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Realiza a contagem de registros
        /// </summary>
        /// <param name="filter">Filtro da contagem</param>
        /// <returns></returns>
        Task<int> CountAsync(FilterBy<T> filter);

        /// <summary>
        /// Realiza a contagem de registros
        /// </summary>
        /// <param name="filter">Filtro da contagem</param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        T Get(Guid id);

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        T GetForUpdate(Guid id);

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        Task<T> GetForUpdateAsync(Guid id);

        /// <summary>
        /// Retorna uma lista paginada de registros
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <param name="select">Seleção da consulta</param>
        /// <param name="orderBy">Ordenação</param>
        /// <param name="page">Página atual</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns></returns>
        Task<ReturnDataPaged<TReturn>> GetPagedAsync<TReturn>(
            FilterBy<T> filter, 
            Expression<Func<T, TReturn>> select,
            Expression<Func<T, object>> orderBy, 
            int page, 
            int pageSize
        );

        /// <summary>
        /// Salva as alterações no banco de dados
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveChangesAsync();
    }

    public abstract class BaseRepository<T>(TaskManagerContext context) : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly TaskManagerContext Context = context;

        /// <summary>
        /// Adiciona um registro
        /// </summary>
        /// <param name="entity">Entidade a ser criada</param>
        public async Task CreateAsync(T entity) 
        {
            await Context.AddAsync(entity);
        }

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada</param>
        public void Update(T entity)
        {
            Context.Update(entity);
        }

        /// <summary>
        /// Deleta um registro
        /// </summary>
        /// <param name="entity">Entidade a ser deletada</param>
        public void Delete(T entity)
        {
            if (entity is BaseEntitySoft soft)
                soft.Deleted = true;
            else
                Context.Remove(entity);
        }

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Query()
        {
            return Context
                .Set<T>();
        }

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <returns></returns>
        public IQueryable<T> Query(FilterBy<T> filter)
        {
            return Query(filter.Filter);
        }

        /// <summary>
        /// Realiza uma consulta
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <returns></returns>
        public IQueryable<T> Query(Expression<Func<T, bool>> filter)
        {
            return Context
                .Set<T>()
                .Where(filter);
        }

        /// <summary>
        /// Verifica se existe algum registro com o filtro indicado
        /// </summary>
        /// <param name="filter">Filtro</param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(FilterBy<T> filter)
        {
            return await AnyAsync(filter.Filter);
        }

        /// <summary>
        /// Verifica se existe algum registro com o filtro indicado
        /// </summary>
        /// <param name="filter">Filtro</param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await Context
                .Set<T>()
                .AnyAsync(filter);
        }

        /// <summary>
        /// Realiza a contagem de registros
        /// </summary>
        /// <param name="filter">Filtro da contagem</param>
        /// <returns></returns>
        public async Task<int> CountAsync(FilterBy<T> filter)
        {
            return await CountAsync(filter.Filter);
        }

        /// <summary>
        /// Realiza a contagem de registros
        /// </summary>
        /// <param name="filter">Filtro da contagem</param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return await Context
                .Set<T>()
                .CountAsync(filter);
        }

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        public T Get(Guid id)
        {
            var context = Context
                .Set<T>();
            
            return context
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        public T GetForUpdate(Guid id)
        {
            var context = Context
                .Set<T>();
            
            return context
                .FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        public async Task<T> GetAsync(Guid id)
        {
            var context = Context
                .Set<T>();
            
            return await context
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Busca um registro pelo Id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <returns></returns>
        public async Task<T> GetForUpdateAsync(Guid id)
        {
            var context = Context
                .Set<T>();
            
            return await context
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Retorna uma lista paginada de registros
        /// </summary>
        /// <param name="filter">Filtro da consulta</param>
        /// <param name="select">Seleção da consulta</param>
        /// <param name="orderBy">Ordenação</param>
        /// <param name="page">Página atual</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns></returns>
        public async Task<ReturnDataPaged<TReturn>> GetPagedAsync<TReturn>(
            FilterBy<T> filter,
            Expression<Func<T, TReturn>> select,
            Expression<Func<T, object>> orderBy,
            int page, 
            int pageSize
        )
        {
            page = page == default ? 1 : page;
            pageSize = pageSize == default ? 20 : pageSize;

            var result = await Query(filter)
                .OrderBy(orderBy)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(select)
                .ToListAsync();

            return new ReturnDataPaged<TReturn>
            {
                Success = true,
                Data = result,
                TotalRecords = result.Count,
                TotalPages = pageSize > 0 ? (int)Math.Ceiling((double)result.Count/pageSize) : 0
            };
        }

        /// <summary>
        /// Salva as alterações no banco de dados
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
