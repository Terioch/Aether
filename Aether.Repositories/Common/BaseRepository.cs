using Aether.Core.Repositories;
using Aether.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Aether.Repositories.Common;

public abstract class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
    protected readonly AetherContext _context;
    protected readonly IAetherConnectionFactory _connectionFactory;
    protected readonly DbSet<TEntity> _entities;

    protected BaseRepository(AetherContext context, IAetherConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;
        _entities = _context.Set<TEntity>();
    }

    public ValueTask<TEntity?> GetAsync(TId id)
    {
        return _entities.FindAsync(id);
    }

    public async ValueTask<TEntity?> AddAsync(TEntity entity)
    {
        var result = await _entities.AddAsync(entity);
        return result.Entity;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return _entities.AddRangeAsync(entities);
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return _entities.ToListAsync();
    }

    public async Task<TEntity?> DeletePermanent(TId id)
    {
        var entity = await _entities.FindAsync(id);

        if (entity == null) return null;

        _entities.Remove(entity);

        return entity;
    }

    public Task DeletePermanent(IEnumerable<TEntity> entities)
    {
        _entities.RemoveRange(entities);

        return Task.CompletedTask;
    }
}
