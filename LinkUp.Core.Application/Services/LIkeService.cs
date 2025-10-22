using AutoMapper;
using LinkUp.Core.Application.Dtos.Like;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class LIkeService : GenericServices<Like, LikeDto>, ILikeService
{
    public LIkeService(ILikeRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}