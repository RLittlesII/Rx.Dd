using Refit;
using System;

namespace Rx.Dd.Data
{
    public interface ISuperheroApiContract
    {
        /// <summary>
        /// Gets an hero with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the appointment to retrieve.</param>
        /// <returns>An observable which signals with the hero.</returns>
        [Get("/{id}")]
        IObservable<SuperHeroRecord> Get(string id);

        /// <summary>
        /// Searches heroes with the specified name.
        /// </summary>
        /// <param name="name">The ID of the appointment to retrieve.</param>
        /// <returns>An observable which signals with the hero.</returns>
        [Get("/search/{name}")]
        IObservable<SuperHeroRecords> Search(string name);
    }
}