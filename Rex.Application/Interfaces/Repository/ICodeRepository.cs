using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with codes in the database.
    /// </summary>
    public interface ICodeRepository : IGenericRepository<Code>
    {
        /// <summary>
        /// Creates a new code in the database.
        /// </summary>
        /// <param name="code">The code to create.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task CreateCodeAsync(Code code, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a code by its ID.
        /// </summary>
        /// <param name="id">The ID of the code.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The code with the specified ID.</returns>
        Task<Code> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken);
        
        /// <summary>
        /// Retrieves a <see cref="Code"/> entity that matches the specified code value.
        /// </summary>
        /// <param name="value">The code value to search for.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The <see cref="Code"/> entity if found; otherwise, null.</returns>
        Task<Code> GetCodeByValueAsync(string value, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a code is valid.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the code is valid; otherwise false.</returns>
        Task<bool> IsCodeValidAsync(string code, CancellationToken cancellationToken);

        /// <summary>
        /// Marks a code as used.
        /// </summary>
        /// <param name="code">The code value to mark as used.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task MarkCodeAsUsedAsync(string code, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a code has already been used.
        /// </summary>
        /// <param name="code">The ID of the code.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the code has been used; otherwise false.</returns>
        Task<bool> IsCodeUsedAsync(string code, CancellationToken cancellationToken);
    }
}
