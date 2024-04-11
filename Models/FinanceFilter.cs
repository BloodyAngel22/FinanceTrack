namespace FinanceTrack.Models
{
	public class FinanceFilter
	{
		public string CategoryName { get; set; } = string.Empty;
		public DateTime DateFrom{ get; set; }
		public DateTime DateTo { get; set; }
	}
}