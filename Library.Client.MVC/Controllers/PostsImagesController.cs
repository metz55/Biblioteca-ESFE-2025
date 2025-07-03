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
    public class PostsImagesController : Controller
    {
        BLPostImages blPostsImages = new BLPostImages();

        public async Task<IActionResult> Index()
        {
            var posts = await blPostsImages.GetAllPostImagesAsync();
            return View(posts);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PostsImages posts)
        {
            var result = await blPostsImages.CreatePostImageAsync(posts);

            if(result == 0)
            {
                return RedirectToAction(nameof(Index));
            }
                
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var post = await blPostsImages.GetPostImagesByIdAsync(new PostsImages(){ID = id});
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostsImages posts)
        {
            var result = await blPostsImages.UpdatePostImageAsync(posts);
            return View(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await blPostsImages.DeletePostImageAsync(new PostsImages(){ID = id});
            return View(nameof(Index));
        }
    }
}
