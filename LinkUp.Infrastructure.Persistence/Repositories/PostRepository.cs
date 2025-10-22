using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class PostRepository :  GenericRepository<Post>, IPostRepository
{
    public PostRepository(LinkUpContext context) : base(context)
    {
    }
}