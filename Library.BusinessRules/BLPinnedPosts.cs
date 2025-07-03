using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules;

public class BLPinnedPosts
{
    DALPinnedPosts DALPinnedPosts = new DALPinnedPosts();
    public async Task<PinnedPosts> GetPinnedPostByIdPost(Posts posts)
    {
        return await DALPinnedPosts.GetPinnedPostByPostId(posts);
    }

    public async Task<bool> IsPostPinned(Posts posts)
    {
        return await DALPinnedPosts.IsPostPinned(posts);
    }

    public async Task<bool> CreatePinnedPost(PinnedPosts pinnedPost)
    {
        return await DALPinnedPosts.CreatePinnedPost(pinnedPost);
    }

    public async Task<bool> DeletePinnedPost(PinnedPosts pinnedPosts)
    {
        return await DALPinnedPosts.DeletePinnedPost(pinnedPosts);
    }
}