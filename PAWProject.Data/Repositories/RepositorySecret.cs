using PAWProject.Models.Entities;
using PAWProject.Data.MSSQL;

namespace PAWProject.Data.Repositories;

public interface IRepositorySecret

{
    Task<bool> UpsertAsync(Secret entity, bool isUpdating);
    Task<bool> CreateAsync(Secret entity);
    Task<bool> DeleteAsync(Secret entity);
    Task<IEnumerable<Secret>> ReadAsync();
    Task<Secret> FindAsync(int id);
    Task<bool> UpdateAsync(Secret entity);
    Task<bool> UpdateManyAsync(IEnumerable<Secret> entities);
    Task<bool> ExistsAsync(Secret entity);
}

public class RepositorySecret : RepositoryBase<Secret>, IRepositorySecret
{
    public RepositorySecret(NewsHubContext context) : base(context)
    {
    }
}