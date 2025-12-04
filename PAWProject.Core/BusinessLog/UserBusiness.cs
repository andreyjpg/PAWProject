using PAWProject.Data.Repositories;
using PAWProject.Models.Entities;

namespace PAWProject.Core.BusinessLog;
    public interface IUserBusiness
    {
        /// <summary>
        /// Deletes the user associated with the user id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        Task<bool> DeleteUserAsync(int id);
        /// <summary>
        /// Gets users. If id is provided, returns only that user; otherwise returns all users.
        /// </summary>
        /// <param name="id">Optional user id.</param>
        /// <returns>A collection of users.</returns>
        Task<IEnumerable<User>> GetUsers(int? id);
        /// <summary>
        /// Saves a user (creates or updates).
        /// </summary>
        /// <param name="user">The user to save.</param>
        /// <returns>True if save was successful, false otherwise.</returns>
        Task<bool> SaveUserAsync(User user);
    }
    public class UserBusiness(IRepositoryUser repositoryUser) : IUserBusiness
    {
        /// <inheritdoc />
        public async Task<bool> SaveUserAsync(User user)
        {
            return await repositoryUser.UpdateAsync(user);
        }
        /// <inheritdoc />
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await repositoryUser.FindAsync(id);
            if (user is null)
                return false;

            return await repositoryUser.DeleteAsync(user);
        }
        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetUsers(int? id)
        {
            if (id is null)
                return await repositoryUser.ReadAsync();

            var user = await repositoryUser.FindAsync(id.Value);

            return user is null ? [] : new[] { user };
        }
    }
