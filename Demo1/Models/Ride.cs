namespace SmartRide.Models
{
    /// <summary>
    /// Represents a customer entity with identification and ride history.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string Name { get; set; } = string.Empty; // Added Name property

        /// <summary>
        /// Gets or sets the list of ride history for the customer.
        /// </summary>
        public List<Ride> RideHistory { get; set; } = new List<Ride>(); // Added RideHistory property
    }

    /// <summary>
    /// Represents a driver entity with availability and location details.
    /// </summary>
    public class Driver
    {
        /// <summary>
        /// Gets or sets the unique identifier for the driver.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the driver.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the availability status of the driver.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the current location of the driver.
        /// </summary>
        public string CurrentLocation { get; set; }

        /// <summary>
        /// Gets or sets the current ride assigned to the driver.
        /// </summary>
        public Ride CurrentRide { get; set; }
    }

    /// <summary>
    /// Represents a ride entity with details such as locations and status.
    /// </summary>
    public class Ride
    {
        /// <summary>
        /// Gets or sets the unique identifier for the ride.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the identifier of the customer associated with the ride.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the driver assigned to the ride.
        /// </summary>
        public string DriverId { get; set; }

        /// <summary>
        /// Gets or sets the pickup location of the ride.
        /// </summary>
        public string PickupLocation { get; set; }

        /// <summary>
        /// Gets or sets the drop-off location of the ride.
        /// </summary>
        public string DropoffLocation { get; set; }

        /// <summary>
        /// Gets or sets the current status of the ride.
        /// </summary>
        public string Status { get; set; } = "Requested";

        /// <summary>
        /// Gets or sets the estimated time of arrival in minutes.
        /// </summary>
        public double ETA { get; set; }
    }

    /// <summary>
    /// View model for transferring ride-related data between controller and view.
    /// </summary>
    public class RideViewModel
    {
        /// <summary>
        /// Gets or sets the pickup location entered by the user.
        /// </summary>
        public string PickupLocation { get; set; }

        /// <summary>
        /// Gets or sets the drop-off location entered by the user.
        /// </summary>
        public string DropoffLocation { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the ride.
        /// </summary>
        public string RideId { get; set; }

        /// <summary>
        /// Gets or sets the current location of the driver.
        /// </summary>
        public string DriverLocation { get; set; }

        /// <summary>
        /// Gets or sets the estimated time of arrival in minutes.
        /// </summary>
        public double ETA { get; set; }
    }
}