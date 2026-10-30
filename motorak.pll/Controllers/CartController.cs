using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using motorak.dal.Entites;
using motorak.dal.Enums.CartEnums;
using motorak.dal.Repo.Abstractions;
using Motorak.DAL.Entites;

namespace Motorak.Controllers
{
    [Authorize] // Ensure only authenticated users can access cart
    public class CartController : Controller
    {
        private readonly ICartRepo _cartRepo;
        private readonly UserManager<User> _userManager;

        public CartController(ICartRepo cartRepo, UserManager<User> userManager)
        {
            _cartRepo = cartRepo;
            _userManager = userManager;
        }

        // GET: Cart/Index - Display cart items
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = await _cartRepo.GetCartItemsByUserIdAsync(userId);
                var cartTotal = await _cartRepo.GetCartTotalAsync(userId);

                ViewBag.CartTotal = cartTotal;
                ViewBag.ItemCount = cartItems.Sum(x => x.Count);
                ViewBag.ProcessingFee = 25.00m;
                ViewBag.TaxRate = 0.085m; // 8.5%
                ViewBag.TaxAmount = cartTotal * (decimal)ViewBag.TaxRate;
                ViewBag.FinalTotal = cartTotal + (decimal)ViewBag.ProcessingFee + (decimal)ViewBag.TaxAmount;

