using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.Client.MVC.services;
using Library.BusinessRules;
using Library.Client.MVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class LoansController : Controller
    {
        BLLoans loansBL = new BLLoans();
        BLLoanTypes loansTypesBL = new BLLoanTypes();
        BLReservationStatus reservationStatusBL = new BLReservationStatus();
        BLBooks booksBL = new BLBooks();
        BLCategories categoriesBL = new BLCategories();
        BLLoanDates loanDatesBL = new BLLoanDates();
        private readonly LoanService _loanService;

        public LoansController(LoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IActionResult> Index(Books pBooks, Loans pLoans = null, string studentCode = "")
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 10;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            // Si se ingresó un código de estudiante, buscar su ID_LENDER
            if (!string.IsNullOrEmpty(studentCode))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{studentCode}");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            pLoans.ID_LENDER = student.Id; // Asigna el ID del estudiante encontrado
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejo de error (opcional: mostrar mensaje en ViewBag)
                        ViewBag.Error = "No se pudo buscar el estudiante: " + ex.Message;
                    }
                }
            }

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);

            ViewBag.Top = pLoans.Top_Aux;
            ViewBag.ShowMenu = true;
            ViewData["studentCode"] = studentCode; // Para que el campo no se borre al recargar
            return View(loans);
        }

        // Resto de los métodos del controlador se mantienen igual
        public async Task<IActionResult> Status2Loans(Books pBooks, Loans pLoans = null)
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 10;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);

            ViewBag.Top = pLoans.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        public async Task<IActionResult> LoansDelite(Books pBooks, Loans pLoans = null, string studentCode = "")
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 20;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            // Si se ingresó un código de estudiante, buscar su ID_LENDER
            if (!string.IsNullOrEmpty(studentCode))
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{studentCode}");
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            pLoans.ID_LENDER = student.Id; // Asigna el ID del estudiante encontrado
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "No se pudo buscar el estudiante: " + ex.Message;
                    }
                }
            }

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);

            ViewBag.Top = pLoans.Top_Aux;
            ViewData["studentCode"] = studentCode; // Para que el campo no se borre al recargar
            return View(loans);
        }


        // GET: BooksController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            loans.LoanTypes = await loansTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = loans.ID_TYPE });
            loans.ReservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = loans.ID_RESERVATION });
            loans.Books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        // GET: BooksController/Create
        public async Task<IActionResult> Create(Books pBooks)
        {
            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loans pLoans, LoanDates pLoanDates, DateTime fechaInicio, DateTime fechaCierre, int resultt, int result, Books2 pBooks)
        {
            try
            {
                var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });
                var reservations = new List<int> { 1, 3, 4 };
                var cantidadPrestamos = reservations
                    .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_BOOK = pLoans.ID_BOOK, ID_RESERVATION = reservation, STATUS = true }))
                    .Select(task => task.Result.Count)
                    .Sum();

                if (pLoans.ID_TYPE > 0 && pLoans.LENDER_CONTACT != null && fechaInicio != DateTime.MinValue && fechaCierre != DateTime.MinValue && pLoans.ID_BOOK > 0)
                {
                    var cantidadPrestamosPorEst = reservations
                        .Select(async reservation => await loansBL.GetLoansAsync(new Loans { ID_LENDER = pLoans.ID_LENDER, ID_RESERVATION = reservation, STATUS = true }))
                        .Select(task => task.Result.Count)
                        .Sum();
                    if (cantidadPrestamosPorEst < 2)
                    {
                        if (cantidadPrestamos < books.EJEMPLARS && books.EXISTENCES > 1)
                        {
                            pLoans.STATUS = true;
                            pLoans.ID_RESERVATION = 1;
                            pLoans.USER_ID = 1;
                            pLoans.COPY = 1;
                            pLoans.FEE = 0;
                            pLoans.REGISTRATION_DATE = DateTime.Now;
                            result = await loansBL.CreateLoansAsync(pLoans);

                            pLoanDates.ID_LOAN = pLoans.LOAN_ID;
                            pLoanDates.START_DATE = fechaInicio;
                            pLoanDates.END_DATE = fechaCierre;
                            pLoanDates.STATUS = 1;
                            resultt = await loanDatesBL.CreateLoanDatesAsync(pLoanDates);

                            pBooks.BOOK_ID = pLoans.ID_BOOK;
                            pBooks.EXISTENCES = books.EXISTENCES - 1;
                            int resultUpdate = await booksBL.UpdateExistencesBooksAsync(pBooks);

                            pLoans.Books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });
                            await _loanService.SendEmailLoanCreated(pLoans, pLoanDates);
                        }
                        else
                        {
                            ViewBag.Alerta2 = "No hay suficientes ejemplares para realizar el registro";
                            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                        }
                    }
                    else
                    {
                        ViewBag.AlertaPrestamoEx = "No se puede realizar el registro porque ya cuenta dos prestamos activos";
                        ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                    }
                }
                else
                {
                    ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                    ViewBag.Alerta = "Por favor ingrese los datos del Prestamo";
                }

                if (result > 0 && resultt > 0)
                {
                    ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                    TempData["Alerta"] = "El Prestamo se registro Exitosamente!!";
                }
                ViewBag.ShowMenu = true;
                return View();
            }
            catch (Exception ex)
            {
                if (result == 0)
                {
                    ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
                }
                ViewBag.Error = ex.Message;
                ViewBag.ShowMenu = true;
                return View();
            }
        }

        // GET: BooksController/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            ViewBag.LoanTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.LoanDates = await loanDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = id });

            var titulo = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            ViewBag.TituloB = titulo.TITLE;
            ViewBag.Portada = titulo.COVER;
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int Maestro, Loans pLoans, DateTime fechaInicio, DateTime fechaCierre, LoanDates pLoanDates, Books2 pBooks)
        {
            try
            {
                if (pLoans.ID_RESERVATION == 2 || pLoans.ID_RESERVATION == 5)
                {
                    var books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = pLoans.ID_BOOK });
                    pBooks.BOOK_ID = pLoans.ID_BOOK;
                    pBooks.EXISTENCES = books.EXISTENCES + 1;
                    int resultUpdate = await booksBL.UpdateExistencesBooksAsync(pBooks);
                }

                if (pLoans.STATUS == false && pLoans.ID_RESERVATION == 2 || pLoans.ID_RESERVATION == 5 && pLoans.STATUS == false)
                {
                    pLoans.STATUS = false;
                }
                else
                {
                    pLoans.STATUS = true;
                }

                if (fechaInicio != DateTime.MinValue && fechaCierre != DateTime.MinValue && pLoans.ID_RESERVATION != 2)
                {
                    pLoanDates.ID_LOAN = id;
                    pLoanDates.START_DATE = fechaInicio;
                    pLoanDates.END_DATE = fechaCierre;
                    pLoanDates.STATUS = 1;

                    if (pLoans.ID_RESERVATION == 1)
                    {
                        pLoans.ID_RESERVATION = 4;
                    }
                    else if (pLoans.ID_RESERVATION == 3)
                    {
                        pLoans.ID_RESERVATION = 1;
                    }
                    int resultt = await loanDatesBL.CreateLoanDatesAsync(pLoanDates);
                    int result = await loansBL.UpdateLoansAsync(pLoans);
                }
                else
                {
                    int result = await loansBL.UpdateLoansAsync(pLoans);
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.ShowMenu = true;
                return View(pLoans);
            }
        }

        public async Task<JsonResult> BuscarLibros(Books pBooks)
        {
            List<Books> lista = await booksBL.GetBooksAsync(pBooks);
            return Json(lista);
        }

        public async Task<JsonResult> ObtenerCategoria(Categories pCategories)
        {
            List<Categories> lista = await categoriesBL.GetCategoriesAsync(pCategories);
            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarEstudiante(string codigo)
        {
            List<Student> StudentList = new List<Student>();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{codigo}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (student != null)
                        {
                            StudentList.Add(student);
                        }
                    }
                }
                catch { }
            }
            return Json(StudentList);
        }

        public async Task<JsonResult> BuscarEstudianteId(long id)
        {
            Student student = new Student();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            if (student != null)
            {
                return Json(new
                {
                    studentCode = student.StudentCode,
                    fullName = $"{student.StudentName} {student.StudentLastName}",
                    career = student.Career?.CareerName ?? "Desconocido"
                });
            }
            return Json(new { studentCode = "No disponible", fullName = "", career = "" });
        }

        public async Task<IActionResult> Delite(long id)
        {
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            var titulo = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });
            var tituloB = titulo.TITLE;
            ViewBag.TituloB = tituloB;
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delite(int id, Loans2 pLoans)
        {
            try
            {
                pLoans.STATUS = false;
                int result = await loansBL.UpdateLoans02Async(pLoans);
                return RedirectToAction(nameof(Status2Loans));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pLoans);
            }
        }

        [HttpGet]
        public async Task<JsonResult> PrestamosPorTipo()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            var tipos = await loansTypesBL.GetAllLoanTypesAsync();
            var resultado = prestamos
                .GroupBy(p => p.ID_TYPE)
                .Select(g => new
                {
                    Tipo = tipos.FirstOrDefault(t => t.TYPES_ID == g.Key)?.TYPES_NAME ?? "Desconocido",
                    Cantidad = g.Count()
                }).ToList();
            return Json(resultado);
        }

        [HttpGet]
        public async Task<JsonResult> PrestamosPorSemestre()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            var currentYear = DateTime.Now.Year;
            int ciclo1 = prestamos.Count(p => p.REGISTRATION_DATE.Year == currentYear && p.REGISTRATION_DATE.Month >= 1 && p.REGISTRATION_DATE.Month <= 6);
            int ciclo2 = prestamos.Count(p => p.REGISTRATION_DATE.Year == currentYear && p.REGISTRATION_DATE.Month >= 7 && p.REGISTRATION_DATE.Month <= 12);
            var resultado = new[]
            {
              new { ciclo = "Ciclo 1", cantidad = ciclo1 },
              new { ciclo = "Ciclo 2", cantidad = ciclo2 }
            };
            return Json(resultado);
        }

        public async Task<IActionResult> AllLoans(Books pBooks, Loans pLoans = null)
        {
            if (pLoans == null)
                pLoans = new Loans();
            if (pLoans.Top_Aux == 0)
                pLoans.Top_Aux = 20;
            else if (pLoans.Top_Aux == -1)
                pLoans.Top_Aux = 0;

            var loans = await loansBL.GetIncludePropertiesAsync(pLoans);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Loans = await loansBL.GetAllLoansAsync();
            ViewBag.LoansTypes = await loansTypesBL.GetAllLoanTypesAsync();
            ViewBag.ReservationStatus = await reservationStatusBL.GetAllReservationStatusAsync();
            ViewBag.Books = await booksBL.GetIncludePropertiesAsync(pBooks);
            ViewBag.Top = pLoans.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(loans);
        }

        [HttpGet]
        public async Task<JsonResult> PrestamosPorMes()
        {
            var prestamos = await loansBL.GetAllLoansAsync();
            int anioActual = DateTime.Now.Year;
            var datos = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Mes = m,
                    Cantidad = prestamos.Count(p =>
                        p.REGISTRATION_DATE.Year == anioActual &&
                        p.REGISTRATION_DATE.Month == m)
                }).ToList();
            return Json(datos);
        }
    }
}

