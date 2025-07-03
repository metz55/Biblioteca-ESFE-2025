using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;



namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class BooksController : Controller
    {
        BLBooks booksBL = new BLBooks();
        BLCategories categoriesBL = new BLCategories();
        BLAcquisitionTypes acquisitionTypesBL = new BLAcquisitionTypes();
        BLEditorials editorialsBL = new BLEditorials();
        BLAuthors authorsBL = new BLAuthors();
        BLEditions editionsBL = new BLEditions();
        BLCountries countriesBL = new BLCountries();
        BLCatalogs catalogsBL = new BLCatalogs(); 

        public async Task<IActionResult> Index(Books pBooks = null)
        {
            if (pBooks == null)
                pBooks = new Books();
            if (pBooks.Top_Aux == 0)
                pBooks.Top_Aux = 10;
            else if (pBooks.Top_Aux == -1)
                pBooks.Top_Aux = 0;


            var books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();

            ViewBag.Top = pBooks.Top_Aux;
            ViewBag.ShowMenu = true;
           
            return View(books);
        }

        // GET: BooksController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Falta terminar 
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
            //Objeto anonimo
            books.Categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
            books.AcquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = books.ID_ACQUISITION });
            books.Editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            books.Authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });
            books.Editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });
            books.Countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = books.ID_COUNTRY });
            books.Catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = books.ID_CATALOG });

            ViewBag.ShowMenu = true;
            return View(books);
        }

        // GET: BooksController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Books pBooks, IFormFile coverImage)
        {
            try
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    // Lógica para guardar la imagen en el sistema de archivos y obtener la ruta
                    string imagePath = SaveCoverImage(coverImage);

                    // Almacena la ruta en la propiedad de la entidad
                    pBooks.CoverImagePath = imagePath;
                }
                else
                {
                    pBooks.CoverImagePath = "SinPortada";
                }

                int result = await booksBL.CrearAsync(pBooks);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Maneja los errores...
                ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
                ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
                ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
                ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
                ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
                ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
                ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private string SaveCoverImage(IFormFile coverImage)
        {
            // Lógica para guardar la imagen y obtener la ruta
            var uploadDir = Path.Combine("wwwroot", "img");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                coverImage.CopyTo(fileStream);
            }

            return Path.Combine("/img", uniqueFileName);
        }



        // GET: BooksController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
            var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            var autor = await authorsBL.GetAuthorsByIdAsync(new Authors {AUTHOR_ID = books.ID_AUTHOR});
            var pais = await countriesBL.GetCountriesByIdAsync(new Countries {COUNTRY_ID = books.ID_COUNTRY});
            var edicion = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });

            ViewBag.EditorialName = editorial.EDITORIAL_NAME;
            ViewBag.AutorName = autor.AUTHOR_NAME;
            ViewBag.PaisName = pais.COUNTRY_NAME;
            ViewBag.EdicionName = edicion.EDITION_NUMBER;

          


            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();


            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(books);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Books pBooks, IFormFile newcoverImage)
        {
            try
            {
                if(newcoverImage != null && newcoverImage.Length > 0)
                {
                    // guardar la imagen en el sistema de archivos y obtener la ruta
                    string imagePath = SaveCoverImage(newcoverImage);

                    // Almacena la ruta en la propiedad de la entidad
                    pBooks.CoverImagePath = imagePath;
                }

                int result = await booksBL.UpdateBooksAsync(pBooks);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pBooks);
            }
        }


        // GET: BooksController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

            books.Categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });

            books.AcquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = books.ID_ACQUISITION });

            books.Editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });

            books.Authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

            books.Editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = books.ID_EDITION });

            books.Countries = await countriesBL.GetCountriesByIdAsync(new Countries { COUNTRY_ID = books.ID_COUNTRY });

            books.Catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = books.ID_CATALOG });

            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(books);
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Books pBooks)
        {
            try
            {
                int result = await booksBL.DeleteBooksAsync(pBooks);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pBooks);
            }
        }
        public async Task<JsonResult> BuscarAutor(Authors pAuthors)
        {
            List<Authors> lista = await authorsBL.GetAuthorsAsync(pAuthors);

            return Json(lista);
        }
        public async Task<JsonResult> BuscarCiudad(Countries pCountries)
        {
            List<Countries> lista = await countriesBL.GetCountriesAsync(pCountries);

            return Json(lista);
        }
        public async Task<JsonResult> BuscarEdicion(Editions pEditions)
        {
            List<Editions> lista = await editionsBL.GetEditionsAsync(pEditions);

            return Json(lista);
        }
        public async Task<JsonResult> BuscarEditorial(Editorials pEditorials)
        {
            List<Editorials> lista = await editorialsBL.GetEditorialsAsync(pEditorials);

            return Json(lista);
        }
    }
}
