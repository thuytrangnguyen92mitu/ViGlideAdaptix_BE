using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_DAL.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ViGlideAdaptixContext _context;
		private IDbContextTransaction? _transaction;

		public IGenericRepository<ShoppingCart> CartRepository { get; }
		public IGenericRepository<ShoppingCartItem> CartItemRepository { get; }
		public IGenericRepository<Product> ProductRepository { get; }
		public IGenericRepository<Customer> CustomerRepository { get; }

		public IGenericRepository<PaymentMethod> PaymentMethodRepository { get; }

		public IGenericRepository<Order> OrderRepository { get; }
		public IGenericRepository<OrderDetail> OrderDetailRepository { get; }
		public IGenericRepository<Rating> RatingRepository { get; }


		public UnitOfWork(ViGlideAdaptixContext context)
		{
			_context = context;
			CartRepository = new GenericRepository<ShoppingCart>(_context);
			CartItemRepository = new GenericRepository<ShoppingCartItem>(_context);
			ProductRepository = new GenericRepository<Product>(_context);
			CustomerRepository = new GenericRepository<Customer>(_context);
			PaymentMethodRepository = new GenericRepository<PaymentMethod>(_context);
			OrderRepository = new GenericRepository<Order>(_context);
			OrderDetailRepository = new GenericRepository<OrderDetail>(_context);
			RatingRepository = new GenericRepository<Rating>(_context);
		}
		public async Task BeginTransactionAsync()
		{
			_transaction = await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransactionAsync()
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}
		public async Task RollbackTransactionAsync()
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
