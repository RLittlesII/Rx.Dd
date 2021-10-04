using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Rx.Dd.Filter
{
    public class SearchViewModel : NavigableViewModelBase
    {
        private SourceCache<SuperHeroRecord, string> _heroCache = new SourceCache<SuperHeroRecord, string>(x => x.Id);
        private ReadOnlyObservableCollection<SuperHeroRecord> _heroes;

        public SearchViewModel(ISuperheroApiContract superheroApiContract)
        {
            this.WhenValueChanged(x => x.SearchText)
               .SelectMany(superheroApiContract.SearchHero)
               .Where(x => x.Heroes != null && x.Heroes.Count > 0)
               .Select(x => x.Heroes)
               .Subscribe(_ => _heroCache.AddOrUpdate(_), RxApp.DefaultExceptionHandler.OnNext);

            _heroCache
               .Connect()
               .RefCount()
               .Bind(out _heroes)
               .Subscribe();
        }

        [Reactive] public string SearchText { get; set; }

        public ReadOnlyObservableCollection<SuperHeroRecord> Heroes => _heroes;
    }
}