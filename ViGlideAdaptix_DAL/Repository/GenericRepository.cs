using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_DAL.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ViGlideAdaptixContext _context;
		private readonly DbSet<T> _dbSet;
		public GenericRepository(ViGlideAdaptixContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}
		public async Task AddAsync(T item)
		{
			await _dbSet.AddAsync(item);
		}

		public void Delete(T item)
		{
			_dbSet.Remove(item);
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _dbSet;

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			return await query.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(int TId)
		{
			return await _dbSet.FindAsync(TId);
		}

		public async Task<T?> GetByIdWithIncludeAsync(int TId,string typeId, params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _dbSet;

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, typeId) == TId);
		}

		public async Task SaveChangeAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Update(T item)
		{
			_dbSet.Update(item);
		}
	}
}


