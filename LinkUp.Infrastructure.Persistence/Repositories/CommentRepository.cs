using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(LinkUpContext context) : base(context)
    {
    }
}