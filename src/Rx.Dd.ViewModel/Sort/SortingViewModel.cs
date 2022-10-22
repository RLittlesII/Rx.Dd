using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rocket.Surgery.Airframe.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd.ViewModel.Sort
{
    public class SortingViewModel : NavigableViewModelBase
    {
        public SortingViewModel(IHeroCache heroCache)
        {
            _heroCache = heroCache;

            var sortChanged =
                this.WhenAnyValue(x => x.SelectedTeam)
                   .Where(x => !string.IsNullOrEmpty(x))
                   .Select(selectedTag =>
                        SortExpressionComparer<Hero>
                           .Ascending(x => x.Teams.Contains(selectedTag)));

            var heroChangeSet = _heroCache.RefCount();

            heroChangeSet
               .TransformMany(x => x.Teams, x => x)
               .DistinctValues(x => x)
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out _teams)
               .DisposeMany()
               .Subscribe()
               .DisposeWith(Garbage);

            heroChangeSet
               .Sort(sortChanged)
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out _heroes, resetThreshold: 1)
               .DisposeMany()
               .Subscribe()
               .DisposeWith(Garbage);
        }

        [Reactive] public string SelectedTeam { get; set; }

        public ReadOnlyObservableCollection<Hero> Heroes => _heroes;

        public ReadOnlyObservableCollection<string> Teams => _teams;

        protected override IObservable<Unit> ExecuteInitialize() => _heroCache.Load();

        private readonly IHeroCache _heroCache;
        private readonly ReadOnlyObservableCollection<string> _teams;
        private readonly ReadOnlyObservableCollection<Hero> _heroes;
    }
}
