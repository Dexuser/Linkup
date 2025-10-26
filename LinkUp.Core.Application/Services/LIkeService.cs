using AutoMapper;
using LinkUp.Core.Application.Dtos.Like;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class LIkeService : GenericServices<Like, LikeDto>, ILikeService
{
    private readonly ILikeRepository _likeRepository;
    private readonly IMapper _mapper;
    public LIkeService(ILikeRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _likeRepository = repository;
        _mapper = mapper;
    }

    public async Task<Result<LikeDto>> GetThisLikeByPostIdAndUserId(int postId, string userId)
    {
        try
        {
            var like = await _likeRepository.GetAllQueryable().FirstOrDefaultAsync(l=>l.PostId == postId && l.UserId == userId);
            var dto = _mapper.Map<LikeDto>(like);
            return Result<LikeDto>.Ok(dto); // En este caso, no importa que el like sea null. Por eso no mandamos un fail si no encontramos
        }
        catch (Exception ex)
        {
            return Result<LikeDto>.Fail(ex.Message);
        }
    }
}