using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class LikeRepository :  GenericRepository<Like>, ILikeRepository
{
    public LikeRepository(LinkUpContext context) : base(context)
    {
    }
}