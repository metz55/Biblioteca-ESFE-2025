using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories
{
    public class DALUsers
    {

        #region CRUD 
        private static void EncryptMD5(Users pUsers)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    var result = md5.ComputeHash(Encoding.UTF8.GetBytes(pUsers.PASSWORD_HASH));
                    var encryptString = "";
                    for (int i = 0; i < result.Length; i++)
                    {
                        encryptString += result[i].ToString("x2").ToLower();
                    }
                    pUsers.PASSWORD_HASH = encryptString;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al encriptar la clave", ex);
            }
        }

        private static async Task<bool> LoginExists(Users pUsers, DBContext pDbContext)
        {
            try
            {
                bool result = false; ;
                var UserNameExist = await pDbContext.Users.FirstOrDefaultAsync(a => a.USER_NAME == pUsers.USER_NAME && a.USER_ID != pUsers.USER_ID);
                if(UserNameExist != null && UserNameExist.USER_ID > 0 && UserNameExist.USER_NAME == pUsers.USER_NAME)
                    result = true;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del este usuario", ex);
            }
        }

        

        public static async Task<int> CreateUsersAsync(Users pUsers)
        {
            int result = 0;
            try
            {
                using (var dbcontext = new DBContext())
                {
                    bool loginExist = await LoginExists(pUsers, dbcontext);
                    if(loginExist == false)
                    {
                        EncryptMD5(pUsers);
                        dbcontext.Add(pUsers);
                        result = await dbcontext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Nombre de Usuario ya exixtente");
                    }
                }
            }
            catch (Exception)
            {
                result = 0;
                throw new Exception("Ocurrio un error interno");
            }
            return result;
        }

        public static async Task<int> UpdateUsersASync(Users pUsers)
        {
            int result = 0;
            try
            {
                using (var dbcontext = new DBContext())
                {
                    bool loginExist = await LoginExists(pUsers, dbcontext);
                    if (loginExist == false)
                    {
                        var users = await dbcontext.Users.FirstOrDefaultAsync(s => s.USER_ID == pUsers.USER_ID);
                        users.ROlE_ID = pUsers.ROlE_ID;
                        users.USER_NAME = pUsers.USER_NAME;
                        users.CREATED_AT = pUsers.CREATED_AT;
                        dbcontext.Update(users);
                        result = await dbcontext.SaveChangesAsync();
                    }
                    else
                        throw new Exception("Nombre de usuario ya existe");
                }
            }
            catch (Exception)
            {
                result = 0;
                throw new Exception("Ocurrio un error interno");
            }
            return result;
        }

        public static async Task<int> DeleteUsersAsync(Users pUsers)
        {
            int result = 0;
            using (var dbcontext = new DBContext())
            {
                var users = await dbcontext.Users.FirstOrDefaultAsync(s => s.USER_ID == pUsers.USER_ID);
                dbcontext.Users.Remove(users);
                result = await dbcontext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Users>> GetAllUsersAsync()
        {
            var users = new List<Users>();
            using (var dbcontext = new DBContext())
            {
                users = await dbcontext.Users.ToListAsync();
            }
            return users;
        }

        public static async Task<Users> GetUsersByIdAsync(Users pUsers)
        {
            var users = new Users();
            using (var dbcontext = new DBContext())
            {
                users = await dbcontext.Users.FirstOrDefaultAsync(s => s.USER_ID == pUsers.USER_ID);
            }
            return users;
        }

        internal static IQueryable<Users> QuerySelect(IQueryable<Users> pQuery, Users pUsers)
        {
            if (pUsers.USER_ID > 0)
                pQuery = pQuery.Where(s => s.USER_ID == pUsers.USER_ID);
            if (pUsers.ROlE_ID > 0)
                pQuery = pQuery.Where(s => s.ROlE_ID == pUsers.ROlE_ID);
            if (!string.IsNullOrWhiteSpace(pUsers.USER_NAME))
                pQuery = pQuery.Where(s => s.USER_NAME.Contains(pUsers.USER_NAME));

            pQuery = pQuery.OrderByDescending(s => s.USER_ID).AsQueryable();
            if (pUsers.Top_Aux > 0)
                pQuery = pQuery.Take(pUsers.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Users>> GetUsersAsync(Users pUsers)
        {
            var users = new List<Users>();
            using (var  dbcontext = new DBContext())
            {
                var select = dbcontext.Users.AsQueryable();
                select = QuerySelect(select, pUsers).Include(r => r.Users_Roles).AsQueryable();
                users = await select.ToListAsync();
            }
            return users;
        }

        public static async Task<List<Users>> GetIncludeRolesASync(Users pUsers)
        {
            var users = new List<Users>();
            using (var dbcontexto = new DBContext())
            {
                var select = dbcontexto.Users.AsQueryable();
                select = QuerySelect(select, pUsers).Include(r => r.Users_Roles).AsQueryable();

                users = await select.ToListAsync();
            }
            return users;
        }

        #endregion

        #region Metodos de LogIn, y cambiar contraseña
        public static async Task<Users> LoginAsync (Users pUsers)
        {
            try
            {
                var users = new Users();
                using (var dbcontext = new DBContext())
                {
                    EncryptMD5(pUsers);
                    users = await dbcontext.Users
                    .Include(u=>u.Users_Roles)
                    .FirstOrDefaultAsync(s => s.USER_NAME == pUsers.USER_NAME && s.PASSWORD_HASH == pUsers.PASSWORD_HASH);
                }

                return users;
            }
            catch(Exception ex)
            {
                throw new Exception("Ocurrio un error al realizar el Login", ex);
            }
        }

        public static async Task<int> ChangePasswordAsync(Users pUsers, string pOldPassword)
        {
            try
            {
                int result = 0;
                var OldUserPassword = new Users { PASSWORD_HASH = pOldPassword };
                EncryptMD5(OldUserPassword);
                using(var dbcontext = new DBContext())
                {
                    var user = await dbcontext.Users.FirstOrDefaultAsync(s => s.USER_ID == pUsers.USER_ID);
                    if (OldUserPassword.PASSWORD_HASH == user.PASSWORD_HASH)
                    {
                        EncryptMD5(pUsers);
                        user.PASSWORD_HASH = pUsers.PASSWORD_HASH;
                        dbcontext.Update(user);
                        result = await dbcontext.SaveChangesAsync();
                    }
                    else
                        throw new Exception("La contraseña actual es incorrecta");
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error al intentar cambiar la contraseña", ex);
            }
        }

        #endregion
    }
}
