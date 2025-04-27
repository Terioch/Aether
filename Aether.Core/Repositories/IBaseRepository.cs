namespace Aether.Core.Repositories;

public interface IBaseRepository<TEntity, in TId> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();

    ValueTask<TEntity?> GetAsync(TId id);

    ValueTask<TEntity?> AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);    

    Task<TEntity?> DeletePermanent(TId id);

    Task DeletePermanent(IEnumerable<TEntity> entities);
}
