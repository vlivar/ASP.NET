using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                return await Task.FromResult(Data.AsEnumerable());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                return await Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<Guid> CreateAsync(T item)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                Data.Add(item);
                return await Task.FromResult(item.Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var oldItem = Data.FirstOrDefault(x => x.Id == item.Id);
                if (oldItem != null)
                {
                    Data.Remove(oldItem);
                    Data.Add(item);
                }

                return await Task.FromResult(Data.FirstOrDefault(x => x.Id == item.Id));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var item = Data.FirstOrDefault(x => x.Id == id);
                var result = Data.Remove(item);
                return await Task.FromResult(result);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}