using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BusinessRules
{
    public class BLUsers
    {
        public async Task<int> CreateUsersAsync(Users pUsers)
        {
            return await DALUsers.CreateUsersAsync(pUsers);
        }

        public async Task<int> UpdateUsersASync(Users pUsers)
        {
            return await DALUsers.UpdateUsersASync(pUsers);
        }

        public async Task<int> DeleteUsersAsync(Users pUsers)
        {
            return await DALUsers.DeleteUsersAsync(pUsers);
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await DALUsers.GetAllUsersAsync();
        }

        public async Task<Users> GetUsersByIdAsync(Users pUsers)
        {
            return await DALUsers.GetUsersByIdAsync(pUsers);
        }

        public async Task<List<Users>> GetUsersAsync(Users pUsers)
        {
            return await DALUsers.GetUsersAsync(pUsers);
        }

        public async Task<List<Users>> GetIncludeRolesASync(Users pUsers)
        {
            return await DALUsers.GetIncludeRolesASync(pUsers);
        }

        public async Task<Users> LoginAsync(Users pUsers)
        {
            return await DALUsers.LoginAsync(pUsers);
        }

        public async Task<int> ChangePasswordAsync(Users pUsers, string pOldPassword)
        {
            return await DALUsers.ChangePasswordAsync(pUsers, pOldPassword);
        }
    }
}
