using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BusinessRules
{
    public class BLUsers_Roles
    {
        public async Task<int> CreateRolesAsync(Users_Roles pUsers_Roles)
        {
            return await DALUsers_Roles.CreateRolesAsync(pUsers_Roles);
        }

        public async Task<int> UpdateRolesAsync(Users_Roles pUsers_Roles)
        {
            return await DALUsers_Roles.UpdateRolesAsync(pUsers_Roles);
        }

        public async Task<int> DeleteRolesAsync(Users_Roles pUser_roles)
        {
            return await DALUsers_Roles.DeleteRolesAsync(pUser_roles);
        }

        public async Task<List<Users_Roles>> GetAllRolesASync()
        {
            return await DALUsers_Roles.GetAllRolesASync();
        }

        public async Task<Users_Roles> GetRolesByIdAsync(Users_Roles pUser_roles)
        {
            return await DALUsers_Roles.GetRolesByIdAsync(pUser_roles);
        }

        public async Task<List<Users_Roles>> GetRolesAsync(Users_Roles pRoles)
        {
            return await DALUsers_Roles.GetRolesAsync(pRoles);
        }
    }
}
