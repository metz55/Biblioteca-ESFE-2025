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
    public class DALEditions
    {
        #region CRUD
        public static async Task<int> CreateEditionsAsync(Editions pEditions)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pEditions);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateEditionsAsync(Editions pEditions)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var editions = await dbContext.Editions.FirstOrDefaultAsync(s => s.EDITION_ID == pEditions.EDITION_ID);
                editions.EDITION_NUMBER = pEditions.EDITION_NUMBER;
                dbContext.Update(editions);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteEditionsAsync(Editions pEditions)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var editions = await dbContext.Editions.FirstOrDefaultAsync(s => s.EDITION_ID == pEditions.EDITION_ID);
                dbContext.Editions.Remove(editions);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Editions>> GetAllEditionsAsync()
        {
            var editions = new List<Editions>();
            using (var dbContext = new DBContext())
            {
                editions = await dbContext.Editions.ToListAsync();
            }
            return editions;
        }

        public static async Task<Editions> GetEditionsByIdAsync(Editions pEditions)
        {
            var editions = new Editions();
            using (var dbContext = new DBContext())
            {
                editions = await dbContext.Editions.FirstOrDefaultAsync(s => s.EDITION_ID == pEditions.EDITION_ID);
            }
            return editions;
        }

        internal static IQueryable<Editions> QuerySelect(IQueryable<Editions> pQuery, Editions pEditions)
        {
            if (pEditions.EDITION_ID > 0)
                pQuery = pQuery.Where(e => e.EDITION_ID == pEditions.EDITION_ID);
            if (!string.IsNullOrWhiteSpace(pEditions.EDITION_NUMBER))
                pQuery = pQuery.Where(e => e.EDITION_NUMBER.Contains(pEditions.EDITION_NUMBER));
            pQuery = pQuery.OrderByDescending(e => e.EDITION_ID).AsQueryable();
            if (pEditions.Top_Aux > 0)
                pQuery = pQuery.Take(pEditions.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Editions>> GetEditionsAsync(Editions pEditions)
        {
            var editions = new List<Editions>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Editions.AsQueryable();
                select = QuerySelect(select, pEditions);
                editions = await select.ToListAsync();
            }
            return editions;
        }
        #endregion


    }
}
