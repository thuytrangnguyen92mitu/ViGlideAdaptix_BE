using System.Linq.Expressions;

namespace ViGlideAdaptix_DAL.Repository
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includeProperties);
		Task<T?> GetByIdAsync(int TId);
		Task<T?> GetByIdWithIncludeAsync(int TId, params Expression<Func<T, object>>[] includeProperties);
		Task AddAsync(T item);
		void Update(T item);
		void Delete(T item);
		Task SaveChangeAsync();
	}
}
