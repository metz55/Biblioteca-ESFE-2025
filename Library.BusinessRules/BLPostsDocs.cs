using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLPostDocs
    {
        public async Task<int> CreatePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.CreatePostDocAsync(pPostDocs);
        }
        public async Task<int> UpdatePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.UpdatePostDocAsync(pPostDocs);
        }
        public async Task<int> DeletePostDocAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.DeletePostDocAsync(pPostDocs);
        }
        public async Task<int> DeleteAllPostsDocsByIdPostAsync(Posts pPosts)
        {
            return await DALPostsDocs.DeleteAllPostsDocsByIdPostAsync(pPosts);
        }
        public async Task<PostsDocs> GetPostDocsByIdAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.GetPostDocsByIdAsync(pPostDocs);
        }
        public async Task<List<PostsDocs>> GetPostDocsByPostIdAsync(Posts pPosts)
        {
            return await DALPostsDocs.GetPostDocsByPostIdAsync(pPosts);
        }
        public async Task<List<PostsDocs>> GetAllPostDocsAsync()
        {
            return await DALPostsDocs.GetAllPostDocsAsync();
        }
        public async Task<List<PostsDocs>> GetPostDocsAsync(PostsDocs pPostDocs)
        {
            return await DALPostsDocs.GetPostDocsAsync(pPostDocs);
        }
    }
}
