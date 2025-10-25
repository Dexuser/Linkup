using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class PostService : GenericServices<Post, PostDto>, IPostService 
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IFriendShipRequestRepository _friendShipRequestRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IMapper _mapper;

    public PostService(IPostRepository repository, IMapper mapper, ICommentRepository commentRepository,
        IFriendShipRequestRepository friendShipRequestRepository, IAccountServiceForWebApp accountServiceForWebApp) : base(repository, mapper)
    {
        _postRepository = repository;
        _mapper = mapper;
        _commentRepository = commentRepository;
        _friendShipRequestRepository = friendShipRequestRepository;
        _accountServiceForWebApp = accountServiceForWebApp;
    }


    public override async Task<Result<PostDto>> GetByIdAsync(int id)
    {
        try
        {
            var post = await _postRepository.GetAllQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            var comments = await _commentRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(c => c.PostId == id)
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Replies).ToListAsync();
        
            var userResult = await _accountServiceForWebApp.GetUserById(post!.UserId);
            if (userResult.IsFailure)
            {
                return Result<PostDto>.Fail(userResult.GeneralError!);
            }
        
            var postDto = _mapper.Map<PostDto>(post);
            postDto.Comments = _mapper.Map<List<CommentDto>>(comments); ;
            return Result<PostDto>.Ok(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Fail(ex.Message);
        }

    }

    public async Task<Result<List<PostDto>>> GetAllPostsOfThisUser(string userId)
    {
        try
        {
            var result = await _accountServiceForWebApp.GetUserById(userId);
            if (result.IsFailure)
            {
                return Result<List<PostDto>>.Fail(result.GeneralError!);
            }
            
            // Todos los posts de este usuario
            var posts = await _postRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            
            var postsDtos = _mapper.Map<List<PostDto>>(posts);
            var postsIds = postsDtos.Select(p => p.Id).ToList();
            
            // Todos los comentarios y sus replies de los posts anteriores
            var comments = await _commentRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(c => postsIds.Contains(c.PostId))
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Replies).ToListAsync();
            
            var commentsDtos = _mapper.Map<List<CommentDto>>(comments);
            
            // Todos los usuarios de los comentarios anteriores (incluyendo los comentarios que son replies de otros)
            var usersIds = commentsDtos.Select(c => c.AuthorId)
                .Concat(commentsDtos.SelectMany(coment => coment.Replies).Select(c => c.AuthorId)) // Esto trae los usuarios de los replies
                .Distinct()
                .ToList();
            var usersResult = await _accountServiceForWebApp.GetUsersByIds(usersIds);
            var users = usersResult.Value!.ToDictionary(user => user.Id, user => user);
            
            foreach (var commentDto in commentsDtos)
            {
                commentDto.Author = users.GetValueOrDefault(commentDto.AuthorId);
                foreach (var reply in commentDto.Replies)
                {
                    reply.Author = users.GetValueOrDefault(reply.AuthorId);
                }
            }
            
            var commentsByPostId = commentsDtos
                .GroupBy(c => c.PostId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var postDto in postsDtos)
            {
                postDto.Comments = commentsByPostId.GetValueOrDefault(postDto.Id) ?? new List<CommentDto>();
                postDto.User = result.Value!;
            }

            return Result<List<PostDto>>.Ok(postsDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostDto>>.Fail(ex.Message);
        }

    }

    public async Task<Result<List<PostDto>>> GetAllFriendsPosts(string userId)
    {
        var friendsIds = await _friendShipRequestRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(f => f.RequestedByUserId == userId || f.TargetUserId == userId)
            .Select(f => (f.RequestedByUserId == userId) ? f.TargetUserId : f.RequestedByUserId)
            .ToListAsync();
        
        var posts = await _postRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(p => friendsIds.Contains(p.UserId)) 
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        
        var postsDtos = _mapper.Map<List<PostDto>>(posts);
        var postsIds = postsDtos.Select(p => p.Id).ToList();
        
        
        // Todos los comentarios y sus replies de los posts anteriores
        var comments = await _commentRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(c => postsIds.Contains(c.PostId))
            .Where(c => c.ParentCommentId == null)
            .Include(c => c.Replies).ToListAsync();
            
        var commentsDtos = _mapper.Map<List<CommentDto>>(comments);
       
        var usersIds = commentsDtos.Select(c => c.AuthorId)
            .Concat(commentsDtos.SelectMany(coment => coment.Replies).Select(c => c.AuthorId)) // Esto trae los usuarios de los replies
            .Distinct()
            .ToList();
        var usersResult = await _accountServiceForWebApp.GetUsersByIds(usersIds);
        var users = usersResult.Value!.ToDictionary(user => user.Id, user => user);
            
        foreach (var commentDto in commentsDtos)
        {
            commentDto.Author = users.GetValueOrDefault(commentDto.AuthorId);
            foreach (var reply in commentDto.Replies)
            {
                reply.Author = users.GetValueOrDefault(reply.AuthorId);
            }
        }
            
        var commentsByPostId = commentsDtos
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var postDto in postsDtos)
        {
            postDto.Comments = commentsByPostId.GetValueOrDefault(postDto.Id) ?? new List<CommentDto>();
        }
            
        return Result<List<PostDto>>.Ok(_mapper.Map<List<PostDto>>(posts));
    }
}