using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Data;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Rx.Dd.Filter
{
    public class FiltersViewModel  : NavigableViewModelBase
    {
        private readonly ReadOnlyObservableCollection<string> _alignments;
        private readonly ReadOnlyObservableCollection<Hero> _heroes;

        public FiltersViewModel()
        {
            // heroService
            //    .Transform(x => x.Alignment)
            //    .Bind(out _alignments)
            //    .Subscribe()
            //    .DisposeWith(Garbage);
            //
            // heroService
            //    .Bind(out _heroes)
            //    .Subscribe()
            //    .DisposeWith(Garbage);
        }

        public ReadOnlyObservableCollection<string> Alignments => _alignments;

        public ReadOnlyObservableCollection<Hero> Heroes => _heroes;

        [Reactive] public string SelectedAlignment { get; set; }
    }
}