using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;


namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class ReservationCreateController : Controller
    {
        BLLoans loansBL = new BLLoans();
        BLLoanTypes loansTypesBL = new BLLoanTypes();
        BLReservationStatus reservationStatusBL = new BLReservationStatus();
        BLBooks booksBL = new BLBooks();
        BLCategories categoriesBL = new BLCategories();

        public async Task<IActionResult> Index(Books pBooks, Loans pLoans = null)
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
        public async Task<IActionResult> Details(int id)
        {
            // Falta terminar 
            var loans = await loansBL.GetLoansByIdAsync(new Loans { LOAN_ID = id });
            //Objeto anonimo
            loans.LoanTypes = await loansTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = loans.ID_TYPE });
            loans.ReservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = loans.ID_RESERVATION });
            loans.Books = await booksBL.GetBooksByIdAsync(new Books { BOOK_ID = loans.ID_BOOK });


            return View(loans);
        }

        [HttpGet]
        public async Task<JsonResult> ReservacionesMensuales()
        {
            var resultado = await loansBL.GetAllLoansAsync();

            var datos = resultado
                .GroupBy(l => l.REGISTRATION_DATE.Month)
                .Select(g => new
                {
                    mes = g.Key,
                    cantidad = g.Count()
                })
                .OrderBy(x => x.mes)
                .ToList();

            var nombresMeses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var datosConNombres = datos.Select(d => new
            {
                mes = nombresMeses[d.mes - 1],
                cantidad = d.cantidad
            });

            return Json(datosConNombres);
        }


    }
}
