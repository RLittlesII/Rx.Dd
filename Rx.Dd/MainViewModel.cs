using ReactiveUI;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Changes;
using Rx.Dd.Filter;
using Rx.Dd.Sort;
using Sextant;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Rx.Dd
{
    public class MainViewModel : NavigableViewModelBase
    {
        public MainViewModel(IParameterViewStackService parameterViewStackService)
        {
            MenuItems = new ReadOnlyObservableCollection<MenuItem>(new ObservableCollection<MenuItem>(Enum.GetValues(typeof(MenuItem)).Cast<MenuItem>().ToList()));

            Navigate = ReactiveCommand.CreateFromTask<MenuItem, Unit>(
                async item =>
                {
                    return item switch
                    {
                        MenuItem.Changes => await parameterViewStackService.PushPage<PropertyChangesViewModel>(),
                        MenuItem.Filter  => await parameterViewStackService.PushPage<FiltersViewModel>(),
                        MenuItem.Search  => await parameterViewStackService.PushPage<SearchViewModel>(),
                        MenuItem.Sort    => await parameterViewStackService.PushPage<SortingViewModel>(),
                        _                => throw new ArgumentOutOfRangeException(nameof(item), item, null)
                    };
                }
            );
        }

        public ReadOnlyObservableCollection<MenuItem> MenuItems { get; set; }

        public ReactiveCommand<MenuItem, Unit> Navigate { get; set; }
    }

    public enum MenuItem
    {
        Filter,
        Search,
        Sort,
        Changes
    }
}