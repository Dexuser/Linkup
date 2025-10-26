using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class CommentService : GenericServices<Comment, CommentDto>, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    public CommentService(ICommentRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _commentRepository = repository;
    }

    public override async Task<Result> DeleteAsync(int id)
    {
        // Borra los comentarios hijos. Solamente puede existir un nivel de Anidamiento en comentarios.
        // es decir, solamente existen cometarios padres y comentarios hijos.
        await _commentRepository.GetAllQueryable().Where(c => c.ParentCommentId == id).ExecuteDeleteAsync();
        return await base.DeleteAsync(id);
    }
}