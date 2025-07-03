using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLPosts
    {
        public async Task<int> CreatePostAsync(Posts pPosts)
        {
            return await DALPosts.CreatePostAsync(pPosts);
        }
        public async Task<int> UpdatePostAsync(Posts pPosts)
        {
            return await DALPosts.UpdatePostAsync(pPosts);
        }
        public async Task<int> DeletePostAsync(Posts pPosts)
        {
            return await DALPosts.DeletePostAsync(pPosts);
        }
        public async Task<Posts> GetPostByIdAsync(Posts pPosts)
        {
            return await DALPosts.GetPostByIdAsync(pPosts);
        }
        public async Task<List<Posts>> GetAllPostsAsync()
        {
            return await DALPosts.GetAllPostsAsync();
        }
        public async Task<List<Posts>> GetPostsAsync(Posts pPosts)
        {
            return await DALPosts.GetPostsAsync(pPosts);
        }

        public async Task<List<Posts>> GetSearchedManagePosts(Posts pPosts)
        {
            return await DALPosts.GetSearchedManagePosts(pPosts);
        }
        public async Task<List<Posts>> GetPinnedPosts()
        {
            return await DALPosts.GetPinnedPosts();
        }
        public async Task<List<Posts>> GetLastPosts()
        {
            return await DALPosts.GetLastPosts();
        }
    }
}
