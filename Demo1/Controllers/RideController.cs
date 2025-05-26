using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.Services;

namespace SmartRide.Controllers
{
    /// <summary>
    /// Controller for managing ride-related operations in the SmartRide application.
    /// </summary>
    public class RideController : Controller
    {
        private readonly BookingManager _bookingManager;
        private readonly PaymentSimulator _paymentSimulator;
        private readonly LocationTracker _locationTracker;

        /// <summary>
        /// Initializes a new instance of the RideController with required services.
        /// </summary>
        /// <param name="bookingManager">The booking manager service.</param>
        /// <param name="paymentSimulator">The payment simulator service.</param>
        /// <param name="locationTracker">The location tracker service.</param>
        public RideController(BookingManager bookingManager, PaymentSimulator paymentSimulator, LocationTracker locationTracker)
        {
            _bookingManager = bookingManager ?? throw new ArgumentNullException(nameof(bookingManager));
            _paymentSimulator = paymentSimulator ?? throw new ArgumentNullException(nameof(paymentSimulator));
            _locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));
        }

        /// <summary>
        /// Displays the ride booking form.
        /// </summary>
        /// <returns>The view with an empty RideViewModel.</returns>
        public IActionResult Index()
        {
            return View(new RideViewModel());
        }

        /// <summary>
        /// Handles the submission of a new ride booking request.
        /// </summary>
        /// <param name="model">The view model containing pickup and drop-off locations.</param>
        /// <returns>Redirects to the tracking page on success, or returns the form with errors.</returns>
        [HttpPost]
        public IActionResult Book(RideViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.PickupLocation) || string.IsNullOrWhiteSpace(model.DropoffLocation))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin địa điểm đón và trả.");
                    return View("Index", model);
                }

                var customer = new Customer { Name = "Người dùng tạm thời" };
                var ride = _bookingManager.BookRide(customer, model.PickupLocation, model.DropoffLocation);
                model.RideId = ride.Id;
                model.ETA = ride.ETA;
                model.DriverLocation = _locationTracker.GetDriverLocation(_bookingManager.GetDriver(ride.DriverId));
                TempData["SuccessMessage"] = "Chuyến đi đã được đặt thành công! Vui lòng theo dõi trạng thái.";
                return RedirectToAction("Track", new { id = model.RideId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đặt chuyến đi. Vui lòng thử lại.");
                return View("Index", model);
            }
        }

        /// <summary>
        /// Displays the tracking information for a specific ride.
        /// </summary>
        /// <param name="id">The unique identifier of the ride.</param>
        /// <returns>The tracking view with ride details, or a 404 if not found.</returns>
        public IActionResult Track(string id)
        {
            var ride = _bookingManager.GetRide(id);
            if (ride == null) return NotFound("Không tìm thấy chuyến đi với ID đã cung cấp.");

            var model = new RideViewModel
            {
                RideId = ride.Id,
                DriverLocation = _locationTracker.GetDriverLocation(_bookingManager.GetDriver(ride.DriverId)),
                ETA = ride.ETA
            };
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            return View(model);
        }

        /// <summary>
        /// Handles the completion of a ride.
        /// </summary>
        /// <param name="id">The unique identifier of the ride to complete.</param>
        /// <returns>The completion view with payment confirmation, or a 404 if invalid.</returns>
        [HttpPost]
        public IActionResult Complete(string id)
        {
            try
            {
                var ride = _bookingManager.GetRide(id);
                if (ride == null || ride.Status != "Đã phân công") return NotFound("Chuyến đi không hợp lệ hoặc đã hoàn tất.");

                _bookingManager.CompleteRide(id);
                ViewBag.Message = _paymentSimulator.ProcessPayment(ride);
                ViewBag.SuccessMessage = "Chuyến đi đã hoàn tất! Cảm ơn quý khách đã sử dụng SmartRide.";
                return View("Completion");
            }
            catch (Exception ex)
            {
                return NotFound("Đã xảy ra lỗi khi hoàn tất chuyến đi.");
            }
        }
    }
}