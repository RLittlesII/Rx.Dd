using Rx.Dd.Data;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd
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
                for (var i = 0; i < 1337; i++)
                {
                    _superheroApiContract.Get(i.ToString()).Subscribe(hero => heroes.Add(hero)).DisposeWith(disposable);
                }

                observer.OnNext(heroes);

                return disposable;
            }
        );


        /// <inheritdoc />
        public IObservable<IEnumerable<SuperHeroRecord>> Find(string name) => Observable.Create<IEnumerable<SuperHeroRecord>>(observer => _superheroApiContract.Search(name).Select(x => x.Heroes).Subscribe(observer));

        private readonly ISuperheroApiContract _superheroApiContract;
    }
}