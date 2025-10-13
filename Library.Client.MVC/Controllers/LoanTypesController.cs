using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class LoanTypesController : Controller
    {
        BLLoanTypes loanTypesBL = new BLLoanTypes();
        // GET: AcquisitionTypesController
        public async Task<IActionResult> Index(LoanTypes pLoanTypes = null)
        {
            if (pLoanTypes == null)
                pLoanTypes = new LoanTypes();
            if (pLoanTypes.Top_Aux == -1)
                pLoanTypes.Top_Aux = 10;
            else
               if (pLoanTypes.Top_Aux == 1)
                pLoanTypes.Top_Aux = 0;
            var loanTypes = await loanTypesBL.GetLoanTypesAsync(pLoanTypes);
            ViewBag.Top = pLoanTypes.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(loanTypes);
        }

        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var loanTypes = await loanTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = id });
            ViewBag.ShowMenu = true;
            return View(loanTypes);
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
        public async Task<ActionResult> Create(LoanTypes pLoanTypes)
        {
            try
            {
                int result = await loanTypesBL.CreateLoanTypesAsync(pLoanTypes);
                TempData["CreateSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
                return View(pLoanTypes);
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var loanTypes = await loanTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = id });
            ViewBag.ShowMenu = true;
            return View(loanTypes);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoanTypes pLoanTypes)
        {
            try
            {
                int result = await loanTypesBL.UpdateLoanTypesAsync(pLoanTypes);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pLoanTypes);
            }
        }

        //// GET: AcquisitionTypesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var editions = await loanTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(editions);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var loanType = await loanTypesBL.GetLoanTypesByIdAsync(new LoanTypes { TYPES_ID = id });
                int result = await loanTypesBL.DeleteLoanTypesAsync(loanType);
                return Json(new { success = true, message = "Tipo de préstamo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
