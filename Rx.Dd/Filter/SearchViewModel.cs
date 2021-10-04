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
        public SearchViewModel(ISuperheroApiContract superheroApiContract)
        {
            this.WhenValueChanged(x => x.SearchText)
               .ObserveOn(RxApp.TaskpoolScheduler)
               .SelectMany(superheroApiContract.SearchHero)
               .Where(x => x.Heroes != null && x.Heroes.Count > 0)
               .Select(x => x.Heroes)
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