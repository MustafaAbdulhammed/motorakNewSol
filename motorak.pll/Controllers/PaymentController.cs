// motorak.pll/Controllers/PaymentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using motorak.bll.ModelVM.Payment;
using motorak.bll.Services.Abstractions;
using motorak.dal.Entites;
using motorak.dal.Enums.CartEnums;
using Motorak.BLL.ModelVM.Purchases;
using Motorak.BLL.ModelVM.Rents;
using Motorak.BLL.Services.Abstractions;

namespace motorak.pll.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IPurchaseService _purchaseService;
        private readonly IRentService _rentService;
        private readonly UserManager<User> _userManager;
        private readonly ICustomerService _customerService;

        public PaymentController(
            ICartService cartService,
            IPurchaseService purchaseService,
            IRentService rentService,
            UserManager<User> userManager,
            ICustomerService customerService)
        {
            _cartService = cartService;
            _purchaseService = purchaseService;
            _rentService = rentService;
            _userManager = userManager;
            _customerService = customerService;
        }

        // ✅ Added missing GET Index action
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _cartService.GetCartItemsAsync(userId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Cart");
            }

            var total = await _cartService.GetCartTotalAsync(userId);
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = total;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var username = _userManager.GetUserName(User);

                var (status, message, customer) = await _customerService.GetCustomerByUsernameAsync(username);
                if (!status || customer == null)
                    return Json(new { success = false, message = "Customer not found" });

                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if (!cartItems.Any())
                    return Json(new { success = false, message = "Cart is empty" });

                var paymentResult = await ProcessPaymentSimulation(request);
                if (!paymentResult.Success)
                    return Json(new { success = false, message = paymentResult.Message });

                var successfulTransactions = new List<int>();

                foreach (var item in cartItems)
                {
                    try
                    {
                        if (item.ItemType == CartItemType.Buy)
                        {
                            var purchaseDto = new PurchaseCreateDto
                            {
                                SellerName = "Motorak Store",
                                PaymentMethod = request.PaymentMethod,
                                TotalPrice = item.TotalPrice,
                                CustomerId = customer.Id,
                                CarId = item.CarId
                            };
                            var purchaseId = await _purchaseService.CreateAsync(purchaseDto);
                            successfulTransactions.Add(purchaseId);
                        }
                        else if (item.ItemType == CartItemType.Rent)
                        {
                            var rentDto = new RentCreateDto
                            {
                                StartDate = item.RentStartDate ?? DateTime.Now,
                                EndDate = item.RentEndDate ?? DateTime.Now.AddDays(1),
                                PaymentMethod = request.PaymentMethod,
                                TotalPrice = (int)item.TotalPrice,
                                CustomerId = customer.Id,
                                CarId = item.CarId
                            };
                            var rentId = await _rentService.CreateAsync(rentDto);
                            successfulTransactions.Add(rentId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing item {item.CarId}: {ex.Message}");
                    }
                }

                if (successfulTransactions.Any())
                {
                    await _cartService.ClearCartAsync(userId);
                    return Json(new
                    {
                        success = true,
                        message = "Payment processed successfully",
                        transactionCount = successfulTransactions.Count
                    });
                }

                return Json(new { success = false, message = "No transactions were processed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Payment processing failed: {ex.Message}" });
            }
        }

        private async Task<PaymentResult> ProcessPaymentSimulation(ProcessPaymentRequest request)
        {
            await Task.Delay(500);

            if (string.IsNullOrEmpty(request.PaymentMethod))
                return new PaymentResult { Success = false, Message = "Payment method is required" };

            if (request.PaymentMethod == "CreditCard")
            {
                if (string.IsNullOrEmpty(request.CardNumber) || request.CardNumber.Length < 16)
                    return new PaymentResult { Success = false, Message = "Invalid card number" };

                if (string.IsNullOrEmpty(request.CVV) || request.CVV.Length < 3)
                    return new PaymentResult { Success = false, Message = "Invalid CVV" };
            }

            return new PaymentResult
            {
                Success = true,
                Message = "Payment processed successfully",
                TransactionId = Guid.NewGuid().ToString()
            };
        }
    }
}