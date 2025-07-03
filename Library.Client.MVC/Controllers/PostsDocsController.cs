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
    public class PostsDocsController : Controller
    {
        BLPostDocs blPostsDocs = new BLPostDocs();

        public async Task<IActionResult> Index()
        {
            var posts = await blPostsDocs.GetAllPostDocsAsync();
            return View(posts);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PostsDocs posts)
        {
            var result = await blPostsDocs.CreatePostDocAsync(posts);

            if(result == 0)
            {
                return RedirectToAction(nameof(Index));
            }
                
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var post = await blPostsDocs.GetPostDocsByIdAsync(new PostsDocs(){Id = id});
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostsDocs posts)
        {
            var result = await blPostsDocs.UpdatePostDocAsync(posts);
            return View(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await blPostsDocs.DeletePostDocAsync(new PostsDocs(){Id = id});
            return View(nameof(Index));
        }
    }
}
