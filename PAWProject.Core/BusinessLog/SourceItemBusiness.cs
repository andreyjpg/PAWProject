using PAWProject.Data.Repositories;
using PAWProject.Models.Entities;

namespace PAWProject.Core.Business;

    public interface ISourceItemBusiness
{
    /// <summary>
    /// Deletes the source item associated with the id.
    /// </summary>
    /// <param name="id">The source item id.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteSourceItemAsync(int id);

    /// <summary>
    /// Gets source items. If id is provided, returns only that item; otherwise returns all items.
    /// </summary>
    /// <param name="id">Optional source item id.</param>
    /// <returns>A collection of source items.</returns>
    Task<IEnumerable<SourceItem>> GetSourceItems(int? id);

    /// <summary>
    /// Saves a source item (creates or updates).
    /// </summary>
    /// <param name="item">The source item to save.</param>
    /// <returns>True if save was successful, false otherwise.</returns>
    Task<bool> SaveSourceItemAsync(SourceItem item);
}

public class SourceItemBusiness(IRepositorySourceItem repositorySourceItem) : ISourceItemBusiness
{
    /// <inheritdoc />
    public async Task<bool> SaveSourceItemAsync(SourceItem item)
    {
        return await repositorySourceItem.UpdateAsync(item);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteSourceItemAsync(int id)
    {
        var item = await repositorySourceItem.FindAsync(id);
        if (item is null)
            return false;

        return await repositorySourceItem.DeleteAsync(item);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SourceItem>> GetSourceItems(int? id)
    {
        if (id is null)
            return await repositorySourceItem.ReadAsync();

        var item = await repositorySourceItem.FindAsync(id.Value);
        return item is null ? [] : new[] { item };
    }
}