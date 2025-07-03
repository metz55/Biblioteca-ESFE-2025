using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLPostsCategories
    {
        public async Task<int> CreatePostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.CreatePostCategoryAsync(pPostsCategories);
        }
        public async Task<int> UpdatePostCategeoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.UpdatePostCategeoryAsync(pPostsCategories);
        }
        public async Task<int> DeletePostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.DeletePostCategoryAsync(pPostsCategories);
        }
        public async Task<PostsCategories> GetPostCategoryByIdAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.GetPostCategoryByIdAsync(pPostsCategories);
        }
        public async Task<List<PostsCategories>> GetAllPostCategoriesAsync()
        {
            return await DALPostsCategories.GetAllPostCategoriesAsync();
        }
        public async Task<List<PostsCategories>> GetPostCategoryAsync(PostsCategories pPostsCategories)
        {
            return await DALPostsCategories.GetPostCategoryAsync(pPostsCategories);
        }
    }
}
