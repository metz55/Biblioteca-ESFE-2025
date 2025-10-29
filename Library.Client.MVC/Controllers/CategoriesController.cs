using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class CategoriesController : Controller
    {
        BLCategories categoriesBL = new BLCategories();
        BLCatalogs catalogsBL = new BLCatalogs();
        // GET: CategoriesController
        public async Task<IActionResult> Index(Categories pCategories = null)
        {
            if (pCategories == null)
                pCategories = new Categories();
            if (pCategories.Top_Aux == -1)
                pCategories.Top_Aux = 10;
            else
               if (pCategories.Top_Aux == 1)
                pCategories.Top_Aux = 0;
            var categories = await categoriesBL.GetCategoriesAsync(pCategories);
            ViewBag.Top = pCategories.Top_Aux;

            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.ShowMenu = true;
            return View(categories);
        }

        // GET: CategoriesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = id });
            ViewBag.ShowMenu = true;
            return View(categories);
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
        public async Task<ActionResult> Create(Categories pCategories)
        {        
            try
            {
                int result = await categoriesBL.CreateCategoriesAsync(pCategories);
                TempData["CreateSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
                return View(pCategories);
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = id });
            ViewBag.ShowMenu = true;
            return View(categories);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categories pCategories)
        {
            try
            {
                int result = await categoriesBL.UpdateCategoriesAsync(pCategories);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCategories);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var categories = await categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = id });
        //    ViewBag.ShowMenu = true;
        //    return View(categories);
        //}

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await categoriesBL.DeleteCategoriesAsync(new Categories { CATEGORY_ID = id });
                return Ok(new { success = true, message = "Categoría eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
