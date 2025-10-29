using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class AcquisitionTypesController : Controller
    {
        BLAcquisitionTypes acquisitionTypesBL = new BLAcquisitionTypes();

        // GET: AcquisitionTypesController
        public async Task<IActionResult> Index(AcquisitionTypes pAcquisitionTypes = null)
        {
            if (pAcquisitionTypes == null)
                pAcquisitionTypes = new AcquisitionTypes();
            if (pAcquisitionTypes.Top_Aux == -1)
                pAcquisitionTypes.Top_Aux = 10;
            else
               if (pAcquisitionTypes.Top_Aux == 1)
                pAcquisitionTypes.Top_Aux = 0;
            var acquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesAsync(pAcquisitionTypes);
            ViewBag.Top = pAcquisitionTypes.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(acquisitionTypes);
        }

        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var acquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = id });
            ViewBag.ShowMenu = false;
            return View(acquisitionTypes);
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
        public async Task<ActionResult> Create(AcquisitionTypes pAcquisitionTypes)
        {
            try
            {
                int result = await acquisitionTypesBL.CreateAcquisitionTypesAsync(pAcquisitionTypes);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
                return View(pAcquisitionTypes);
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var acquisitionTypes = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = id });
            ViewBag.ShowMenu = true;
            return View(acquisitionTypes);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AcquisitionTypes pAcquisitionTypes)
        {
            try
            {
                int result = await acquisitionTypesBL.UpdateAcquisitionTypesAsync(pAcquisitionTypes);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pAcquisitionTypes);
            }
        }

        // GET: AcquisitionTypesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var editions = await acquisitionTypesBL.GetAcquisitionTypesByIdAsync(new AcquisitionTypes { ACQUISITION_ID = id });
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
                int result = await acquisitionTypesBL.DeleteAcquisitionTypesAsync(new AcquisitionTypes { ACQUISITION_ID = id });
                return Ok(new { success = true, message = "Tipo de adquisición eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
