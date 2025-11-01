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
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly ILikeRepository _likeRepository;
    private readonly IMapper _mapper;

    public PostService(IPostRepository repository, IMapper mapper, ICommentRepository commentRepository,
        IFriendShipRepository friendShipRequestRepository, IAccountServiceForWebApp accountServiceForWebApp,
        ILikeRepository likeRepository) : base(repository, mapper)
    {
        _postRepository = repository;
        _mapper = mapper;
        _commentRepository = commentRepository;
        _friendShipRepository = friendShipRequestRepository;
        _accountServiceForWebApp = accountServiceForWebApp;
        _likeRepository = likeRepository;
    }


    public override async Task<Result<PostDto>> GetByIdAsync(int id)
    {
        try
        {
            var post = await _postRepository.GetAllQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            var postDto = _mapper.Map<PostDto>(post);
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
            // Obtener el usuario actual
            var userResult = await _accountServiceForWebApp.GetUserById(userId);
            if (userResult.IsFailure)
                return Result<List<PostDto>>.Fail(userResult.GeneralError!);
            var user = userResult.Value!;

            // Obtener posts del usuario
            var posts = await _postRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            if (!posts.Any())
                return Result<List<PostDto>>.Ok(new List<PostDto>());

            var postDtos = _mapper.Map<List<PostDto>>(posts);
            var postIds = postDtos.Select(p => p.Id).ToList();

            // Obtener comentarios y replies de esos posts
            var comments = await _commentRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(c => postIds.Contains(c.PostId))
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<CommentDto>>(comments);

            // Obtener IDs de todos los autores de comentarios y replies
            var userIds = commentDtos.Select(c => c.AuthorId)
                .Concat(commentDtos.SelectMany(c => c.Replies).Select(r => r.AuthorId))
                .Append(userId) // incluir el autor de los posts
                .Distinct()
                .ToList();

            // Obtener usuarios
            var usersResult = await _accountServiceForWebApp.GetUsersByIds(userIds);
            if (usersResult.IsFailure)
                return Result<List<PostDto>>.Fail(usersResult.GeneralError!);

            var usersDict = usersResult.Value!.ToDictionary(u => u.Id, u => u);

            // Asignar autores en DTOs (no entidades)
            foreach (var commentDto in commentDtos)
            {
                commentDto.Author = usersDict.GetValueOrDefault(commentDto.AuthorId);
                foreach (var reply in commentDto.Replies)
                    reply.Author = usersDict.GetValueOrDefault(reply.AuthorId);
            }

            // Agrupar comentarios por postId
            var commentsByPostId = commentDtos
                .GroupBy(c => c.PostId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Obtener likes/dislikes del usuario actual
            var likesAndDislikes = await _likeRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(l => l.UserId == userId && postIds.Contains(l.PostId))
                .ToDictionaryAsync(l => l.PostId, l => (bool?)l.IsLiked);

            // Asignar datos finales
            foreach (var postDto in postDtos)
            {
                postDto.User = user; // este es el dueño del post
                postDto.Comments = commentsByPostId.GetValueOrDefault(postDto.Id) ?? new List<CommentDto>();
                postDto.IsLikedByUserInThisSession = likesAndDislikes.GetValueOrDefault(postDto.Id);
            }

            return Result<List<PostDto>>.Ok(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostDto>>.Fail(ex.Message);
        }
    }

    public async Task<Result<List<PostDto>>> GetAllFriendsPosts(string userId)
    {
        try
        {
            var friendsIds = await _friendShipRepository.GetAllTheFriendsIds(userId);
            if (friendsIds == null || !friendsIds.Any())
                return Result<List<PostDto>>.Ok(new List<PostDto>());

            // Posts de los amigos
            var posts = await _postRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(p => friendsIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            if (!posts.Any())
                return Result<List<PostDto>>.Ok(new List<PostDto>());

            var postDtos = _mapper.Map<List<PostDto>>(posts);
            var postIds = postDtos.Select(p => p.Id).ToList();

            // Comentarios con sus replies
            var comments = await _commentRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(c => postIds.Contains(c.PostId))
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<CommentDto>>(comments);

            // IDs de usuarios de posts, comentarios y replies
            var userIds = postDtos.Select(p => p.UserId)
                .Concat(commentDtos.Select(c => c.AuthorId))
                .Concat(commentDtos.SelectMany(c => c.Replies).Select(r => r.AuthorId))
                .Distinct()
                .ToList();

            // Obtener todos los usuarios
            var usersResult = await _accountServiceForWebApp.GetUsersByIds(userIds);
            if (usersResult.IsFailure)
                return Result<List<PostDto>>.Fail(usersResult.GeneralError!);

            var usersDict = usersResult.Value!.ToDictionary(u => u.Id, u => u);

            // Asignar autores en los DTOs
            foreach (var commentDto in commentDtos)
            {
                commentDto.Author = usersDict.GetValueOrDefault(commentDto.AuthorId);
                foreach (var reply in commentDto.Replies)
                    reply.Author = usersDict.GetValueOrDefault(reply.AuthorId);
            }

            var commentsByPostId = commentDtos
                .GroupBy(c => c.PostId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Likes/dislikes del usuario actual
            var likesAndDislikes = await _likeRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(l => l.UserId == userId && postIds.Contains(l.PostId))
                .ToDictionaryAsync(l => l.PostId, l => (bool?)l.IsLiked);

            // Asignar usuarios, comentarios y likes a los posts
            foreach (var postDto in postDtos)
            {
                postDto.User = usersDict.GetValueOrDefault(postDto.UserId);
                postDto.Comments = commentsByPostId.GetValueOrDefault(postDto.Id) ?? new List<CommentDto>();
                postDto.IsLikedByUserInThisSession = likesAndDislikes.GetValueOrDefault(postDto.Id);
            }

            return Result<List<PostDto>>.Ok(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostDto>>.Fail(ex.Message);
        }
    }


    public override async Task<Result> DeleteAsync(int id)
    {
        await _commentRepository.GetAllQueryable().Where(c => c.PostId == id).ExecuteDeleteAsync();
        return await base.DeleteAsync(id);
    }
}