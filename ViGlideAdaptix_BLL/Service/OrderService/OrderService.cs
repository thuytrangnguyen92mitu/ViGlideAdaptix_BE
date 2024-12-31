using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

namespace ViGlideAdaptix_BLL.Service.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Confirm Order
		/// orderId
		/// return isConfirm + Message
		/// </summary>
		public async Task<(bool, string)> ConfirmOrder(int orderId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundOrder = await _unitOfWork.OrderRepository.GetByIdWithIncludeAsync(orderId, "OrderId", order => order.OrderDetails);
				if (foundOrder == null)
					return (false, "Cannot find order with that ID");
				foundOrder.Status = 1;
				_unitOfWork.OrderRepository.Update(foundOrder);

				var orderDetail = foundOrder.OrderDetails.ToList();
				foreach (var detail in orderDetail)
				{
					var product = detail.Product;
					if (product == null || product.Quantity < detail.Quantity)
					{
						return (false, "This product it not in stock anymore or the quantity is lower than the cart quantity");
					}

					product.Quantity -= detail.Quantity;
					_unitOfWork.ProductRepository.Update(product);
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Confirm order successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error confirm order", ex);
			}
		}

		/// <summary>
		/// Cancel Order
		/// orderId
		/// return IsCancel + Message
		/// </summary>
		public async Task<(bool, string)> CancelOrder(int orderId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundOrder = await _unitOfWork.OrderRepository.GetByIdWithIncludeAsync(orderId, "OrderId", order => order.OrderDetails);
				if (foundOrder == null)
					return (false, "Cannot find order with that ID");
				foundOrder.Status = 1;
				_unitOfWork.OrderRepository.Update(foundOrder);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return (true, "Cancel order successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error cancel order", ex);
			}
		}

		/// <summary>
		/// Rating Product
		/// Find by orderId
		/// return isRated + Message
		/// </summary>
		public async Task<(bool, string)> RatingProduct(int orderId, RatingRequestDTO ratingRequest)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var foundOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
				if (foundOrder == null)
					return (false, "Cannot find order with that ID");

				if (foundOrder.Status == 2)
					return (false, "This order cannot rating yet");
				if (foundOrder.Status == 1)
				{
					var detailList = foundOrder.OrderDetails.ToList();
					var foundDetail = detailList.FirstOrDefault(x => x.OrderItemId == ratingRequest.OrderItemId);
					if (foundDetail != null)
					{
						var isRated = await IsOrderDetailRated(ratingRequest.OrderItemId);
						if (isRated)
							return (false, "Order detail has already been rated");

						var newRating = new Rating
						{
							OrderItemId = ratingRequest.OrderItemId,
							CustomerId = foundOrder.CustomerId,
							Score = ratingRequest.Score,
							Comment = ratingRequest.Comment,
							RatingDate = ratingRequest.RatingDate,
							ProductId = foundDetail.ProductId,
						};
					
						await _unitOfWork.RatingRepository.AddAsync(newRating);
						await _unitOfWork.SaveChangesAsync();
						await _unitOfWork.CommitTransactionAsync();
						return (true, "Rating successfully");
					}
				}
				return (false, "Cannot find order with that ID");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error rating product", ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderdetailid"></param>
		/// <returns></returns>
		private async Task<bool> IsOrderDetailRated(int orderDetailId)
		{
			// Check directly if a rating exists for the order detail
			var listRating = await _unitOfWork.RatingRepository.GetAllAsync();
			return listRating.FirstOrDefault(x => x.OrderItemId == orderDetailId) != null;
		}
	}
}
