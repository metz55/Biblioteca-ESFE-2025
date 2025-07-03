using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLEditions
    {

        public async Task<int> CreateEditionsAsync(Editions pEditions)
        {
            return await DALEditions.CreateEditionsAsync(pEditions);
        }

        public async Task<int> UpdateEditionsAsync(Editions pEditions)
        {
            return await DALEditions.UpdateEditionsAsync(pEditions);
        }

        public async Task<int> DeleteEditionsAsync(Editions pEditions)
        {
            return await DALEditions.DeleteEditionsAsync(pEditions);
        }

        public async Task<List<Editions>> GetAllEditionsAsync()
        {
            return await DALEditions.GetAllEditionsAsync();
        }

        public async Task<Editions> GetEditionsByIdAsync(Editions pEditions)
        {
            return await DALEditions.GetEditionsByIdAsync(pEditions);
        }

        public async Task<List<Editions>> GetEditionsAsync(Editions pEditions)
        {

            return await DALEditions.GetEditionsAsync(pEditions);
        }
    }
}
