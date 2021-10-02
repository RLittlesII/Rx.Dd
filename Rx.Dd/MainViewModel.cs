using ReactiveUI;
using Rocket.Surgery.Airframe.ViewModels;
using Rx.Dd.Filter;
using Sextant;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

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
                        MenuItem.Filter => await parameterViewStackService.PushPage<FiltersViewModel>(),
                        _               => throw new ArgumentOutOfRangeException(nameof(item), item, null)
                    };
                }
            );
        }

        public ReadOnlyObservableCollection<MenuItem> MenuItems { get; set; }

        public ReactiveCommand<MenuItem, Unit> Navigate { get; set; }
    }

    public enum MenuItem
    {
        Filter
    }
}