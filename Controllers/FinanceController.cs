using FinanceTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrack.Controllers
{
	[Route("finance")]
	public class FinanceController : Controller
	{
		private ApplicationDbContext _context;

		public FinanceController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index(FinanceFilter model)
		{
			var query = from financeNote in _context.FinanceNotes
									join category in _context.Categories on financeNote.CategoryId equals category.Id
									select new FinanceNoteModel
									{
										Id = financeNote.Id,
										ShopName = financeNote.ShopName,
										Price = financeNote.Price,
										DateOfPurchase = financeNote.DateOfPurchase,
										CategoryName = category.Name
									};
			var orderedQuery = query.OrderBy(x => x.DateOfPurchase).ToList();

			if (!string.IsNullOrEmpty(model.CategoryName) && model.DateFrom != default && model.DateTo != default)
			{
				orderedQuery = orderedQuery.Where(x => x.CategoryName == model.CategoryName).Where(x => x.DateOfPurchase >= model.DateFrom).Where(x => x.DateOfPurchase <= model.DateTo).ToList();
			}

			return View(orderedQuery);
		}

		[HttpGet("add-note")]
		public IActionResult AddNote()
		{
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			return View();
		} 

		[HttpPost("add-note")]
		public IActionResult AddNote(FinanceNoteModel model)
		{
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			if (ModelState.IsValid)
			{
				var category = _context.Categories.FirstOrDefault(c => c.Name == model.CategoryName);
				if (category != null)
				{
					var financeNote = new FinanceNote
					{
						ShopName = model.ShopName,
						Price = model.Price,
						DateOfPurchase = DateTime.SpecifyKind(model.DateOfPurchase, DateTimeKind.Utc),
						CategoryId = category.Id
					};
					_context.FinanceNotes.Add(financeNote);
					_context.SaveChanges();
					return RedirectToAction(nameof(Index));
				}
			}
			return View(model);
		}

		[HttpGet("delete-note")]
		public IActionResult DeleteNote(int id)
		{
			var financeNote = _context.FinanceNotes.Find(id);
			if (financeNote != null)
			{
				_context.FinanceNotes.Remove(financeNote);
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet("edit-note")]
		public IActionResult EditNote(int id)
		{
			var financeNote = _context.FinanceNotes.Find(id);
			if (financeNote != null)
			{
				var model = new FinanceNoteModel
				{
					Id = financeNote.Id,
					ShopName = financeNote.ShopName,
					Price = financeNote.Price,
					DateOfPurchase = financeNote.DateOfPurchase
				};
				var financeCategories = _context.Categories.ToList();
				financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
				ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
				return View(model);
			}
			return NotFound("Note not found");
		}

		[HttpPost("edit-note")]
		public IActionResult EditNote(FinanceNoteModel model)
		{
			if (ModelState.IsValid)
			{
				var category = _context.Categories.FirstOrDefault(c => c.Name == model.CategoryName);
				if (category != null)
				{
					var financeNote = _context.FinanceNotes.Find(model.Id);
					if (financeNote != null)
					{
						financeNote.ShopName = model.ShopName;
						financeNote.Price = model.Price;
						financeNote.DateOfPurchase = DateTime.SpecifyKind(model.DateOfPurchase, DateTimeKind.Utc);
						financeNote.CategoryId = category.Id;
						_context.FinanceNotes.Update(financeNote);
						_context.SaveChanges();
						return RedirectToAction(nameof(Index));
					}
				}
			}
			return View(model);
		}

		[HttpGet("add-category")]
		public IActionResult AddCategory() => View();

		[HttpPost("add-category")]
		public IActionResult AddCategory(Category model)
		{
			if (ModelState.IsValid)
			{
				_context.Categories.Add(model);
				_context.SaveChanges();
				return RedirectToAction(nameof(AddNote));
			}
			return View(model);
		}

		[HttpGet("edit-category")]
		public IActionResult EditCategory(){
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			return View();
		} 

		[HttpPost("edit-category")]
		public IActionResult EditCategory(Category model, string NewName)
		{
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			if (ModelState.IsValid)
			{
				var notes = _context.FinanceNotes.ToList();
				var categoryToEdit = _context.Categories.FirstOrDefault(c => c.Name == model.Name);
				if (categoryToEdit != null)
				{
					categoryToEdit.Name = NewName;
					_context.Categories.Update(categoryToEdit);

					foreach (var note in notes)
					{
						if (note.CategoryId == categoryToEdit.Id)
						{
							note.CategoryId = categoryToEdit.Id;
							_context.FinanceNotes.Update(note);
						}
					}

					_context.SaveChanges();
					return RedirectToAction(nameof(AddNote));
				}
			}
			return View(model);
		}

		[HttpGet("delete-category")]
		public IActionResult DeleteCategory()
		{
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			return View();
		}

		[HttpPost("delete-category")]
		public IActionResult DeleteCategory(Category model)
		{
			if (ModelState.IsValid)
			{
				var categoryToDelete = _context.Categories.FirstOrDefault(c => c.Name == model.Name);
				if (categoryToDelete != null)
				{
					_context.Categories.Remove(categoryToDelete);
					_context.SaveChanges();
					return RedirectToAction(nameof(AddNote));
				}
			}
			return View(model);
		}

		[HttpGet("filter")]
		public IActionResult Filter()
		{
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			return View();
		}

		[HttpPost("filter")]
		public IActionResult Filter(FinanceFilter model)
		{
			if (model.DateFrom == default)
			{
				model.DateFrom = DateTime.Today;
			}
			if (model.DateTo == default)
			{
				model.DateTo = DateTime.Today;
			}
			var financeCategories = _context.Categories.ToList();
			financeCategories.Sort((x, y) => x.Id.CompareTo(y.Id));
			ViewBag.FinanceCategories = new SelectList(financeCategories, "Name", "Name");
			return RedirectToAction(nameof(Index), model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
	}
}