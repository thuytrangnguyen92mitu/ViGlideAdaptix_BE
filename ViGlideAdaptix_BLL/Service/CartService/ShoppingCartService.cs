using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

namespace ViGlideAdaptix_BLL.Service.CartService
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ShoppingCartService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Get Cart of Customer 
		/// CartRequestDTO
		/// Return Cart + Message
		/// </summary>
		public async Task<(CartResponseDTO?, string)> GetCartOfCustomer(CartRequestDTO request)
		{
			try
			{
				var foundCustomer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId);
				if (foundCustomer == null)
					return (null, "Cannot find Customer with that ID!");

				var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId,"CartId", cart => cart.ShoppingCartItems);
				if (foundCart != null)
				{
					var convertedCart = ConvertToCartResponse(foundCart);
					return (convertedCart, "Get Cart successfully!");
				}

				return (null, "Fail to get Shopping Cart");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error retrieving cart", ex);
			}
		}

		/// <summary>
		/// Use to clear cart
		/// CartID
		/// Return status + message
		/// </summary>
		public async Task<(bool, string)> ClearCart(int cartId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(cartId,"CartId", cart => cart.ShoppingCartItems);
				if (foundCart == null)
					return (false, "Cannot find cart with that ID");

				foreach (var item in foundCart.ShoppingCartItems)
				{
					_unitOfWork.CartItemRepository.Delete(item);
				}

				_unitOfWork.CartRepository.Delete(foundCart);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Cart cleared successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error clearing cart", ex);
			}
		}

		/// <summary>
		/// Add new item to Cart
		/// AddToCartRequestDTO
		/// Return status + Message
		/// </summary>
		public async Task<(bool, string)> AddItemToCart(AddToCartRequestDTO request)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId, "CartId", cart => cart.ShoppingCartItems);
				if (foundCart == null)
					return (false, "Cannot find cart with that ID");

				var foundProduct = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
				if (foundProduct == null || foundProduct.Status == 0 || foundProduct.Quantity <= 0)
					return (false, "Product is out of stock or unavailable");

				var cartItemsDict = foundCart.ShoppingCartItems.ToDictionary(
																item => item.ProductId,
																item => item
															);
				if (cartItemsDict.ContainsKey(request.ProductId))
				{
					cartItemsDict[request.ProductId].Quantity++;
				}
				else
				{
					var newItem = new ShoppingCartItem
					{
						CartId = foundCart.CartId,
						ProductId = request.ProductId,
						Quantity = 1,
						UnitPrice = foundProduct.UnitPrice
					};
					await _unitOfWork.CartItemRepository.AddAsync(newItem);
				}

				foundProduct.Quantity--;
				_unitOfWork.ProductRepository.Update(foundProduct);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Add item successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error adding item to cart", ex);
			}
		}

		/// <summary>
		/// Remove item from cart
		/// RemoveFromCartDTO
		/// Return status + Message
		/// </summary>
		public async Task<(bool, string)> RemoveItemFromCart(RemoveFromCartDTO request)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId, "CartId", cart => cart.ShoppingCartItems);
				if (foundCart == null)
					return (false, "Cannot find cart with that ID");

				var cartItemsDict = foundCart.ShoppingCartItems.ToDictionary(
				item => item.ProductId,
				item => item);
				if (!cartItemsDict.ContainsKey(request.ProductId))
					return (false, "Product not found in cart");

				var cartItem = cartItemsDict[request.ProductId];
				if (cartItem.Quantity > 1)
				{
					cartItem.Quantity--;
				}
				else
				{
					_unitOfWork.CartItemRepository.Delete(cartItem);
				}

				var foundProduct = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
				if (foundProduct != null)
				{
					foundProduct.Quantity++;
					_unitOfWork.ProductRepository.Update(foundProduct);
				}


				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Item removed successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error removing item from cart", ex);
			}
		}

		/// <summary>
		/// Convert ShoppingCart to CartResponseDTO
		/// </summary>
		private CartResponseDTO ConvertToCartResponse(ShoppingCart cart)
		{
			return new CartResponseDTO
			{
				SubTotal = cart.SubTotal,
				ShippingPrice = cart.ShippingPrice,
				TotalPrice = cart.TotalPrice,
				CartItemsList = cart.ShoppingCartItems.Select(item => new CartItemDTO
				{
					CartItemId = item.CartItemId,
					CartId = item.CartId,
					ProductId = item.ProductId,
					Quantity = item.Quantity,
					UnitPrice = item.UnitPrice
				}).ToList()
			};
		}

		public async Task<(bool, string)> CheckOutCartToPayment(CheckOutRequestDTO request)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId, "CartId", cart => cart.ShoppingCartItems);
				if (foundCart == null)
					return (false, "Cart not found with this ID");

				if (foundCart.CustomerId != request.CustomerId)
					return (false, "Cart does not belong to the specified user.");

				var paymentMethod = await _unitOfWork.PaymentMethodRepository.GetByIdAsync(request.PaymentMethodId);
				if (paymentMethod == null)
					return (false, "Invalid or inactive payment method");

				foreach (var item in foundCart.ShoppingCartItems)
				{
					var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
					if (product == null || product.Quantity < item.Quantity)
						return (false, $"Insufficient stock for product ID {item.ProductId}.");
				}

				foundCart.TotalPrice = foundCart.CalculateTotalPrice();

				var order = new Order
				{
					CartId = foundCart.CartId,
					CustomerId = foundCart.CustomerId,
					CreatedDate = DateTime.UtcNow,
					TotalPrice = foundCart.TotalPrice,
					PaymentMethodId = paymentMethod.PaymentMethodId,
					EstDeliveryDate = DateTime.UtcNow,
					Tax = (Double)foundCart.TotalPrice * 0.1, //transfer to constant
					Status = 1
				};

				await _unitOfWork.OrderRepository.AddAsync(order);
				await _unitOfWork.SaveChangesAsync();

				foreach (var item in foundCart.ShoppingCartItems)
				{
					var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);

					var orderDetail = new OrderDetail
					{
						ProductId = item.ProductId,
						Quantity = item.Quantity,
						UnitPrice = item.UnitPrice,
						TotalPrice = item.CalculateItemTotal()
					};
					await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);

					if(product != null)
					{
						product.Quantity -= item.Quantity;
						_unitOfWork.ProductRepository.Update(product);
					}
				}

				foundCart.IsActive = false;
				_unitOfWork.CartRepository.Update(foundCart);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Checkout successful. Order created.");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Checkout failed.", ex);
			}
		}
	}
}
