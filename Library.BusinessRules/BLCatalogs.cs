using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLCatalogs
    {
        public async Task<int> CreateCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.CreateCatalogsAsync(pCatalogs);
        }

        public async Task<int> UpdateCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.UpdateCatalogsAsync(pCatalogs);
        }

        public async Task<int> DeleteCatalogsAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.DeleteCatalogsAsync(pCatalogs);
        }

        public async Task<List<Catalogs>> GetAllCatalogsAsync()
        {
            return await DALCatalogs.GetAllCatalogsAsync();
        }

        public async Task<Catalogs> GetCatalogsByIdAsync(Catalogs pCatalogs)
        {
            return await DALCatalogs.GetCatalogsByIdAsync(pCatalogs);
        }

        public async Task<List<Catalogs>> GetCatalogsAsync(Catalogs pCatalogs)
        {

            return await DALCatalogs.GetCatalogsAsync(pCatalogs);
        }
    }
}
