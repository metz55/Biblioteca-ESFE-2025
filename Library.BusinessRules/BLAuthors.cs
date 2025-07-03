using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLAuthors
    {
        public async Task<int> CreateAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.CreateAuthorsAsync(pAuthors);
        }

        public async Task<int> UpdateAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.UpdateAuthorsAsync(pAuthors);
        }

        public async Task<int> DeleteAuthorsAsync(Authors pAuthors)
        {
            return await DALAuthors.DeleteAuthorsAsync(pAuthors);
        }

        public async Task<List<Authors>> GetAlAuthorsAsync()
        {
            return await DALAuthors.GetAllAuthorsAsync();
        }

        public async Task<Authors> GetAuthorsByIdAsync(Authors pAuthors)
        {
            return await DALAuthors.GetAuthorsByIdAsync(pAuthors);
        }

        public async Task<List<Authors>> GetAuthorsAsync(Authors pAuthors)
        {

            return await DALAuthors.GetAuthorsAsync(pAuthors);
        }
    }
}
