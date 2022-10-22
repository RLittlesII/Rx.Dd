using System;
using System.Collections.Generic;

namespace Rx.Dd.Data
{
    /// <summary>
    /// Interface representing an api client for <see cref="Hero"/>.
    /// </summary>
    public interface IHeroApiClient
    {
        /// <summary>
        /// Get a hero with the provided id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="forceUpdate">Force an update.</param>
        /// <returns>A hero dto.</returns>
        IObservable<SuperHeroRecord> GetHero(string id, bool forceUpdate = false);

        /// <summary>
        /// Gets a list of stores.
        /// </summary>
        /// <param name="forceUpdate">Force an update.</param>
        /// <returns>A list of heroes.</returns>
        IObservable<IEnumerable<SuperHeroRecord>> GetHeroes(bool forceUpdate = false);

        /// <summary>
        /// Find a superhero by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IObservable<IEnumerable<SuperHeroRecord>> Find(string name);
    }
}