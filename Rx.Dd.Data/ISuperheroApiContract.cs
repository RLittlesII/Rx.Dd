using Refit;
using Rx.Dd.Data;
using System;
using System.Collections.Generic;

namespace Rx.Dd
{
    public interface ISuperheroApiContract
    {
            /// <summary>
            /// Gets an store with the specified ID.
            /// </summary>
            /// <param name="name">The ID of the appointment to retrieve.</param>
            /// <returns>An observable which signals with the store.</returns>
            [Get("/search/{name}")]
            IObservable<SuperHeroRecords> SearchHero(string name);
    }
}