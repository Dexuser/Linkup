using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class AttackRepository : GenericRepository<Attack>, IAttackRepository
{
    public AttackRepository(LinkUpContext context) : base(context)
    {
    }
}