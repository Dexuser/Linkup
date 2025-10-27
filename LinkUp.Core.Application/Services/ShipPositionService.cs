
using AutoMapper;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class ShipPositionService : GenericServices<ShipPosition, ShipPositionDto>, IShipPositionService  
{
    public ShipPositionService(IShipPositionRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}