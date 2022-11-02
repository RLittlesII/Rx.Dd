using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;

namespace Rx.Dd.Data
{
    public class SuperHeroApiMock : ISuperheroApiContract
    {
        public SuperHeroApiMock()
        {
            var heroes =
                JsonSerializer
                   .Deserialize<IEnumerable<SuperHeroRecord>>(SuperHeroesJson.Heroes)?
                   .Where(x => !string.IsNullOrEmpty(x.Id)) ??
                Enumerable.Empty<SuperHeroRecord>();

            _heroCache.Edit(action => action.Load(heroes));
        }

        public IObservable<SuperHeroRecord> Get(string id) => Observable.Create<SuperHeroRecord>(
            observer =>
            {
                var optional = _heroCache.Lookup(id);

                if (optional.HasValue)
                {
                    observer.OnNext(optional.Value);
                }

                observer.OnCompleted();
                return Disposable.Empty;
            }
        );

        public IObservable<SuperHeroRecords> Search(string name) => Observable.Empty<SuperHeroRecords>();

        private readonly SourceCache<SuperHeroRecord, string> _heroCache = new SourceCache<SuperHeroRecord, string>(x => x.Id);
    }
}