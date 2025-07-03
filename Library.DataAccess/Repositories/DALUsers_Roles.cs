using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories
{
    public class DALUsers_Roles
    {
        #region CRUD

        public static async Task<int> CreateRolesAsync(Users_Roles pUsers_Roles)
        {
            int result = 0;
            using (var dbcontext = new DBContext())
            {
                dbcontext.Add(pUsers_Roles);
                result = await dbcontext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateRolesAsync(Users_Roles pUsers_Roles)
        {
            int result = 0;
            using (var dbcontext = new DBContext())
            {
                var users_roles = await dbcontext.Users_Roles.FirstOrDefaultAsync(s => s.USER_ROLE_ID == pUsers_Roles.USER_ROLE_ID);
                users_roles.ROLE = pUsers_Roles.ROLE;
                dbcontext.Update(users_roles);
                result = await dbcontext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteRolesAsync(Users_Roles pUser_roles)
        {
            int result = 0;
            using (var dbcontext = new DBContext())
            {
                var roles = await dbcontext.Users_Roles.FirstOrDefaultAsync(s => s.USER_ROLE_ID == pUser_roles.USER_ROLE_ID);
                dbcontext.Users_Roles.Remove(roles);
                result = await dbcontext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Users_Roles>> GetAllRolesASync()
        {
            var roles = new List<Users_Roles>();
            using (var dbcontext = new DBContext())
            {
                roles = await dbcontext.Users_Roles.ToListAsync();
            }
            return roles;
        }

        public static async Task<Users_Roles> GetRolesByIdAsync(Users_Roles pUser_roles)
        {
            var roles = new Users_Roles();
            using (var dbcontext = new DBContext())
            {
                roles = await dbcontext.Users_Roles.FirstOrDefaultAsync(s => s.USER_ROLE_ID == pUser_roles.USER_ROLE_ID);
            }
            return roles;
        }
        internal static IQueryable<Users_Roles> QuerySelect(IQueryable<Users_Roles> pQuery, Users_Roles pRoles)
        {
            if (pRoles.USER_ROLE_ID > 0)
                pQuery = pQuery.Where(s => s.USER_ROLE_ID == pRoles.USER_ROLE_ID);
            if (!string.IsNullOrWhiteSpace(pRoles.ROLE))
                pQuery = pQuery.Where(s => s.ROLE.Contains(pRoles.ROLE));
            pQuery = pQuery.OrderByDescending(s => s.USER_ROLE_ID).AsQueryable();
            if (pRoles.Top_Aux > 0)
                pQuery = pQuery.Take(pRoles.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Users_Roles>> GetRolesAsync(Users_Roles pRoles)
        {
            var roles = new List<Users_Roles>();
            using(var dbcontext = new DBContext())
            {
                var select = dbcontext.Users_Roles.AsQueryable();
                select = QuerySelect(select, pRoles);
                roles = await select.ToListAsync();
            }
            return roles;
        }
        #endregion
    }
}
