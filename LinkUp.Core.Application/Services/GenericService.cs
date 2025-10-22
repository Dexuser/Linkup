using AutoMapper;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class GenericServices<TEntity, TDtoModel> : IGenericService<TDtoModel>
    where TEntity : class
    where TDtoModel : class
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public GenericServices(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<Result<List<TDtoModel>>> GetAllAsync()
    {
        var dtos = _mapper.Map<List<TDtoModel>>(await _repository.GetAllAsync());
        return Result<List<TDtoModel>>.Ok(dtos);
    }

    public virtual async Task<Result<TDtoModel>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return Result<TDtoModel>.Fail("The record was not found");
            }
            return Result<TDtoModel>.Ok(_mapper.Map<TDtoModel>(entity));
        }
        catch (Exception e)
        {
            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<TDtoModel>> AddAsync(TDtoModel dtoModel)
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(dtoModel);
            TEntity? returnEntity = await _repository.AddAsync(entity);

            TDtoModel dto = _mapper.Map<TDtoModel>(returnEntity);
            return Result<TDtoModel>.Ok(dto);
        }
        catch (Exception e)
        {
            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<List<TDtoModel>>> AddRangeAsync(List<TDtoModel> dtomodels)
    {
        try
        {
            List<TEntity> entity = _mapper.Map<List<TEntity>>(dtomodels);
            List<TEntity> returnEntities = await _repository.AddRangeAsync(entity);
            
            List<TDtoModel> dtos = _mapper.Map<List<TDtoModel>>(returnEntities);
            return Result<List<TDtoModel>>.Ok(dtos);
        }
        catch (Exception e)
        {
            return Result<List<TDtoModel>>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<TDtoModel>> UpdateAsync(int id, TDtoModel dtoModel)
    {
        try
        {
            TEntity entity = _mapper.Map<TEntity>(dtoModel);
            TEntity? returnEntity = await _repository.UpdateAsync(id, entity);
            if (returnEntity == null)
            {
                return Result<TDtoModel>.Fail("The record could not be updated");
            }

            var dto = _mapper.Map<TDtoModel>(returnEntity);
            return Result<TDtoModel>.Ok(dto);
        }
        catch (Exception e)
        {

            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result> DeleteAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}