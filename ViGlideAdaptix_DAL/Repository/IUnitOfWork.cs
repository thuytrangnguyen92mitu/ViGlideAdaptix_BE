using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_DAL.Repository
{
	public interface IUnitOfWork : IDisposable
	{
		IGenericRepository<ShoppingCart> CartRepository { get; }
		IGenericRepository<ShoppingCartItem> CartItemRepository { get; }
		IGenericRepository<Product> ProductRepository { get; }
		IGenericRepository<Customer> CustomerRepository { get; }
		IGenericRepository<PaymentMethod> PaymentMethodRepository { get; }
		IGenericRepository<Order> OrderRepository { get; }
		IGenericRepository<OrderDetail> OrderDetailRepository { get; }

		IGenericRepository<Rating> RatingRepository { get; }

		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollbackTransactionAsync();
		Task SaveChangesAsync();
	}
}
