using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.Client.MVC.services;
using Library.BusinessRules;
using Library.Client.MVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class PostsCategoriesController : Controller
    {
        BLPostsCategories blPostsCategories = new BLPostsCategories();
        public async Task<IActionResult> Index()
        {
            var posts = await blPostsCategories.GetAllPostCategoriesAsync();
            ViewBag.ShowMenu = true;
            return View(posts);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PostsCategories posts)
        {
            var result = await blPostsCategories.CreatePostCategoryAsync(posts);

            if(result == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData["CreateSuccess"] = true;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var post = await blPostsCategories.GetPostCategoryByIdAsync(new PostsCategories(){Id = id});
            ViewBag.ShowMenu = true;
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostsCategories posts)
        {
            var result = await blPostsCategories.UpdatePostCategeoryAsync(posts);
            TempData["EditSuccess"] = true;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await blPostsCategories.DeletePostCategoryAsync(new PostsCategories() { Id = id });
                if (result == 1) 
                {
                    return Json(new { success = true, message = "Categoría eliminada correctamente." });
                }
                return Json(new { success = false, message = "No se pudo eliminar la categoría." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
