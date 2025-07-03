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
    public class DALCountries
    {
        #region CRUD
        public static async Task<int> CreateCountriesAsync(Countries pCountries)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pCountries);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateCountriesAsync(Countries pCountries)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var countries = await dbContext.Countries.FirstOrDefaultAsync(s => s.COUNTRY_ID == pCountries.COUNTRY_ID);
                countries.COUNTRY_NAME = pCountries.COUNTRY_NAME;
                dbContext.Update(countries);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteCountriesAsync(Countries pCountries)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var countries = await dbContext.Countries.FirstOrDefaultAsync(s => s.COUNTRY_ID == pCountries.COUNTRY_ID);
                dbContext.Countries.Remove(countries);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Countries>> GetAllCountriesAsync()
        {
            var countries = new List<Countries>();
            using (var dbContext = new DBContext())
            {
                countries = await dbContext.Countries.ToListAsync();
            }
            return countries;
        }

        public static async Task<Countries> GetCountriesByIdAsync(Countries pCountries)
        {
            var countries = new Countries();
            using (var dbContext = new DBContext())
            {
                countries = await dbContext.Countries.FirstOrDefaultAsync(s => s.COUNTRY_ID == pCountries.COUNTRY_ID);
            }
            return countries;
        }

        internal static IQueryable<Countries> QuerySelect(IQueryable<Countries> pQuery, Countries pCountries)
        {
            if (pCountries.COUNTRY_ID > 0)
                pQuery = pQuery.Where(s => s.COUNTRY_ID == pCountries.COUNTRY_ID);
            if (!string.IsNullOrWhiteSpace(pCountries.COUNTRY_NAME))
                pQuery = pQuery.Where(s => s.COUNTRY_NAME.Contains(pCountries.COUNTRY_NAME));
            pQuery = pQuery.OrderByDescending(s => s.COUNTRY_ID).AsQueryable();
            if (pCountries.Top_Aux > 0)
                pQuery = pQuery.Take(pCountries.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Countries>> GetCountriesAsync(Countries pCountries)
        {
            var countries = new List<Countries>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Countries.AsQueryable();
                select = QuerySelect(select, pCountries);
                countries = await select.ToListAsync();
            }
            return countries;
        }
        #endregion
    }
}
