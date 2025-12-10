using PAWProject.Data.Repositories;
using PAWProject.Models.Entities;

namespace PAWProject.Core.BusinessLog;
    public interface ISourceBusiness
    {
        /// <summary>
        /// Deletes the source associated with the id.
        /// </summary>
        /// <param name="id">The source id.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteSourceAsync(int id);
        /// <summary>
        /// Gets sources. If id is provided, returns only that source; otherwise returns all sources.
        /// </summary>
        /// <param name="id">Optional source id.</param>
        /// <returns>A collection of sources.</returns>
        Task<IEnumerable<Source>> GetSources(int? id);
        /// <summary>
        /// Saves a source (creates or updates).
        /// </summary>
        /// <param name="source">The source to save.</param>
        /// <returns>True if save was successful, false otherwise.</returns>
        Task<bool> SaveSourceAsync(Source source);
    }
    public class SourceBusiness(IRepositorySource repositorySource) : ISourceBusiness
    {
        /// <inheritdoc />
        public async Task<bool> SaveSourceAsync(Source source)
        {
            return await repositorySource.UpdateAsync(source);
        }
        /// <inheritdoc />
        public async Task<bool> DeleteSourceAsync(int id)
        {
            var source = await repositorySource.FindAsync(id);
            if (source is null)
                return false;

            return await repositorySource.DeleteAsync(source);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Source>> GetSources(int? id)
        {
            if (id is null)
                return await repositorySource.ReadAsync();

            var source = await repositorySource.FindAsync(id.Value);
            return source is null ? [] : new[] { source };
        }
    }
