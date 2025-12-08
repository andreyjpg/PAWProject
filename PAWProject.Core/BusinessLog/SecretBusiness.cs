using PAWProject.Data.Repositories;
using PAWProject.Models.Entities;

namespace PAWProject.Core.Business;
    public interface ISecretBusiness
    {
        /// <summary>
        /// Deletes the secret associated with the id.
        /// </summary>
        /// <param name="id">The secret id.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteSecretAsync(int id);

        /// <summary>
        /// Gets secrets. If id is provided, returns only that secret; otherwise returns all secrets.
        /// </summary>
        /// <param name="id">Optional secret id.</param>
        /// <returns>A collection of secrets.</returns>
        Task<IEnumerable<Secret>> GetSecrets(int? id);

        /// <summary>
        /// Saves a secret (creates or updates).
        /// </summary>
        /// <param name="secret">The secret to save.</param>
        /// <returns>True if save was successful, false otherwise.</returns>
        Task<bool> SaveSecretAsync(Secret secret);
    }

    public class SecretBusiness(IRepositorySecret repositorySecret) : ISecretBusiness
    {
        /// <inheritdoc />
        public async Task<bool> SaveSecretAsync(Secret secret)
        {
            return await repositorySecret.UpdateAsync(secret);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteSecretAsync(int id)
        {
            var secret = await repositorySecret.FindAsync(id);
            if (secret is null)
                return false;

            return await repositorySecret.DeleteAsync(secret);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Secret>> GetSecrets(int? id)
        {
            if (id is null)
                return await repositorySecret.ReadAsync();

            var secret = await repositorySecret.FindAsync(id.Value);
            return secret is null ? [] : new[] { secret };
        }
    }