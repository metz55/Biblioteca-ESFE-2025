using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class EditorialsController : Controller
    {

        BLEditorials editorialsBL = new BLEditorials();
        BLCategories categoriesBL = new BLCategories();
        BLCatalogs catalogsBL = new BLCatalogs();

        // GET: AcquisitionTypesController
        public async Task<IActionResult> Index(Editorials pEditorials = null)
        {
            if (pEditorials == null)
                pEditorials = new Editorials();
            if (pEditorials.Top_Aux == -1)
                pEditorials.Top_Aux = 10;
            else
               if (pEditorials.Top_Aux == 1)
                pEditorials.Top_Aux = 0;
            var editorials = await editorialsBL.GetEditorialsAsync(pEditorials);
            ViewBag.Categories = await categoriesBL.GetAllCategoriesAsync();
            ViewBag.Catalogs = await catalogsBL.GetAllCatalogsAsync();
            ViewBag.Top = pEditorials.Top_Aux;
            ViewBag.ShowMenu = true;
            return View(editorials);
        }

        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var editorials = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
            ViewBag.ShowMenu = true;
            return View(editorials);
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
        public async Task<ActionResult> Create(Editorials pEditorials)
        {
            try
            {
                int result = await editorialsBL.CreateEditorialsAsync(pEditorials);

                // revisa si la peticion es un AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (result > 0)
                    {
                        return Json(new { success = true, editoriaL_ID = pEditorials.EDITORIAL_ID, editoriaL_NAME = pEditorials.EDITORIAL_NAME });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Ocurrio un error en el guardado de la Editorial" });
                    }
                }
                else
                {
                    // seguimineot regular para las peticiones que no seas AJAX
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ee)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ee.Message });
                }
                else
                {
                    ViewBag.Error = ee.Message;
                    return View(pEditorials);
                }
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var editions = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
            ViewBag.ShowMenu = false;
            return View(editions);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Editorials pEditorials)
        {
            try
            {
                int result = await editorialsBL.UpdateEditorialsAsync(pEditorials);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pEditorials);
            }
        }

        // GET: CategoriesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var editions = await editorialsBL.GetEditorialsByIdAsync(new Editorials { EDITORIAL_ID = id });
            ViewBag.ShowMenu = true;
            return View(editions);
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Editorials pEditorials)
        {
            try
            {
                int result = await editorialsBL.DeleteEditorialsAsync(new Editorials { EDITORIAL_ID = id });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pEditorials);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string nombre)
        {
            var editorial = new Editorials
            {
                EDITORIAL_NAME = nombre,
                Top_Aux = 10
            };

            var lista = await editorialsBL.GetEditorialsAsync(editorial);

            var resultado = lista.Select(x => new {
                editorialId = x.EDITORIAL_ID,
                editorialName = x.EDITORIAL_NAME
            });

            return Json(resultado);
        }

    }
}
