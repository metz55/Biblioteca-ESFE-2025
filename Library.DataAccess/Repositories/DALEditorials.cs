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
    public class DALEditorials
    {

        #region CRUD
        public static async Task<int> CreateEditorialsAsync(Editorials pEditorials)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pEditorials);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateEditorialsAsync(Editorials pEditorials)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var editorials = await dbContext.Editorials.FirstOrDefaultAsync(s => s.EDITORIAL_ID == pEditorials.EDITORIAL_ID);
                editorials.EDITORIAL_NAME = pEditorials.EDITORIAL_NAME;
                dbContext.Update(editorials);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteEditorialsAsync(Editorials pEditorials)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var editorials = await dbContext.Editorials.FirstOrDefaultAsync(s => s.EDITORIAL_ID == pEditorials.EDITORIAL_ID);
                dbContext.Editorials.Remove(editorials);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Editorials>> GetAllEditorialsAsync()
        {
            var editorials = new List<Editorials>();
            using (var dbContext = new DBContext())
            {
                editorials = await dbContext.Editorials.ToListAsync();
            }
            return editorials;
        }

        public static async Task<Editorials> GetEditorialsByIdAsync(Editorials pEditorials)
        {
            var editorials = new Editorials();
            using (var dbContext = new DBContext())
            {
                editorials = await dbContext.Editorials.FirstOrDefaultAsync(s => s.EDITORIAL_ID == pEditorials.EDITORIAL_ID);
            }
            return editorials;
        }

        internal static IQueryable<Editorials> QuerySelect(IQueryable<Editorials> pQuery, Editorials pEditorials)
        {
            if (pEditorials.EDITORIAL_ID > 0)
                pQuery = pQuery.Where(s => s.EDITORIAL_ID == pEditorials.EDITORIAL_ID);
            if (!string.IsNullOrWhiteSpace(pEditorials.EDITORIAL_NAME))
                pQuery = pQuery.Where(s => s.EDITORIAL_NAME.Contains(pEditorials.EDITORIAL_NAME));
            pQuery = pQuery.OrderByDescending(s => s.EDITORIAL_ID).AsQueryable();
            if (pEditorials.Top_Aux > 0)
                pQuery = pQuery.Take(pEditorials.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Editorials>> GetEditorialsAsync(Editorials pEditorials)
        {
            var editorials = new List<Editorials>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Editorials.AsQueryable();
                select = QuerySelect(select, pEditorials);
                editorials = await select.ToListAsync();
            }
            return editorials;
        }
        #endregion
    }
}