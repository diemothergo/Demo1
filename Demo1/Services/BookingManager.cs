using Demo1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo1.Services
{
    public class BookingManager
    {
        private readonly List<Ride> _rides = new();
        private readonly LocationTracker _locationTracker;

        public BookingManager(LocationTracker locationTracker)
        {
            _locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));
        }

        public Ride BookRide(Customer customer, string pickupLocation, string dropoffLocation)
        {
            var ride = new Ride
            {
                Id = Guid.NewGuid().ToString(),
                Customer = customer,
                PickupLocation = pickupLocation,
                DropoffLocation = dropoffLocation,
                Status = RideStatus.Booked,
                ETA = 15,
                DriverId = Guid.NewGuid().ToString()
            };
            _rides.Add(ride);
            return ride;
        }

        public Ride? GetRide(string id) => _rides.FirstOrDefault(r => r.Id == id);

        public List<Ride> GetAllRides() => _rides;

        public Driver GetDriver(string driverId)
        {
            return new Driver { Id = driverId, Location = "29H-123.45" };
        }

        public void CompleteRide(string id)
        {
            var ride = GetRide(id);
            if (ride != null) ride.Status = RideStatus.Completed;
        }

        public void CancelRide(string id)
        {
            var ride = GetRide(id);
            if (ride != null)
            {
                _rides.Remove(ride);
                ride.Status = RideStatus.Cancelled;
            }
        }
    }
}
