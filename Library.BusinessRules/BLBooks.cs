using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLBooks
    {
        //En los metodos siguientes  se devuelven las funcionalidades programada en los metodos DAL
        public async Task<int> CrearAsync(Books pBooks)
        {
            return await DALBooks.CreateBooksAsync(pBooks);
        }
        public async Task<int> UpdateBooksAsync(Books pBooks)
        {
            return await DALBooks.UpdateBooksAsync(pBooks);
        }
        public async Task<int> UpdateExistencesBooksAsync(Books2 pBooks)
        {
            return await DALBooks.UpdateExistencesBooksAsync(pBooks);
        }
        public async Task<int> DeleteBooksAsync(Books pBooks)
        {
            return await DALBooks.DeleteBooksAsync(pBooks);
        }
        public async Task<Books> GetBooksByIdAsync(Books pBooks)
        {
            return await DALBooks.GetBooksByIdAsync(pBooks);
        }
        public async Task<List<Books>> GetAllBooksAsync()
        {
            return await DALBooks.GetAllBooksAsync();
        }
        public async Task<List<Books>> GetBooksAsync(Books pBooks)
        {
            return await DALBooks.GetBooksAsync(pBooks);
        }
        public async Task<List<Books>> GetIncludePropertiesAsync(Books pBooks)
        {
            return await DALBooks.GetIncludePropertiesAsync(pBooks);
        }

        public async Task<List<Books>> BuscarPorTituloAsync(string pTitulo)
        {
            // Llama al método DAL que busca libros por título (o parte del título)
            return await DALBooks.GetBooksByTitleAsync(pTitulo);
        }

    }
}
