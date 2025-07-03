using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.DataAccess.Domain;
using Library.BusinessRules;
using Microsoft.AspNetCore.Authorization;

namespace Library.Client.MVC.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class UserRolesController : Controller
    {
        BLUsers_Roles usersRolesBL = new BLUsers_Roles();
        // GET: UserRolesController
        public async Task<IActionResult> Index(Users_Roles pUsersRoles = null)
        {
           if(pUsersRoles == null)
                pUsersRoles = new Users_Roles();
            if (pUsersRoles.Top_Aux == 0)
                pUsersRoles.Top_Aux = 10;
            else if (pUsersRoles.Top_Aux == -1)
                pUsersRoles.Top_Aux = 0;

            var userRoles = await usersRolesBL.GetRolesAsync(pUsersRoles);
            ViewBag.Top = pUsersRoles.Top_Aux;
            return View(userRoles);
        }

        // GET: UserRolesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var userRoles = await usersRolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = id });
            return View(userRoles);
        }

        // GET: UserRolesController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: UserRolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Users_Roles pUserRoles)
        {
            try
            {
                int result = await usersRolesBL.CreateRolesAsync(pUserRoles);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pUserRoles);
            }
        }

        // GET: UserRolesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userRoles = await usersRolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = id });
            return View(userRoles);
        }

        // POST: UserRolesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Users_Roles pUserRoles)
        {
            try
            {
                int result = await usersRolesBL.UpdateRolesAsync(pUserRoles);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pUserRoles);
            }
        }

        // GET: UserRolesController/Delete/5
        public async  Task<IActionResult> Delete(int id)
        {
            var userRoles = await usersRolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = id });
            return View(userRoles);
        }

        // POST: UserRolesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Users_Roles pUserRoles)
        {
            try
            {
                int result = await usersRolesBL.DeleteRolesAsync(new Users_Roles { USER_ROLE_ID = id });
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pUserRoles);
            }
        }
    }
}
