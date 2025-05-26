using SmartRide.Models;
using System.Text.Json;

namespace SmartRide.Services
{
    /// <summary>
    /// Manages the booking and lifecycle of rides within the SmartRide system.
    /// </summary>
    public class BookingManager
    {
        private readonly List<Driver> _drivers;
        private readonly List<Ride> _rides;
        private readonly LocationTracker _locationTracker;
        private readonly DataRepository _dataRepository;

        /// <summary>
        /// Initializes a new instance of the BookingManager with required dependencies.
        /// </summary>
        /// <param name="tracker">The location tracker service.</param>
        /// <param name="repository">The data repository service.</param>
        /// <exception cref="ArgumentNullException">Thrown when tracker or repository is null.</exception>
        public BookingManager(LocationTracker tracker, DataRepository repository)
        {
            _locationTracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _dataRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _drivers = _dataRepository.LoadDrivers();
            _rides = _dataRepository.LoadRides();
            if (!_drivers.Any())
            {
                _drivers.Add(new Driver { Name = "Nguyễn Văn A", CurrentLocation = "Trung tâm TP. Hà Nội" });
                _dataRepository.SaveDrivers(_drivers);
            }
        }

        /// <summary>
        /// Books a new ride for the specified customer with given locations.
        /// </summary>
        /// <param name="customer">The customer requesting the ride.</param>
        /// <param name="pickupLocation">The pickup location.</param>
        /// <param name="dropoffLocation">The drop-off location.</param>
        /// <returns>The newly created ride object.</returns>
        /// <exception cref="ArgumentException">Thrown when locations are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no drivers are available.</exception>
        public Ride BookRide(Customer customer, string pickupLocation, string dropoffLocation)
        {
            if (string.IsNullOrWhiteSpace(pickupLocation) || string.IsNullOrWhiteSpace(dropoffLocation))
                throw new ArgumentException("Cả hai địa điểm đón và trả đều phải được nhập.");

            var ride = new Ride
            {
                CustomerId = customer.Id,
                PickupLocation = pickupLocation,
                DropoffLocation = dropoffLocation
            };

            var driver = AssignDriver(pickupLocation);
            if (driver == null) throw new InvalidOperationException("Không có tài xế nào khả dụng.");

            ride.DriverId = driver.Id;
            ride.Status = "Đã phân công";
            ride.ETA = _locationTracker.CalculateETA(driver.CurrentLocation, pickupLocation);
            driver.IsAvailable = false;
            driver.CurrentRide = ride;

            _rides.Add(ride);
            customer.RideHistory.Add(ride);
            _dataRepository.SaveRides(_rides);
            return ride;
        }

        /// <summary>
        /// Assigns an available driver based on proximity to the pickup location.
        /// </summary>
        /// <param name="pickupLocation">The pickup location to match.</param>
        /// <returns>The assigned driver or null if none available.</returns>
        private Driver AssignDriver(string pickupLocation)
        {
            return _drivers.FirstOrDefault(d => d.IsAvailable);
        }

        /// <summary>
        /// Completes the specified ride and updates driver availability.
        /// </summary>
        /// <param name="rideId">The unique identifier of the ride to complete.</param>
        /// <exception cref="ArgumentException">Thrown when the ride is not found.</exception>
        public void CompleteRide(string rideId)
        {
            var ride = _rides.FirstOrDefault(r => r.Id == rideId);
            if (ride == null) throw new ArgumentException("Không tìm thấy chuyến đi.");

            ride.Status = "Hoàn tất";
            var driver = _drivers.FirstOrDefault(d => d.Id == ride.DriverId);
            if (driver != null)
            {
                driver.IsAvailable = true;
                driver.CurrentRide = null;
            }
            _dataRepository.SaveRides(_rides);
            _dataRepository.SaveDrivers(_drivers);
        }

        /// <summary>
        /// Retrieves a ride by its unique identifier.
        /// </summary>
        /// <param name="rideId">The unique identifier of the ride.</param>
        /// <returns>The ride object or null if not found.</returns>
        public Ride GetRide(string rideId) => _rides.FirstOrDefault(r => r.Id == rideId);

        /// <summary>
        /// Retrieves a driver by its unique identifier.
        /// </summary>
        /// <param name="driverId">The unique identifier of the driver.</param>
        /// <returns>The driver object or null if not found.</returns>
        public Driver GetDriver(string driverId) => _drivers.FirstOrDefault(d => d.Id == driverId);
    }

    /// <summary>
    /// Simulates location tracking and ETA calculations.
    /// </summary>
    public class LocationTracker
    {
        /// <summary>
        /// Calculates the estimated time of arrival based on locations.
        /// </summary>
        /// <param name="driverLocation">The current location of the driver.</param>
        /// <param name="pickupLocation">The pickup location.</param>
        /// <returns>The ETA in minutes (simulated as a constant).</returns>
        public double CalculateETA(string driverLocation, string pickupLocation) => 10.0;

        /// <summary>
        /// Retrieves the current location of a driver.
        /// </summary>
        /// <param name="driver">The driver object.</param>
        /// <returns>The driver's location or "Không xác định" if null.</returns>
        public string GetDriverLocation(Driver driver) => driver?.CurrentLocation ?? "Không xác định";
    }

    /// <summary>
    /// Simulates payment processing with a confirmation message.
    /// </summary>
    public class PaymentSimulator
    {
        /// <summary>
        /// Processes the payment for a completed ride.
        /// </summary>
        /// <param name="ride">The ride object to process payment for.</param>
        /// <returns>A confirmation message.</returns>
        public string ProcessPayment(Ride ride) => ride == null ? "Chuyến đi không hợp lệ" : $"Thanh toán cho chuyến đi {ride.Id} đã được xử lý thành công!";
    }

    /// <summary>
    /// Manages persistent storage of drivers and rides using JSON files.
    /// </summary>
    public class DataRepository
    {
        private readonly string _driversFile = "drivers.json";
        private readonly string _ridesFile = "rides.json";

        /// <summary>
        /// Loads the list of drivers from a JSON file.
        /// </summary>
        /// <returns>A list of driver objects.</returns>
        public List<Driver> LoadDrivers()
        {
            if (!File.Exists(_driversFile)) return new List<Driver>();
            var json = File.ReadAllText(_driversFile);
            return JsonSerializer.Deserialize<List<Driver>>(json) ?? new List<Driver>();
        }

        /// <summary>
        /// Loads the list of rides from a JSON file.
        /// </summary>
        /// <returns>A list of ride objects.</returns>
        public List<Ride> LoadRides()
        {
            if (!File.Exists(_ridesFile)) return new List<Ride>();
            var json = File.ReadAllText(_ridesFile);
            return JsonSerializer.Deserialize<List<Ride>>(json) ?? new List<Ride>();
        }

        /// <summary>
        /// Saves the list of drivers to a JSON file.
        /// </summary>
        /// <param name="drivers">The list of drivers to save.</param>
        public void SaveDrivers(List<Driver> drivers)
        {
            File.WriteAllText(_driversFile, JsonSerializer.Serialize(drivers, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Saves the list of rides to a JSON file.
        /// </summary>
        /// <param name="rides">The list of rides to save.</param>
        public void SaveRides(List<Ride> rides)
        {
            File.WriteAllText(_ridesFile, JsonSerializer.Serialize(rides, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}