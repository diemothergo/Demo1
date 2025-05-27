namespace Demo1.Models
{
    public class RideViewModel
    {
        public string RideId { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public string DriverLocation { get; set; }
        public int ETA { get; set; }
    }
}