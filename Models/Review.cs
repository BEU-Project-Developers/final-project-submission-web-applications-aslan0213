namespace HotelManagementSystem.Models
{
	public class Review
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string Comment { get; set; }
		public int Rating { get; set; }
		public DateTime ReviewDate { get; set; }
		public User User { get; set; }
		public Room Room { get; set; }
	}
}
