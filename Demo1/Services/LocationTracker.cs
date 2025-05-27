using Demo1.Models;

namespace Demo1.Services
{
    public class LocationTracker
    {
        public string GetDriverLocation(Driver driver)
        {
            return driver.Location ?? "29H-123.45";
        }
    }
}