using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class BattleshipGameRepository : GenericRepository<BattleshipGame>, IBattleshipGameRepository
{
    public BattleshipGameRepository(LinkUpContext context) : base(context)
    {
    }
}