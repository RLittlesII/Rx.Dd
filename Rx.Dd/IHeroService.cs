using DynamicData;
using Rx.Dd.Data;
using System;
using System.Reactive.Linq;

namespace Rx.Dd
{
    public interface IHeroService : IObservable<IChangeSet<Hero, string>>
    {
    }

    public class HeroService : IHeroService
    {
        SourceCache<Hero, string> _sourceCache = new SourceCache<Hero, string>(x => x.Id);

        public IDisposable Subscribe(IObserver<IChangeSet<Hero, string>> observer) => _sourceCache.Connect().RefCount().Subscribe(observer);
    }
}