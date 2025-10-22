namespace LinkUp.Core.Application.Interfaces;

public interface IGenericService<TDtoModel>
{
    Task<Result<List<TDtoModel>>> GetAllAsync();
    Task<Result<TDtoModel>> GetByIdAsync(int id);
    Task<Result<TDtoModel>> AddAsync(TDtoModel dtoModel);
    Task<Result<TDtoModel>> UpdateAsync(int id, TDtoModel model);
    Task<Result> DeleteAsync(int id);
}