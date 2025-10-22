namespace LinkUp.Core.Domain.Interfaces;

public interface IGenericRepository<TEntity>
{
    Task<List<TEntity>> GetAllAsync(TEntity entity);
    
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
    Task<TEntity?> UpdateAsync(int id, TEntity entity);
    Task DeleteAsync(int entity);
    IQueryable<TEntity> GetAllQueryable();
}