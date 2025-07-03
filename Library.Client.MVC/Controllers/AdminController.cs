
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Library.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public IActionResult Index()
        {
            ViewBag.ShowNavbar = true;
            return View();
        }
    }
}

