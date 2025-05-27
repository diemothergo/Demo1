using Demo1.Models;

namespace Demo1.Models
{
    public class Ride
    {
        public string Id { get; set; }
        public Customer Customer { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public string Status { get; set; }
        public int ETA { get; set; }
        public string DriverId { get; set; }
    }
}