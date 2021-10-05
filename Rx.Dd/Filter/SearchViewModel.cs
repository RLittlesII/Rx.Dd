using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rocket.Surgery.Airframe;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
               .Subscribe(heroes => _heroCache.EditDiff(heroes, (first, second) => first.Id == second.Id), RxApp.DefaultExceptionHandler.OnNext)
               .DisposeWith(Garbage);

            _heroCache
               .Connect()
               .RefCount()
               .ObserveOn(RxApp.MainThreadScheduler)
               .Bind(out _heroes)
               .Subscribe(_ => {}, RxApp.DefaultExceptionHandler.OnNext)
               .DisposeWith(Garbage);
        }

        [Reactive] public string SearchText { get; set; }

        public ReadOnlyObservableCollection<SuperHeroRecord> Heroes => _heroes;

        private readonly ReadOnlyObservableCollection<SuperHeroRecord> _heroes;
        private readonly SourceCache<SuperHeroRecord, string> _heroCache = new SourceCache<SuperHeroRecord, string>(x => x.Id);
    }
}