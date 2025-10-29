using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using Library.DataAccess.Domain;
using Library.Client.MVC.services;
using Library.BusinessRules;
using Library.Client.MVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Markdig;
using Library.Client.MVC.Models.DTO;

namespace Library.Client.MVC.Controllers
{
    public class PostsController : Controller
    {
        BLPostsCategories blCategories = new BLPostsCategories();
        BLPosts blPosts = new BLPosts();
        BLPostImages blPostsImages = new BLPostImages();
        BLPostDocs blPostDocs = new BLPostDocs();
        BLPostsCategories bLPostsCategories = new BLPostsCategories();
        BLPinnedPosts blPinnedPosts = new BLPinnedPosts();

        [AllowAnonymous]
        public async Task<IActionResult> Index() //Feed
        {
            var categories = await blCategories.GetAllPostCategoriesAsync();
            var pinnedPosts = await blPosts.GetPinnedPosts();
            var lastPosts = await blPosts.GetLastPosts();
            ViewBag.Categories = categories;
            ViewBag.pinnedPosts = pinnedPosts;
            ViewBag.lastPosts = lastPosts;
            ViewBag.BlogNavbar = true;
            ViewBag.ShowMenu = true;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(SearchPostDTO searchPostDTO)
        {
            ViewBag.ShowMenu = true;
            ViewBag.BlogNavbar = true;
            var posts = new List<Posts>();
            var listPosts = new List<Posts>();
            var categories = await bLPostsCategories.GetAllPostCategoriesAsync();
            ViewBag.Categories = categories;
            if(string.IsNullOrEmpty(searchPostDTO.Query) && searchPostDTO.CategoryId == 0 && searchPostDTO.Date == DateTime.MinValue)
            {
                return View(posts);
            }
            try{
                listPosts = await blPosts.GetSearchedManagePosts(new Posts(){CONTENT=searchPostDTO.Query, TITLE=searchPostDTO.Query, CATEGORYID=searchPostDTO.CategoryId, CREATED_AT=searchPostDTO.Date});
                foreach(var p in listPosts)
                {
                    var isPinned = await blPinnedPosts.IsPostPinned(new Posts(){ID=p.ID});
                    p.IS_PINNED = isPinned;
                    posts.Add(p);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Cant search: "+e);
            }
            return View(posts);
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Manage(string search = "")
        {
            var posts = new List<Posts>();
            var listPosts = new List<Posts>(); //la lista para el foreach
            if(!string.IsNullOrEmpty(search))
            {
                listPosts = await blPosts.GetSearchedManagePosts(new Posts(){CONTENT=search, TITLE=search});
                foreach(var p in listPosts)
                {
                    var isPinned = await blPinnedPosts.IsPostPinned(new Posts(){ID=p.ID});
                    p.IS_PINNED = isPinned;
                    posts.Add(p);
                }
                ViewBag.ShowMenu = true;
                return View(posts);
            }

            listPosts = await blPosts.GetAllPostsAsync();
            foreach(var p in listPosts)
            {
                var isPinned = await blPinnedPosts.IsPostPinned(new Posts(){ID=p.ID});
                p.IS_PINNED = isPinned;
                posts.Add(p);
            }
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            return View(posts);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(long id)
        {
            ViewBag.BlogNavbar = true;
            Posts posts = await blPosts.GetPostByIdAsync(new Posts(){ID = id});
            if(posts == null)
            {
                return NotFound();
            }
            posts.CONTENT = Markdig.Markdown.ToHtml(posts.CONTENT);
            ViewBag.ShowMenu = true;

            var postDocs = await blPostDocs.GetPostDocsByPostIdAsync(new Posts() { ID = id });
            ViewBag.PostDocs = postDocs;
            return View(posts);
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create()
        {
            ViewBag.BlogNavbar = true;
            List<PostsCategories> categories = await bLPostsCategories.GetAllPostCategoriesAsync();
            ViewBag.error = TempData["error"] ?? false;
            ViewBag.titleError = TempData["titleError"] ?? "";
            ViewBag.contentError = TempData["contentError"] ?? "";
            ViewBag.categoryError = TempData["categoryError"] ?? "";
            ViewBag.categories = categories;
            ViewBag.ShowMenu = true;
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Posts posts)
        {
            var error = false;
            if (string.IsNullOrEmpty(posts.TITLE))
            {
                error = true;
                TempData["titleError"] = "El Titulo es requerido";
            }
            if (posts.TITLE.Length > 250)
            {
                error = true;
                TempData["titleError"] = "Caracteres maximos 250";
            }
            if (string.IsNullOrEmpty(posts.CONTENT))
            {
                error = true;
                TempData["contentError"] = "El Contenido es requerido";
            }
            if (posts.CONTENT.Length > 3000)
            {
                error = true;
                TempData["contentError"] = "Caracteres maximos 3000";
            }
            if (posts.CATEGORYID == 0)
            {
                error = true;
                TempData["categoryError"] = "La categoria es obligatoria";
            }
            TempData["error"] = error;
            if (error) { return RedirectToAction(nameof(Create)); }

            var result = await blPosts.CreatePostAsync(posts);
            if (result == 0)
            {
                TempData["error"] = true;
                TempData["messageError"] = "No se pudo crear el post. Revisa que los datos esten correctos";
                return RedirectToAction(nameof(Manage));
            }
            else
            {
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        if (file.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(file.FileName).ToLower();
                            var uploadDir = "";
                            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                            {
                                // Procesar imágenes
                                uploadDir = Path.Combine("C:\\", "ImagenesBlog");
                                if (!Directory.Exists(uploadDir))
                                {
                                    Directory.CreateDirectory(uploadDir);
                                }
                                var uniqueFileName = $"{posts.ID}_{Guid.NewGuid()}.jpg";
                                var filePath = Path.Combine(uploadDir, uniqueFileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                var imagePath = $"/ImagenesBlog/{uniqueFileName}";
                                // Guardar la ruta en la base de datos
                                var postsImages = new PostsImages
                                {
                                    PATH = imagePath,
                                    POSTID = posts.ID
                                };
                                await blPostsImages.CreatePostImageAsync(postsImages);
                            }
                            else
                            {
                                // Procesar documentos
                                uploadDir = Path.Combine("C:\\", "DocumentosBlog");
                                if (!Directory.Exists(uploadDir))
                                {
                                    Directory.CreateDirectory(uploadDir);
                                }
                                var uniqueFileName = $"{posts.ID}_{Guid.NewGuid()}{fileExtension}";
                                var filePath = Path.Combine(uploadDir, uniqueFileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                var docPath = $"/DocumentosBlog/{uniqueFileName}";
                                var postsDocs = new PostsDocs
                                {
                                    Path = docPath,
                                    PostId = posts.ID
                                };
                                await blPostDocs.CreatePostDocAsync(postsDocs);
                            }
                        }
                    }
                }
                // Agregar mensaje de éxito
                TempData["success"] = true;
                TempData["messageSuccess"] = "El post se creo correctamente.";
            }
            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(long id = 0)
        {
            if(id == 0)
            {
                return RedirectToAction(nameof(Manage));
            }
            var post = await blPosts.GetPostByIdAsync(new Posts(){ID = id});
            List<PostsCategories> postsCategories = await bLPostsCategories.GetAllPostCategoriesAsync();
            ViewBag.error = TempData["error"] ?? false;
            ViewBag.titleError = TempData["titleError"] ?? "";
            ViewBag.contentError = TempData["contentError"] ?? "";
            ViewBag.categoryError = TempData["categoryError"] ?? "";
            ViewBag.categories = postsCategories;
            ViewBag.ShowMenu = true;
            return View(post);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Posts posts, string removeImages, string removeDocs)
        {
            var error = false;
            if (string.IsNullOrEmpty(posts.TITLE))
            {
                error = true;
                TempData["titleError"] = "El Titulo es requerido";
            }
            if (posts.TITLE.Length > 250)
            {
                error = true;
                TempData["titleError"] = "Caracteres maximos 250";
            }
            if (string.IsNullOrEmpty(posts.CONTENT))
            {
                error = true;
                TempData["contentError"] = "El Contenido es requerido";
            }
            if (posts.CONTENT.Length > 3000)
            {
                error = true;
                TempData["contentError"] = "Caracteres maximos 3000";
            }
            if (posts.CATEGORYID == 0)
            {
                error = true;
                TempData["categoryError"] = "La categoria es obligatoria";
            }
            TempData["error"] = error;
            if (error)
            {
                return RedirectToAction(nameof(Edit), new { id = posts.ID });
            }

            var postsImages = await blPostsImages.GetPostImagesByIdPostAsync(posts);
            var postsDocs = await blPostDocs.GetPostDocsByPostIdAsync(posts);

            // Eliminar imágenes si se indica
            if (removeImages == "true")
            {
                foreach (var image in postsImages)
                {
                    string uploadDir = Path.Combine("C:\\");
                    string fullPath = Path.Combine(uploadDir, image.PATH.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        try
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Archivo no encontrado: {fullPath}");
                    }
                }
                await blPostsImages.DeletePostsImagesByIdPostAsync(posts);
            }

            // Eliminar documentos si se indica
            if (removeDocs == "true")
            {
                foreach (var doc in postsDocs)
                {
                    string uploadDir = Path.Combine("C:\\");
                    string fullPath = Path.Combine(uploadDir, doc.Path.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        try
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Archivo no encontrado: {fullPath}");
                    }
                }
                await blPostDocs.DeleteAllPostsDocsByIdPostAsync(posts);
            }

            // Procesar nuevos archivos si se suben
            if (Request.Form.Files.Count > 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            // Eliminar imágenes anteriores si se sube una nueva imagen
                            foreach (var image in postsImages)
                            {
                                string uploadDir = Path.Combine("C:\\");
                                string fullPath = Path.Combine(uploadDir, image.PATH.TrimStart('/'));
                                if (System.IO.File.Exists(fullPath))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Archivo no encontrado: {fullPath}");
                                }
                            }
                            await blPostsImages.DeletePostsImagesByIdPostAsync(posts);
                        }
                        else
                        {
                            // Eliminar documentos anteriores si se sube un nuevo documento
                            foreach (var doc in postsDocs)
                            {
                                string uploadDir = Path.Combine("C:\\");
                                string fullPath = Path.Combine(uploadDir, doc.Path.TrimStart('/'));
                                if (System.IO.File.Exists(fullPath))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(fullPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Archivo no encontrado: {fullPath}");
                                }
                            }
                            await blPostDocs.DeleteAllPostsDocsByIdPostAsync(posts);
                        }
                    }
                }

                // Añadir nuevos archivos
                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            PostsImages _postsimages = new PostsImages();
                            var uploadDir = Path.Combine("C:\\", "ImagenesBlog");
                            if (!Directory.Exists(uploadDir))
                            {
                                Directory.CreateDirectory(uploadDir);
                            }
                            var uniqueFileName = posts.ID.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                            var filePath = Path.Combine(uploadDir, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            _postsimages.PATH = $"/ImagenesBlog/{uniqueFileName}";
                            _postsimages.POSTID = posts.ID;
                            await blPostsImages.CreatePostImageAsync(_postsimages);
                        }
                        else
                        {
                            PostsDocs _postsDocs = new PostsDocs();
                            var uploadDir = Path.Combine("C:\\", "DocumentosBlog");
                            if (!Directory.Exists(uploadDir))
                            {
                                Directory.CreateDirectory(uploadDir);
                            }
                            var uniqueFileName = $"{posts.ID}_{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadDir, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            _postsDocs.Path = $"/DocumentosBlog/{uniqueFileName}";
                            _postsDocs.PostId = posts.ID;
                            await blPostDocs.CreatePostDocAsync(_postsDocs);
                        }
                    }
                }
            }

            var result = await blPosts.UpdatePostAsync(posts);
            TempData["EditSuccess"] = true;
            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pin(long postId)
        {
            var success = await blPinnedPosts.CreatePinnedPost(new PinnedPosts(){POSTID = postId});
            TempData["success"] = success;
            if (success)
            {
                TempData["message"] = "Se ha fijado el post.";
            }
            else{
                TempData["message"] = "No se pudo fijar el post";
            }

            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpin(long postId)
        {
            var success = await blPinnedPosts.DeletePinnedPost(new PinnedPosts(){POSTID = postId});
            TempData["success"] = success;
            if (success)
            {
                TempData["message"] = "Se ha desfijado el post.";
            }
            else{
                TempData["message"] = "No se pudo desfijar el post";
            }

            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var isPinned = await blPinnedPosts.IsPostPinned(new Posts(){ID = id});
            if(isPinned){
                var success = await blPinnedPosts.DeletePinnedPost(new PinnedPosts(){POSTID = id});
                TempData["success"] = success;
                if(!success)
                {
                    TempData["message"] = "No se pudo eliminar el post";
                    return RedirectToAction(nameof(Manage));
                }
            }
            var post = await blPosts.GetPostByIdAsync(new Posts(){ID = id});
            if (post == null)
            {
                TempData["error"] = "El post no existe.";
                TempData["success"] = false;
                return RedirectToAction(nameof(Manage));
            }
            // Elimina los archivos físicamente
            foreach (var image in post.IMAGES)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string fullPath = Path.Combine(uploadDir, image.PATH.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Archivo no encontrado: {fullPath}");
                }
            }
            foreach (var doc in post.DOCS)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string fullPath = Path.Combine(uploadDir, doc.Path.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al eliminar archivo {fullPath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Archivo no encontrado: {fullPath}");
                }
            }
            //deletes
            await blPostsImages.DeletePostsImagesByIdPostAsync(post);
            await blPostDocs.DeleteAllPostsDocsByIdPostAsync(post);
            await blPosts.DeletePostAsync(new Posts(){ID = id});
            TempData["success"] = true;
            TempData["message"] = "El post ha sido eliminado.";

            return RedirectToAction(nameof(Manage));
        }
    }
}
