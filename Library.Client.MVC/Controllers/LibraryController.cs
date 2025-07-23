using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Library.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    public class LibraryController : Controller
    {
        BLBooks booksBL = new BLBooks();
        DALBooks booksDAL = new DALBooks();
        BLCategories categoriesBL = new BLCategories();
        BLAcquisitionTypes acquisitionTypesBL = new BLAcquisitionTypes();
        BLEditorials editorialsBL = new BLEditorials();
        BLAuthors authorsBL = new BLAuthors();
        BLEditions editionsBL = new BLEditions();
        BLCountries countriesBL = new BLCountries();
        BLCatalogs catalogsBL = new BLCatalogs();
        BLLoanTypes loanTypesBL = new BLLoanTypes();
        BLLoans loansBL = new BLLoans();

        Books booksEN = new Books();
        public async Task<IActionResult> Index(Books pBooks = null)
        {
            if (pBooks == null)
                pBooks = new Books();
            if (pBooks.Top_Aux == 0)
                pBooks.Top_Aux = 12;
            else if (pBooks.Top_Aux == -1)
                pBooks.Top_Aux = 0;

            var books = await booksBL.GetIncludePropertiesAsync(pBooks);

            ViewBag.books = await booksBL.GetAllBooksAsync();
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.AcquisitionTypes = await acquisitionTypesBL.GetAllAcquisitionTypesAsync();
            ViewBag.Editorials = await editorialsBL.GetAllEditorialsAsync();
            ViewBag.Authors = await authorsBL.GetAlAuthorsAsync();
            ViewBag.Editions = await editionsBL.GetAllEditionsAsync();
            ViewBag.Countries = await countriesBL.GetAllCountriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Top = pBooks.Top_Aux;
            return View(books);
        }


        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> CreateLoansSt(int id)
        {
            var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

            var reservations = new List<int> { 1, 3, 4 };
            var cantidadPrestamos = reservations
                .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_BOOK = id, ID_RESERVATION = reservation, STATUS = true }))
                .Select(task => task.Result.Count)
                .Sum();

            if (cantidadPrestamos < books.EJEMPLARS && books.EXISTENCES > 1)
            {
                ViewBag.AlertaLibro = "Disponible para Reservación";
            }
            else
            {
                ViewBag.AlertaLibro2 = "No hay suficientes ejemplares para realizar la reservación";
            }

            ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();

            var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
            var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
            var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

            ViewBag.Editorial = editorial.EDITORIAL_NAME;
            ViewBag.Categoria = categoria.CATEGORY_NAME;
            ViewBag.Autor = authors.AUTHOR_NAME;
            ViewBag.Imagen = books.COVER;
            ViewBag.Titulo = books.TITLE;
            ViewBag.Year = books.YEAR;

            return View(books);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLoansSt(int id, Books pBooks, string telefono, int tipoPrestamo, Loans pLoans, int result, Books2 pBooks2, long idLender)
        {
            try
            {
                var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });

                var reservations = new List<int> { 1, 3, 4 };
                var cantidadPrestamos = reservations
                    .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_BOOK = id, ID_RESERVATION = reservation, STATUS = true }))
                    .Select(task => task.Result.Count)
                    .Sum();

                if (telefono != null && tipoPrestamo > 0)
                {
                    var cantidadPrestamosPorEst = reservations
                       .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_LENDER = idLender, ID_RESERVATION = reservation, STATUS = true }))
                       .Select(task => task.Result.Count)
                       .Sum();

                    if (cantidadPrestamosPorEst < 1)
                    {
                        // Cambié aquí EXISTENCES > 1 a EXISTENCES > 0
                        if (cantidadPrestamos < books.EJEMPLARS && books.EXISTENCES > 0)
                        {
                            pLoans.LENDER_CONTACT = telefono;
                            pLoans.ID_BOOK = id;
                            pLoans.ID_LENDER = idLender;
                            pLoans.ID_TYPE = tipoPrestamo;
                            pLoans.STATUS = true;
                            pLoans.ID_RESERVATION = 3;
                            pLoans.USER_ID = 1;
                            pLoans.COPY = 1;
                            pLoans.FEE = 0;
                            pLoans.REGISTRATION_DATE = DateTime.Now;
                            result = await loansBL.CreateLoansAsync(pLoans);

                            pBooks2.BOOK_ID = pLoans.ID_BOOK;
                            pBooks2.EXISTENCES = books.EXISTENCES - 1;
                            int resultUpdate = await booksBL.UpdateExistencesBooksAsync(pBooks2);
                        }
                        else
                        {
                            ViewBag.Alerta2 = "No hay suficientes ejemplares para realizar el registro";
                        }
                    }
                    else
                    {
                        ViewBag.AlertaPrestamoEx = "No se puede realizar el registro porque ya cuenta con un préstamo o reservación pendiente o activo";
                    }
                }
                else
                {
                    ViewBag.Alerta = "Por favor ingrese los datos del Préstamo";
                }

                // Cargar datos comunes para vista en cualquier caso
                ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();
                var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
                var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
                var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

                ViewBag.Editorial = editorial.EDITORIAL_NAME;
                ViewBag.Categoria = categoria.CATEGORY_NAME;
                ViewBag.Autor = authors.AUTHOR_NAME;
                ViewBag.Imagen = books.COVER;
                ViewBag.Titulo = books.TITLE;
                ViewBag.Year = books.YEAR;

                if (result > 0)
                {
                    TempData["Alerta"] = "La reservación se registró exitosamente!!";
                }

                return View();
            }
            catch (Exception ex)
            {
                var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = id });
                ViewBag.LoanTypes = await loanTypesBL.GetAllLoanTypesAsync();
                var editorial = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = books.ID_EDITORIAL });
                var categoria = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = books.ID_CATEGORY });
                var authors = await authorsBL.GetAuthorsByIdAsync(new Authors { AUTHOR_ID = books.ID_AUTHOR });

                ViewBag.Editorial = editorial.EDITORIAL_NAME;
                ViewBag.Categoria = categoria.CATEGORY_NAME;
                ViewBag.Autor = authors.AUTHOR_NAME;
                ViewBag.Imagen = books.COVER;
                ViewBag.Titulo = books.TITLE;
                ViewBag.Year = books.YEAR;
                ViewBag.Alerta = "Por favor ingrese los datos del Préstamo";
                ViewBag.Error = ex.Message;

                return View(pBooks);
            }
        }


    }
}
