using RepositoryContracts;
using Entities;

namespace DefaultNamespace;

public class ManagePostsView
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly User _user;

    public ManagePostsView(IPostRepository postRepository, ICommentRepository commentRepository, User user)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _user = user;
    }

    public async Task ShowAsync()
    {
        var listPostsView = new ListPostsView(_postRepository);
        var singlePostView = new SinglePostView(_postRepository, _commentRepository, _user);
        while (true)
        {
            var postId = await listPostsView.ShowAsync();
            if (postId == null)
                break;
            await singlePostView.ShowAsync(postId.Value);
        }
    }
}