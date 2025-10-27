using Library.Client.MVC.services;
using Library.DataAccess.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json; 


namespace Library.Client.MVC.Controllers
{
    public class BooksImportController : Controller
    {
        private readonly DBContext _db;
        private readonly BookImportService _importService;

        public BooksImportController(DBContext db)
        {
            _db = db;
            _importService = new BookImportService(db);
        }

        // --- otras acciones como Index, Create, etc. ---

        // GET: Books/ImportBooks
        public IActionResult ImportBooks()
        {
            return View("~/Views/BookImport/ImportBooks.cshtml");
        }

        // POST: Books/ImportBooks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportBooks(IFormFile excelFile, bool actualizarExistentes = true, bool crearCatalogosPorNombre = true)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Por favor seleccione un archivo Excel.";
                return RedirectToAction(nameof(ImportBooks));
            }

            using var stream = excelFile.OpenReadStream();
            var result = await _importService.ImportarAsync(stream, actualizarExistentes, crearCatalogosPorNombre);

            TempData["Creados"] = result.Creados;
            TempData["Actualizados"] = result.Actualizados;

            //serializa los errores antes de guardarlos
            TempData["Errores"] = result.Errores.Count > 0
                ? JsonSerializer.Serialize(result.Errores)
                : null;

            return RedirectToAction(nameof(ImportBooks));
        }

    }
}
