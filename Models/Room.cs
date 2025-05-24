using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace HotelManagementSystem.Models
{
	public class Room
	{
		public int Id { get; set; }
		public string RoomNumber { get; set; }
		public string Type { get; set; } 
		public decimal Price { get; set; }
		public bool Status { get; set; }
		public int HotelId { get; set; }
		public string Image { get; set; }
		public int Capacity { get; set; } 
		public List<string> Amenities { get; set; }
		public List<Booking> Bookings { get; set; }
		public List<Review> Reviews { get; set; }
	}
}
