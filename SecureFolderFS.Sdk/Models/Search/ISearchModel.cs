﻿using System.Collections.Generic;
using System.Threading;

namespace SecureFolderFS.Sdk.Models.Search
{
    /// <summary>
    /// Provides a common API ground for search.
    /// </summary>
    public interface ISearchModel
    {
        /// <summary>
        /// Searches for results based on provided <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">Cancellation token of the action.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of type <see cref="ISearchResult"/> of found items.</returns>
        IAsyncEnumerable<ISearchResult> SearchAsync(string query, CancellationToken cancellationToken = default);
    }
}