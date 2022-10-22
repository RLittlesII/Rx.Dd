using Rx.Dd.Data;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd.ViewModel
{
    public class HeroApiClient : IHeroApiClient
    {
        public HeroApiClient(ISuperheroApiContract superheroApiContract) =>
            _superheroApiContract = superheroApiContract;

        /// <inheritdoc />
        public IObservable<SuperHeroRecord> GetHero(string id, bool forceUpdate = false) =>
            Observable.Create<SuperHeroRecord>(observer => _superheroApiContract.Get(id).Subscribe(observer));

        /// <inheritdoc />
        public IObservable<IEnumerable<SuperHeroRecord>> GetHeroes(bool forceUpdate = false) =>
            Observable.Create<IEnumerable<SuperHeroRecord>>(observer =>
            {
                var disposable = new CompositeDisposable();
                var heroes = new List<SuperHeroRecord>();

                Observable.Range(0, 100)
                   .Select(id => _superheroApiContract.Get(id.ToString()))
                   .Merge(6)
                   .Subscribe(hero => heroes.Add(hero), () => observer.OnNext(heroes))
                   .DisposeWith(disposable);

                return disposable;
            }
        );

        /// <inheritdoc />
        public IObservable<IEnumerable<SuperHeroRecord>> Find(string name) => Observable.Create<IEnumerable<SuperHeroRecord>>(observer => _superheroApiContract.Search(name).Select(x => x.Heroes).Subscribe(observer));

        private readonly ISuperheroApiContract _superheroApiContract;
    }
}
