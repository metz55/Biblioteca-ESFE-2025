using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class EditionsController : Controller
    {
        BLEditions editionsBL = new BLEditions();

        public async Task<IActionResult> Index(Editions pEditions = null, int page = 1, int pageSize = 10)
        {
            if (pEditions == null)
                pEditions = new Editions();
            if (pEditions.Top_Aux == -1)
                pEditions.Top_Aux = 0;
            else if (pEditions.Top_Aux == 1)
                pEditions.Top_Aux = 0;

            var allEditions = await editionsBL.GetEditionsAsync(pEditions);
            allEditions = allEditions.OrderBy(e => e.EDITION_ID).ToList();

            // Aplicar paginación
            int totalRegistros = allEditions.Count();
            int totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling((double)totalRegistros / pageSize) : 1;
            ViewBag.TotalPaginas = totalPaginas;
            var editions = allEditions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.ShowMenu = true;

            return View(editions);
        }


        // GET: AcquisitionTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = id });
            ViewBag.ShowMenu = true;
            return View(editions);
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
        public async Task<ActionResult> Create(Editions pEditions)
        {
            try
            {
                int result = await editionsBL.CreateEditionsAsync(pEditions);

                // revisa si la peticion es un AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (result > 0)
                    {
                        return Json(new { success = true, editioN_ID = pEditions.EDITION_ID, editioN_NUMBER = pEditions.EDITION_NUMBER });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Ocurrio un error en el guardado de la Edición" });
                    }
                }
                else
                {
                    // seguimineot regular para las peticiones que no seas AJAX
                    TempData["CreateSuccess"] = true;
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
                    return View(pEditions);
                }
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = id });
            ViewBag.ShowMenu = true;
            return View(editions);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Editions pEditions)
        {
            try
            {
                int result = await editionsBL.UpdateEditionsAsync(pEditions);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pEditions);
            }
        }

        // GET: CategoriesController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var editions = await editionsBL.GetEditionsByIdAsync(new Editions { EDITION_ID = id });
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
                int result = await editionsBL.DeleteEditionsAsync(new Editions { EDITION_ID = id });
                return Ok(new { success = true, message = "Edición eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string nombre)
        {
            var lista = await editionsBL.GetEditionsAsync(new Editions { EDITION_NUMBER = nombre });
            var resultado = lista.Select(e => new
            {
                editionId = e.EDITION_ID,
                editionNumber = e.EDITION_NUMBER
            });
            return Json(resultado);
        }

    }
}
