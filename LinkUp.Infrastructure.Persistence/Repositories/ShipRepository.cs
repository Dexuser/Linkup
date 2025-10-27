using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class ShipRepository : GenericRepository<Ship>, IShipRepository
{
    public ShipRepository(LinkUpContext context) : base(context)
    {
    }
}