using Demo1.Models;

namespace Demo1.Models
{
    public class Ride
    {
        public required string Id { get; set; }
        public required Customer Customer { get; set; }
        public required string PickupLocation { get; set; }
        public required string DropoffLocation { get; set; }
        public RideStatus Status { get; set; }
        public int ETA { get; set; }
        public required string DriverId { get; set; }
    }
}