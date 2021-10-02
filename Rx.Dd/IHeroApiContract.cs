using Refit;
using Rx.Dd.Data;
using System;
using System.Collections.Generic;

namespace Rx.Dd
{
    public interface IHeroApiContract
    {
            /// <summary>
            /// Gets an store with the specified ID.
            /// </summary>
            /// <param name="storeId">The ID of the appointment to retrieve.</param>
            /// <param name="parameters">The azure function parameters.</param>
            /// <returns>An observable which signals with the store.</returns>
            [Get("/api/stores/{storeId}")]
            IObservable<Hero> GetHeroes(Guid storeId, [Query] FunctionParameters parameters);

            /// <summary>
            /// Gets all the available stores.
            /// </summary>
            /// <param name="parameters">The azure function parameters.</param>
            /// <returns>An observable which signals with the stores.</returns>
            [Get("/api/stores")]
            IObservable<IEnumerable<Hero>> GetHeroes([Query] FunctionParameters parameters);
    }
}