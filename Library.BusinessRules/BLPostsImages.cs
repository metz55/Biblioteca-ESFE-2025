using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLPostImages
    {
        public async Task<int> CreatePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.CreatePostImageAsync(pPostImages);
        }
        public async Task<int> UpdatePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.UpdatePostImageAsync(pPostImages);
        }
        public async Task<int> UpdatePostImageByIdPostAsync(Posts pPosts, PostsImages pPostsImages)
        {
            return await DALPostsImages.UpdatePostImageByIdPostAsync(pPosts, pPostsImages);
        }
        public async Task<int> DeletePostImageAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.DeletePostImageAsync(pPostImages);
        }
        public async Task<int> DeletePostsImagesByIdPostAsync(Posts pPosts)
        {
            return await DALPostsImages.DeletePostsImagesByIdPostAsync(pPosts);
        }
        public async Task<List<PostsImages>> GetPostImagesByIdAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.GetPostImagesByIdAsync(pPostImages);
        }
        public async Task<List<PostsImages>> GetPostImagesByIdPostAsync(Posts pPosts)
        {
            return await DALPostsImages.GetPostImagesByIdPostAsync(pPosts);
        }
        public async Task<List<PostsImages>> GetAllPostImagesAsync()
        {
            return await DALPostsImages.GetAllPostImagesAsync();
        }
        public async Task<List<PostsImages>> GetPostImagesAsync(PostsImages pPostImages)
        {
            return await DALPostsImages.GetPostImagesAsync(pPostImages);
        }
    }
}
