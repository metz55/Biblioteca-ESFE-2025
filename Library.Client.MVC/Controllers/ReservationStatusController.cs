using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class ReservationStatusController : Controller
    {
        BLReservationStatus reservationStatusBL = new BLReservationStatus();

        // GET: AcquisitionTypesController
        public async Task<IActionResult> Index(ReservationStatus pReservationStatus = null)
        {
            if (pReservationStatus == null)
                pReservationStatus = new ReservationStatus();
            if (pReservationStatus.Top_Aux == -1)
                pReservationStatus.Top_Aux = 10;
            else
               if (pReservationStatus.Top_Aux == 1)
                pReservationStatus.Top_Aux = 0;
            var reservationStatus = await reservationStatusBL.GetReservationStatusAsync(pReservationStatus);
            ViewBag.Top = pReservationStatus.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(reservationStatus);
        }

        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var reservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = id });
            ViewBag.ShowMenu = true;
            return View(reservationStatus);
        }

        // GET: CategoriesController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ReservationStatus pReservationStatus)
        {
            try
            {
                int result = await reservationStatusBL.CreateReservationStatusAsync(pReservationStatus);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
                return View(pReservationStatus);
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = id });
            ViewBag.ShowMenu = true;
            return View(reservationStatus);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationStatus pReservationStatus)
        {
            try
            {
                int result = await reservationStatusBL.UpdateReservationStatusAsync(pReservationStatus);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pReservationStatus);
            }
        }

        // GET: AcquisitionTypesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var reservationStatus = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(reservationStatus);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var status = await reservationStatusBL.GetReservationStatusByIdAsync(new ReservationStatus { RESERVATION_ID = id });
                int result = await reservationStatusBL.DeleteReservationStatusAsync(status);
                return Json(new { success = true, message = "Estado de reservación eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