                return View(cartItems);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load cart items. Please try again.";
                return View(new List<CartItem>());
            }
        }

        // POST: Cart/AddToCart - Add item to cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int carId, CartItemType itemType, int count = 1,
            DateTime? rentStartDate = null, DateTime? rentEndDate = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Validate rental dates if it's a rental item
                if (itemType == CartItemType.Rent)
                {
                    if (!rentStartDate.HasValue || !rentEndDate.HasValue)
                    {
                        return Json(new { success = false, message = "Rental dates are required for rent items" });
                    }

                    if (rentStartDate >= rentEndDate)
                    {
                        return Json(new { success = false, message = "End date must be after start date" });
                    }

                    if (rentStartDate < DateTime.Today)
                    {
                        return Json(new { success = false, message = "Start date cannot be in the past" });
                    }
                }

                var cartItem = new CartItem
                {
                    CarId = carId,
                    UserId = userId,
                    Count = count,
                    ItemType = itemType,
                    RentStartDate = rentStartDate,
                    RentEndDate = rentEndDate,
                    DateAdded = DateTime.UtcNow
                };

                await _cartRepo.AddCartItemAsync(cartItem);

                return Json(new { success = true, message = "Item added to cart successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add item to cart" });
            }
        }

        // POST: Cart/UpdateQuantity - Update item quantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            try
            {
                if (quantity < 1)
                {
                    return Json(new { success = false, message = "Quantity must be at least 1" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = await _cartRepo.GetCartItemAsync(itemId);

                if (cartItem == null || cartItem.UserId != userId)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                cartItem.Count = quantity;
                await _cartRepo.UpdateCartItemAsync(cartItem);

                // Calculate new totals
                var cartTotal = await _cartRepo.GetCartTotalAsync(userId);
                var processingFee = 25.00m;
                var taxRate = 0.085m;
                var taxAmount = cartTotal * taxRate;
                var finalTotal = cartTotal + processingFee + taxAmount;

                return Json(new
                {
                    success = true,
                    message = "Quantity updated successfully",
                    cartTotal = cartTotal.ToString("C"),
                    taxAmount = taxAmount.ToString("C"),
                    finalTotal = finalTotal.ToString("C")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update quantity" });
            }
        }

        // POST: Cart/UpdateDates - Update rental dates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDates(int itemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = await _cartRepo.GetCartItemAsync(itemId);

                if (cartItem == null || cartItem.UserId != userId)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                if (cartItem.ItemType != CartItemType.Rent)
                {
                    return Json(new { success = false, message = "This item is not a rental" });
                }

                // Validate dates
                if (startDate >= endDate)
                {
                    return Json(new { success = false, message = "End date must be after start date" });
                }

                if (startDate < DateTime.Today)
                {
                    return Json(new { success = false, message = "Start date cannot be in the past" });
                }

                cartItem.RentStartDate = startDate;
                cartItem.RentEndDate = endDate;
                await _cartRepo.UpdateCartItemAsync(cartItem);

                return Json(new { success = true, message = "Rental dates updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update dates" });
            }
        }

        // POST: Cart/RemoveItem - Remove item from cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = await _cartRepo.GetCartItemAsync(itemId);

                if (cartItem == null || cartItem.UserId != userId)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                await _cartRepo.DeleteCartItemAsync(itemId);

                // Calculate new totals
                var cartTotal = await _cartRepo.GetCartTotalAsync(userId);
                var cartItems = await _cartRepo.GetCartItemsByUserIdAsync(userId);
                var itemCount = cartItems.Sum(x => x.Count);

                if (itemCount == 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Item removed from cart",
                        cartEmpty = true
                    });
                }

                var processingFee = 25.00m;
                var taxRate = 0.085m;
                var taxAmount = cartTotal * taxRate;
                var finalTotal = cartTotal + processingFee + taxAmount;

                return Json(new
                {
                    success = true,
                    message = "Item removed from cart",
                    cartTotal = cartTotal.ToString("C"),
                    taxAmount = taxAmount.ToString("C"),
                    finalTotal = finalTotal.ToString("C"),
                    itemCount = itemCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to remove item" });
            }
        }

        // POST: Cart/ClearCart - Clear all cart items
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _cartRepo.ClearUserCartAsync(userId);

                TempData["Success"] = "Cart cleared successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to clear cart";
                return RedirectToAction("Index");
            }
        }

        // GET: Cart/GetCartCount - Get cart item count (for navbar)
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { count = 0 });
                }

                var cartItems = await _cartRepo.GetCartItemsByUserIdAsync(userId);
                var count = cartItems.Sum(x => x.Count);

                return Json(new { count = count });
            }
            catch (Exception ex)
            {
                return Json(new { count = 0 });
            }
        }

        // POST: Cart/Checkout - Process checkout and redirect to payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = await _cartRepo.GetCartItemsByUserIdAsync(userId);

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty";
                    return RedirectToAction("Index");
                }

                // Validate all rental items have proper dates
                var rentalItems = cartItems.Where(x => x.ItemType == CartItemType.Rent);
                foreach (var item in rentalItems)
                {
                    if (!item.RentStartDate.HasValue || !item.RentEndDate.HasValue)
                    {
                        TempData["Error"] = $"Please set rental dates for {item.Car?.Brand} {item.Car?.Model}";
                        return RedirectToAction("Index");
                    }

                    if (item.RentStartDate >= item.RentEndDate)
                    {
                        TempData["Error"] = $"Invalid rental dates for {item.Car?.Brand} {item.Car?.Model}";
                        return RedirectToAction("Index");
                    }

                    if (item.RentStartDate < DateTime.Today)
                    {
                        TempData["Error"] = $"Rental start date cannot be in the past for {item.Car?.Brand} {item.Car?.Model}";
                        return RedirectToAction("Index");
                    }
                }

                // Calculate totals
                var cartTotal = await _cartRepo.GetCartTotalAsync(userId);
                var processingFee = 25.00m;
                var taxRate = 0.085m;
                var taxAmount = cartTotal * taxRate;
                var finalTotal = cartTotal + processingFee + taxAmount;

                // Store checkout data in TempData for the payment controller
                TempData["CheckoutData"] = System.Text.Json.JsonSerializer.Serialize(new
                {
                    UserId = userId,
                    CartItems = cartItems.Select(x => new {
                        x.Id,
                        x.CarId,
                        Car = new { x.Car.Brand, x.Car.Model, x.Car.Year, x.Car.Price },
                        x.Count,
                        x.ItemType,
                        x.RentStartDate,
                        x.RentEndDate
                    }),
                    CartTotal = cartTotal,
                    ProcessingFee = processingFee,
                    TaxAmount = taxAmount,
                    FinalTotal = finalTotal,
                    ItemCount = cartItems.Sum(x => x.Count)
                });

                // Redirect to payment controller
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred during checkout. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // GET: Cart/CheckoutSummary - Display checkout summary (optional step before payment)
        [HttpGet]
        public async Task<IActionResult> CheckoutSummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = await _cartRepo.GetCartItemsByUserIdAsync(userId);

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty";
                    return RedirectToAction("Index");
                }

                var cartTotal = await _cartRepo.GetCartTotalAsync(userId);
                ViewBag.CartTotal = cartTotal;
                ViewBag.ProcessingFee = 25.00m;
                ViewBag.TaxRate = 0.085m;
                ViewBag.TaxAmount = cartTotal * 0.085m;
                ViewBag.FinalTotal = cartTotal + 25.00m + (cartTotal * 0.085m);

                return View(cartItems);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load checkout summary";
                return RedirectToAction("Index");
            }
        }
    }
}