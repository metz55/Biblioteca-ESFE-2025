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
    public class DALCatalogs
    {
        #region CRUD
        public static async Task<int> CreateCatalogsAsync(Catalogs pCatalogs)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pCatalogs);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateCatalogsAsync(Catalogs pCatalogs)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var catalogs = await dbContext.Catalogs.FirstOrDefaultAsync(s => s.CATALOG_ID == pCatalogs.CATALOG_ID);
                catalogs.CATALOG_NAME = pCatalogs.CATALOG_NAME;
                dbContext.Update(catalogs);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteCatalogsAsync(Catalogs pCatalogs)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var catalogs = await dbContext.Catalogs.FirstOrDefaultAsync(s => s.CATALOG_ID == pCatalogs.CATALOG_ID);
                dbContext.Catalogs.Remove(catalogs);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Catalogs>> GetAllCatalogsAsync()
        {
            var catalogs = new List<Catalogs>();
            using (var dbContext = new DBContext())
            {
                catalogs = await dbContext.Catalogs.ToListAsync();
            }
            return catalogs;
        }

        public static async Task<Catalogs> GetCatalogsByIdAsync(Catalogs pCatalogs)
        {
            var catalogs = new Catalogs();
            using (var dbContext = new DBContext())
            {
                catalogs = await dbContext.Catalogs.FirstOrDefaultAsync(s => s.CATALOG_ID == pCatalogs.CATALOG_ID);
            }
            return catalogs;
        }

        internal static IQueryable<Catalogs> QuerySelect(IQueryable<Catalogs> pQuery, Catalogs pCatalogs)
        {
            if (pCatalogs.CATALOG_ID > 0)
                pQuery = pQuery.Where(s => s.CATALOG_ID == pCatalogs.CATALOG_ID);
            if (!string.IsNullOrWhiteSpace(pCatalogs.CATALOG_NAME))
                pQuery = pQuery.Where(s => s.CATALOG_NAME.Contains(pCatalogs.CATALOG_NAME));
            pQuery = pQuery.OrderByDescending(s => s.CATALOG_ID).AsQueryable();
            if (pCatalogs.Top_Aux > 0)
                pQuery = pQuery.Take(pCatalogs.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Catalogs>> GetCatalogsAsync(Catalogs pCatalogs)
        {
            var catalogs = new List<Catalogs>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Catalogs.AsQueryable();
                select = QuerySelect(select, pCatalogs);
                catalogs = await select.ToListAsync();
            }
            return catalogs;
        }
        #endregion
    }
}
