using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ShopEgypt.Infrastructure.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);
        
        Task<T> GetByUserIdAsync (string userId);

        Task <IEnumerable<T>> GetAllAsync();
        Task SaveChangesAsync();
        Task<T> AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
