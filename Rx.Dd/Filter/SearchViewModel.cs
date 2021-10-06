using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Dd.Filter
{
    public class SearchViewModel : NavigableViewModelBase
    {
        public SearchViewModel(IHeroApiClient heroApiClient)
        {
            this.WhenValueChanged(x => x.SearchText)
               .Throttle(TimeSpan.FromMilliseconds(300), RxApp.TaskpoolScheduler)
               .SelectMany(heroApiClient.Find)
               .Where(x => x != null)
               .Select(heroes => heroes.Select(HeroCache.Convert))
               .Subscribe(heroes => _heroCache.EditDiff(heroes, (first, second) => first.Id == second.Id), RxApp.DefaultExceptionHandler.OnNext)
               .DisposeWith(Garbage);

            _heroCache
               .Connect()
               .RefCount()
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out _heroes)
               .DisposeMany()
               .Subscribe(_ => {}, RxApp.DefaultExceptionHandler.OnNext)
               .DisposeWith(Garbage);
        }

        [Reactive] public string SearchText { get; set; }

        public ReadOnlyObservableCollection<Hero> Heroes => _heroes;

        private readonly ReadOnlyObservableCollection<Hero> _heroes;
        private readonly SourceCache<Hero, string> _heroCache = new SourceCache<Hero, string>(x => x.Id);
    }
}