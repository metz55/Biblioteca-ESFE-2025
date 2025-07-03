using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLEditorials
    {
        public async Task<int> CreateEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.CreateEditorialsAsync(pEditorials);
        }

        public async Task<int> UpdateEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.UpdateEditorialsAsync(pEditorials);
        }

        public async Task<int> DeleteEditorialsAsync(Editorials pEditorials)
        {
            return await DALEditorials.DeleteEditorialsAsync(pEditorials);
        }

        public async Task<List<Editorials>> GetAllEditorialsAsync()
        {
            return await DALEditorials.GetAllEditorialsAsync();
        }

        public async Task<Editorials> GetEditorialsByIdAsync(Editorials pEditorials)
        {
            return await DALEditorials.GetEditorialsByIdAsync(pEditorials);
        }

        public async Task<List<Editorials>> GetEditorialsAsync(Editorials pEditorials)
        {

            return await DALEditorials.GetEditorialsAsync(pEditorials);
        }

    }
}
