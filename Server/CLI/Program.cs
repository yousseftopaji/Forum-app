using CLI.UI;
using FileRepository;
using RepositoryContracts;

Console.WriteLine("Starting CLI...");
IUserRepository userRepository = new UserInFileRepository();
IPostRepository postRepository = new PostInFileRepository();
ICommentRepository commentRepository = new CommentInFileRepository();

CliApp app = new(userRepository, postRepository, commentRepository);
await app.StartAsync();