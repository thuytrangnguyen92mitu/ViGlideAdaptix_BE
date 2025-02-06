using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

                var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId, "CartId", cart => cart.ShoppingCartItems);
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

                var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(cartId, "CartId", cart => cart.ShoppingCartItems);
                if (foundCart == null)
                    return (false, "Cannot find cart with that ID");

                foreach (var item in foundCart.ShoppingCartItems)
                {
                    _unitOfWork.CartItemRepository.Delete(item);
                }

                //_unitOfWork.CartRepository.Delete(foundCart);

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

                //foundProduct.Quantity--;
                //_unitOfWork.ProductRepository.Update(foundProduct);

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
                    _unitOfWork.CartItemRepository.Update(cartItem);
                }
                else
                {
                    _unitOfWork.CartItemRepository.Delete(cartItem);
                }

                var foundProduct = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
                //if (foundProduct != null)
                //{
                //	foundProduct.Quantity++;
                //	_unitOfWork.ProductRepository.Update(foundProduct);
                //}


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

        public async Task<(bool, string, int,int)> CheckOutCartToPayment(CheckOutRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var foundCart = await _unitOfWork.CartRepository.GetByIdWithIncludeAsync(request.CartId, "CartId", cart => cart.ShoppingCartItems);
                if (foundCart == null)
                    return (false, "Cart not found with this ID", 0,0);

                if (foundCart.CustomerId != request.CustomerId)
                    return (false, "Cart does not belong to the specified user.", 0, 0);

                var paymentMethod = await _unitOfWork.PaymentMethodRepository.GetByIdAsync(request.PaymentMethodId);
                if (paymentMethod == null)
                    return (false, "Invalid or inactive payment method", 0, 0);

                var cartItems = foundCart.ShoppingCartItems;
                if (cartItems != null)
                {
                    foundCart.CalculateTotalPrice();
                    var order = new Order
                    {
                        CartId = foundCart.CartId,
                        CustomerId = foundCart.CustomerId,
                        CreatedDate = DateTime.UtcNow,
                        TotalPrice = foundCart.CalculateTotalPrice(),
                        PaymentMethodId = paymentMethod.PaymentMethodId,
                        EstDeliveryDate = DateTime.UtcNow,
                        //Tax = (Double)foundCart.TotalPrice * 0.1, //transfer to constant
                        Status = 2
                    };
                    order.Tax = (Double)order.TotalPrice * 0.1;

                    await _unitOfWork.OrderRepository.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    foreach (var item in cartItems)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                        if (product == null)
                        {
                            continue;
                        }
                        int quantityToDeduct = Math.Min(product.Quantity, item.Quantity);

                        var orderDetail = new OrderDetail
                        {
                            ProductId = item.ProductId,
                            Quantity = quantityToDeduct,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.UnitPrice * quantityToDeduct,
                            OrderId = order.OrderId, // Use the correct populated OrderId
                        };

                        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);

                        product.Quantity -= quantityToDeduct;
                        product.Purchases += quantityToDeduct;
                        _unitOfWork.ProductRepository.Update(product);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    foundCart.IsActive = false;
                    _unitOfWork.CartRepository.Update(foundCart);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();

                    var orderId = GetOrderIdOfCart(foundCart);
                    var newCart = CreateNewCartOfCustomer(request.CustomerId);
                    var newCartId = newCart.Result.CartId;

                    return (true, "Checkout successful. Order created.", orderId, newCartId);

                }
                else
                {
                    return (false, "Cannot checkout with empty cart", 0, 0);
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApplicationException("Checkout failed.", ex);
            }
        }

        private int GetOrderIdOfCart(ShoppingCart cart)
        {
            var foundOrder = cart.Orders?.FirstOrDefault(x => x.CartId == cart.CartId);
            return foundOrder?.OrderId ?? 0;
        }

        private async Task<ShoppingCart> CreateNewCartOfCustomer(int customerId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var newCart = new ShoppingCart()
                {
                    CreatedDate = DateTime.UtcNow,
                    CustomerId = customerId,
                    SubTotal = 0,
                    IsActive = true
                };

                await _unitOfWork.CartRepository.AddAsync(newCart);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return newCart;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }


    }
}

