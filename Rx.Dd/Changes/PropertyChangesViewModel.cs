using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Rocket.Surgery.Airframe.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rx.Dd.Changes
{
    public class PropertyChangesViewModel : NavigableViewModelBase
    {
        public PropertyChangesViewModel(IHeroCache heroCache)
        {
            _heroCache = heroCache;

            heroCache
               .AutoRefresh(x => x.RealName)
               .Batch(TimeSpan.FromMilliseconds(1500), RxApp.TaskpoolScheduler)
               .Sort(SortExpressionComparer<Hero>
                   .Ascending(x => x.RealName))
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out _heroes, resetThreshold: 1)
               .DisposeMany()
               .Subscribe()
               .DisposeWith(Garbage);

            _interval = Observable.Interval(TimeSpan.FromSeconds(8), RxApp.TaskpoolScheduler).Publish();

            _interval
               .Select(_ => Heroes.FirstOrDefault(x => string.IsNullOrEmpty(x.RealName)))
               .Where(x => x is not null)
               .ObserveOn(RxApp.MainThreadScheduler)
               .Subscribe(
                    hero => { hero.RealName = hero.Name; },
                    RxApp.DefaultExceptionHandler.OnNext
                );
        }


        public ReadOnlyObservableCollection<Hero> Heroes => _heroes;
        protected override IObservable<Unit> ExecuteInitialize() => _heroCache.Load().Do(_ => _interval.Connect().DisposeWith(Garbage));

        private readonly IHeroCache _heroCache;
        private readonly ReadOnlyObservableCollection<Hero> _heroes;
        private readonly IConnectableObservable<long> _interval;
    }
}