using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Запросить все сущности в базе.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены. </param>
        /// <param name="asNoTracking"> Вызвать с AsNoTracking. </param>
        /// <returns> Список сущностей. </returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);

        /// <summary>
        /// Получить сущность по Id.
        /// </summary>
        /// <param name="id"> Id сущности. </param>
        /// <param name="cancellationToken"></param>
        /// <returns> Cущность. </returns>
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Добавить в базу одну сущность.
        /// </summary>
        /// <param name="entity"> Сущность для добавления. </param>
        /// <returns> Добавленная сущность. </returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Удалить сущность.
        /// </summary>
        /// <param name="id"> Id удалённой сущности. </param>
        /// <returns> Была ли сущность удалена. </returns>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Обновить сущность.
        /// </summary>
        /// <param name="entity"> Сущность для обновления. </param>
        /// <returns> Обновленная сущность. </returns>
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Сохранить изменения.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}