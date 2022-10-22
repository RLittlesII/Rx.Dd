using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using MenuItem = Rx.Dd.ViewModel.MenuItem;

namespace Rx.Dd.Forms
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.WhenActivated(
                disposables =>
                {
                    this.WhenAnyValue(x => x.ViewModel.MenuItems)
                       .BindTo(this, x => x.Menu.ItemsSource)
                       .DisposeWith(disposables);

                    Menu
                       .Events()
                       .ItemTapped
                       .Select(x => x.Item)
                       .Cast<MenuItem>()
                       .InvokeCommand(this, x => x.ViewModel.Navigate)
                       .DisposeWith(disposables);

                    Menu
                       .Events()
                       .ItemSelected
                       .Where(x => x != null)
                       .Subscribe(_ => Menu.SelectedItem = null)
                       .DisposeWith(disposables);
                }
            );
        }
    }
}
