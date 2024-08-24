namespace Aether.Repositories.Common;

/*public abstract class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
    protected readonly ArtemisContext _context;
    protected readonly IArtemisConnectionFactory _connectionFactory;
    protected readonly DbSet<TEntity> _entities;

    protected BaseRepository(ArtemisContext context, IArtemisConnectionFactory connectionFactory)
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
}*/
