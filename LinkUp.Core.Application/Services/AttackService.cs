using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class AttackService : GenericServices<Attack, AttackDto>, IAttackService
{
    public AttackService(IAttackRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}