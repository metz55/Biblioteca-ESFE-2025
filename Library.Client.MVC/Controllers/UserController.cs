using Library.BusinessRules;
using Library.DataAccess.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class UserController : Controller
    {

        BLUsers usersBL = new BLUsers();
        BLUsers_Roles rolesBL = new BLUsers_Roles();
        // GET: UserController
        public async Task<IActionResult> Index(string USER_NAME, int page = 1, int pageSize = 5)
        {
            var filtro = new Users
            {
                USER_NAME = USER_NAME,
                Top_Aux = -1 // para traer todos y luego paginar en memoria
            };

            var taskSearch = usersBL.GetIncludeRolesASync(filtro);
            var taskGetRoles = rolesBL.GetAllRolesASync();

            var allUsers = await taskSearch;

            // Ordenar por USER_ID (puedes cambiar a USER_NAME si quieres alfabético)
            allUsers = allUsers
                .OrderBy(u => u.USER_ID)
                .ToList();

            int totalRegistros = allUsers.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);

            var users = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Users_Roles = await taskGetRoles;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaActual = page;
            ViewBag.Top = pageSize;
            ViewBag.ShowMenu = true;

            return View(users);
        }


        // GET: UserController/Details/5
        public async  Task<IActionResult> Details(int id)
        {
            var user = await usersBL.GetUsersByIdAsync(new Users { USER_ID = id });
            user.Users_Roles = await rolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = user.ROlE_ID});
            ViewBag.ShowMenu = true;
            return View(user);
        }

        // GET: UserController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users_Roles = await rolesBL.GetAllRolesASync();
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Users pUsers)
        {
            try
            {
                var date = DateTime.Now;
                var roleId = 1;
                pUsers.CREATED_AT = date;
                pUsers.ROlE_ID = roleId;

                int result = await usersBL.CreateUsersAsync(pUsers);

                TempData["Success"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Users_Roles = await rolesBL.GetAllRolesASync();
                return View(pUsers);
            }
        }


        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var user = await usersBL.GetUsersByIdAsync(new Users { USER_ID = id});
            var roles = await rolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = user.ROlE_ID });           
            ViewBag.Users_Roles = await rolesBL.GetAllRolesASync();
            ViewBag.Users = await  usersBL.GetAllUsersAsync();
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Users pUsers)
        {
            try
            {
                int result = await usersBL.UpdateUsersASync(pUsers);
                TempData["EditSuccess"] = true;
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Users_Roles = await rolesBL.GetAllRolesASync();
                return View(pUsers);
            }
        }

        // GET: UserController/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var user = await usersBL.GetUsersByIdAsync(new Users { USER_ID = id });
        //    user.Users_Roles = await rolesBL.GetRolesByIdAsync(new Users_Roles {  USER_ROLE_ID = user.ROlE_ID });
        //    ViewBag.Error = "";
        //    ViewBag.ShowMenu = true;
        //    return View(user);
        //}

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await usersBL.GetUsersByIdAsync(new Users { USER_ID = id });
                int result = await usersBL.DeleteUsersAsync(user);
                return Ok(new { success = true, message = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: UsuarioController/CambiarPassword



    }
}
