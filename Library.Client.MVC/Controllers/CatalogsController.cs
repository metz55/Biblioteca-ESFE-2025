using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class CatalogsController : Controller
    {
        BLCatalogs catalogsBL = new BLCatalogs();
        BLCategories categoriesBL = new BLCategories();
        // GET: CategoriesController
        public async Task<IActionResult> Index(Catalogs pCatalogs = null)
        {
            if (pCatalogs == null)
                pCatalogs = new Catalogs();
            if (pCatalogs.Top_Aux == -1)
                pCatalogs.Top_Aux = 10;
            else
               if (pCatalogs.Top_Aux == 1)
                pCatalogs.Top_Aux = 0;
            var authors = await catalogsBL.GetCatalogsAsync(pCatalogs);
            ViewBag.Top = pCatalogs.Top_Aux;

            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.ShowMenu = true;
            return View(authors);

        }

        // GET: CategoriesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = id });
            ViewBag.ShowMenu = true;
            return View(catalogs);
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
        public async Task<ActionResult> Create(Catalogs pCatalogs)
        {
            try
            {
                int result = await catalogsBL.CreateCatalogsAsync(pCatalogs);
                TempData["CreateSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
                return View(pCatalogs);
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var authors = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = id });
            ViewBag.ShowMenu = true;
            return View(authors);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Catalogs pCatalogs)
        {
            try
            {
                int result = await catalogsBL.UpdateCatalogsAsync(pCatalogs);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCatalogs);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var catalogs = await catalogsBL.GetCatalogsByIdAsync(new Catalogs { CATALOG_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(catalogs);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await catalogsBL.DeleteCatalogsAsync(new Catalogs { CATALOG_ID = id });
                return Ok(new { success = true, message = "Catálogo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
