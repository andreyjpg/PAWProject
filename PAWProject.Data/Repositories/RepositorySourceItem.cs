using PAWProject.Models.Entities;
using PAWProject.Data.MSSQL;

namespace PAWProject.Data.Repositories;
    public interface IRepositorySourceItem
    {
        Task<bool> UpsertAsync(SourceItem entity, bool isUpdating);
        Task<bool> CreateAsync(SourceItem entity);
        Task<bool> DeleteAsync(SourceItem entity);
        Task<IEnumerable<SourceItem>> ReadAsync();
        Task<SourceItem> FindAsync(int id);
        Task<bool> UpdateAsync(SourceItem entity);
        Task<bool> UpdateManyAsync(IEnumerable<SourceItem> entities);
        Task<bool> ExistsAsync(SourceItem entity);
    }

    public class RepositorySourceItem : RepositoryBase<SourceItem>, IRepositorySourceItem
    {
        public RepositorySourceItem(NewsHubContext context) : base(context)
        {
        }
}