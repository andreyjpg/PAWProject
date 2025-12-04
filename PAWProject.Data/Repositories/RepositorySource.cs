using PAWProject.Models.Entities;
using PAWProject.Data.MSSQL;

namespace PAWProject.Data.Repositories
{
    public interface IRepositorySource
    {
        Task<bool> UpsertAsync(Source entity, bool isUpdating);
        Task<bool> CreateAsync(Source entity);
        Task<bool> DeleteAsync(Source entity);
        Task<IEnumerable<Source>> ReadAsync();
        Task<Source> FindAsync(int id);
        Task<bool> UpdateAsync(Source entity);
        Task<bool> UpdateManyAsync(IEnumerable<Source> entities);
        Task<bool> ExistsAsync(Source entity);
    }

    public class RepositorySource : RepositoryBase<Source>, IRepositorySource
    {
        public RepositorySource(NewsHubContext context) : base(context)
        {
        }
    }
