using Rx.Dd.Data;
using System;
using System.Collections.Generic;

namespace Rx.Dd
{
    /// <summary>
    /// Interface representing an api client for <see cref="Hero"/>.
    /// </summary>
    public interface IHeroApiClient
    {
        /// <summary>
        /// Get a store with the provided id.
        /// </summary>
        /// <param name="storeId">The store id.</param>
        /// <param name="forceUpdate">Force an update.</param>
        /// <returns>A store dto.</returns>
        IObservable<Hero> GetHero(Guid storeId, bool forceUpdate = false);

        /// <summary>
        /// Gets a list of stores.
        /// </summary>
        /// <param name="forceUpdate">Force an update.</param>
        /// <returns>A list of stores.</returns>
        IObservable<IEnumerable<Hero>> GetHeroes(bool forceUpdate = false);
    }
}