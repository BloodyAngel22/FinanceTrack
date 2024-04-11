namespace FinanceTrack.Models
{
	public class FinanceNote
	{
		public int Id { get; set; }
		public string ShopName { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public DateTime DateOfPurchase { get; set; }
		public int CategoryId { get; set; }
	}
}