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
        public async Task<IActionResult> Index(Users pUsers = null)
        {
            if(pUsers == null)
                pUsers = new Users();
            if (pUsers.Top_Aux == 0)
                pUsers.Top_Aux = 10;
            else if (pUsers.Top_Aux == -1)
                pUsers.Top_Aux = 0;
            var taskSearch = usersBL.GetIncludeRolesASync(pUsers);
            var taskGetRoles = rolesBL.GetAllRolesASync();
            var user = await taskSearch;
            ViewBag.Top = pUsers.Top_Aux;
            ViewBag.Users_Roles = taskGetRoles;
            ViewBag.ShowMenu = true;
            return View(user);
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

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Users pUsers)
        {
            try
            {
                var date = DateTime.Now;
                var roleId = 1; //Este ID es para Administrador, es momentaneo y en caso de usar un CRUD para los roles, se debe validar desde la vista
                pUsers.CREATED_AT = date;
                pUsers.ROlE_ID = roleId;
                int result = await usersBL.CreateUsersAsync(pUsers);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
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
        public async Task<IActionResult> Delete(int id)
        {
            var user = await usersBL.GetUsersByIdAsync(new Users { USER_ID = id });
            user.Users_Roles = await rolesBL.GetRolesByIdAsync(new Users_Roles {  USER_ROLE_ID = user.ROlE_ID });
            ViewBag.Error = "";
            ViewBag.ShowMenu = true;
            return View(user);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Users pUsers)
        {
            try
            {
                int result = await usersBL.DeleteUsersAsync(pUsers);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pUsers);
            }
        }

        // GET: UsuarioController/CambiarPassword
        

        
    }
}
