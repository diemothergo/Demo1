using Microsoft.AspNetCore.Mvc;
using Demo1.Models;
using Demo1.Services;

namespace Demo1.Controllers
{
    public class RideController : Controller
    {
        private readonly BookingManager _bookingManager;
        private readonly PaymentSimulator _paymentSimulator;
        private readonly LocationTracker _locationTracker;

        public RideController(BookingManager bookingManager, PaymentSimulator paymentSimulator, LocationTracker locationTracker)
        {
            _bookingManager = bookingManager ?? throw new ArgumentNullException(nameof(bookingManager));
            _paymentSimulator = paymentSimulator ?? throw new ArgumentNullException(nameof(paymentSimulator));
            _locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));
        }

        public IActionResult Index()
        {
            return View(new RideViewModel());
        }

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
                TempData["RideId"] = ride.Id;
                return RedirectToAction("Track", new { id = model.RideId });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đặt chuyến đi. Vui lòng thử lại.");
                return View("Index", model);
            }
        }

        public IActionResult Track(string id)
        {
            var ride = _bookingManager.GetRide(id);
            if (ride == null) return NotFound("Không tìm thấy chuyến đi với ID đã cung cấp.");

            var model = new RideViewModel
            {
                RideId = ride.Id,
                DriverLocation = _locationTracker.GetDriverLocation(_bookingManager.GetDriver(ride.DriverId)),
                ETA = ride.ETA,
                DropoffLocation = ride.DropoffLocation
            };
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            return View(model);
        }

        [HttpPost]
        public IActionResult Cancel(string id)
        {
            try
            {
                var ride = _bookingManager.GetRide(id);
                if (ride == null) return NotFound("Không tìm thấy chuyến đi với ID đã cung cấp.");

                _bookingManager.CancelRide(id);
                TempData["SuccessMessage"] = "Chuyến đi đã được hủy thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi hủy chuyến đi. Vui lòng thử lại.";
                return RedirectToAction("Track", new { id });
            }
        }

        [HttpPost]
        public IActionResult Complete(string id)
        {
            try
            {
                var ride = _bookingManager.GetRide(id);
                if (ride == null || ride.Status != RideStatus.Booked) return NotFound("Chuyến đi không hợp lệ hoặc đã hoàn tất.");

                _bookingManager.CompleteRide(id);
                ViewBag.Message = _paymentSimulator.ProcessPayment(ride);
                ViewBag.SuccessMessage = "Chuyến đi đã hoàn tất! Cảm ơn quý khách đã sử dụng Demo1.";
                return View("Completion");
            }
            catch (Exception)
            {
                return NotFound("Đã xảy ra lỗi khi hoàn tất chuyến đi.");
            }
        }

        public IActionResult History()
        {
            var rides = _bookingManager.GetAllRides();
            var model = new RideHistoryViewModel { Rides = rides };
            return View(model);
        }
    }
}