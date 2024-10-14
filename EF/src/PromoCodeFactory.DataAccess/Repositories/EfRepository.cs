using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataContext Context;
        private readonly DbSet<T> _entitySet;

        public EfRepository(DataContext context)
        {
            Context = context;
            _entitySet = Context.Set<T>();
        }

        /// <summary>
        /// Добавить в базу одну сущность.
        /// </summary>
        /// <param name="entity"> Сущность для добавления. </param>
        /// <returns> Добавленная сущность. </returns>
        public async Task<T> AddAsync(T entity)
        {
            return (await _entitySet.AddAsync(entity)).Entity;
        }

        /// <summary>
        /// Удалить сущность.
        /// </summary>
        /// <param name="id"> Id удаляемой сущности. </param>
        /// <returns> Была ли сущность удалена. </returns>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var deleteEntity = await _entitySet.FindAsync(id);
            if (deleteEntity == null)
            {
                return false;
            }
            _entitySet.Remove(deleteEntity);
            return true;
        }

        /// <summary>
        /// Запросить все сущности в базе.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены </param>
        /// <param name="asNoTracking"> Вызвать с AsNoTracking. </param>
        /// <returns> Список сущностей. </returns>
        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
        {
            return asNoTracking ?
               await _entitySet.AsNoTracking().ToListAsync(cancellationToken) :
               await _entitySet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Получить сущность по Id.
        /// </summary>
        /// <param name="id"> Id сущности. </param>
        /// <param name="cancellationToken"></param>
        /// <returns> Cущность. </returns>
        public async Task<T> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _entitySet.FindAsync(id);
        }

        /// <summary>
        /// Сохранить изменения.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
