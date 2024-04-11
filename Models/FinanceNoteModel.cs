using System.ComponentModel.DataAnnotations;

namespace FinanceTrack.Models
{
	public class FinanceNoteModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Введите название магазина, где была совершена покупка")]
		[StringLength(70)]
		public string ShopName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Укажите дату покупки")]
		[DataType(DataType.Date)]
		public DateTime DateOfPurchase { get; set; }

		[Required(ErrorMessage = "Выберите категорию")]
		public string CategoryName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Укажите цену")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Цена не может быть меньше нуля")]
		public decimal Price { get; set; }
	}
}