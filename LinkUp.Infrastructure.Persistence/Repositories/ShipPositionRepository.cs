using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class ShipPositionRepository : GenericRepository<ShipPosition>, IShipPositionRepository
{
    public ShipPositionRepository(LinkUpContext context) : base(context)
    {
    }
}